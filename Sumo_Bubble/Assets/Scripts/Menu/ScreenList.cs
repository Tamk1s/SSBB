using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>Defines and handles a ScreenList object (list of ScreenItems)</summary>
public class ScreenList : MonoBehaviour
{
    private Audio audio;                        //Audio script
    public List<ScreenItem> items;              //List of screenitems
    public int yStep = 4;                       //YStep
    public int defaultItem = 0;                 //Default index in the list
    public bool selectDefault;                  //If true, activate default item on reset
    public bool isFNConsole = false;            //Is this the FNConsole? If so, do a different wrap formula for Up(), Down(), Prev(), and Next() funcs
    private bool suppressActivation = false;    //Temporarily suppresses activation events when set; false=allow
    public bool allowSFX = true;                //Allow sfx on highlight move?

    //[HideInInspector]
    public int currentIndex = 0;                //Current index in list
    public bool enabled = true;                 //Is screenList enabled for handling list controls?

    void Start()
    {
        //Add an Audio script if DNE
        if (!audio){audio = this.gameObject.AddComponent<Audio>();}
        //Toggle the list of items based on platform support
        ScreenListPlatform();
    }

    /// <summary>Removes elements from items if not supported on platform</summary>
    private void ScreenListPlatform()
    {
        //Is this running platform Win, Linux, Mac, or demo?
        bool isWin = false;
        bool isMac = false;
        bool isLin = false;
        //bool isXB = false;
        bool isDemo = false;
        //!@ And and support isAndroid and isIOS here!

        //Set flag for appropriate platform
        #if UNITY_STANDALONE_WIN
            isWin = true;
        #endif

        #if UNITY_STANDALONE_OSX
            isMac = true;
        #endif

        #if UNITY_STANDALONE_LINUX
            isLin = true;
        #endif

        /*
        #if UNITY_XBOXONE
            isXB = true;
        #endif
        */

        #if UNITY_DEMO
            isDemo = true;
        #endif

        ScreenItem[] _items = items.ToArray();  //Convert items to array (we are going to remove items from list; can't iterate through self like that)

        //Iterate through all ScreenItems
        foreach (ScreenItem S in _items)
        {
            bool removeIt = false;          //Should we remove this element
            //If this platform is Windows, but it's not supported, set remove flag
            if (isWin){removeIt = !(S.supWin);}
            //If Mac
            if (isMac){removeIt = !(S.supMac);}
            //Linux
            if (isLin){removeIt = !(S.supLinux);}
            //Evil XBone
            //if (isXB){removeIt = !(S.supXB);}
            //Demo build
            if (isDemo){removeIt = !(S.supDemo);}
            //!@ Add isAndroid and isIOS support here!

            //Remove appropriate elements
            if (removeIt)
            {
                items.Remove(S);                    //Remove ScreenItem from list
                S.gameObject.SetActive(false);      //Disable its gameobject
            }
        }
        ResetList();
    }

    /// <summary>Resets the focus state for the list of items</summary>
    public void ResetList()
    {
        //Unfocus everything
        for (int i = 0; i < items.Count; i++) items[i].UnFocus();

        //Set current index to default, focus on default
        currentIndex = defaultItem;
        items[defaultItem].Focus();

        //Activate the default if allowed
        if (selectDefault){items[defaultItem].Activate();}
    }

    /// <summary>Handles next (right) item in list</summary>
    public void Next()
    {
        if (!enabled){return;}
        //Unfocus on current index
        items[currentIndex].UnFocus();

        //Do special wrapping for FN console
        if (isFNConsole == true)
        {
            //!@ This may need modified?

            /* Special wrapping:
             * 9 next 29
             * 19 next 39
             * 28 next 37
             * 29 next 0
             * 36 next 38
             * 37 next 20
             * 38 next 30
             * 39 next 10
             * (index % 10 == 9) then -=9
            */
            if (currentIndex == 9)
            {
                currentIndex = 29;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 19)
            {
                currentIndex = 39;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 28)
            {
                currentIndex = 37;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 29)
            {
                currentIndex = 0;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 36)
            {
                currentIndex = 38;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 37)
            {
                currentIndex = 20;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 38)
            {
                currentIndex = 30;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 39)
            {
                currentIndex = 10;
                items[currentIndex].Focus();
                return;
            }            

            if (currentIndex % 10 == 9)
            {
                currentIndex -= 9;
                items[currentIndex].Focus();
                return;
            }
        }

        //For normal menus
        //Increment current index if less than max; else set to 0
        if ((currentIndex + 1) < items.Count)
        {currentIndex++;}
        else{currentIndex = 0;}

        //Focus on current index
        items[currentIndex].Focus();
    }

