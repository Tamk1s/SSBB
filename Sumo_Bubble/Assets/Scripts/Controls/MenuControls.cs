using ByteSheep.Events;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Rewired;

/// <summary>Handles controlling a menu</summary>
public class MenuControls : MonoBehaviour
{    
    //public bool isCutscene = false;             //Is this scene a cutscene?
    //public bool isFNConsole = false;            //Is this the FN console? If so, use period(.) key for onA. (Allows input of spacebar for kbd typing)
    public bool allowInit = false;              //If true, run an even onInit
    public ControlledBy controlledBy;           //Who controls this menu? (Default is PLAYERS.PLAYER1)

    [Tooltip("Optional. Active only when given menu screen is current one")]
    public MenuScreen relativeScreen;
    
    public bool sendConstantly = false;             //If true, send vert/horiz menu movement constantly. Used for accelerating input speed (for NUD menu elements)
    public bool simulatedByKeyboard = false;        //Can this menu be handled by the kbd?

    public AdvancedFloatEvent leftStickHorizontal;  //Events to run on left stick horiz
    public AdvancedFloatEvent leftStickVertical;    //~ left stick vert
    public AdvancedFloatEvent rightStickHorizontal; //Events to run on right stick horiz
    public AdvancedFloatEvent dpadHorizontal;       //~ dpad/kbd horiz
    public AdvancedFloatEvent dpadVertical;         //~ dpad/kbd vert
    public AdvancedFloatEvent cpadHorizontal;       //~ dpad/kbd horiz

    public AdvancedEvent onInit;                    //~ on Initz (if allowInit is true)
    public AdvancedEvent onBack;                    //~ on Back
    public AdvancedEvent onStart;                   //~ on Start 

    public AdvancedEvent onA;                       //~ onA btn
    public AdvancedEvent onB;                       //~ onB btn
    //public AdvancedEvent onX;                     //~ onX btn
    public AdvancedEvent onY;                       //~ onY btn
    public AdvancedEvent onLB;                      //~ on Left bumper btn
    public AdvancedEvent onRB;                      //~ on Right bumper btn
    public AdvancedEvent onEnter;                   //~ on Enter key, non-remappable. Used for kbd remapping
    

    private float restoreTime = .33f;               //Delay timelimit for vert/horiz movement
    private float restoreTimer;                     //Delay timer

    private bool isStickReady = true;               //  Stick       is ready when timer has expired
    //private bool isDPadReady = true;              //~ Dpad        ~
    private bool ready = false;                     //Semaphore for start on script

    IEnumerator Start()
    {
        //Run onInit events if applicable
        if(allowInit == true)
        {
            onInit.Invoke();
            yield return new WaitForSeconds(.1f);
        }

        /*We need to yield some seconds realtime, to prevent race condition of CutSceneController in Cutscenes not getting ready in time.
         * Pausing game and then unpausing would prevent CSC from starting, and need realtime due to pausing
         * (otherwise, PauseScreen would also lock during time freezing)
        */
        yield return new WaitForSecondsRealtime(1f);
        ready = true;   //We are now ready!
    }

    /// <summary>Handles all controls, only if ready</summary>
    void Update()
    {
        if (ready == true)
        {
            //Check if on correct screen
            if (relativeScreen)
            {
                if (MainMenuController.instance)
                {
                    if (MainMenuController.instance.currentScreen != relativeScreen)
                    {return;}
                }
            }

            //Handle restore timer
            //if (restoreTimer < restoreTime && (!isStickReady || !isDPadReady))
            if ((restoreTimer < restoreTime) && (!isStickReady))
            {restoreTimer += Time.unscaledDeltaTime;}
            else
            {
                //isDPadReady = isStickReady = true;
                isStickReady = true;
                restoreTimer = 0f;
            }

            //Handle kbd controls if applicable
            //HandleKbd();

            //Handle gamepad controls
            /*
            if (controlledBy == ControlledBy.ANY_PLAYER)
            {
                for (int i = 0; i < ControlsHandler.instance.devicesCount; i++)
                {
                    InvokeEventsFromPlayer(i);
                }
            }
            else
            {
            */
                InvokeEventsFromPlayer((int)controlledBy);
            //}
        }
    }

