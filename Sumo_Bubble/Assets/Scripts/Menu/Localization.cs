using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Script helps game in localizing strings</summary>
public class Localization : MonoBehaviour
{
    #region LangPack_StructDef
    /// <summary>Struct holding localization strings for each language</summary>
    public struct LangPack
    {
        private readonly string English;
        private readonly string Spanish;
        private readonly string French;
        private readonly string Chinese;
        private readonly string Russian;

        public LangPack(LangPack newPack)
        {
            this.English = newPack.english;
            this.Spanish = newPack.Spanish;
            this.French = newPack.french;
            this.Chinese = newPack.chinese;
            this.Russian = newPack.russian;
        }

        public LangPack(string English, string Spanish, string French, string Chinese, string Russian)
        {
            this.English = English;
            this.Spanish = Spanish;
            this.French = French;
            this.Chinese = Chinese;
            this.Russian = Russian;
        }

        /// <summary>Overload of LangPack, that sets all localized strings to one version. Used for setting all langs to one string</summary>
        /// <param name="msg">string for all langs</param>
        public LangPack(string msg)
        {
            this.English = msg;
            this.Spanish = msg;
            this.French = msg;
            this.Chinese = msg;
            this.Russian = msg;
        }

        /// <summary>Overload of LangPack, that sets english string, others to whatever. Used for placeholders mostly</summary>
        /// <param name="English">English string</param>
        /// <param name="Foreign">Foreign string</param>
        public LangPack(string English, string Foreign)
        {
            this.English = English;
            this.Spanish = Foreign;
            this.French = Foreign;
            this.Chinese = Foreign;
            this.Russian = Foreign;
        }
        public string english { get { return English; } }
        public string spanish { get { return Spanish; } }
        public string french { get { return French; } }
        public string chinese { get { return Chinese; } }
        public string russian { get { return Russian; } }

        /// <summary>Psuedo-GET function for fetching appropriate string for language set in GameManager</summary>
        public string language
        {
            get
            {
                string result = "";                                     //String to return
                GameManager.Language lang = GetLangCurrent;
                //Fetch the appropriate string for this language
                switch (lang)
                {
                    case GameManager.Language.LANG_ENGLISH:
                        result = english;
                        break;
                    case GameManager.Language.LANG_SPANISH:
                        result = spanish;
                        break;
                    case GameManager.Language.LANG_FRENCH:
                        result = french;
                        break;
                    case GameManager.Language.LANG_CHINESE:
                        result = chinese;
                        break;
                    case GameManager.Language.LANG_RUSSIAN:
                        result = russian;
                        break;
                }
                return result;
            }
        }

        /// <summary>Gets the current language</summary>
        public GameManager.Language GetLangCurrent
        {
            get
            {
                //Language to return
                GameManager.Language lang = GameManager.Language.LANG_NONE;

                //Get the GM instance; if exists, fetch it's lang
                GameManager GM = null;
                bool exists = GetGM(ref GM);
                if (exists) { lang = GameManager.instance.lang; }
                return lang;
            }
        }
    }

    /// <summary>Overload of LangPack, with members as public and mutable. Used for serialization with TextSwitch components etc</summary>
    [System.Serializable]
    public struct LangPack2
    {
        public string English;
        public string Spanish;
        public string French;
        public string Chinese;
        public string Russian;

        public LangPack2(LangPack2 newPack)
        {
            this.English = newPack.english;
            this.Spanish = newPack.spanish;
            this.French = newPack.french;
            this.Chinese = newPack.chinese;
            this.Russian = newPack.russian;
        }

        public LangPack2(string English, string Spanish, string French, string Chinese, string Russian)
        {
            this.English = English;
            this.Spanish = Spanish;
            this.French = French;
            this.Chinese = Chinese;
            this.Russian = Russian;
        }

