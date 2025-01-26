using System.Collections;
using UnityEngine;

//!@ Refactor:
//Add bool instanceRef flag, and if flagged, make all music_ funcs run code/update properties for the instance
//This would allow any events run on Audio script ref with the flag set 
//to apply music changes to the music singleton instance (for Events to run music stuff properly without referencing a singleton)

/// <summary>Handles sfx, bgm, and vox</summary>
public class Audio : MonoBehaviour
{
    private const string MixerGroup_Master = "Master";
    private const string uscore = "_";

    //!@ Add max constants here for bgm, sfx, vox enums
    #region ClipEnums
    /// <summary>List of SFX enum IDs</summary>
    public enum SFX
    {
        SFX_NULL,                           //NULL SFX        
        SFX_MENU_HIGHLIGHT,                 //Menu higlight SFX
        SFX_MENU_SELECT,                    //Menu select SFX

        SFX_PUMP,                           //Pump
        SFX_BOOST,                          //Boost
        
        SFX_HIT_PLAYER,                     //Hit SFX (Player to Player)
        SFX_HIT_SPIKES,                     //Spikes
        SFX_HIT_DEATH,                      //Spikes
        SFX_DEATH_PLAYER,                   //Death SFX (Player)
        SFX_VICTORY_PLAYER,                 //Victory SFX (Player)
        MAX = SFX_VICTORY_PLAYER + 0x01     //MAX SFX
    };

    /// <summary>Filenames for each sfx, aligned with SFX enum</summary>
    private string[] sfx = new string[(int)(SFX.MAX)+1]
    {
        "NULL",
        "MenuHighlight",           //Menu higlight SFX
        "MenuSelect",              //Menu select SFX

        "Pump",                    //Pump
        "Deflate",                   //Boost
        
        "Hit_Player",              //Hit SFX
        "Hit_Spikes",              //Hit SPikes SFX
        "Hit_Death",                //Hit death wall
        "Death_Player",            //Death SFX
        "Victory_Player",          //Victory SFX
        "MAX",
    };

    /// <summary>List of base vocal FX IDs</summary>
    public enum VOX
    {
        VOX_NULL,
        VOX_TITLE,
        VOX_MM_PLAY,
        VOX_MM_OPTIONS,
        VOX_MM_CREDITS,
        VOX_MM_QUIT,
        MAX = VOX_MM_QUIT + 0x01
    }

    /// <summary>Base filenames for each vox, aligned with VOX enum</summary>
    private string[] vox = new string[(int)(VOX.MAX)+1]
    {
        "NULL",
        "Title",
        "MM_Play",
        "MM_Options",
        "MM_Credits",
        "MM_Quit",
        "MAX",
    };
    #endregion


    #region BgmPack_StructDef
    /// <summary>List of BGM ID enums</summary>
    public enum BGM
    {
        BGM_NULL,
        BGM_MENU,
        BGM_BATTLE,
        BGM_VICTORY,
        BGM_CREDITS,
        MAX = BGM_CREDITS + 0x01
    };

    /// <summary>Struct holding data for each BGM def</summary>
    public struct BgmPack
    {
        private readonly string Path;                           //Filename of song
        private readonly float Start;                           //Start loop cuept
        private readonly float End;                             //End loop cuept

        /// <summary>Init a BGMPack</summary>
        /// <param name="Path">Filename of song</param>
        /// <param name="Start">Start loop cuept</param>
        /// <param name="End">End loop cuept</param>
        public BgmPack(string Path, float Start, float End)
        {
            this.Path = Path;
            this.Start = Start;
            this.End = End;
        }

        /// <summary>Get path</summary>
        public string path { get { return Path; } }

        /// <summary>Get start cuept</summary>
        public float start { get { return Start; } }

        /// <summary>Get end cuept</summary>
        public float end { get { return End; } }
    }

    private const float MinToSec = 60f;                     //Constant converting mins to sec

    /// <summary>BgmPacks for all music, aligned with BGM enum</summary>
    public BgmPack[] BGMs = new BgmPack[(int)(BGM.MAX) + 1]
    {
        new BgmPack("BGM_NULL",0f,0f),
        new BgmPack("BGM_MENU",0f,0f),
        new BgmPack("BGM_BATTLE",0f,0f),
        new BgmPack("BGM_VICTORY",0f,0f),
        new BgmPack("BGM_CREDITS",0f,0f),
        new BgmPack("MAX",0f,0f),
    };
    #endregion

