using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using UnityEngine;

using Rewired;
using TMPro;

/// <summary>Global GameManager singleton. MISSION-CRITICAL!</summary>
public class GameManager : MonoBehaviour
{
    //Singleton instance
    public static GameManager instance;
    /// <summary>Debug mode</summary>
    public bool DEBUG = true;

    //[Header("Singleton comp refs")]
    //!@SNEEP

    [Header("Audio")]
    //Audio stuff
    /// <summary>Global Audio script</summary>
    //public Audio audio;
    /// <summary>BGM Audio Mixer</summary>
    public UnityEngine.Audio.AudioMixer AM_BGM;
    /// <summary>SFX Audio Mixer</summary>
    public UnityEngine.Audio.AudioMixer AM_SFX;     
    //More sfx stuff
    /// <summary>Music volume</summary>
    public float mvol = 1f;
    /// <summary>Min and max DB range for BGM</summary>
    private float[] bgm_rng = new float[2] { -80f, 0f };
    /// <summary>Sound volume</summary>
    public float svol = 1f;
    /// <summary>Min and max DB range for SFX</summary>
    private float[] sfx_rng = new float[2] { -80f, 0f };       

    [Header("Control")]
    /// <summary>Scene Name</summary>
    public string SceneName;
    //Important global prefabs/data
    /// <summary>Allow cursor?</summary>
    public bool cursor = true;          
    
    /// <summary>Localization lagnuages</summary>
    public enum Language
    {
        LANG_ENGLISH,
        LANG_SPANISH,
        LANG_FRENCH,
        LANG_CHINESE,
        LANG_RUSSIAN,
        LANG_NONE,
        LANG_MAX= LANG_NONE
    };
    /// <summary>Max amount of languages. Aligned witht he GameManager.Language enum</summary>
    public const byte maxLanguage = (byte)(Language.LANG_MAX);

    [Header("Language/localz")]

    /// <summary>
    /// List of LangPack fonts that require capitalization to work properly. TMPro font assets
    /// https://en.wikipedia.org/wiki/Cyrillic_script_in_Unicode
    /// https://discussions.unity.com/t/creating-asset-from-font-with-a-lot-of-characters/708290/27
    /// https://github.com/notofonts/noto-cjk/blob/main/google-fonts/NotoSansSC%5Bwght%5D.ttf . !@ ADD FONT GPL LICENSE TO CREDITS!!!
    /// </summary>
    [NamedArrayAttribute(typeof(Language))]
    public TMPro.TMP_FontAsset[] CapFonts_TMP = new TMPro.TMP_FontAsset[maxLanguage];
    /// <summary>OnTop variant</summary>
    [NamedArrayAttribute(typeof(Language))]
    public TMPro.TMP_FontAsset[] CapFonts_OnTop_TMP = new TMPro.TMP_FontAsset[maxLanguage];

    [NamedArrayAttribute(typeof(Language))]
    /// <summary>List of LangPack fonts that require capitalization to work properly. Std Font assets</summary>
    public Font[] CapFonts = new Font[maxLanguage];
    /// <summary>Current localization language</summary>
    public Language lang = Language.LANG_ENGLISH;

    //!@ Insert game SAVE stuff here; moerge into new class etc.
    /// <summary>Current Coontrols</summary>
    public static ControlsHolder currentControls;               

    private IEnumerator OnLevelWasLoaded()
    {
        string scene = Application.loadedLevelName;       
        yield return null;
        //!@ Do level loaded stuff here!
    }

    private void Awake()
    {        
        if(instance != null)
        {
            Destroy(gameObject);
            return;            
        }        
        DEBUG = true;   //TOGGLE DEBUG MODE

		//if(SteamManager.Initialized){}

        //If an Audio script DNE, create one
        //if (!audio){audio = this.gameObject.AddComponent<Audio>();}

        //Set the instance, and DDoL
        instance = this;
        DontDestroyOnLoad(gameObject);

        //controls setup
        /*
		ControlsHolder controls = (ControlsHolder)ClassSerializer.Load(Application.persistentDataPath, CONTROLFILE);
        if (controls == null)
        {
        */
        //file dont exists, create default controls handler
        CreateDefaultControls();
        /*
        }
        else
        {
            //Otherwise, set current to save ones in PlayerPrefs file
            currentControls = controls;
        }
        */
        ToggleCursor(true);             //Enable display of cursor
    }

    /// <summary>Toggles mouse cursor</summary>
    /// <param name="state">State</param>
    public void ToggleCursor(bool state)
    {
        cursor = state;
        Cursor.visible = cursor;
    }

    private void Update()
    {
    } 

    /// <summary>When application quits, do this stuff</summary>
    public void OnApplicationQuit()
    {
        Debug.Log("Killing application...");

        //Save Steam stats, shtudown SteamAPI if SteamManager initzd
        /*
        if (SteamManager.Initialized)
        {
            Debug.Log("Saving steamstats...");
            SteamUserStats.StoreStats();
            Debug.Log("Shutting down steamworks");
            SteamAPI.Shutdown();    //Attempt to shutdown Steamworks
        }
        */
        
        //Quit the application
        Application.Quit();
    }

    /// <summary>Creates the default controls for the player</summary>
    public void CreateDefaultControls()
    {
        currentControls = new ControlsHolder(ControlsSetup.DefaultPlayer1Setup());
        //SaveControls();
    }

    /// <summary>Sets a random seed </summary>
    public void RandomSeed()
    {
        int value = UnityEngine.Time.frameCount;
        UnityEngine.Random.InitState(value);
    }
}