        /// <summary>Overload of LangPack, that sets all localized strings to one version. Used by XBone or things I'm too lazy for</summary>
        /// <param name="English"></param>
        public LangPack2(string English)
        {
            this.English = English;
            this.Spanish = English;
            this.French = English;
            this.Chinese = English;
            this.Russian = English;
        }
        public string english { get { return English; } }
        public string spanish { get { return Spanish; } }
        public string french { get { return French; } }
        public string chinese { get { return Chinese; } }
        public string russian { get { return Russian; } }

        /// <summary>Psuedo-GET function for fetching appropriate string for language set in GameManager</summary>
        public string language
        {
            get
            {
                string result = "";                                     //String to return
                GameManager.Language lang = GetLangCurrent;
                //Fetch the appropriate string for this language
                switch (lang)
                {
                    case GameManager.Language.LANG_ENGLISH:
                        result = english;
                        break;
                    case GameManager.Language.LANG_SPANISH:
                        result = spanish;
                        break;
                    case GameManager.Language.LANG_FRENCH:
                        result = french;
                        break;
                    case GameManager.Language.LANG_CHINESE:
                        result = chinese;
                        break;
                    case GameManager.Language.LANG_RUSSIAN:
                        result = russian;
                        break;
                }
                return result;
            }
        }

        /// <summary>GET function for fetching appropriate string for language set in GameManager</summary>
        public string language2(GameManager.Language lang)
        {
            string result = "";                                     //String to return
            //Fetch the appropriate string for this language
            switch (lang)
            {
                case GameManager.Language.LANG_ENGLISH:
                    result = english;
                    break;
                case GameManager.Language.LANG_SPANISH:
                    result = spanish;
                    break;
                case GameManager.Language.LANG_FRENCH:
                    result = french;
                    break;
                case GameManager.Language.LANG_CHINESE:
                    result = chinese;
                    break;
                case GameManager.Language.LANG_RUSSIAN:
                    result = russian;
                    break;
            }
            return result;
        }

        /// <summary>Gets the current language</summary>
        public GameManager.Language GetLangCurrent
        {
            get
            {
                GameManager.Language lang = GameManager.Language.LANG_NONE;
                
                GameManager GM = null;
                bool exists = GetGM(ref GM);
                if (exists){lang = GameManager.instance.lang;}
                return lang;
            }
        }
    }
    #endregion

    //private GameManager.Language oldLang = GameManager.Language.LANG_ENGLISH;   //Language on previous frame

    #region Functions
    /*
    public void _Start()
    {
        StartCoroutine(Start());
    }

    public IEnumerator Start()
    {
        //Yield until GameManager is found
        yield return new WaitForSeconds(.1f);
    }
    
    /// <summary>Update items when language changes. !@ OLD CODE</summary>
    void Update()
    {
        //If GM exists
        if (GameManager.instance)
        {
            GameManager.Language Lang = GameManager.instance.lang;  //Get current language

            //If a delta occured with the language
            if (oldLang != Lang)
            {
                oldLang = GameManager.instance.lang;                //Update old language (deadlock out of spamming this block)
                LangPackComp[] comp = GameObject.FindObjectsByType<LangPackComp>(FindObjectsSortMode.None);
                foreach (LangPackComp c in comp){c.UpdateText(Lang);}
            }
            oldLang = GameManager.instance.lang;                    //Update old language
        }
    }
    */

    /// <summary>Attempts to set a font asset to the applicable text object type (Text, TMPro, TMProUGUI)</summary>
    /// <param name="O">Text object thingy</param>
    /// <param name="lang">Font language</param>
    /// <param name="onTop">use onTop variant?</param>
    /// <returns>Success?</returns>
    public static bool SetFontAsset_Localz(System.Object O, GameManager.Language lang, bool onTop)
    {
        bool success = false;   //Successful?

        try
        {
            //Try casting the object as each type
            Text T = (Text)(O);
            TMPro.TextMeshPro pro = (TMPro.TextMeshPro)(O);
            TMPro.TextMeshProUGUI proUI = (TMPro.TextMeshProUGUI)(O);

            //If thingy found, attempt localizing it for the current language
            if (T)
            {
                success = SetFontAsset_Localz(T, lang, onTop);
            }
            else if (pro)
            {
                success = SetFontAsset_Localz(pro, lang, onTop);
            }
            else if (proUI)
            {
                success = SetFontAsset_Localz(proUI, lang, onTop);
            }
        }
        catch { }
        return success;
    }