    //Ref scripts and objects
    //public __GameData GameData;                                   //GameData
    public static Audio instance;                                   //Instance of the Audio script playing music
    public BGM songOnStart = BGM.BGM_NULL;                          //Song to play onStart (if any)
    public SFX sfxOnStart = SFX.SFX_NULL;                           //Sfx to play onStart (if any)
    public sfxType sfxOnStart_type = sfxType.SFX_NULL;              //Type of sfx to play onStart (if any)
    public bool sfxOnStart_loop = false;                            //Loop the onStart sfx?
    public float sfxOnStart_volume = 1f;                            //Volume of onStart sfx
    public float sfxOnStart_pitch = 1f;                             //Pitch of onStart sfx
    public float sfxOnStart_panStereo = 0f;                         //Stereo pan of onStart sfx

    /// <summary>Status of the jingle playing</summary>
    public enum jinglePlayStatus
    {
        jingleFadeOut = -0x01,  //Fading out main level song
        jinglePlay = 0x00,      //Playing the jingle
        jingleFadeIn = 0x01     //Fading back into main level song
    };
    private jinglePlayStatus jingleStatus = jinglePlayStatus.jingleFadeOut;
    public bool jinglePlaying = false;                              //Is a jingle playing? (Don't un/pause bgm when jingle playing in PauseMenu!
    private BGM prevSong;                                           //Previous song before a jingle
    private float prevSongTime = 0f;                                //Previous song's cached time


    /// <summary>Type of SFX for onStart</summary>
    public enum sfxType
    {
        SFX_NULL,                                                   //No type of sfx
        SFX_ONESHOT,                                                //Oneshot sfx
        SFX_MOD,                                                    //Not oneshot, used for dynamic tweaking
        MAX = SFX_MOD
    };
    /// <summary>Max amount of sfxType s. Aligned with the sfxType enum</summary>
    public const byte max_SfxType = (byte)(sfxType.MAX) + 0x01;

    public AudioSource audio_sfx;                                   //Vox/sfx audiosource
    public AudioSource audio_sfxR;                                  //SFX audiosource (reversed)
    public AudioSource audio_bgm;                                   //Music audiosource
    [HideInInspector]public BGM currentSong;                        //Current song being played
    private float currentTime = 0f;                                 //Current time of song
    private float loopStart = -1f;                                  //Loop start cuept of song (in seconds). -1f use 0f
    private float loopEnd = -1f;                                    //Loop start cuept of song (in seconds). -1f = EOF    

    public const string sfx_path = "Audio/Sfx/";                    //Relative path for SFX assets in Resources folder
    public const string bgm_path = "Audio/Bgm/";                    //Relative path for BGM assets in Resources folder
    public const string vox_path = "Audio/Vox/";                    //Relative path for VOX assets in Resources folder

    //local paths for each language+prefix for each clip
    Localization.LangPack vox_path_lang = new Localization.LangPack("Eng/Eng_","Spn/Spn_","Frn/Frn_","Chi/Chi_","Rus/Rus_");
    bool ready = false;                                             //Semafore for Start()

