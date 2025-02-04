using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Cutscene actor info singleton storage container</summary>
public class CutSceneActorInfo : MonoBehaviour
{
    /// <summary>Singleton instance</summary>
    public static CutSceneActorInfo instance;

    /// <summary>Holds actor info</summary>
    [System.Serializable]
    public struct Actor
    {
        /// <summary>Sprite for player</summary>
        public Sprite byImage;
        /// <summary>Name for player (with localization)</summary>
        public Localization.LangPack2 byName;
        /// <summary>JawPrefab to spawn</summary>
        //public GameObject byJaw;                    //!@ 3D gameobject rotating Jaws need removed, and just changed to a prefab sprite that changes frames as vocal peaks are met from audio clip
    }

    // !@ UPDATE ME
    /// <summary>Enum of Actor IDs</summary>
    public enum ActorID
    {
        NONE,           //NONE
        //SIGN,           //Generic sign/message board
        //NPC,            //Generic test NPC

        //Village NPCs
        //SHOPKEEPER,
        //FARMER,
        //BLACKSMITH,

        //PLAYER,
        MAX = NONE + 0x01
    }
    /// <summary>Max amount of actors. Aligned with the ActorID enum/MAX value</summary>
    public const byte maxActor = (byte)(ActorID.MAX) + 0x01;

    /// <summary>Actor info</summary>
    //[NamedArrayAttribute(typeof(ActorID))]    
    public Actor[] actors = new Actor[maxActor];

    /// <summary>
    /// Setup singleton instance onStart
    /// </summary>
    private void Start()
    {
        instance = this;
    }

    /// <summary>Retrieve the actor sprite by ActorID</summary>
    /// <param name="id">ActorID</param>
    /// <returns>Sprite</returns>
    public Sprite GetImage(ActorID id)
    {
        byte ID = (byte)(id);                   //Convert enum to byte index
        Sprite result = actors[ID].byImage;     //Fetch image from index
        return result;                          //Return
    }

    /// <summary>Get the actor name by ActorID</summary>
    /// <param name="id">ActorID</param>
    /// <returns>Localzd actor name</returns>
    public string GetName(ActorID id)
    {
        string result = GetName2(id).language;
        return result;                          
    }

    /// <summary>Get the actor name by ActorID (LangPack2)</summary>
    /// <param name="id">ActorID</param>
    /// <returns>LangPack2</returns>
    public Localization.LangPack2 GetName2(ActorID id)
    {
        byte ID = (byte)(id);                               //Convert enum to byte index
        Localization.LangPack2 result = actors[ID].byName;  //Get actor name langPack2
        return result;                                      //Return
    }

    #region 0xDEADC0DE
    /*
    /// <summary>Creates a jaw for actor, and plays localized vocal clip</summary>
    /// <param name="id">ActorID</param>
    /// <param name="parent">Transform parent to place jaw</param>
    /// <param name="vox">Vocal clip to play</param>
    /// <returns>Jaw prefab instantiated ref</returns>
    public GameObject CreateJaw(ActorID id, Transform parent, Audio.VOX vox)
    {        
        GameObject newObj = null;               //Jaw instantiated
        byte ID = (byte)(id);                   //Convert ActorID enum to byte index

        ToggleHealthBars(id);                   //Just show his healthbar
        GameObject prefab = actors[ID].byJaw;   //Get prefab to instantiate
        if (prefab)
        {
            //if prefab found, instantiate jaw
            newObj = GameObject.Instantiate(prefab, parent, false);
            if (newObj)
            {
                //If jaw instantiated

                //Get the audio component fromthe jaw; if exists, play the vocal clip
                //!@
                Audio aud = newObj.gameObject.GetComponent<Audio>();
                if (aud){aud.vox_play2(vox);}
            }
        }
        return newObj;                          //Return instantiate jaw ref
    }
    */

    /*
    /// <summary>Destroys a jaw prefab</summary>
    /// <param name="jawref">Jaw to destroy</param>
    public void DestroyJaw(GameObject jawref)
    {
        Destroy(jawref);
    }

    /// <summary>Toggles the CutScene's panels healthbars images. The specified ID will be toggled by state, while others are hiddened (Only one viewable at a time)</summary>
    /// <param name="id">ActorID of health bar to toggle</param>
    /// <param name="state">State of his health bar</param>
    private void ToggleHealthBars(ActorID id)
    {
        switch (id)
        {
            case ActorID.EAGLE:
            case ActorID.BUNNY:
            case ActorID.DOG:
            case ActorID.OWL:
                //0-based index for the healthbars (the ID - MIN, aligned with Characters enum)
                byte index = (byte)(id - ActorID.MIN);

                //Generic iterator. Iterate through all gameobjects in healthbars array
                byte i = 0;
                foreach(GameObject g in CutSceneController.instance.panel.CutSceneHealthBars)
                {
                    bool newState = (i == index);
                    ToggleHealthBarImg(g, newState);
                    //if (i != index)
                    //{
                    //    //If not = actorID index, disabled
                    //    ToggleHealthBarImg(g, false);
                    //}
                    //else
                    //{
                    //    //If = actorID index, toggle
                    //    ToggleHealthBarImg(g, true);
                    //}                    
                    i++;
                }
                break;
            
            default:
                //If not a character actor, disable all healthbars
                foreach(GameObject g in CutSceneController.instance.panel.CutSceneHealthBars)
                {
                    ToggleHealthBarImg(g, false);
                }
                break;
        }
    }

    /// <summary>Toggles the images for a healthbar</summary>
    /// <param name="bar">The bar</param>
    /// <param name="state">The state</param>
    private void ToggleHealthBarImg(GameObject bar, bool state)
    {
        //Get all iamges from the bar
        Image[] img = bar.GetComponentsInChildren<Image>();
        if (img != null)
        {
            //If array found

            //Toggle each state
            foreach (Image i in img)
            {
                i.enabled = state;
            }
        }
    }
    */
    #endregion
}