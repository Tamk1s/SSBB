using System.Collections;
using System.Collections.Generic;
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
            CheckForGameover();
            UpdateAir();
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
            BM.ToggleReady(false);
            music.music_play(Audio.BGM.BGM_VICTORY);            
            if (p1ded)
            {
                ToggleWinner(p2);
            }
            else if(p2ded)
            {
                ToggleWinner(p1);
            }
            //reset.gameObject.SetActive(true);
            LS.LoadMainMenuDelayed(5f);
            ready = false;
        }
    }

    private void UpdateAir()
    {
        const byte p1 = (byte)(ControlledBy.PLAYER1);
        const byte p2 = (byte)(ControlledBy.PLAYER2);
        float val = balls[p1].air.currentAir;
        val /= 100f;
        air[p1].text = val.ToString("P2");
        val = balls[p2].air.currentAir;
        val /= 100f;
        air[p2].text = val.ToString("P2");
    }

    private void ToggleWinner(byte index)
    {
        winner[index].gameObject.SetActive(true);
    }
}