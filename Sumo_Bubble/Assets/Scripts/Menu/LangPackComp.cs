using System;
using ByteSheep.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Monobeh component script for LangPack2</summary>
public class LangPackComp : MonoBehaviour
{
    #region Setup
    [Header("Setup")]
    /// <summary>The LangPack2</summary>
    public Localization.LangPack2 pack;

    [Header("Params")]
    /// <summary>Use onTop font material variant?</summary>
    public bool onTop = false;
    /// <summary>Printf vars for printing (if any)</summary>
    public object[] vars = null;

    [Header("Events")]
    /// <summary>Update event. Usually you should use StdUpdate(Language) event</summary>
    public AdvancedIntEvent onUpdate = null;    //Event to run onUpdate
    /// <summary>The previous language. If language changed, run onUpdate</summary>
    private GameManager.Language oldLang = GameManager.Language.LANG_NONE;
    /// <summary>Force an update (from var update etc)?</summary>
    private bool forceUpdate = false;
    #endregion

    #region StdUnityEvents
    public void Update()
    {
        DoUpdateText();
    }
    #endregion

    #region Methods
    /// <summary>Function to update the text based on current language; runs onUpdate event
    public void DoUpdateText()
    {
        //Get the Getmanager and check if onUpdate event exists
        GameManager GM = null;
        bool exists = GetGM(ref GM);
        bool exists2 = (onUpdate != null);
        bool good = (exists && exists2);
        if (good)
        {
            //If everything good

            //Get the current Language and conv to Int
            GameManager.Language newLang = GM.lang;
            int L = (int)(newLang);

            //We are good to update if the new lang is different from oldLang (delta)
            bool _update = ((newLang != oldLang) || (forceUpdate));
            if (_update){onUpdate.Invoke(L);}
            if (forceUpdate){forceUpdate = false;}
        }
    }

    /// <summary>Standard update event, using Language cast as int
    /// <param name="lang">Int lang enum value</param>
    public void StdUpdate(int lang)
    {
        GameManager.Language L = (GameManager.Language)(lang);
        StdUpdate(L);
    }

    /// <summary>Standard update event. Retrieves either text, TMPro, or TMProUI element on component and updates localized text</summary>
    /// <param name="lang">Localz language</param>
    public void StdUpdate(GameManager.Language lang)
    {
        //Set the old language
        Set_oldLang(lang);

        //Get the localized string from pack
        string text = pack.language2(lang);
        //If has variables, then printf the string with current variables
        bool hasV = hasVars();
        if (hasV){text = String.Format(text, vars);}

        //Try getting either a std text element, TMPro, or TMProUGUI text comp; if something exists, set text and language font
        try
        {
            Text t = this.gameObject.GetComponent<Text>();
            TMPro.TextMeshPro pro = this.gameObject.GetComponent<TMPro.TextMeshPro>();
            TMPro.TextMeshProUGUI proUI = this.gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            if (t)
            {
                t.text = text;
                Localization.SetFontAsset_Localz(t, lang, onTop);
            }
            else if (pro)
            {
                pro.text = text;
                Localization.SetFontAsset_Localz(pro, lang, onTop);
            }
            else if (proUI)
            {
                proUI.text = text;
                Localization.SetFontAsset_Localz(proUI, lang, onTop);
            }
        }
        catch
        {
            //I AM ERROR!
            Debug.LogError("Failed to get valid text component for LangPackComp!");
        }
    }

    /// <summary>Changes to a new langPack for updates, and force an update</summary>
    /// <param name="newPack">New langPack</param>
    public void SetPack_New(Localization.LangPack2 newPack)
    {
        pack = new Localization.LangPack2(newPack);
        forceUpdate = true;
    }

    /// <summary>Setup variables, and force update</summary>
    /// <param name="v">Variables</param>
    public void SetVars(object[] v)
    {
        vars = v;
        forceUpdate = true;
    }

    /// <summary>Sets the oldLang language enum value// </summary>
    /// <param name="old">old Language enum value</param>
    public void Set_oldLang(GameManager.Language old)
    {
        oldLang = old;
    }
    #endregion

    #region HelperFuncs
    /// <summary>Does this text comp have vars?</summary>
    /// <returns>Has?</returns>
    private bool hasVars()
    {
        //Has vars if NOT null AND has length
        bool result = false;
        if (vars != null){result = (vars.Length > 0);}
        return result;
    }

    /// <summary>Gets the Gamemanager (if exists)</summary>
    /// I/O (ByRef):
    /// <param name="GM">GameManager</param>
    /// Output (ByVal):
    /// <returns>Exists?</returns>
    private bool GetGM(ref GameManager GM)
    {
        GM = GameManager.instance;
        bool result = (GM != null);
        return result;
    }
    #endregion
}