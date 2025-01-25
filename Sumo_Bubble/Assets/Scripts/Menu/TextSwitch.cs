﻿using System;
using System.Collections;
using UnityEngine;
using ByteSheep.Events;
using UnityEngine.UI;

/// <summary>Class handles a menu element which can have toggled value by pressing left/right buttons</summary>
public class TextSwitch : MonoBehaviour
{
    private bool enabled = true;                    //Is this object enabled for usage?
    public bool enableAlt=false;                    //Enable toggling of image elements?
    public Sprite[] sprs;                           //Sprites to use instead of images
    public Sprite[] sprsAlt;                        //Alt sprites
    public bool[] sprStat;                          //Enable state of each sprite
    public Localization.LangPack2[] names;          //Array of names via LangPacks
    public bool useImage=false;                     //Use an image instead of text?
    
    public int currentIndex = 0;                    //Current index of the element

    //Dynamic min and max limits of the TS element.
    //(int)(.x)=min, (int)(.y)=max. .x is 0-based,
    //.y should be max + 1. Used for dynamically changing TS elements for VectorVision menu
    public Vector2 dynLims = Vector2.zero;          //!@

    public bool useTarget = false;                  //Use a targetIndex for onTarg event?
    public int[] targetIndex = new int[2];          //The target indices

    //Suppress onIndexChange from running every change.
    //Use RunIndexChange/ForceInvoke to run. Used for TS components that need have an action sent later.
    //(VectorVision elements on Apply button)
    public AdvancedIntEvent onIndexChange;                      //Event to run on index change
    public AdvancedStringEvent onTextChange;                      //Event to run on text change
    public AdvancedEvent[] onTarget = new AdvancedEvent[2];     //Event to run when currentIndex==targetIndex[some index]
    public AdvancedEvent onInit;                                //Event to run on initz 

    public bool activation=false;                                           //Should we allow running onActivate[index] events on activation of a TS item?
    public AdvancedEvent[] onActivate;                                      //If so, these are the events to run for each index

    public bool nud = false;                                                //Should this element be a Windows-like numeric up/down element for inputting a value?
    private GameManager.Language oldLang=GameManager.Language.LANG_ENGLISH; //Language on previous frame
    private bool ready = false;                                             //Are we ready?

    //Text/Image components
    private Text UIText;                                                    //Text
    private Image img;                                                      //Image

    private void Awake()
    {
        //Fetch either text/img component, based on option
        if (useImage == false)
        {UIText = GetComponent<Text>();}
        else{img = GetComponent<Image>();}        

        //Run the init event; then we are now ready!
        onInit.Invoke();
        ready = true;
    }

    /// <summary>Update MMI items when language changes if applicable</summary>
    void Update()
    {        
        if (ready)
        {
            //Does this TS script use names?
            bool hasNames = (names.GetLength(0) != 0);
            if (hasNames)
            {
                //If GM exists
                if (GameManager.instance)
                {
                    GameManager.Language Lang = GameManager.instance.lang;  //Get current language

                    //If delta
                    if (oldLang != Lang)
                    {
                        //Update old language (deadlock out of spamming this block)
                        oldLang = GameManager.instance.lang;
                        string newName = names[currentIndex].language;         //Update the text byLang
                        UIText.text = newName;
                        onTextChange.Invoke(newName);
                    }
                    oldLang = GameManager.instance.lang;                    //Update old language                        
                }
            }
        }
    }

    /// <summary>Handles horizontal movement</summary>
    /// <param name="x">DirStick axis.x</param>
    public void OnHorizontal(float x)
    {
        //Process menu highlighting if enabled
        if (enabled == true)
        {
            //If positive right axis, do next; else if negative left axis, do Prev(), else NOP 
            if (x > 0f)
            {Next();}
            else if (x < 0f){Prev();}
        }
    }

