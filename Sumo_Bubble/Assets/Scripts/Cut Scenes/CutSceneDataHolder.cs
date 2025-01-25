using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class holding CutSceneData for dynamic cutscenes</summary>
public class CutSceneDataHolder : MonoBehaviour
{
    /// <summary>A container for the actual CutSceneData</summary>
    public CutSceneController.CutSceneData CSData;

    /// <summary>Copies the data to the Controller and start it</summary>
    public void StartCutscene()
    {
        StartCutscene(0);
    }

    public void StartCutscene(int chunkID)
    {
        //Get the CutSceneController instance
        bool exists = false;
        CutSceneController CS = GetCS(ref exists);
        if (exists)
        {
            //If exists
            bool dewIt = CS.CSData.interruptable;
            if (dewIt)
            {
                //If the cutscene is interruptable, execute

                //Open up cutscene
                OpenScreen(MainMenuController.GameMenus.CUTSCENE);

                //Copy over the cutscene data to the CutSceneController instance, then start it with its delay timer
                CS.CSData = CSData;
                CS.StartCutscene(chunkID, CSData.delay);
            }
        }
    }

    /// <summary>Start the options subdialog</summary>
    public void StartOptions()
    {
        OpenScreen(MainMenuController.GameMenus.CUTSCENE_OPTION);
    }

    /// <summary>Close the options subdialog</summary>
    public void CloseOptions()
    {
        //Get the MainMenuController instance; if exists, then close current screen and restore previous
        MainMenuController MMC = MainMenuController.instance;
        if (MMC)
        {
            int id = (int)(MainMenuController.GameMenus.CUTSCENE);
            MMC.CurrentScreenClose();
            MMC.OpenScreen(id);
        }
    }

    /// <summary>Opens up a particular MenuScreen</summary>
    /// <param name="screen">MenuScreen enum ID</param>
    private void OpenScreen(MainMenuController.GameMenus screen)
    {
        //Get the MainMenuController instance, then open up cutscene
        MainMenuController MMC = MainMenuController.instance;
        if (MMC)
        {
            int id = (int)(screen);
            MMC.OpenScreen(id);
        }
    }

    /// <summary>Toggles the locked state</summary>
    /// <param name="state">State</param>
    public void SetLock(bool state)
    {
        //Get the CutSceneController instance; if exists, then set new state
        bool exists = false;
        CutSceneController CS = GetCS(ref exists);
        if (exists){CS.isLocked = state;}
    }

    /// <summary>Toggles the interruptable state</summary>
    /// <param name="state">State</param>
    public void SetInterruptable(bool state)
    {
        //Get the CutSceneController instance; if exists, then set new interruptable state
        bool exists = false;
        CutSceneController CS = GetCS(ref exists);
        if (exists){CS.CSData.interruptable = state;}
    }

    /// <summary>Ends the cutscene</summary>
    public void EndCutScene()
    {
        //Get the CutSceneController instance; if exists, then set interruptable state
        bool exists = false;
        CutSceneController CS = GetCS(ref exists);
        if (exists){CS.CSData.interruptable = true;}

        //Get the MainMenuController instance; if exists, then close current screen and restore previous
        MainMenuController MMC = MainMenuController.instance;
        if (MMC)
        {
            MMC.CurrentScreenClose();
            MMC.SetPrev();
        }
    }

    /// <summary>Ends the cutscene</summary>
    public void EndCutScene(MainMenuController.GameMenus ID)
    {
        //Get the CutSceneController instance; if exists, then set interruptable state
        bool exists = false;
        CutSceneController CS = GetCS(ref exists);
        if (exists){ CS.CSData.interruptable = true; }

        //Get the MainMenuController instance; if exists, then close current screen and restore previous
        MainMenuController MMC = MainMenuController.instance;
        if (MMC)
        {
            int id = (int)(ID);
            MMC.CurrentScreenClose();
            MMC.OpenScreen(id);
        }
    }

    /// <summary>Ends the cutscene, with MenuScreen override to open</summary>
    /// <param name="MS">New MenuScreen to open</param>
    public void EndCutScene(MenuScreen MS)
    {
        //Get the CutSceneController instance if exists; then set interruptable state
        bool exists = false;
        CutSceneController CS = GetCS(ref exists);
        if (exists) { CS.CSData.interruptable = true;}

        //Get MainMenuController instance; if exists, then close current screen and open new screen
        MainMenuController MMC = MainMenuController.instance;
        if (MMC)
        {
            MMC.CurrentScreenClose();
            MMC.OpenScreen(MS);
        }
    }

    /// <summary>Starts the specified chunk</summary>
    /// <param name="index">Chunk ID</param>
    public void StartChunk(int index)
    {
        //Get the CutSceneController instance; if exists, then start chunk ID
        bool exists = false;
        CutSceneController CS = GetCS(ref exists);
        if (exists) { CS.StartChunk(index);}
    }

    #region DynamicStuff
    public void SetExpression(int chunkID, int chunkExpID, Localization.LangPack2 pack2)
    {
        CSData.cutSceneData[chunkID].expressions[chunkExpID].expression = pack2;
    }
    #endregion

    #region Quest_Shims
    /// <summary>Gets CutSceneController instance; and if it exists (ByRef)</summary>
    /// I/O:
    /// <param exists="exists"></param>
    /// Output (ByRef)
    /// <returns>CutSceneController singleton</returns>
    private CutSceneController GetCS(ref bool exists)
    {
        CutSceneController CS = CutSceneController.instance;
        exists = (CS != null);
        return CS;
    }
    #endregion
}