    void Start()
    {
        //Find the GameData
        //GameData = GameObject.Find("  GameData").GetComponent<__GameData>();

        //If have no audio_sfx source for sfx, create it
        if (audio_sfx == null) { audio_sfx = gameObject.AddComponent<AudioSource>(); }
        //Assign the SFX mixer to sfx
        try { audio_sfx.outputAudioMixerGroup = MixerGroup(GameManager.instance.AM_SFX, MixerGroup_Master); } catch { }
        //If have no audio_sfx source for sfx, create it
        if (audio_sfxR == null) { audio_sfxR = gameObject.AddComponent<AudioSource>(); }
        //Assign the SFX mixer to sfx
        try { audio_sfxR.outputAudioMixerGroup = MixerGroup(GameManager.instance.AM_SFX, MixerGroup_Master); } catch { }

        //If have no audio_bgm source for music, create it
        if (audio_bgm == null) { audio_bgm = gameObject.AddComponent<AudioSource>(); }
        //Assign the BGM mixer to bgm
        try { audio_bgm.outputAudioMixerGroup = MixerGroup(GameManager.instance.AM_BGM, MixerGroup_Master); } catch { }

        //Handle songOnstart if not null
        if (songOnStart != BGM.BGM_NULL) { music_play(songOnStart); }

        //Handle sfxOnstart if not null
        if (sfxOnStart != SFX.SFX_NULL)
        {
            if (sfxOnStart_type == sfxType.SFX_ONESHOT)
            {
                //Play oneshot if appropriate
                sfx_play(sfxOnStart);
            }
            else if (sfxOnStart_type == sfxType.SFX_MOD)
            {
                //Play not onsehot if appropriate, assign initial params
                audio_sfx.loop = sfxOnStart_loop;
                audio_sfx.pitch = sfxOnStart_pitch;
                audio_sfx.volume = sfxOnStart_volume;
                audio_sfx.panStereo = sfxOnStart_panStereo;
                sfx_play2(sfxOnStart);
            }
        }
        ready = true;   //We're ready!
    }

    private void Update()
    {
        HandleLoopCue();
        //DebugMusic_Toggle();
    }

    /*
    private void DebugMusic_Toggle()
    {
        const KeyCode M = KeyCode.M;
        bool pressed = Input.GetKeyDown(M);
        if (pressed){audio_bgm.enabled = (!audio_bgm.enabled);}
    }
    */

    /// <summary>Handles start/end loop cue pts of song</summary>
    private void HandleLoopCue()
    {
        if (audio_bgm.isPlaying)
        {
            //If music is playing

            //Determine if we have loop cuepts (not -1f for both cuepts)
            bool loop = (loopStart != -1f && loopEnd != -1f);
            if (loop)
            {
                //Do we have normal loop cue pts (SOF/EOF)? If not 0, then not normal
                bool normal = (loopStart == 0f && loopEnd == 0f);
                if (!normal)
                {
                    //If special cuepts
                    currentTime = audio_bgm.time;   //Set currenttime
                    //If currenttime is past LoopEnd
                    if (currentTime > loopEnd)
                    {
                        audio_bgm.time = loopStart; //Set current time back to loopStart
                        audio_bgm.Play();           //Replay the music
                    }
                }
            }
        }
    }

    /// <summary>Gets an SFX audio clip by enum</summary>
    /// <param name="tune">VOX Enum</param>
    /// <returns>AudioClip</returns>
    public AudioClip getClip(VOX tune)
    {
        string lang = vox_path_lang.language;
        string sound = vox_path + lang + vox[(byte)(tune)];  //Get path
        Debug.Log("Now playing VOX: " + sound);
        AudioClip clip = (AudioClip)(Resources.Load(sound));                   //Load audioclip
        return clip;
    }

    /// <summary>Gets an SFX audio clip by enum</summary>
    /// <param name="tune">SFX Enum</param>
    /// <returns>AudioClip</returns>
    public AudioClip getClip(SFX tune)
    {
        string sound = sfx_path + sfx[(byte)(tune)];
        AudioClip clip = (AudioClip)(Resources.Load(sound));
        return clip;
    }

    /// <summary>Gets a BGM audio clip by enum</summary>
    /// <param name="tune">BGM Enum</param>
    /// <returns>AudioClip</returns>
    public AudioClip getClip(BGM tune)
    {
        byte index = (byte)(tune);
        string sound = bgm_path + BGMs[index].path;
        AudioClip clip = (AudioClip)(Resources.Load(sound));
        return clip;
    }

    /// <summary>Plays an SFX with oneShot</summary>
    /// <param name="tune">SFX ID</param>
    public void sfx_play(SFX tune)
    {
        if (audio_sfx)
        {
            AudioClip clip = getClip(tune);
            audio_sfx.pitch = 1.0f;                                 //Set pitch to 1.0f
            audio_sfx.PlayOneShot(clip);                            //Play it one shot
            //Debug.Log("Now playing sfx " + sound);
        }
    }