    /// <summary>Handles changing the index to the next one</summary>
    public void Next()
    {
        //Process current array type
        Vector2 lims;   //Limits
        int max = 0;    //Max amount of elements
        string newName; //New text string from LangPack
        bool dynamic = hasDynLims();
        if (dynamic)
        {
            //If limits are dynamic, copy dynLims to Lims
            lims.x = dynLims.x;
            lims.y = dynLims.y;
        }
        else
        {
            //If limits are static, set minimum to 0, and max to lenght of names or sprites appropriately
            lims.x = 0;
            if (useImage == false)
            {max = names.Length;}
            else{max = sprs.Length;}
            lims.y = max;
        }

        //If next index is less than max limit, increment; else set to minimum
        if ((currentIndex + 1) < (int)(lims.y))
        {currentIndex++;}
        else{currentIndex = (int)(lims.x);}
        
        if (nud)
        {
            //If this is a NUD, just update the text to currentindex.toString()
            newName = currentIndex.ToString();
            UIText.text = newName;
            onTextChange.Invoke(newName);
        }
        else
        {
            //Change text/img as approriately
            if (useImage == false)
            {
                //Fetch appropriate name from LangPack if not an image TS
                newName = names[currentIndex].language;
                UIText.text = newName;
                onTextChange.Invoke(newName);
            }
            else
            {
                //If an image
                if (enableAlt == false)
                {
                    //If not using alts, just update the sprite
                    img.sprite = sprs[currentIndex];
                }
                else
                {
                    //If using alts, get the state for this sprite, and set appropriate sprite
                    bool state = sprStat[currentIndex];
                    if (state == false)
                    {img.sprite = sprsAlt[currentIndex];}
                    else{img.sprite = sprs[currentIndex];}
                }
            }
        }

        //Run onIndexChange event
        onIndexChange.Invoke(currentIndex);
        
        //If target met, run event
        if(useTarget == true)
        {
            byte j = 0;
            foreach (int i in targetIndex)
            {
                if (currentIndex == i){onTarget[j].Invoke();}
                j++;
            }
        }
    }

    /// <summary>Handles changing the index to the previous one</summary>
    public void Prev()
    {
        //Process current array type
        Vector2 lims;                   //Min/max limits
        int max = 0;                    //Max value
        bool dynamic = hasDynLims();    //Does this feature have dynamic limits?
        string newName;                 //New textswitch component name
        
        if (dynamic)
        {
            //If dynamic, get x/y min/max values from dynLims
            lims.x = dynLims.x;
            lims.y = dynLims.y;
        }
        else
        {
            //If static
            lims.x = 0;             //Set min to 0

            //Get max value from appropriate component element (images or sprites)
            if (useImage == false)
            {max = names.Length;}
            else{max = sprs.Length;}
            lims.y = max;           //Set max to max
        }

        //If the current index -1 is above minimum, then decrement index ; else set index to wrap goto (max -1)
        if (currentIndex - 1 >= ((int)(lims.x)))
        {currentIndex--;}
        else{currentIndex = ((int)(lims.y)) - 1;}

        //Change text/img as appropriately
        if (nud)
        {
            //If NUD element
            newName = currentIndex.ToString();  //Get value as string
            //Apply new value name/invoke event
            UIText.text = newName;
            onTextChange.Invoke(newName);
        }
        else
        {
            //IF NOT a NUD
            if (useImage == false)
            {
                // if using images

                //Fetch appropriate name from LangPack, update, and invoke textChange event
                newName = names[currentIndex].language;
                UIText.text = newName;
                onTextChange.Invoke(newName);
            }
            else
            {
                //if NOT using images
                if (enableAlt == false)
                {
                    //If not ALT version, then just update image sprite by index from array
                    img.sprite = sprs[currentIndex];
                }
                else
                {
                    //If using ALT versions

                    //Get state of this sprite
                    bool state = sprStat[currentIndex];

                    //If disabled, use alt version; else use regular version
                    if (state == false)
                    {img.sprite = sprsAlt[currentIndex];}
                    else{img.sprite = sprs[currentIndex];}
                }
            }
        }

        //invoke onIndexChange() events
        onIndexChange.Invoke(currentIndex);

        //If target met, run event
        if (useTarget == true)
        {
            byte j = 0;
            foreach (int i in targetIndex)
            {
                if (currentIndex == i){onTarget[j].Invoke();}
                j++;
            }
        }
    }

    /// <summary>Does this TS element use dynamic limits?</summary>
    /// <returns>State of dynamic limits</returns>
    private bool hasDynLims()
    {
        //Has dynamic limits of r vector is NOT V2.0 (0,0)
        bool result = (dynLims != Vector2.zero);
        return result;
    }

    /// <summary>Update stuff at specified index</summary>
    /// <param name="index">Da index</param>
    public void AtIndex(int index)
    {
        //Set currentindex to index
        string newName;
        currentIndex = index;

        //Change text/img as appropriately
        if (nud)
        {
            //if NUD element
            newName = currentIndex.ToString();  //Get index value as name
            //Update text, invoke event
            UIText.text = newName;
            onTextChange.Invoke(newName);
        }
        else
        {
            //if NOT NUD element
            if (useImage == false)
            {
                //If NOT using images

                //Fetch appropriate name from LangPack
                newName = names[currentIndex].language;
                //Update text, invoke event
                UIText.text = newName;
                onTextChange.Invoke(newName);
            }
            else
            {
                //If using images

                //if NOT using ALT version, just fetch regular version
                if (enableAlt == false)
                {img.sprite = sprs[currentIndex];}
                else
                {
                    //if using ALT version

                    //Get state of this sprite                    
                    bool state = sprStat[currentIndex];

                    //If disabled, use alt version; else use regular version
                    if (state == false)
                    {img.sprite = sprsAlt[currentIndex];}
                    else{img.sprite = sprs[currentIndex];}
                }
            }
        }

        //invoke onIndexChange() event
        onIndexChange.Invoke(currentIndex);

        //If target met, run event
        if (useTarget == true)
        {
            byte j = 0;
            foreach (int i in targetIndex)
            {
                if (currentIndex == i){onTarget[j].Invoke();}
                j++;
            }
        }
    }