    /// <summary>Handles prev (left) item in list</summary>
    public void Prev()
    {
        if (!enabled){return;}
        //Unfocus on current index
        items[currentIndex].UnFocus();

        //Do special wrapping for FN console
        if (isFNConsole == true)
        {
            //!@ This may need modified!

            /*Special wrapping:
             * 0 prev 29
             * 10 prev 39
             * 20 prev 37
             * 29 prev 9
             * 30 prev 38
             * 37 prev 28
             * 38 prev 36
             * 39 prev 19
             * (index % 10 == 9) then +=9
            */
            if (currentIndex == 0)
            {
                currentIndex = 29;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 10)
            {
                currentIndex = 39;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 20)
            {
                currentIndex = 37;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 29)
            {
                currentIndex = 9;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 30)
            {
                currentIndex = 38;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 37)
            {
                currentIndex = 28;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 38)
            {
                currentIndex = 36;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 39)
            {
                currentIndex = 19;
                items[currentIndex].Focus();
                return;
            }

            if ((currentIndex % 10) == 0)
            {
                currentIndex += 9;
                items[currentIndex].Focus();
                return;
            }
        }

        //Normal menus
        //If current index >= min 0, then decrement; else set to max
        if ((currentIndex - 1) >= 0)
        {currentIndex--;}
        else{currentIndex = (items.Count - 1);}

        //Focus on current index
        items[currentIndex].Focus();
    }

    /// <summary>Handles down item in list</summary>
    public void Down()
    {
        if (!enabled){return;}
        
        //An offset, set to 0 if NOT FNConsole, else 1 if no special wrapping occurs with FNConsole
        byte offset = 0;
        if (isFNConsole == false)
        {
            offset = 0;
        }
        else
        {
            //!@ This may need modified!

            //Handle special wrapping for FNConsole

            /*Special Wrapping:
             * 19 down 9
             * 27 down 7
             * 28 down 8
             * 29 down 39
             * 37 down 38
             * 38 down 29
             * 39 down 37
             * All others offset = 1 for normal wrapping
            */
            
            //!@
            if (currentIndex == 19)
            {
                items[currentIndex].UnFocus();
                currentIndex = 9;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 27)
            {
                items[currentIndex].UnFocus();
                currentIndex = 7;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 28)
            {
                items[currentIndex].UnFocus();
                currentIndex = 8;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 29)
            {
                items[currentIndex].UnFocus();
                currentIndex = 39;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 37)
            {
                items[currentIndex].UnFocus();
                currentIndex = 38;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 38)
            {
                items[currentIndex].UnFocus();
                currentIndex = 29;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 39)
            {
                items[currentIndex].UnFocus();
                currentIndex = 37;
                items[currentIndex].Focus();
                return;
            }
            offset = 1;
        }

        //Normal menu items/nonspecial wrapping for FNConsole
        //Unfocus on current index
        items[currentIndex].UnFocus();

        //If currentIndex + ystep < max items - offset, then increment by ystep; else modulo by it
        if ((currentIndex + yStep) < (items.Count - offset))
        {currentIndex += yStep;}
        else{currentIndex %= yStep;}

        //Focus on current index
        items[currentIndex].Focus();
    }

    /// <summary>Hnadles up item in list</summary>
    public void Up()
    {
        if (!enabled){return;}
        
        //Unfocus on current index
        items[currentIndex].UnFocus();

        //If FNConsole, handle special wrapping
        if (isFNConsole)
        {
            //!@ This may need modified!

            /* Special wrapping
             * 7 up 27
             * 8 up 28
             * 9 up 19
             * 29 up 38
             * 37 up 39
             * 38 up 37
             * 39 up 29
             * Others +=30
            */

            if (currentIndex == 7)
            {
                items[currentIndex].UnFocus();
                currentIndex = 27;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 8)
            {
                items[currentIndex].UnFocus();
                currentIndex = 28;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 9)
            {
                items[currentIndex].UnFocus();
                currentIndex = 19;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 29)
            {
                items[currentIndex].UnFocus();
                currentIndex = 38;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 37)
            {
                items[currentIndex].UnFocus();
                currentIndex = 39;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 38)
            {
                items[currentIndex].UnFocus();
                currentIndex = 37;
                items[currentIndex].Focus();
                return;
            }

            if (currentIndex == 39)
            {
                items[currentIndex].UnFocus();
                currentIndex = 29;
                items[currentIndex].Focus();
                return;
            }
        }

        //Normal menus/FNConsole other
        //Unfocus on current index
        items[currentIndex].UnFocus();

        //If currentindex - ystep >= 0, then just -=step
        if ((currentIndex - yStep) >= 0)
        {
            currentIndex -= yStep;
        }
        else
        {
            //Otherwise
            if (isFNConsole == false)
            {
                //Normal menus
                //Subtract (ystep-currentindex) from items count
                currentIndex = items.Count - (yStep - currentIndex);
            }
            else
            {
                //FNConsole all others increment by 30
                currentIndex += 30;
            }
        }

        //Focus on current index
        items[currentIndex].Focus();
    }