    /// <summary>Plays the appropriate vocal clip for the current language (oneShot)</summary>
    /// <param name="vocal">Vocal enum</param>
    public void vox_play(VOX vocal)
    {
        if (audio_sfx)
        {
            AudioClip clip = getClip(vocal);                                        //Get vox clip
            audio_sfx.pitch = 1.0f;                                                 //Set pitch to 1.0f
            audio_sfx.PlayOneShot(clip);                                            //Play it one shot
            Debug.Log("Now playing vox " + clip.ToString());
        }
    }

    /// <summary>Same as vox_play, but without oneshot. Used for pitch manipulation/looping/SimpleSync lipsync</summary>
    /// <param name="vocal">Vox Enum</param>
    public void vox_play2(VOX vocal)
    {
        if (audio_sfx)
        {
            AudioClip clip = getClip(vocal);                            //Get vox clip
            audio_sfx.clip = clip;                                      //Set audio_sfx clip to clip
            audio_sfx.pitch = 1.0f;                                     //Set pitch to 1.0f
            audio_sfx.Play();                                           //Play it
        }
    }

    /// <summary>Same as sfx_play, but without oneshot. Used for pitch manipulation/looping</summary>
    /// <param name="tune">SFX Enum</param>
    public void sfx_play2(SFX tune)
    {
        if (audio_sfx)
        {
            AudioClip clip = getClip(tune);
            audio_sfx.clip = clip;                                      //Set audio_sfx clip to clip
            audio_sfx.pitch = 1.0f;                                     //Set pitch to 1.0f
            audio_sfx.Play();                                           //Play it
            //Debug.Log("Now playing sfx " + sound);
        }
    }

    /// <summary>
    /// Same as sfx_play, but without oneshot (reverse). Used for pitch manipulation/looping
    /// https://youtu.be/g6Z4rtQpqAY?si=mBSBAAeJbi-Kf63n
    /// </summary>
    /// <param name="tune">SFX Enum</param>
    public void sfx_play2_rev(SFX tune)
    {
        if (audio_sfxR)
        {
            sfxR_stop();

            AudioClip clip = getClip(tune);
            audio_sfxR.clip = clip;
            audio_sfxR.pitch = -1.0f;                                //Set pitch to -1.0f (reverse)
            audio_sfxR.timeSamples = (clip.samples - 0x01);          //Goto EOF
            audio_sfxR.Play();                                       //Play it
            //Debug.Log("Now playing sfxR " + sound);
        }
    }

    /// <summary>For Sfx_play2, stops an sfx</summary>
    public void sfx_stop()
    {
        if (audio_sfx)
        {
            audio_sfx.Stop();
            //Debug.Log("SFX Stopped");
        }
    }

    /// <summary>For Sfx_play2R, stops an sfxR</summary>
    public void sfxR_stop()
    {
        if (audio_sfxR)
        {
            audio_sfxR.Stop();
            //Debug.Log("SFX Stopped");
        }
    }

    /// <summary>For Sfx_play2, pauses an sfx</summary>
    public void sfx_pause()
    {
        if (audio_sfx){audio_sfx.Pause();}
    }

    /// <summary>For Sfx_play2, resumes a paused sfx</summary>
    public void sfx_unpause()
    {
        if (audio_sfx){audio_sfx.UnPause();}
    }

    /// <summary>For Sfx_play2R, pauses an sfxR</summary>
    public void sfx_pauseR()
    {
        if (audio_sfxR) { audio_sfxR.Pause(); }
    }

    /// <summary>For Sfx_play2R, resumes a paused sfxR</summary>
    public void sfx_unpauseR()
    {
        if (audio_sfxR) { audio_sfxR.UnPause(); }
    }

