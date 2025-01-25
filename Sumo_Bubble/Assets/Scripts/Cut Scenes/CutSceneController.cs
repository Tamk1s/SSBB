using ByteSheep.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>Controller for the CutScene system</summary>
public class CutSceneController : MonoBehaviour
{
    #region Properties
    public static CutSceneController instance;          //This singleton instance
    public MenuControls controls;
    public bool options = false;
    public static int maxCharsPerLine = 80-1;           //Max amount of characters per line.
                                                        //public GameObject currentJaw;                     //!@Current jaw jabbering

    public CutScenePanel panel;                         //The CutScene Panel for displaying the text/faces
    public bool onStart = false;                        //Begin the cutscene on script Start?
    public bool isLocked = false;                       //Is player control of this cutscene locked?    
    private bool oldInterruptable = false;              //Previous Interruptable state
    private UnityEngine.Coroutine oldWriteMsg = null;

    /// <summary>CSPanel element IDs</summary>
    private enum CSPanel_Elements
    {
        MAIN,
        OPTION_CTRLS,
    };
    private const byte elements_MAX = (byte)(CSPanel_Elements.OPTION_CTRLS + 0x01);
    [NamedArrayAttribute(typeof(CSPanel_Elements))]
    public CanvasGroup[] groups = new CanvasGroup[elements_MAX];
    [NamedArrayAttribute(typeof(CSPanel_Elements))]
    public CanvasGroupAlpha[] groupAlphas = new CanvasGroupAlpha[elements_MAX];
    #endregion

    #region Constructor
    //!@
    /// <summary>Data for this current cutscene. This allows playing of several cutscene dialogs from CutSceneData holder scripts by struct copying!</summary>
    [System.Serializable]
    public struct CutSceneData
    {
        [Header("Dialog parameters")]
        public bool interruptable;              //Can this CSData be interrupted? (Example, Starfox 64 Slippy moaning when hit while other cutscene dialog is happening). Set to false to not allow interrupting (ex: post-boss level explosions cutscenes)
        public bool delay;                      //Use onStartDelay?
        public float onStartDelay;              //Amount of time to delay the onStart event
        //public float typeSpeed;               //Speed of typing each char
        //public float skipTypeSpeed;           //Faster speed of typing each char when speeding up
        public AdvancedEvent onCutsceneStart;   //Events to fire onCutsceneStart
        public AdvancedEvent onCutsceneEnd;     //Events to fire onCutsceneEnd
        public CutSceneChunk[] cutSceneData;    //Array of cutscene chunks to execute

        /// <summary>Creates defaults for a new CutSceneData struct</summary>
        /// <param name="dum">Dummy param</param>
        public CutSceneData(byte dum = 0x00)
        {
            interruptable = true;
            delay = true;
            onStartDelay = 1f;
            //typeSpeed = .1f;
            //skipTypeSpeed = .01f;
            onCutsceneStart = new AdvancedEvent();
            onCutsceneEnd = new AdvancedEvent();
            cutSceneData = new CutSceneChunk[1];
        }
    }
    public CutSceneData CSData = new CutSceneData();    //CutsceneData for this controller
    #endregion

    #region Variables
    //!@Disable the [HideInInspector] attributes for debugging
    //[HideInInspector]
    public int currentChunk = 0;            //Current chunk being processed
    //[HideInInspector]
    public int currentChunkExpression = 0;  //Current expression being processed
    //[HideInInspector]
    public bool isWritingExpression = false;//Is the CSC writing some text?
    //[HideInInspector]
    public bool isWaiting = false;          //Is the CSC done with printing of text and waiting for user input to goto next expression/chunk?
    //private float currentTypeSpeed;         //Current typing speed
    #endregion

    #region Events
    private void Start()
    {
        //Setup the singleton instance to this
        instance = this;

        //If onStart event, invoke the StartCutscene function with the delay
        if (onStart) { StartCutscene(CSData.delay); }
    }

    private void Update()
    {
        HandleInterruption();
    }
    #endregion