    /// <summary>Handles gamepad buttons for menus</summary>
    /// <param name="index">Index of player</param>
    private void InvokeEventsFromPlayer(int index)
    {
        /*
        if (ControlsHandler.instance.inputDevices[index] == null)
        {
            return;
        }

        //Debug.Log("GM_CC_C_Count: " + (GameManager.currentControls.controls.Count-1).ToString());

        if (ControlsHandler.instance.inputDevices[index] == null)
        {
            //Debug.Log("Null device detected! Not running controller code...");
            return;
        }

        //Debug.Log("GM_CC_C_Count: " + (GameManager.currentControls.controls.Count - 1).ToString());
        
        if (index>GameManager.currentControls.controls.Count-1)
        {
            //Debug.Log("Controller index OOB. Not running controller code...");
            return;
        }

        //Gamepad typ
        int StandX360 = 0;  
        StandX360 = GameManager.currentControls.controls[index].joystickType;        

        //Debug.Log("Controller type: "+GameManager.currentControls.controls[index].joystickType.ToString());
        if (StandX360 == 0)
        {
        */
        
        Vector2 axis;
        Vector2 deadAxis;
        Vector2 deadArea;

        int id = -1;        
        float pitch_invert = 1f;
        /*
        bool yawPitch_swap = GameManager.currentControls.controls[index].yawRoll_Swap;          
        bool invert = GameManager.currentControls.controls[index].pitch_invert;
        if (invert)
        {
            pitch_invert = -1f;
        }
        else
        {
            pitch_invert = 1f;
        }
        */
        
        Player player = ReInput.players.GetPlayer(index);
        //ActionElementMap throttle = player.controllers.maps.GetFirstAxisMapWithAction(RewiredConsts.Action.Throttle, true);
        //Handle standard Xbox 360 controller

        //Handle Left stick vert/horiz
        /*
        if (throttle != null)
        {
            if(yawPitch_swap)
            {
                id = RewiredConsts.Action.Roll;
            }
            else
            {
                id = RewiredConsts.Action.Yaw;
            }
        }
        else
        {
        */
            //Override pitch inversion flags on maps with false (no inversion for menus)
            //GameManager.currentControls.controls[index].HandlePitchInvert(true, false);
            id = RewiredConsts.Action.Yaw;
        //}
            axis = player.GetAxis2D(id, RewiredConsts.Action.Pitch);
            deadAxis = RewiredConsts.Action.deadNorm;
            deadArea = ControlsSetup.RW_DeadAxis(axis, deadAxis, 0);        
            //if (ControlsHandler.instance.inputDevices[index].LeftStick.State && (isStickReady || sendConstantly))
            if ((deadArea != Vector2.zero) && (isStickReady || sendConstantly))
            {
                //if (Mathf.Abs(ControlsHandler.instance.inputDevices[index].LeftStick.X) > Math.Abs(ControlsHandler.instance.inputDevices[index].LeftStick.Y))
                //axis.y *= pitch_invert;
                if (Mathf.Abs(axis.x) > Math.Abs(axis.y))
                    //leftStickHorizontal.Invoke(ControlsHandler.instance.inputDevices[index].LeftStick.X);
                    leftStickHorizontal.Invoke(axis.x);
                else
                    //leftStickVertical.Invoke(ControlsHandler.instance.inputDevices[index].LeftStick.Y);
                    leftStickVertical.Invoke(axis.y);
                DisableStick();
            }

            /*
            if (throttle != null)
            {
                if (yawPitch_swap)
                {
                    id = RewiredConsts.Action.Yaw;
                }
                else
                {
                    id = RewiredConsts.Action.Roll;
                }
            }
            else
            {
            */
                id = RewiredConsts.Action.Roll;
            //}
            axis = new Vector2(player.GetAxis(id), 0f);
            deadAxis = RewiredConsts.Action.deadNorm;
            deadArea = ControlsSetup.RW_DeadAxis(axis, deadAxis, -1);
            //if (ControlsHandler.instance.inputDevices[index].RightStick.State && (isStickReady || sendConstantly))
            if ((deadArea != Vector2.zero) && (isStickReady || sendConstantly))
            {
                //if (Mathf.Abs(ControlsHandler.instance.inputDevices[index].RightStick.X) > Math.Abs(ControlsHandler.instance.inputDevices[index].RighStick.Y))
                    //rightStickHorizontal.Invoke(ControlsHandler.instance.inputDevices[index].RightStick.X);
                    rightStickHorizontal.Invoke(axis.x);
                //else
                    //leftStickVertical.Invoke(ControlsHandler.instance.inputDevices[index].LeftStick.Y);

                DisableStick();
            }

            //Handle dpad buttons
            /*
            if (ControlsHandler.instance.inputDevices[index].DPad.Up.WasPressed)
            {
                dpadVertical.Invoke(1f);
                DisableDPad();
            }
            else if (ControlsHandler.instance.inputDevices[index].DPad.Down.WasPressed)
            {
                dpadVertical.Invoke(-1f);
                DisableDPad();
            }
            else if (ControlsHandler.instance.inputDevices[index].DPad.Right.WasPressed)
            {
                dpadHorizontal.Invoke(1f);
                DisableDPad();
            }
            else if (ControlsHandler.instance.inputDevices[index].DPad.Left.WasPressed)
            {
                dpadHorizontal.Invoke(-1f);
                DisableDPad();
            }
            else if (ControlsHandler.instance.inputDevices[index].DPad.State && (isDPadReady || sendConstantly))
            {
                dpadHorizontal.Invoke(ControlsHandler.instance.inputDevices[index].DPad.X);
                dpadVertical.Invoke(ControlsHandler.instance.inputDevices[index].DPad.Y);
                DisableDPad();
            }
            */

            //Handle back button
            //XBone uses view button, insetad of "back"
            //!@ Xbone sux and iDontCare
            //bool back=((ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Back).WasPressed)||(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.View).WasPressed));
            //bool back = ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Back).WasPressed;
            bool back = player.GetButtonDown(RewiredConsts.Action.Back);
            if(back){onBack.Invoke();}

            //Handle start button
            //!@ Ditto with Xbone using Menu istead of Start
            bool doStart = false;
            //doStart = ((ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Start).WasPressed) || (ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Menu).WasPressed));
            //doStart = ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Start).WasPressed;
            doStart = player.GetButtonDown(RewiredConsts.Action.Start);
            if(doStart){onStart.Invoke();}

            //Handle A button
            //if (ControlsHandler.instance.inputDevices[index].Action1.WasPressed)
            if(player.GetButtonDown(RewiredConsts.Action.Shoot))
                onA.Invoke();

            //Handle B button
            //if (ControlsHandler.instance.inputDevices[index].Action2.WasPressed)
            if (player.GetButtonDown(RewiredConsts.Action.Special))
                onB.Invoke();

            //if (ControlsHandler.instance.inputDevices[index].Action3.WasPressed)
            //    StartCoroutine(InvokeUnityEvent(onX));

            //Handle Y button
            //if (ControlsHandler.instance.inputDevices[index].Action4.WasPressed)
            if (player.GetButtonDown(RewiredConsts.Action.UTurn))
                onY.Invoke();

            //Handle RB button
            //if (ControlsHandler.instance.inputDevices[index].LeftBumper.WasPressed)
            if (player.GetButtonDown(RewiredConsts.Action.BRollM))
            {
                onLB.Invoke();
            }
            //else if (ControlsHandler.instance.inputDevices[index].RightBumper.WasPressed)
            else if (player.GetButtonDown(RewiredConsts.Action.BRollP))
                onRB.Invoke();
    /*
    }
        else
        {
            //3rd-party controller.
            //Must be dual-joystick, and have 9 buttons to qualify:
            //4 for ABXY
            //2 for LB/RB
            //1 for DirStick button
            //2 for Start/Back

            //if (ControlsHandler.instance.inputDevices[index].LeftStick.State && (isStickReady || sendConstantly))

            //Handle dirstick/dpad
            if (((Mathf.Abs(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog0).Value) > 0f) || (Mathf.Abs(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog1).Value) > 0f)) && (isStickReady || sendConstantly))
            {
                //if (Mathf.Abs(ControlsHandler.instance.inputDevices[index].LeftStick.X) > Math.Abs(ControlsHandler.instance.inputDevices[index].LeftStick.Y))
                if (Mathf.Abs(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog0).Value) > Mathf.Abs(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog1).Value))
                {
                    leftStickHorizontal.Invoke(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog0).Value);
                }
                else
                {
                    leftStickVertical.Invoke(-(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog1).Value));
                }
                DisableStick();
            }

            //!@
            if ((Mathf.Abs(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog2).Value) > 0f) && (isStickReady || sendConstantly))
            {
                rightStickHorizontal.Invoke(ControlsHandler.instance.inputDevices[index].GetControl(InControl.InputControlType.Analog2).Value);
            }

            //if (ControlsHandler.instance.inputDevices[index].DPad.Up.WasPressed)
            //{
            //    dpadVertical.Invoke(1f);
            //   DisableDPad();
            //}
            //else if (ControlsHandler.instance.inputDevices[index].DPad.Down.WasPressed)
            //{
            //    dpadVertical.Invoke(-1f);
            //    DisableDPad();
            //}
            //else if (ControlsHandler.instance.inputDevices[index].DPad.Right.WasPressed)
            //{
            //    dpadHorizontal.Invoke(1f);
            //    DisableDPad();
            //}
            //else if (ControlsHandler.instance.inputDevices[index].DPad.Left.WasPressed)
            //{
            //    dpadHorizontal.Invoke(-1f);
            //    DisableDPad();
            //}
            //else if (ControlsHandler.instance.inputDevices[index].DPad.State && (isDPadReady || sendConstantly))
            //{
            //    dpadHorizontal.Invoke(ControlsHandler.instance.inputDevices[index].DPad.X);
            //    dpadVertical.Invoke(ControlsHandler.instance.inputDevices[index].DPad.Y);
            //    DisableDPad();
            //}

            //Handle back button
            InControl.InputControlType testval;
            testval = GameManager.currentControls.controls[index].backBtn;            
            if (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed)
                onBack.Invoke();

            //Start button
            testval = GameManager.currentControls.controls[index].startBtn;
            bool doStart = false;
            doStart = (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed);
            if(doStart)
            {
                onStart.Invoke();
            }

            //A button
            testval = GameManager.currentControls.controls[index].aBtn;
            if (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed)
                onA.Invoke();

            //B button
            testval = GameManager.currentControls.controls[index].bBtn;
            if (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed)
                onB.Invoke();

            //testval = GameManager.currentControls.controls[index].xBtn;
            //if (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed)
            //    StartCoroutine(InvokeUnityEvent(onX));

            //Y button
            testval = GameManager.currentControls.controls[index].yBtn;
            if (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed)
                onY.Invoke();

            //RB Button
            testval = GameManager.currentControls.controls[index].LBBtn;
            if (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed)
            {
                onLB.Invoke();
            }
            else
            {
                testval = GameManager.currentControls.controls[index].RBBtn;
                if (ControlsHandler.instance.inputDevices[index].GetControl(testval).WasPressed)
                {
                    onRB.Invoke();
                }
            }
        }
        */
    }

    /*
    /// <summary>
    /// Disables the DPad controls
    /// </summary>
    /// <param name="force">Force it disabled?</param>
    private void DisableDPad(bool force = false)
    {
        //If not sending constantly or force it disabled
        if (!sendConstantly || force)
        {
            //Disable dpad, reset timer
            isDPadReady = false;
            restoreTimer = 0f;
        }
    }
    */

    /// <summary>Disables the dirstick</summary>
    /// <param name="force">Force it disabled?</param>
    private void DisableStick(bool force = false)
    {
        //If not sending constantly or force it disabled
        if (!sendConstantly || force)
        {
            //Disable dirstick, reset timer
            isStickReady = false;
            restoreTimer = 0f;
        }
    }

    /// <summary>Toggles SendConstantly state. Used by NUD element dir speed up</summary>
    public void ToggleSendConstantly()
    {
        sendConstantly = !sendConstantly;
    }
}