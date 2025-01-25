using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public PlayerControls controls;
    public ControlledBy controlledBy;                           //Who controls this player?
    public PositionTween positionTween;                         //Generic positionTween. Probably dead Coffee code

    [HideInInspector]public bool isPumping;                     //Is player pumping?
    [HideInInspector] public bool isBoosting;                   //Is player boosting?

    //get components
    [HideInInspector]public Animator animator;                  //Animator for sprite
    [HideInInspector]public Audio audio;                        //Audio script for sfx
    [HideInInspector]public Rigidbody body;                    

    //Misc state mgmt
    public bool isLocked = false;                                       //Locked movement controls?
    public bool ready = false;                                          //Init/ready semaphore

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(.1f);                           //Wait for Bootstrap to update GameState!

        //Get stuff here
        animator = GetComponentInChildren<Animator>();
        audio = this.GetComponent<Audio>();
        body = this.GetComponent<Rigidbody>();
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

        //sign up for events
        controls.MoveAxis += onMove;
        controls.onA += onBoostDown;
        controls.onAHeld += onBoostHeld;
        controls.onAReleased += onBoostUp;

        controls.onX += onPump;

        controls.onBack += onBack;
        controls.onStart += onStart;
        ready = true;   //We are ready for update/whatever threads
    }

    public void Update()
    {

    }

    public void onMove(Vector2 axis)
    {
        Debug.Log("onMove:  " + axis.ToString());
    }

    public void onBoostDown()
    {
        Debug.Log("onBoostDown called");
    }

    public void onBoostUp()
    {
        Debug.Log("onBoostUp called");
    }

    public void onBoostHeld()
    {
        Debug.Log("onBoostHeld called");
    }

    public void onPump()
    {
        Debug.Log("onPump called");
    }

    public void onBack()
    {
        Debug.Log("onBack called");
    }

    public void onStart()
    {
        Debug.Log("onStart called");
    }
}