    #region Cutscene_funcs
    /// <summary>Is the cutscene running?</summary>
    /// <returns>Running?</returns>
    public bool isRunning()
    {
        bool result = (isWaiting || isWritingExpression);
        return result;
    }

    /// <summary>Handle interrruption of cutscene as appropriate</summary>
    private void HandleInterruption()
    {
        //Get interruptable flag from data/options
        bool state = false;

        if (options)
        { state = false; }
        else
        { state = CSData.interruptable; }
        controls.enabled = state;
    }

    /// <summary>Function runs when cutscene starts</summary>
    /// <param name="delay">Delay the cutscene by onStartDelay?</param>
    public void StartCutscene(bool delay = false)
    {
        StartCutscene(0, delay);
    }

    /// <summary>Same as StartCutscene(delay), but can specify the starting currentChunk and currentChunkExpression. For dynamic battle stuff</summary>
    /// <param name="_currentChunk">ChunkID</param>
    /// <param name="delay">Delay</param>
    public void StartCutscene(int _currentChunk, bool delay = false)
    {
        //!@ Since we can now dump new CSData and start new cutscenes dynamically (with interruptions),
        //we need to reset these scratch RAM variables
        //to init for proper running of new cutscenes!
        //And delete the old jaw (if any)

        StopAllCoroutines();                        //Stop all corouts
        oldInterruptable = CSData.interruptable;    //Stash previous interruptable state
        CSData.interruptable = false;               //Reset interruptable flag
        isLocked = true;                            //Lock ctrls while delaying!

        //Reset currentChunk/expression to 0, reset isWaiting & isWritingExpression flags, set typeSpeed to 0f
        currentChunk = _currentChunk;
        currentChunkExpression = 0;
        isWritingExpression = false;
        isWaiting = false;
        //currentTypeSpeed = 0f;

        //Start cutscene corout with delay
        StartCoroutine(_StartCutScene(currentChunk, delay));
    }

    /// <summary>Actually runs the cutscene coroutine (for character typing)</summary>
    /// <param name="delay">Start delay timer</param>
    /// <returns>Coroutine cutscene typing</returns>
    private IEnumerator _StartCutScene(int chunkID = 0, bool delay = false)
    {
        //If delay flag set, then yield by timer
        if (delay) { yield return new WaitForSeconds(CSData.onStartDelay); }
        CSData.onCutsceneStart.Invoke();            //Invoke the onCutsceneStart event
        /*
        QuestSystem QS = QuestSystem.instance;
        if (QS)
        {
            //Fetch appropriate chunk for questID status, if any
            QuestSystem.questID questID = CSData.QuestID;                                                               //questID of cutscene
            bool validQuest = QuestSystem.isValidQuest(questID);
            if (validQuest)
            {
                //If valid quest, fetch appropriate chunkID            
                QuestSystem.questState state = QS.GetStatus(questID);                                                   //Get the current state of this quest
                List<CutSceneChunk> data = CSData.cutSceneData.ToList<CutSceneChunk>();                                //Convert CSData.cutsceneData to list for easier processing
                CutSceneChunk _chunk = data.Where(t => (t.questStateType == state)).FirstOrDefault<CutSceneChunk>();    //Get the 1st/only chunk from data where the questStates match
                chunkID = data.IndexOf(_chunk);                                                                         //Get chunk's index
            }
            */
            StartChunk(chunkID);                        //Start appropriate chunk
            isLocked = false;                           //Restore controls
            CSData.interruptable = oldInterruptable;    //Restore previous interruptable state
            yield break;                                //Dummy yield NOP
        //}
    }

    /// <summary>Starts the specified chunk index</summary>
    /// <param name="index">Chunk ID</param>
    public void StartChunk(int index)
    {
        isWaiting = false;
        currentChunk = index;                   //Set current chunk to index

        //if current chunk is not final, set expression to 0, then start the specified chunk and 0th expression; else invoke CutsceneEnd event
        if (currentChunk < CSData.cutSceneData.Length)
        {
            currentChunkExpression = 0;
            StartExpressionSequence(currentChunk, currentChunkExpression);
        }
        else
        {
            //!@ Destroy old jaw
            //CutSceneActorInfo.instance.DestroyJaw(currentJaw);
            StopVox();
            CSData.onCutsceneEnd.Invoke();
        }
    }

