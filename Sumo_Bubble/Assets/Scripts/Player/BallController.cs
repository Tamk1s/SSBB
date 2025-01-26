using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region Variables
    public PlayerControls controls;
    public ControlledBy controlledBy;                           //Who controls this player?
    public PositionTween positionTween;                         //Generic positionTween. Probably dead Coffee code
    public BallPhysics physics;
    public BubbleAir air;

    [HideInInspector]
    public bool isPumping;                  //Is player pumping?
    [HideInInspector]
    public bool isBoosting;                 //Is player boosting?
    [HideInInspector]
    public bool isDead;                     //Is player pumping?
    [HideInInspector]
    private bool isWinner;                  //Victory?

    //get components
    public BallAnimator animator;                  //Animator for sprite
    public Audio Audio;                        //Audio script for sfx
    public Rigidbody body;

    //Misc state mgmt
    public bool isLocked = false;                                       //Locked movement controls?
    public bool ready = false;                                          //Init/ready semaphore
    #endregion

    #region StdUnityEvents
    public IEnumerator Start()
    {
        yield return new WaitForSeconds(.1f);                           //Wait for Bootstrap to update GameState!

        //Get stuff here
        controls = this.gameObject.AddComponent<PlayerControls>();

        //Good screens for player controls (LevelScreen, Title Card, CutScene screen)        
        /*
        byte index = 0;
        controls.relativeScreens = new List<MenuScreen>();
        try
        {
            index = (byte)(MainMenuController.GameMenus.TITLECARD); controls.relativeScreens.Add(MainMenuController.instance.menu[index]);
            index = (byte)(MainMenuController.GameMenus.LVL); controls.relativeScreens.Add(MainMenuController.instance.menu[index]);
            index = (byte)(MainMenuController.GameMenus.CUTSCENE); controls.relativeScreens.Add(MainMenuController.instance.menu[index]);
        }
        catch
        {
            Debug.LogError("Init of CCSE.controls.relativeScreens failed! NRE of standard MMC setup?");
        }
        */

        //If a PLAYER playerType, loadPlayerSetup for contgrols and set controlledBy to player1
        if (controlledBy == ControlledBy.PLAYER1)
        {
            controls.LoadPlayer1Setup();
        }
        else if (controlledBy == ControlledBy.PLAYER2)
        {
            controls.LoadPlayer2Setup();
        }
        Toggle_Ctrl_CawBacks(true, onBack, onStart);
        ready = true;   //We are ready for update/whatever threads
    }

    public void OnCollisionEnter(Collision collision)
    {
        onHit(collision.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        onHit(other.gameObject);
    }

    private void onHit(GameObject other)
    {
        if (isDead){return;}

        int layer = other.layer;
        switch (layer)
        {
            case (int)(CameraFollow.Layer.LYR_BALL):
                DoHurt(air.hurtLossPerSecond, BubbleAir.hurtType.HT_BALL, Audio.SFX.SFX_HIT_PLAYER);
                Debug.Log("Ball collision");
                break;

            case (int)(CameraFollow.Layer.LYR_DEATHWALL):
                DoHurt(1000f, BubbleAir.hurtType.HT_OBSTACLE, Audio.SFX.SFX_HIT_DEATH);
                Debug.Log("DeathWall");
                break;

            case (int)(CameraFollow.Layer.LYR_HURT):
                DoHurt(air.spikeLossPerSecond, BubbleAir.hurtType.HT_OBSTACLE, Audio.SFX.SFX_HIT_SPIKES);
                Debug.Log("Obstacle collide");
                break;
        }
    }
    #endregion

    #region Functions

    public void DoHurt(float hurt, BubbleAir.hurtType hurtType, Audio.SFX clip)
    {
        air.DoBlow_Hurt(hurt, hurtType, clip);
    }

    public void ToggleDeath(bool state)
    {
        isDead = state;
        if (state)
        {
            Audio.sfx_play(Audio.SFX.SFX_DEATH_PLAYER);
            Toggle_Ctrl_CawBacks(false, NOP, NOP);
            air.ToggleReady(false);
            physics.ToggleReady(false);
            animator.SetDead();
        }
    }

    public void Toggle_Ctrl_CawBacks(bool state, System.Action back, System.Action start)
    {
        //sign up for events
        if (state)
        {
            controls.MoveAxis = onMove;        //x
            controls.onA = onBoostDown;
            controls.onAHeld = onBoostHeld;
            controls.onAReleased = onBoostUp;
            controls.onX = onPump;
        }
        else
        {
            controls.MoveAxis = NOP;
            controls.onA = NOP;
            controls.onAHeld = NOP;
            controls.onAReleased = NOP;
            controls.onX = NOP;
        }
        controls.onBack = back;
        controls.onStart = start;
    }
    #endregion

    #region ControlCawbacks
    public void NOP()
    {
        //NOP!
    }

    public void NOP(Vector2 axis)
    {
        NOP();
    }

    public void onMove(Vector2 axis)
    {
        physics.SetDirectionalInput(axis);
        animator.SetMovement_Anim(axis);
    }

    public void onBoostDown()
    {
        isBoosting = true;
        air.DoBlow();
        animator.SetBoost(true);
    }

    public void onBoostUp()
    {
        isBoosting = false;
        air.StopBlow();
        animator.SetBoost(false);
    }

    public void onBoostHeld()
    {
        onBoostDown();
    }

    public void onPump()
    {
        bool p = air.PumpUp();
        if (p)
        {
            bool inf = animator.GetInflating();
            if (!inf)
            {
                animator.SetInflate();
                animator.SetInflating(true);
            }
        }
    }

    public void onBack()
    {
        DebugReset();
    }

    private void DebugReset()
    {
        GameManager GM = GameManager.instance;        
        bool good = (GM != null);
        if (good)
        {
            LoadScene LS = GM.LS;
            good = (LS != null);
            if (good){LS.LoadMainMenuDelayed(0f);}
        }
    }

    public void onStart()
    {
        Debug.Log("onStart called");
    }
    #endregion
}