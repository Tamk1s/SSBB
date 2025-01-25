﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.Data;
using Rewired.Utils.Libraries.TinyJson;

/// <summary>Formerly handled setting up controls, just stores some global settings, constants, and game-specific Rewired functions</summary>
[Serializable]
public class ControlsSetup
{
    public ControlledBy player;                                                                     //Who controls this device?
    //public int joystickId;                      //Joystick ID for char
    //public int joystickType;                    //Joystick type (0=X360, 1=3rd party)

    private const string xinput = "XInput";                 //String for XInput types of controllers
    private const string xbox = "XBOX";                     //String for XBox controllers

    /// <summary>Returns the default controls for player 1</summary>
    /// <returns>ControlsSetup</returns>
    public static ControlsSetup DefaultPlayer1Setup()
    {
        return new ControlsSetup()
        {
            player = ControlledBy.PLAYER1,
        };
    }

    /// <summary>Returns the area of axis(es) releative to a a deadzone</summary>
    /// <param name="axis">Analog axis(es) to check</param>
    /// <param name="deadArea">DeadZone range. .x=min, .y=max</param>
    /// <param name="ax">Which axis to check. -1=x, 1=y, 0=both</param>
    /// <returns>Vector2. Components: .x=area for x axis, .y=area of yaxis. Values: -1=below deadzone min, 1=above deadzone max, 0=in deadzone</returns>
    public static Vector2 RW_DeadAxis(Vector2 axis, Vector2 deadArea, sbyte ax)
    {   
        Vector2 area = Vector2.zero;    //Result to return
        float val = 0f;                 //Axis value to use

        //If just checking x-axis, just set val to xaxis value; else to y-axis if just y
        if (ax == -1)
        {
            val = axis.x;
        }
        else if (ax == 1)
        {
            val = axis.y;
        }

        if(ax!=0)
        {
            //If not checking both axises
            val = GetZone(deadArea, val);   //Get the zone for the approriate axis

            //Shove the zone into the appropriate vector component, set other to 0f (duh)
            if (ax == -1)
            {
                area = new Vector2(val, 0f);
            }
            else
            {
                area = new Vector2(0f, val);
            }
        }
        else
        {
            //Get zones of each axis, shove into area result
            area = new Vector2(GetZone(deadArea, axis.x), GetZone(deadArea, axis.y));
        }
        return area;
    }

    /// <summary>Actually gets the zone of an axis vs. a deadzone area</summary>
    /// <param name="deadArea">DeadZone range. .x=min, .y=max</param>
    /// <param name="val">Axis value to check</param>
    /// <returns>Float. -1=below deadzone min, 1=above deadzone max, 0=in deadzone</returns>
    private static float GetZone(Vector2 deadArea, float val)
    {
        float result = 0f;
        if ((val >= deadArea.x) && (val <= deadArea.y))
        {
            result = 0f;
        }
        else if (val < deadArea.x)
        {
            result = -1;
        }
        else if (val > deadArea.y)
        {
            result = 1;
        }
        return result;
    }