    /// <summary>
    /// Plays a jingle.
    /// (Fades out current song, fades in/out a jingle, then resumes previous song)
    /// </summary>
    /// <param name="jingle">Jingle song</param>
    /// <param name="fadeAngle">Lenght of fading</param>
    public void music_playJingle(BGM jingle, float fadeAngle)
    {
        if (music_singleton() == false) { return; }

        BGM song = BGM.BGM_NULL;
        //If a jingle is already playing, handle cancelling the previous jingle correctly based on state of jingle playing
        if (jinglePlaying)
        {
            //Kill all jingle/fade coroutines!
            StopAllCoroutines();

            switch (jingleStatus)
            {
                //During fadeout, the main level song is faededout with volume decreasing, currentsong still = main level song
                case jinglePlayStatus.jingleFadeOut:                    
                case jinglePlayStatus.jingleFadeIn:
                    //Restore max volume, to reinit a new jingle of main level song
                    song = instance.currentSong;
                    audio_bgm.volume = 1f;
                    break;

                //During jingle fade in/out, the old song and its time is cached, and volume is manipulated for jingle
                case jinglePlayStatus.jinglePlay:
                    //Restore previous song, max volume, and its old time
                    song = instance.prevSong;
                    audio_bgm.volume = 1f;
                    audio_bgm.time = prevSongTime;
                    break;
            }
        }
        else
        {
            song = instance.currentSong;                                //Get current song
        }
        StartCoroutine(_music_playJingle(jingle, song, fadeAngle));     //do the jingle
    }

    /// <summary>
    /// Plays a jingle.
    /// (Fades out current song, fades in/out a jingle, then resumes previous song)
    /// </summary>
    /// <param name="jingle">Jingle song</param>    
    /// <param name="song">Song to play post-Jingle</param>
    /// <param name="fadeAngle">Lenght of fading</param>
    /// <returns>Jingle effect/yield</returns>
    private IEnumerator _music_playJingle(BGM jingle, BGM song, float fadeAngle)
    {
        if (music_singleton() == false) { yield break ; }
        
        //Fadeout current song
        //Do not allow interruption until main level song cached!        
        jinglePlaying = true;
        prevSong = song;
        jingleStatus = jinglePlayStatus.jingleFadeOut;
        AudioClip clip=getClip(jingle);                 //Get jingle clip
        float wait = clip.length;                       //Get length of jingle clip
        music_fadeOut(fadeAngle,false);                 //Fade out current song
        yield return new WaitForSeconds(fadeAngle);     //Wait for fade length

        //Play jingle (with fade in/out)
        jingleStatus = jinglePlayStatus.jinglePlay;
        //Play jingle with fadeIn, wait to fadeOut jingle, then play song with restored pos
        prevSongTime = audio_bgm.time;            //Cache currenttime of song
        music_play(jingle);
        music_fadeIn(fadeAngle);
        yield return new WaitForSeconds(wait-fadeAngle);
        music_fadeOut(fadeAngle, true);
        yield return new WaitForSeconds(fadeAngle);

        //Fade in previous song. Do not allow jingle interruption until _currentTime has been restored!
        jingleStatus = jinglePlayStatus.jingleFadeIn;
        music_play(song);
        music_fadeIn(fadeAngle);
        audio_bgm.time = prevSongTime;                  //Restore current time for song
        yield return new WaitForSeconds(fadeAngle);
        jinglePlaying = false;
    }

    /// <summary>Fades out a song</summary>
    /// <param name="angle">fade length</param>
    /// <param name="Stop">Stop song onFadeOut?</param>
    public void music_fadeOut(float angle, bool Stop)
    {
        if (music_singleton() == false){return;}
        StartCoroutine(music_fade(false,Stop,angle));
    }

    /// <summary>Fades in a song</summary>
    /// <param name="angle">fade length</param>
    public void music_fadeIn(float angle)
    {
        if (music_singleton() == false) { return; }
        StartCoroutine(music_fade(true,false,angle));
    }

    /// <summary>Fades in/out current song</summary>
    /// <param name="fadeIn">FadeIn?</param>
    /// <param name="Stop">Stop onFadeOut?</param>
    /// <param name="angle">length of fade</param>
    /// <returns>Yield/Fade effect</returns>
    private IEnumerator music_fade(bool fadeIn, bool Stop, float angle)
    {
        if (music_singleton() == false) { yield break; }
        float timer = 0f;   //UnityEngine.Time.deltaTime timer
        float T = 0f;       //LerpT factor
        float volume = 1f;  //New volume to apply for fade

        //While timer is running
        while (timer < angle)
        {
            timer += UnityEngine.Time.deltaTime;    //Increase timer
            T = (timer / angle);                    //Get LerpT factor (timer/angle fade length)
            if (fadeIn)
            {
                //Lerp volume from 0 to 1 if fadein
                volume = Mathf.Lerp(0f, 1f, T);     
            }
            else
            {
                //Lerp volume from 1 to 0 if fadeOut
                volume = Mathf.Lerp(1f, 0f, T);
            }
            audio_bgm.volume = volume;              //Set new volume
            yield return new WaitForSeconds(.01f);   //Safety NOP
        }     

        if(!fadeIn)
        {
            //If fading out and stop flag, stop song
            if(Stop){music_stop();}
        }
        yield break;
    }