    /// <summary>Speeds up the text or starts next chunk as appropriate</summary>
    public void SpeedUpOrStartNext(bool lockable = true)
    {
        //If lockable and locked, return
        if (lockable)
        {
            if (isLocked) { return; }
        }

        //If not waiting, and if writing expression, set current speed to faster one
        if (!isWaiting)
        {
            //Debug.Log("Not waiting, but writing expression");
            //if (isWritingExpression) { currentTypeSpeed = CSData.skipTypeSpeed; }
        }
        else
        {
            //Otherwise if waiting, do next chunk
            DoNext(false);
        }

        //We aren't waiting any more
        isWaiting = false;
    }

    /// <summary>Do next chunk</summary>
    /// <param name="wait">Reset wait flag?</param>
    public void DoNext(bool wait = true)
    {
        //If next expression is less than length, then increment chunk expression and then run it
        if ((currentChunkExpression + 1) < CSData.cutSceneData[currentChunk].expressions.Length)
        {
            Debug.Log("Doing next expression");
            currentChunkExpression++;
            StartExpressionSequence(currentChunk, currentChunkExpression);
        }
        else
        {
            Debug.Log("Doing next chunk");
            //Otherwise, reset next indicator arrow, and run onChunkComplete event
            panel.nextMarker.SetActive(false);
            CSData.cutSceneData[currentChunk].onChunkComplete.Invoke();
        }

        //If wait flag, reset waiting
        if (wait) { isWaiting = false; }
    }

    /// <summary>Skips a chunk</summary>
    public void SkipChunk()
    {
        //If locked, return
        if (isLocked) { return; }

        //If next chunk is less than cutscenedata length, then increment currentCHunk, set expression to 0, and start the chunk
        if ((currentChunk + 1) < CSData.cutSceneData.Length)
        {
            currentChunk++;
            currentChunkExpression = 0;
            StartChunk(currentChunk);
        }
        else
        {
            //Otherwise run onCutsceneEnd event
            CSData.onCutsceneEnd.Invoke();
        }
    }

    /// <summary>Toggles the locked state</summary>
    /// <param name="state">State</param>
    public void SetLock(bool state)
    {
        isLocked = state;
    }

    /// <summary>Toggles the interruptable state</summary>
    /// <param name="state"></param>
    public void SetInterruptable(bool state)
    {
        CSData.interruptable = state;
    }

    /// <summary>Starts the specified chunkIndex and expression</summary>
    /// <param name="chunkIndex">ChunkIndex ID</param>
    /// <param name="expressionIdex">ExpressionIndex ID</param>
    public void StartExpressionSequence(int chunkIndex, int expressionIdex)
    {
        Localization.LangPack2 pack;                                                                        //LangPack2 register

        isWritingExpression = true;                                                                         //We are writing an expression
        panel.expressionLabel.text = "";                                                                    //Set label to empty string
        //currentTypeSpeed = CSData.typeSpeed;                                                                //Set current speed to typespeed
        CSData.cutSceneData[chunkIndex].expressions[expressionIdex].onExpressionStart.Invoke();             //Invoke onExpressionStart for chunkindex/expression combo

        //CurrentActor
        CutSceneActorInfo.ActorID currentActor = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].byActor;

        //!@ Fetch the appropriate face of the talker
        //if (currentActor != null)
        Sprite newSprite = CutSceneActorInfo.instance.GetImage(currentActor);
        panel.byIcon.sprite = newSprite;

        //!@ Get the localized actor name
        //panel.byName.text = cutSceneData[chunkIndex].expressions[expressionIdex].byName;
        Localization.LangPack2 name = CutSceneActorInfo.instance.GetName2(currentActor);
        panel.byName.text = name.language;   //Set the name
        Localization.SetFontAsset_Localz(panel.byName, name.GetLangCurrent, false);