    /// <summary>Given the playerID, determines the template/types of peripherals supported by the game foreach controller Type</summary>
    /// <param name="playerID">PlayerID to check controllers</param>
    /// <param name="types">bool[# of controllers,maxOf(RewiredConsts.Layout.JoyStick.Types)] byref result</param>
    public static void FindTypesOfGamePads(int playerID, ref bool[,] types)
    {        
        Player player = ReInput.players.GetPlayer(playerID);   //Get player(playerID)
        if (player!=null)
        {
            //if player

            sbyte maxCtrls = -1;                                                    //Max amount of controllers currently for player 0
            sbyte maxTypes = (byte)(RewiredConsts.Layout.Joystick.Types.MAX)+0x01;  //Max amount of enum types supported by the game

            IEnumerable<Controller> ContCol = player.controllers.Controllers;
            //Iterate through all controllers for player playerID, get count as 1-based
            foreach (Controller c in ContCol)
            {
                maxCtrls++;
            }
            maxCtrls++;

            //Redim bool array
            types = new bool[maxCtrls + 1, maxTypes];

            //If no controllers found, return
            if(maxCtrls == 0){return;}

            byte ID = 0x00;     //ID of controller in enumerable array
            byte index = 0x00;  //Generic byte index for RewiredConsts.Layout.Joystick.Types enums

            //Iterate through all controllers for playerID
            foreach (Controller c in ContCol)
            {
                //Dummy ByRef param of array of RewiredConsts.Layout.Joystick.Types
                bool[] T = new bool[1];

                //Return ByRef all types in array T for this gamepad
                FindTypesOfGamePad(c, ref T);
                

                byte b = 0x00;  //Index of this bool t
                //Iterate through all Types in T
                foreach (bool t in T)
                {
                    types[ID, b] = t;   //Set index to t flag
                    b++;                //increment index
                }

                //Increment ID for next iteration
                ID++;
            }
        }
    }

    /// <summary>Returns the templates/types for a particular controller</summary>
    /// <param name="c">Controller to check</param>
    /// <param name="types">ByRef return of types it implements</param>
    public static void FindTypesOfGamePad(Controller c, ref bool[] types)
    {
        //bool array of types
        byte index = 0;
        types = new bool[(byte)(RewiredConsts.Layout.Joystick.Types.MAX) + 1];
        
        //If a controller got disconnected, skip
        if (c.isConnected)
        {
            if (c.type == ControllerType.Keyboard)
            {
                index = (byte)(RewiredConsts.Layout.Joystick.Types.KBD); types[index] = true;
            }
            else if (c.type == ControllerType.Mouse)
            {
                index = (byte)(RewiredConsts.Layout.Joystick.Types.MOUSE); types[index] = true;
            }
            else if (c.type == ControllerType.Custom)
            {
                //If custom set custom
                index = (byte)(RewiredConsts.Layout.Joystick.Types.CUSTOM); types[index] = true;
            }
            else if (c.type == ControllerType.Joystick)
            {
                //If joystick

                Joystick j = (Joystick)(c); //Get joystick ref from controller

                bool unknown = (j.hardwareTypeGuid == Guid.Empty);
                if (unknown)
                {
                    //If joystick's hardwareTypeGuid is all 0s, then it's unknown
                    index = (byte)(RewiredConsts.Layout.Joystick.Types.UNKNOWN); types[index] = true;
                }
                else
                {
                    //If not unknown

                    //See if controller has template types of IController, IGamePad, and IDualAnalog; if so set the flags appropriately

                    //!@ PURCHASE/UPGRADE Rewired for template code support! For now, we will set these types as true
                    /*
                    IControllerTemplate ICT = c.GetTemplate<IControllerTemplate>();
                    IGamePadTemplate IGPT = c.GetTemplate<IGamePadTemplate>();
                    IDualAnalogGamepadTemplate IDual = c.GetTemplate<IDualAnalogGamepadTemplate>();                        
                    if (ICT)
                    {
                    */
                    index = (byte)(RewiredConsts.Layout.Joystick.Types.CTRLR); types[index] = true;
                    /*
                    }
                    if (IGPT)
                    {
                    */
                    index = (byte)(RewiredConsts.Layout.Joystick.Types.GPAD); types[index] = true;
                    /*
                    }
                    if (IDual)
                    {
                    */
                    index = (byte)(RewiredConsts.Layout.Joystick.Types.T_DUAL_ANALOG); types[index] = true;
                    //}

                    //See if controller is an xbox controller (check hw string names)
                    bool xb = (CheckSpecificJoyType(j, xinput, false) || CheckSpecificJoyType(j, xbox, true));
                    if (xb)
                    {
                        //Set XInput flag
                        index = (byte)(RewiredConsts.Layout.Joystick.Types.XINPUT); types[index] = true;

                        //!@ Add code here to determine X360 vs. XBone (plug in real 1st-party wired/less controllers to determine strings!
                        //For now, set both as true :shruggie:
                        index = (byte)(RewiredConsts.Layout.Joystick.Types.X360); types[index] = true;
                        index = (byte)(RewiredConsts.Layout.Joystick.Types.XBONE); types[index] = true;
                    }
                }
            }
        }
    }