    /// <summary>Force onIndexChange for current index</summary>
    public void RunIndexChange()
    {
        onIndexChange.Invoke(currentIndex);
    }

    /// <summary>Same as RunIndexChange()</summary>
    public void ForceInvoke()
    {
        RunIndexChange();
    }

    /// <summary>Enables an Image</summary>
    /// <param name="ind">Index of image</param>
    public void SetImgState(int ind)
    {
        sprStat[ind] = true;
    }

    /// <summary>Disable an image</summary>
    /// <param name="ind">Index of image</param>
    public void ResetImgState(int ind)
    {
        sprStat[ind] = false;
    }

    /// <summary>If activation flag set, runs events onActivation of the currentIndex</summary>
    public void OnActivate()
    {
        //Only usable if activation flag set, and using images with alt flag
        if((activation == false)||(useImage==false)||(enableAlt==false))
        {
            return;
        }

        bool state = sprStat[currentIndex]; //Get state of index

        //If true, activate its events
        if (state == true)
        {onActivate[currentIndex].Invoke();}
    }

    /// <summary>Toggle the enable state of this TextSwitch</summary>
    /// <param name="state">State</param>
    public void ToggleEnable(bool state)
    {
        enabled = state;
    }

    /// <summary>Sets Image to white</summary>
    /// <param name="img">Image</param>
    public void SetImgColor(Image img)
    {
        try{img.color = Color.white;}
        catch (System.Exception ex)
        {
            Debug.Log("Caught error in TextSwitch.SetImgColor");
            Debug.LogWarning(ex.Message);
        }
    }

    /// <summary>Sets Image to grey</summary>
    /// <param name="img">Image</param>
    public void ResetImgColor(Image img)
    {
        const float OhPtFive = 0.5f;
        const float One = 1f;
        try{img.color = new Color(OhPtFive, OhPtFive, OhPtFive, One);}
        catch (System.Exception ex)
        {
            Debug.Log("Caught error in TextSwitch.ResetImgColor");
            Debug.LogWarning(ex.Message);
        }
    }

    /// <summary>Meat of TempToggleUseTarget() (Coroutine)</summary>
    /// <returns></returns>
    private IEnumerator _TempToggleUseTarget()
    {
        //Reset useTarget, wait 1f seconds, then set useTarget
        useTarget = false;
        yield return new WaitForSeconds(1f);
        useTarget = true;
    }

    /// <summary>Sets the textswitch names by a LangPack2 for localization</summary>
    /// <param name="arr">LangPack2 with localized names for each lang</param>
    public void SetNames(Localization.LangPack2[] arr)
    {
        names = arr;
    }

    /// <summary>Sets the red color of this text</summary>
    /// <param name="r">red byte</param>
    public void SetTextColor_r(int r)
    {
        Color32 clr = Color.black;
        clr.r = (byte)(r);
        UIText.color = clr;
    }

    /// <summary>Sets the green color of this text</summary>
    /// <param name="g">green byte</param>
    public void SetTextColor_g(int g)
    {
        Color32 clr = Color.black;
        clr.g = (byte)(g);
        UIText.color = clr;
    }

    /// <summary>Sets the blue color of this text</summary>
    /// <param name="b">green byte</param>
    public void SetTextColor_b(int b)
    {
        Color32 clr = Color.black;
        clr.b = (byte)(b);
        UIText.color = clr;
    }

    /// <summary>Sets the alpha color of this text. Set equal rgb components to represent alpha via opaque white</summary>
    /// <param name="a">alpha byte</param>
    public void SetTextColor_a(int a)
    {        
        byte _a = (byte)(a);
        Color32 clr = new Color32(_a, _a, _a, 255);
        UIText.color = clr;
    }

    /// <summary>Invokes onTextChange event. Primarily used by NUD elements</summary>
    public void invoke_onTextChange()
    {
        string newName; //New text string
        if (!nud)
        {
            //If not a NUD, get localized text string
            newName = names[currentIndex].language;
        }
        else
        {
            //If a NUD, just change text to currentIndex string value
            newName = currentIndex.ToString();
        }
        //Invoke onTextChange event
        onTextChange.Invoke(newName);
    }
}