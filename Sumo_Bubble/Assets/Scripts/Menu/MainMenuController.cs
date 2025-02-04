﻿using System.Collections;
using System.Collections.Generic;
using Rewired;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the main menu
/// </summary>
public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;  //Singleton instance
    public TextSwitch PTS;                      //Instance of the controls TextSwitch in the Options menu

    /// <summary>
    /// Hardcoded IDs for in-game MMC menus
    /// !@ Update me for project-apricorn!
    /// </summary>
    public enum GameMenus
    {
        LVL,            //0x00
        PAUSE,          //0x01
        BATTLE_MAIN,    //0x02
        BATTLE_WEAPON,
        QUEST_REWARD,   //0x04
        BATTLE_ITEM,
        BATTLE_SPECIAL, //0x06
        CUTSCENE,       //0x07
        STATUS,
        VICTORY,
        GAMEOVER,
        CUTSCENE_OPTION,//0x0B
        MAX = CUTSCENE_OPTION,
    }

    //Hardcoded indices for Options' control TextSwitch menu item
    //!@ UPdate me for project apricorn!
    private const byte Kbd = 0;                  //Kbd index
    private const byte GPad = 1;                 //Gamepad index for Controls Sprite TextSwitch in Options menu
    private const byte Flightpad = 2;            //Flighstick index for Controls Sprite TextSwitch in Options menu
    
    //public ControlsSwitch CS;                 //Control switch script, for forcing player 1 to be controlled by joypad if one is plugged in
    //private bool CSrunonce = false;
    public List<MenuScreen> menu;               //List of Menuscreens for the menu

    //[HideInInspector]
    public MenuScreen currentScreen;            //Current MenuScreen
    //[HideInInspector]
    public int currentScreenIndex;              //Iindex of currentScreen
    //[HideInInspector]
    public MenuScreen previous;                 //Previous MenuScreen

    /// <summary>Setup singleton instnace</summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Handles toggling gamepad menu activator in options and disables buttons in the gamepad menus if disconnect
    /// </summary>
    void Update()
    {
        /*
        //If current scene Main Menu?
        string scenename = Application.loadedLevelName;
        bool isMM = (scenename == "Main Menu");
        if (isMM==true)
        {
            //!@ UPdate me for project apricorn!
            //Handle toggling options' available control menus
            if (PTS != null){HandlePerph();}
        }
        */
    }

    /*
    /// <summary>Handles toggling 1P index in Controls TS in Options menu</summary>
    public void HandlePerph()
    {
        bool[,] types = new bool[1, 1];                    //Dummy ByRef value result for FindTypesOfGamePad
        ControlsSetup.FindTypesOfGamePads(0, ref types);   //Return all ByRef types of joysticks 

        //Are any of those types a kbd?
        bool kbd = ControlsSetup.FindTypesAnyWithType(types, RewiredConsts.Layout.Joystick.Types.KBD);

        //Check if any joysticks from result are of flighstick (HOTAS template or ST290 Pro)
        bool flight = ((ControlsSetup.FindTypesAnyWithType(types, RewiredConsts.Layout.Joystick.Types.T_HOTAS))
                    || (ControlsSetup.FindTypesAnyWithType(types, RewiredConsts.Layout.Joystick.Types.ST290P)));

        //Are any of the joysticks just generic valid gamepads?
        bool gpad = false;
        if (!flight)
        {
            //If not flight

            //Check if any joysticks from result are of XInput or remappable (XInput, Unknown, or Dual analog template)
            gpad = (ControlsSetup.FindTypesAnyWithType(types, RewiredConsts.Layout.Joystick.Types.T_DUAL_ANALOG)
                    || (ControlsSetup.FindTypesAnyWithType(types, RewiredConsts.Layout.Joystick.Types.XINPUT))
                    || (ControlsSetup.FindTypesAnyWithType(types, RewiredConsts.Layout.Joystick.Types.UNKNOWN)));
        }

        //Toggle Kbd, GPad, and Flightpad indices of options' controls TS appropriately
        ToggleOptionsControlsTS(kbd, Kbd);
        ToggleOptionsControlsTS(gpad, GPad);
        ToggleOptionsControlsTS(flight, Flightpad);
    }
    */

    /// <summary>Toggles an options controls TS index by state</summary>
    /// <param name="state">State of index</param>
    /// <param name="index">Index</param>
    private void ToggleOptionsControlsTS(bool state, byte index)
    {
        //If enable, then set image state; else disable
        if (state)
        {PTS.SetImgState(index);}
        else{PTS.ResetImgState(index);}
    }

    /// <summary>Sets the currentScreen</summary>
    /// <param name="screen">New MenuScreen</param>
    public void SetCurrentScreen(MenuScreen screen)
    {
        //Cache current screen as previous, update current screen, then update currentscreen index
        previous = currentScreen;
        currentScreen = screen;
        currentScreenIndex = menu.IndexOf(screen);
    }

    /// <summary>Sets the currentScreen</summary>
    /// <param name="index">New index</param>
    public void SetCurrentScreen(int index)
    {
        //Update current screen index; cache previous screen, then open new screen
        currentScreenIndex = index;
        previous = currentScreen;
        currentScreen = menu[index];
    }

    /// <summary>Closes the current screen</summary>
    public void CurrentScreenClose()
    {
        //if currentScreen exists, then close it
        if (currentScreen){currentScreen.Close();}
    }

    /// <summary>Opens a new screen</summary>
    /// <param name="index">Index</param>
    public void OpenScreen(int index)
    {
        SetCurrentScreen(index);
        currentScreen.Open();
    }

    /// <summary>Opens a new screen</summary>
    /// <param name="screen">Menuscreen</param>
    public void OpenScreen(MenuScreen screen)
    {
        SetCurrentScreen(screen);
        currentScreen.Open();
    }

    /// <summary>Closes a screen</summary>
    /// <param name="index">Index</param>
    public void CloseScreen(int index)
    {
        MenuScreen screen = menu[index];
        if (screen){CloseScreen(screen);}
    }

    /// <summary>Closes a screen</summary>
    /// <param name="screen">Menuscreen</param>
    public void CloseScreen(MenuScreen screen)
    {
        screen.Close();
    }

    /// <summary>Opens previous screen</summary>
    public void OpenPrev()
    {
        SetCurrentScreen(previous);
        currentScreen.Open();
    }

    /// <summary>Set current screen to previous</summary>
    public void SetPrev()
    {
        SetCurrentScreen(previous);
    }

    /// <summary>Opens next screen in list</summary>
    public void Forward()
    {
        if(currentScreenIndex < (menu.Count - 1))
        {
            CurrentScreenClose();
            OpenScreen(currentScreenIndex + 1);
        }
    }

    /// <summary>Opens previuos sccreen in list</summary>
    public void Back()
    {
        if (currentScreenIndex > 0)
        {
            CurrentScreenClose();
            OpenScreen(currentScreenIndex - 1);
        }
    }

    /// <summary>Quits the application</summary>
    public void CloseApp()
    {
        GameManager.instance.OnApplicationQuit();
    }    
}