    /// <summary>Attempts to set a language font asset for a TMPro.TextMeshPro object
    /// <param name="pro">TMPro.TextMeshPro text object</param>
    /// <param name="lang">Font language</param>
    /// <param name="onTop">use onTop variant?</param>
    /// <returns>Successful?</returns>
    public static bool SetFontAsset_Localz(TMPro.TextMeshPro pro, GameManager.Language lang, bool onTop)
    {
        bool success = false;                   //Successful?
        TMPro.TMP_FontAsset font = null;        //Font to apply (if any)

        try
        {
            //Get the GM ref
            GameManager GM = null;
            bool exists = GetGM(ref GM);
            if (exists)
            {
                //If exists, conv language to index, fetch font from GameManager CapFonts_TMP
                byte index = (byte)(lang);

                //if font exists, override
                if (onTop){font = GM.CapFonts_OnTop_TMP[index];}
                else{font = GM.CapFonts_TMP[index];}
                if (font != null) { pro.font = font; }

                //Successful!
                success = true;
            }
        }
        catch { }
        return success;
    }

    /// <summary>Attempts to set a language font asset for a TMPro.TextMeshProUGUI object
    /// <param name="pro">TMPro.TextMeshPro text object</param>
    /// <param name="lang">Font language</param>
    /// <param name="onTop">use onTop variant?</param>
    /// <returns>Successful?</returns>
    public static bool SetFontAsset_Localz(TMPro.TextMeshProUGUI pro, GameManager.Language lang, bool onTop)
    {
        bool success = false;                   //Successful?
        TMPro.TMP_FontAsset font = null;        //Font to apply (if any)

        try
        {
            //Get the GM ref
            GameManager GM = null;
            bool exists = GetGM(ref GM);
            if (exists)
            {
                //If exists, conv language to index, fetch font from GameManager CapFonts_TMP
                byte index = (byte)(lang);

                //if font exists, override
                if (onTop) { font = GM.CapFonts_OnTop_TMP[index]; }
                else { font = GM.CapFonts_TMP[index]; }
                if (font != null) { pro.font = font; }

                //Successful!
                success = true;
            }
        }
        catch { }
        return success;
    }

    /// <summary>Attempts to set a language font asset for a standard Text object
    /// <param name="T">Text object</param>
    /// <param name="lang">Font language</param>
    /// <param name="onTop">use onTop variant?</param>
    /// <returns>Successful?</returns>
    public static bool SetFontAsset_Localz(Text T, GameManager.Language lang, bool onTop)
    {
        bool success = false;       //Successful?
        Font font = null;           //Font to apply (if any)
        try
        {
            //Get the GM ref
            GameManager GM = null;
            bool exists = GetGM(ref GM);
            if (exists)
            {
                //If exists, conv language to index, fetch font from GameManager CapFonts
                byte index = (byte)(lang);
                font = GM.CapFonts[index];
                //if font exists, override
                if (font != null) { T.font = font; }

                //Successful!
                success = true;
            }
        }
        catch { }
        return success;
    }
    #endregion

    #region HelperFuncs
    /// <summary>Gets the Gamemanager (if exists)</summary>
    /// I/O (ByRef):
    /// <param name="GM">GameManager</param>
    /// Output (ByVal):
    /// <returns>Exists?</returns>
    private static bool GetGM(ref GameManager GM)
    {
        GM = GameManager.instance;
        bool result = (GM != null);
        return result;
    }
    #endregion
}