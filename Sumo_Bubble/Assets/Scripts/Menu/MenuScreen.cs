using ByteSheep.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Defins and handles a MenuScreen object</summary>
public class MenuScreen : MonoBehaviour
{
    //!@
    //public UnityEvent onClose;
    //public UnityEvent onOpen;
    public AdvancedEvent onClose;   //Event to run onClose
    public AdvancedEvent onOpen;    //Event to run onOpen
    public bool @default;           //Is this the default menu screen?
    public bool default_open;       //If default, also run open events
    public bool opened=false;       //Is this MenuScreen opened?

    void Start()
    {
        if (@default)
        {
            //If default menu, and MMC instance exists
            if (MainMenuController.instance)
            {
                //If default, set the MMC current screen to this
                MainMenuController.instance.SetCurrentScreen(this);                
                if (default_open == true){Open(); }  //If default open flag set, then open it
            }
        }
    }

    /// <summary>Opens this menuscreen</summary>
    public void Open()
    {
        //Set opened flag, and invoke opened event
        opened = true;
        onOpen.Invoke();
    }

    /// <summary>Closes this menuscreen</summary>
    public void Close()
    {
        //Reset opened flag, and invoke closed event
        opened = false;
        onClose.Invoke();
    }

    /// <summary>Changes the canvas mode</summary>
    /// <param name="c">Canvas to change</param>
    /// <param name="overlay">Set to overlay? Else set to Camera space</param>
    private void ChangeCanvasMode(Canvas c, bool overlay)
    {
        Camera main = Camera.main;  //Get main camera
        c.worldCamera = main;       //Set canvas's world camera to main
        c.pixelPerfect = true;      //Enable pixelperfect mode

        //Set correct mode. If NOT overlay, then screenspace; else Overlay
        if (overlay == false)
        {c.renderMode = RenderMode.ScreenSpaceCamera;}
        else{c.renderMode = RenderMode.ScreenSpaceOverlay;}        
    }

    /// <summary>Changes CanvasMode to Camera</summary>
    /// <param name="c">Canvas to change</param>
    public void ChangeCanvasMode_Camera(Canvas c)
    {
        ChangeCanvasMode(c, false);
    }

    /// <summary>Changes CanvasMode to Overlay</summary>
    /// <param name="c">Canvas to change</param>
    public void ChangeCanvasMode_Overlay(Canvas c)
    {
        ChangeCanvasMode(c, true);
    }

    //!@Dead Code
    /*
    public void SpawnObject(GameObject prefab)
    {
        Debug.Log("Spawning new " + prefab.gameObject.name.ToString());
        //We need to now parent under GameUI, so that PSAlive doesn't go over XBumMsg like stuff
        //Transform parent=GameObject.Find("PauseScreen").transform;        
        Transform parent = GameObject.Find("Game UI").transform;          
        GameObject obj=Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        obj.transform.localPosition = Vector3.zero;
    }
    */
}