        //!@ Destroy old jaw, create new
        //if (currentJaw){CutSceneActorInfo.instance.DestroyJaw(currentJaw);}
        StopVox();
        Transform parent = panel.byIcon.gameObject.transform.parent;                                        //Get the byIcon parent

        //!@ For demo, just override all speech with NONE for nothing
        //Audio.VOX vox = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].speech;                 //Get the speech vox
        Audio.VOX vox = Audio.VOX.VOX_NULL;
        Debug.Log("Vox: " + vox.ToString());

        //currentJaw = CutSceneActorInfo.instance.CreateJaw(currentActor, parent, vox);                       //Create new jaw at parent with speech
        //Play the localized vocal clip
        Audio aud = this.gameObject.GetComponent<Audio>();
        if (aud) { aud.vox_play2(vox); }


        //!@ Write the options (if any)
        options = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].useOptions;
        ToggleCanvasElements(!options);
        if (options)
        {
            panel.item_OptionA.onActivate = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].onActivate_OptA;
            panel.item_OptionB.onActivate = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].onActivate_OptB;

            //Get the expressionOptA langpack, then localized text
            pack = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].expressionOptA;
            string msg2 = pack.language;
            WriteOption(msg2, true, pack.GetLangCurrent);
            //Get the expressionOptB langpack, then localized text
            pack = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].expressionOptB;
            msg2 = pack.language;
            WriteOption(msg2, false, pack.GetLangCurrent);
        }

        //Write the message 
        //Get the current expression pack and localized text
        pack = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].expression;
        string msg = pack.language;
        bool dynamic = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].dynamic;
        if (dynamic)
        {
            object[] objs = CSData.cutSceneData[chunkIndex].expressions[expressionIdex].vars;
            msg = string.Format(msg, objs);
        }

        //Stop the previous write message if still running, then write new message
        if (oldWriteMsg != null) { StopCoroutine(oldWriteMsg); }
        oldWriteMsg = StartCoroutine(WriteExpression(msg, pack.GetLangCurrent));
    }

    /// <summary>Writes an expression.</summary>
    /// <param name="expressionText">Expression to write</param>
    /// <param name="lang">Font language to use</param>
    /// <returns>IEnumerator coroutine</returns>
    public IEnumerator WriteExpression(string expressionText, GameManager.Language lang)
    {
        //int currentLetterIndex = 0;                                             //currentLetter index
        panel.nextMarker.SetActive(false);                                      //Set the next marker thing active

        Localization.SetFontAsset_Localz(panel.expressionLabel, lang, false);          //Setup the font language
        //While the currentletter is not the max
        //while (currentLetterIndex < expressionText.Length)
        //{
            //Yield by type speed
            //yield return new WaitForSeconds(currentTypeSpeed);

            //Add the char to the label text
            panel.expressionLabel.text = expressionText;
            
            //panel.expressionLabel.text += expressionText[currentLetterIndex];
            //currentLetterIndex++;   //Increment letter index
            yield return new WaitForSeconds(.25f);
        //}

        bool inter = CSData.interruptable;                                      //Get current interruptable state
        panel.nextMarker.SetActive(inter);                                      //Toggle the nextmarker thing
        isWaiting = true;                                                       //Toggle waiting
        isWritingExpression = false;                                            //We aren't writing an expression

        //If not interruptable, then run WriteExpressionInterEnd()
        if (!inter) { StartCoroutine(WriteExpressionInterEnd()); }
    }

    /// <summary>Sets the text for a particular option dialog</summary>
    /// <param name="msg">Text</param>
    /// <param name="optionA">Use OptionA; else OptionB</param>
    /// <param name="lang">Language for font</param>
    private void WriteOption(string msg, bool optionA, GameManager.Language lang)
    {
        if (optionA)
        {
            //Setup the localized font language
            Localization.SetFontAsset_Localz(panel.txt_OptionA, lang, false);
            panel.txt_OptionA.text = msg;
        }
        else
        {
            //Setup the localized font language
            Localization.SetFontAsset_Localz(panel.txt_OptionB, lang, false);
            panel.txt_OptionB.text = msg;
        }
    }

    /// <summary>!@ Stop any lingering vocals</summary>
    private void StopVox()
    {
        Audio aud = this.gameObject.GetComponent<Audio>();
        if (aud) { aud.sfx_stop(); }
    }

    /// <summary>Ends WriteExpression function when interruption flag is disabled. Yields ending cutscene expression while jaw vocal is jabbering</summary>
    /// <returns>Yield coroutine</returns>
    private IEnumerator WriteExpressionInterEnd()
    {
        //if (currentJaw)
        //{
        //If currentJaw found

        //Get Audio component from jaw; while it is playing, safety NOP until done vocal is done jabbering
        //!@ Audio audio = currentJaw.gameObject.GetComponent<Audio>();
        Audio audio = this.gameObject.GetComponent<Audio>();
        bool isPlaying = audio.audio_sfx.isPlaying;
        while (isPlaying)
        {
            yield return new WaitForSeconds(.1f);
            isPlaying = audio.audio_sfx.isPlaying;
        }
        //}

        //DO next expression but don't wait
        DoNext(false);
    }

    /// <summary>Toggles all CanvsGroup(Alpha) components in children. Used to allow option dialog clicking (UI)</summary>
    /// <param name="state">State</param>
    public void ToggleCanvasElements(bool optState, bool mainState = false)
    {
        byte _i = 0x00;         //Generic byte iterator
        byte _max = 0x00;       //Generic max index
        bool newState = false;  //Generic bool
        //Generic gameobject, Button, Image, Sprite refs
        GameObject g = null;
        EventTrigger T = null;
        Button B = null;
        Image I = null;
        Sprite S = null;

        //Iterat through all canvasgroup/alpha elements
        CSPanel_Elements i = CSPanel_Elements.MAIN;
        CSPanel_Elements max = CSPanel_Elements.OPTION_CTRLS;
        for (i = CSPanel_Elements.MAIN; i <= max; i++)
        {
            //Get index, get group/alpha components
            _i = (byte)(i);
            CanvasGroup group = groups[_i];
            CanvasGroupAlpha alpha = groupAlphas[_i];

            //Get appropriate state
            newState = false;
            switch (i)
            {
                case CSPanel_Elements.MAIN:
                    newState = mainState;
                    break;

                case CSPanel_Elements.OPTION_CTRLS:
                    newState = optState;
                    break;
            }

            //Set new states
            group.enabled = newState;
            alpha.enabled = newState;
        }

        //Set newState for controls sprites as opposite of optState
        newState = optState;

        //Iterate through all controls elements
        _max = CutScenePanel.maxControls;
        for (_i = 0x00; _i < _max; _i++)
        {
            //Find the control gameobject
            g = panel.controls[_i].gameObject;
            if (g)
            {
                //If found, tryGet Button and Image components
                T = g.GetComponent<EventTrigger>();
                B = g.GetComponent<Button>();
                I = g.GetComponent<Image>();
                if (B && I && T)
                {
                    //if found

                    //Set new Button and Trigger enable states
                    T.enabled = newState;
                    B.enabled = newState;

                    //Set new sprite, based on new enableState
                    if (newState)
                    { S = panel.enableSprites[_i]; }
                    else
                    { S = panel.disableSprites[_i]; }
                    I.sprite = S;
                }
            }
        }
    }
    #endregion

    #region Shims
    /// <summary>Gets the MMC singleton instance, and if it exists</summary>
    /// I/O (ByRef):
    /// <param name="exists">Exists?</param>
    /// Output:
    /// <returns>MainMenuController singleton</returns>
    private MainMenuController GetMMC(ref bool exists)
    {
        MainMenuController MMC = MainMenuController.instance;
        exists = (MMC != null);
        return MMC;
    }
    #endregion
}