    /// <summary>Plays song via ID</summary>
    /// <param name="tune">BGM enum</param>
    public void music_play(BGM tune)
    {
        //This and music_stop are the only funcs that don't check for singleton instance, due to it setting it
        if (instance!=null){instance.music_stop();}
        instance = this;                                        //Set music instance to this Audio script
        currentSong = tune;                                     //Set currentSong to this tune
        byte index = (byte)(currentSong);                       //Get index of currentSong
        audio_bgm.clip = getClip(currentSong);
        audio_bgm.pitch = 1.0f;                                 //Set pitch to 1.0f
        
        loopStart = BGMs[index].start;                          //Get the song's loopStart cuept
        loopEnd = BGMs[index].end;                              //Get the song's loopEnd cuept
        bool loop = (loopStart != 1f && loopEnd != 1f);         //Determine loop
        audio_bgm.loop = loop;                                  //Set loop appropriately
        audio_bgm.volume = 1f;                                  //Set the volume
        audio_bgm.Play();                                       //Play it                
        //Debug.Log("Now playing song " + song);
    }

    /// <summary>Stops currently playing song</summary>
    public void music_stop()
    {
        //This and music_stop are the only funcs that don't check for singleton instance, due to it setting it
        audio_bgm.Stop();
    }

    /// <summary>Pauses currently play song</summary>
    public void music_pause()
    {
        if (music_singleton() == false) { return; }
        audio_bgm.Pause();
    }

    /// <summary>Resumes currently paused song</summary>
    public void music_unpause()
    {
        if (music_singleton() == false) { return; }
        audio_bgm.UnPause();
    }

    /// <summary>
    /// Returns if the audio script attempting to use music functions is the designated singleton for bgm.
    /// There really only should be one Audio script doing bgm stuff; use the result to prevent bgm funcs on non-singleton
    /// </summary>
    /// <returns>This audio script == singleton?</returns>
    public bool music_singleton()
    {
        bool singleton = (this == Audio.instance);
        if (!singleton)
        {
            Debug.LogError("Attempted to handle music on non-singleton!");
        }
        else
        {
            singleton = (audio_bgm != null);
            if (!singleton)
            {
                Debug.LogError("Attempted to handle music withou audio_bgm ref on singleton!");
            }
        }
        return singleton;
    }

    /// <summary>Returns an array of AudioMixers</summary>
    /// <param name="audioMixer">AM type</param>
    /// <returns>Array of MixerGroups</returns>
    public UnityEngine.Audio.AudioMixerGroup[] AllMixerGroups(UnityEngine.Audio.AudioMixer audioMixer)
    {
        UnityEngine.Audio.AudioMixerGroup[] AMG=audioMixer.FindMatchingGroups(string.Empty);
        return AMG;
    }

    /// <summary>Returns an array of AudioMixers</summary>
    /// <param name="audioMixer">AudioMixer</param>
    /// <param name="name">Name</param>
    /// <returns>Array of MixerGroups</returns>
    public UnityEngine.Audio.AudioMixerGroup[] AllMixerGroups(UnityEngine.Audio.AudioMixer audioMixer, string name)
    {
        UnityEngine.Audio.AudioMixerGroup[] AMG = audioMixer.FindMatchingGroups(name);
        return AMG;
    }

    /// <summary>Returns an AudioMixer by name</summary>
    /// <param name="audioMixer">AudioMixer</param>
    /// <param name="name">Name</param>
    /// <returns>AudioMixer</returns>
    public UnityEngine.Audio.AudioMixerGroup MixerGroup(UnityEngine.Audio.AudioMixer audioMixer, string name)
    {
        UnityEngine.Audio.AudioMixerGroup[] AMG = audioMixer.FindMatchingGroups(name);
        return AMG[0];
    }
}