    /// <summary>Using the ref bool[,] results from FindTypesOfGamePad, check if any controllers are of type RewiredConsts.Layout.Joystick.Types T</summary>
    /// <param name="types">bool[,] types from FindTypesOfGamePad result</param>
    /// <param name="T">Type of joystick to check any for</param>
    /// <returns>Were any controller of type T?</returns>
    public static bool FindTypesAnyWithType(bool[,] types, RewiredConsts.Layout.Joystick.Types T)
    {
        bool found = false;                     //Any of type T found?
        byte i;                                 //Generic iterator for controller dim of types
        byte iMax = (byte)(types.GetLength(0)); //Max amount of controllers for controller dim
        byte j = (byte)(T);                     //Type T as byte

        //Iterate through all controllers
        for (i = 0; i < iMax; i++)
        {
            //Check if any of type j were found; if so, found!
            if (types[i, j] == true)
            {
                found = true;
                break;
            }
        }
        return found;                           //Return result
    }

    /// <summary>Given a ref bool[,] result from FindTypesOfGamePad, return indices for all controller types that are of type T</summary>
    /// <param name="types">bool[,] result from FindTypesOfGamePad</param>
    /// <param name="T">Type to find</param>
    /// <param name="indices">ByRef result of Array of indices from types that are of type T</param>
    public static void FindTypesOfT(bool[,] types, RewiredConsts.Layout.Joystick.Types T, ref byte[] indices)
    {
        const byte invalid = 0xFF;              //Token to signify indices not of type T
        byte i;                                 //Generic iterator for controller dim of types
        byte iMax = (byte)(types.GetLength(0)); //Max amount of controllers for controller dim
        byte j = (byte)(T);                     //Type T as byte

        //Create a list of bytes of size iMax (amt of controllers).
        //This holds controller indices from types of type T
        //If not of Type T, mark as invalid
        List<byte> listIndices = new List<byte>(iMax);  

        //Iterate through all controllers
        for (i = 0; i < iMax; i++)
        {
            //Check if type j was found
            if (types[i, j] == true)
            {
                //If found, record the index in the list
                listIndices[i] = i;
            }
            else
            {
                //If not found, mark index as invalid in list
                listIndices[i]=invalid;
            }
        }

        //RemoveAll lambad https://www.dotnetperls.com/removeall
        //Remove all entries indices that equal invalid
        listIndices.RemoveAll(item => item == invalid);

        if (listIndices == null)
        {
            //If all removed (none of type T found) return ByRef indices as null
            indices = null;
        }
        else
        {
            //If some found, return ByRef indices as the array of listIndices result
            indices = listIndices.ToArray();
        }
    }

    /// <summary>Checks for existence of a specific Joystick type by string hw name</summary>
    /// <param name="j">Joystick ref to check</param>
    /// <param name="type">Type of joystick by string keyword</param>
    /// <param name="Case">Ignore case? (Set to upper for comparisons)</param>
    /// <returns>If this joystick is of the specified type</returns>
    private static bool CheckSpecificJoyType(Joystick j, string type, bool Case)
    {        
        bool found = false; //Was the joystick type found?
        if(Case)
        {
            //If case insensitive, convert type and string names ToUpper for comparisons
            type = type.ToUpper();
            found = (j.hardwareName.ToUpper().Contains(type) || j.name.ToUpper().Contains(type) || j.hardwareIdentifier.ToUpper().Contains(type));
        }
        else
        {
            //If exact case, just do straight cmps
            found = (j.hardwareName.Contains(type) || j.name.Contains(type) || j.hardwareIdentifier.Contains(type));
        }
        return found;   //Return state
    }
}