    /// <summary>Sets current ScreenList index; used for UI mouse/mobile events</summary>
    /// <param name="index">New index</param>
    public void SetCurrentIndex(int index)
    {
        //If NOT enabled OR same index, skip
        if((!enabled)||(currentIndex == index))
        {
            if (currentIndex == index){PlayHighlight();}
            return;
        }

        //Unfocus on old index, then focus on current
        items[currentIndex].UnFocus();
        currentIndex = index;
        items[currentIndex].Focus();
        //Play hightlight SFX
        PlayHighlight();
    }

    /// <summary>Handles horizontal movement</summary>
    /// <param name="x">XDir movement</param>
    public void Horizontal(float x)
    {
        if (!enabled){return;}

        if (x > 0f)
        {
            //If x+ (right), play highlight sfx if enabled, and do Next
            PlayHighlight();
            Next();
        }
        else if (x < 0f)
        {
            //If x+- (left), play highlight sfx if enabled, and do Prev
            PlayHighlight();
            Prev();
        }
        //0=NOP
    }

    /// <summary>Handles vertical movement</summary>
    /// <param name="y">YDir Movement</param>
    public void Vertical(float y)
    {
        if (!enabled){return;}
        if (y > 0f)
        {
            //if y+ (up), play highlight sfx if enabled, and then do Up
            PlayHighlight();
            Up();
        }
        else if (y < 0f)
        {
            //if y- (down), play highlight sfx if enabled, and then do Down
            PlayHighlight();
            Down();
        }
    }

    /// <summary>Run onValue on value</summary>
    /// <param name="value">Value</param>
    public void CurrentValue(float value)
    {
        if (!enabled){return;}
        items[currentIndex].OnValue(value);
    }

    /// <summary>Actives current item</summary>
    public void ActivateCurrent()
    {
        if (!enabled){return;}
        //Don't activate if supressed
        if (suppressActivation == false)
        {
            //if NOT FNConsole and currentindex enabled
            if ((isFNConsole == false) && (items[currentIndex].enabled == true))
            {
                /*
                 *Count # of items in activate() event.
                 *Prevents bug of sfx playing if no items present
                 *(eg, attempting to activate toggles in MM's Option menu)
                 */
                int count = items[currentIndex].GetActivateCount();
                if (count > 0)
                {
                    PlaySelect();
                }
            }
            //Activate current index
            items[currentIndex].Activate();
        }
    }

    /// <summary>Plays highlight sfx if allowed</summary>
    public void PlayHighlight()
    {
        //!@ Play Higlight SFX
        if (allowSFX == true){audio.sfx_play(Audio.SFX.SFX_MENU_HIGHLIGHT);}
    }

    /// <summary>Plays select sfx if allowed</summary>
    public void PlaySelect()
    {
        //!@ Play Menu SFX
        if (allowSFX == true){audio.sfx_play(Audio.SFX.SFX_MENU_SELECT);}
    }

    /// <summary>Toggles suppression</summary>
    public void ToggleSupression()
    {
        StartCoroutine(ToggleSupression(false));
    }

    /// <summary>Does the actual toggling of suppression</summary>
    /// <param name="dummy">A dummy param</param>
    /// <returns>Supression toggled/yield</returns>
    private IEnumerator ToggleSupression(bool dummy)
    {
        //Set activation, wait .1f, then reset
        suppressActivation = true;
        yield return new WaitForSeconds(.1f);
        suppressActivation = false;
    }

    /// <summary>Used by events for toggling allowSFX</summary>
    /// <param name="value">New state as bit</param>
    public void ToggleSFX(int value)
    {
        allowSFX = (value == 1);
    }

    /// <summary>Toggle the enable state of this Screenlist</summary>
    /// <param name="value">New state as bit</param>
    public void ToggleEnabled(int value)
    {        
        enabled = (value == 1);
    }

    #region DynamicMenus
    /// <summary>Add a gameobject prefab with screenItem to end of children</summary>
    /// <param name="prefab">Prefab to spawn</param>
    /// <param name="parent">Parent to spawn children under</param>
    /// <returns>Spawned GameObject</returns>
    public GameObject AddItem(GameObject prefab, Transform parent)
    {
        //Instantiate the prefab underneat the parent
        GameObject obj= GameObject.Instantiate(prefab, parent);
        if (obj)
        {
            //if prefab spawned

            //Get its ScreenItem
            ScreenItem SI = obj.GetComponent<ScreenItem>();

            //If screenItem found, add it to the items list
            if (SI != null){items.Add(SI);}
        }
        return obj;
    }

    /// <summary>Delete all screenItems in range, and the children gameobjects</summary>
    /// <param name="startIndex">Start index for deletion</param>
    /// <param name="parent">Parent holding the children</param>
    public void DeleteItems(byte startIndex, Transform parent)
    {
        byte count = (byte)(items.Count);       //Get the count of items
        count -= startIndex;                    //count=count-1 because 1st index is reserved for Back button
        try
        {
            items.RemoveRange(startIndex, count);   //Remove those items from list
            if (parent) { parent.DestroyAllChildren(); }//Delete all children!
        }
        catch { }
    }
    #endregion
}