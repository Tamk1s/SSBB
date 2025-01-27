using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public const byte maxPlayers = (byte)(ControlledBy.MAX) + 0x01;
    public BallController[] balls = new BallController[maxPlayers];

    public Audio music;
    public Image[] winner = new Image[maxPlayers];
    public GameObject reset;
    public BoundsManager BM;
    public LoadScene LS;

    public TextMeshProUGUI[] air = new TextMeshProUGUI[maxPlayers];
    public TextMeshProUGUI timer;
    public bool ready = false;

    public void Start()
    {
        StartCoroutine(_start());
    }

    private IEnumerator _start()
    {
        yield return new WaitForSeconds(.1f);
        music.music_play(Audio.BGM.BGM_BATTLE);
        ready = true;
    }

    public void Update()
    {
        if (ready)
        {
            UpdateTimer();
            UpdateAir();
            CheckForGameover();
        }
    }

    public void UpdateTimer()
    {
        //https://stackoverflow.com/a/40867297
        const string format = "{0}:{1:00.000}";
        Color clrStart = Color.green;
        Color clrEnd = Color.red;

        float time = BM.GetTimer_Countdown();
        object a = (int)(time / 60);
        object b = time % 60;
        string result = string.Format(CultureInfo.InvariantCulture, format, a, b);
        timer.text = result;

        float t = BM.GetTimer_Percent();
        Color clr = Color.Lerp(clrStart, clrEnd, t);
        timer.color = clr;

        MusicTempo();
    }

    private void MusicTempo()
    {
        const float fastMusic_percent = (1f / 2f);
        const float fastMusic_tempoStart = 1f;
        const float fastMusic_tempoEnd = 3f;

        float time = BM.GetTimer_Percent();
        float limit = (1f - fastMusic_percent);
        bool fast = (time > limit);
        if(fast)
        {
            time -= limit;
            float t = (time / limit);
            float tempo = Mathf.Lerp(fastMusic_tempoStart, fastMusic_tempoEnd, t);
            music.music_changePitch(tempo);
        }
    }

    public void CheckForGameover()
    {
        const byte p1 = (byte)(ControlledBy.PLAYER1);
        const byte p2 = (byte)(ControlledBy.PLAYER2);
        bool p1ded = balls[p1].isDead;
        bool p2ded = balls[p2].isDead;

        bool gameover = (p1ded || p2ded);
        if (gameover)
        {
            ready = false;
            BM.ToggleReady(false);
            music.music_play(Audio.BGM.BGM_VICTORY);
            music.music_changePitch(1f);
            if (p1ded)
            {
                balls[p2].ToggleDeath(true, false);
                ToggleWinner(p2);
            }
            else if (p2ded)
            {
                balls[p1].ToggleDeath(true, false);
                ToggleWinner(p1);
            }
            //reset.gameObject.SetActive(true);
            LS.LoadMainMenuDelayed(5f);            
        }
    }

    private void UpdateAir()
    {
        const string crlf = "\n";
        const string P1 = "P1 Air:" + crlf;
        const string P2 = "P2 Air:" + crlf;
        const string formatter = "P2";
        const byte maxByte = 0xFF;
        const byte alpha = 0xC0;
        const byte p1 = (byte)(ControlledBy.PLAYER1);
        const byte p2 = (byte)(ControlledBy.PLAYER2);
        Color32 clr = new Color32(maxByte, maxByte, maxByte, maxByte);

        float val = balls[p1].air.currentAir;
        clr = balls[p1].air.ColorLerp(val);
        balls[p1].animator.Change_ModelColor(clr);
        clr = clr.changeAlpha(alpha);
        val /= 100f;
        air[p1].text = P1 + val.ToString(formatter);
        air[p1].color = clr;

        val = balls[p2].air.currentAir;
        clr = balls[p2].air.ColorLerp(val);
        balls[p2].animator.Change_ModelColor(clr);
        clr = clr.changeAlpha(alpha);
        val /= 100f;
        air[p2].text = P2 + val.ToString(formatter);
        air[p2].color = clr;
    }

    private void ToggleWinner(byte index)
    {
        balls[index].Audio.sfx_play(Audio.SFX.SFX_VICTORY_PLAYER);
        balls[index].animator.SetVictory();        
        winner[index].gameObject.SetActive(true);
    }
}