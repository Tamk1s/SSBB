using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Rewired;
using System.Linq;
using ByteSheep.Events;

/// <summary>
/// Allows remapping of controls for Rewired for StarEagle
/// Based on SimpleControlRemapping, but extended for better flexibility
/// </summary>
public class ControlRemapper : MonoBehaviour
{
    private const string clrGreen = "<color=#55FF55>";  //Green richtext
    private const string clrWhite = "<color=#FFFFFF>";  //White richtext
    private const string endColor = "</color>";         //Richtext Color closing tag
    private const string category = "Default";          //category
    private const string layout = "Default";            //layout

    private InputMapper inputMapper = new InputMapper();    //InputMapper

    //!@
    public MenuControls MC;                                 //MenuControls to toggle when mapping
    private RectTransform origParent;                       //Original parent of this gameobject
    public RectTransform newParent;                         //New parent of this gameobject to move to (goes to DummyMenu). Needed to prevent raycast blocking for buttons
    public RectTransform assignmentsParent;                 //Root RT of assignments
    
    //!@
    //public GameObject buttonPrefab;
    //public GameObject textPrefab;
    //public RectTransform fieldGroupTransform;
    //public RectTransform actionGroupTransform;    
    //public Text controllerNameUIText;

    //Array of buttons, aligned with ActionsFull enum    
    [NamedArrayAttribute(typeof(RewiredConsts.Action.ActionsFull))]
    public GameObject[] buttonGo = new GameObject[(byte)((RewiredConsts.Action.ActionsFull.MAX) + 1)];
    public Text statusUIText;   //Status text

    //!@
    public int playerID;                                                    //PlayerID for this remapper
    public ControllerType selectedControllerType = ControllerType.Keyboard; //Main type of controller
    [NamedArrayAttribute(typeof(RewiredConsts.Layout.Joystick.Types))]
    //Previous subtypes for the main joystick. Used for delta-checking between current and old to detect changes
    private bool[] oldTypes = new bool[(byte)(RewiredConsts.Layout.Joystick.Types.MAX) + 1];

    //!@
    //Only used if selectedControllerType == ControllerType.Joystick
    //Types of joysticks available for name displaying (any type)
    [NamedArrayAttribute(typeof(RewiredConsts.Layout.Joystick.Types))]
    public bool[] inc_joyStickTypes = new bool[(byte)(RewiredConsts.Layout.Joystick.Types.MAX) + 1];

    //!@
    //Only used if selectedControllerType == ControllerType.Joystick
    //Types of joysticks unavailable for name displaying (any type). If controller is any of these types, ignore
    [NamedArrayAttribute(typeof(RewiredConsts.Layout.Joystick.Types))]
    public bool[] exc_joyStickTypes = new bool[(byte)(RewiredConsts.Layout.Joystick.Types.MAX) + 1];

    //!@
    //Only used if selectedControllerType == ControllerType.Joystick
    //Types of joysticks available for remapping, but readonly display of hardcoded, default maps (ex. XInput controllers)
    //True values should be a subset of inc_joyStickTypes true values
    [NamedArrayAttribute(typeof(RewiredConsts.Layout.Joystick.Types))]
    public bool[] joyStickTypes_R = new bool[(byte)(RewiredConsts.Layout.Joystick.Types.MAX) + 1];
    //Events to run when a type of joystick equals the appropriate joyStickTypes_R types
    public AdvancedEvent[] joyStickTypes_R_Events = new AdvancedEvent[2];   //0th=events to run when no matches, 1st= events to run when matches
    public bool eventsReady = false;                                        //Semaphore if ready to run events?

    //!@
    //Enable state for each action, aligned with ActionsFull enum
    //This allows toggling remapping of certain actions (ex: disallow remapping of dedicated throttle axis for gamepad joystick menu)
    [NamedArrayAttribute(typeof(RewiredConsts.Action.ActionsFull))]
    public bool[] actionStates = new bool[(byte)((RewiredConsts.Action.ActionsFull.MAX) + 1)];

    //Current controllerID. Only handle the 0th of type (no multiple gamepads)
    private int selectedControllerId = 0;                                   
    public List<Row> rows = new List<Row>();    //List of rows

    private Player player { get { return ReInput.players.GetPlayer(playerID); } }
    private ControllerMap controllerMap
    {
        get
        {
            if (controller == null){return null;}
            return player.controllers.maps.GetMap(controller.type, controller.id, category, layout);
        }
    }
    private Controller controller
    {
        get{return player.controllers.GetController(selectedControllerType, selectedControllerId);}
    }

    private void Start()
    {
        //!@ Get the origParent onStart
        origParent = this.gameObject.transform.parent.gameObject.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // don't run if Rewired hasn't been initialized
        if (!ReInput.isReady) return;

        // Timeout remapper after 5 seconds of listening
        inputMapper.options.timeout = 5f;

        // Ignore Mouse X and Y axes, modifier keys for remapper
        inputMapper.options.ignoreMouseXAxis = true;
        inputMapper.options.ignoreMouseYAxis = true;

        //!@
        //Disallow modifier keys for remapper
        inputMapper.options.allowKeyboardKeysWithModifiers = false;
        inputMapper.options.allowKeyboardModifierKeyAsPrimary = false;

        // Subscribe to events
        ReInput.ControllerConnectedEvent += OnControllerChanged;
        ReInput.ControllerDisconnectedEvent += OnControllerChanged;
        inputMapper.InputMappedEvent += OnInputMapped;
        inputMapper.StoppedEvent += OnStopped;

        // Create UI elements
        InitializeUI();
    }

    private void OnDisable()
    {
        // Make sure the input mapper is stopped first
        inputMapper.Stop();

        // Unsubscribe from events
        inputMapper.RemoveAllEventListeners();
        ReInput.ControllerConnectedEvent -= OnControllerChanged;
        ReInput.ControllerDisconnectedEvent -= OnControllerChanged;
    }

    /// <summary>Updates the UI buttons' text for name/actions</summary>
    private void RedrawUI()
    {
        if (controller == null)
        {
            // no controller is selected
            ClearUI();
            return;
        }

        // Update joystick name in UI
        //controllerNameUIText.text = controller.name;

        // Update each button label with the currently mapped element identifier
        for (int i = 0; i < rows.Count; i++)
        {
            Row row = rows[i];
            InputAction action = rows[i].action;

            string name = string.Empty;
            int actionElementMapId = -1;

            // Find the first ActionElementMap that maps to this Action and is compatible with this field type
            foreach (var actionElementMap in controllerMap.ElementMapsWithAction(action.id))
            {
                if (actionElementMap.ShowInField(row.actionRange))
                {
                    name = actionElementMap.elementIdentifierName;
                    actionElementMapId = actionElementMap.id;
                    break;
                }                
            }

            //!@
            // Set the label in the field button
            string actName = row.actionDescriptiveName;     //Get name of the action
            string btnName = clrWhite + name + endColor;    //Get the buttons' name (in white color)
            string newName = actName + "=" + btnName;       //Set newname as action = button
            row.text.text = newName;                        //Apply new name

            // Set the field button callback
            row.button.onClick.RemoveAllListeners(); // clear the button event listeners first
            int index = i; // copy variable for closure

            //!@
            //row.button.onClick.AddListener(() => OnInputFieldClicked(index, actionElementMapId));
            row.button.onClick.AddListener(() => OnInputFieldClicked(MC, index, actionElementMapId));
        }
    }

    /// <summary>Clears all btn names</summary>
    private void ClearUI()
    {
        //Clear the controller name
        //if (selectedControllerType == ControllerType.Joystick) controllerNameUIText.text = "No joysticks attached";
        //else controllerNameUIText.text = string.Empty;

        // Clear button labels
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].text.text = string.Empty;
        }
    }

    /// <summary>Initz button names/elements</summary>
    private void InitializeUI()
    {
        //!@
        /*
        // Delete placeholders
        foreach (Transform t in actionGroupTransform)
        {
            Object.Destroy(t.gameObject);
        }
        foreach (Transform t in fieldGroupTransform)
        {
            Object.Destroy(t.gameObject);
        }
        */

        // Create Action fields and input field buttons
        //!@
        byte ID = 0;    //ID of action iteration, aligned with RewiredConsts.Action.ActionsFull enum
        foreach (var action in ReInput.mapping.Actions)
        {
            if (action.type == InputActionType.Axis)
            {
                // Create a full range, one positive, and one negative field for Axis-type Actions
                //CreateUIRow(action, AxisRange.Full, action.descriptiveName, ID);  //!@
                CreateUIRow(action, AxisRange.Negative, !string.IsNullOrEmpty(action.negativeDescriptiveName) ? action.negativeDescriptiveName : action.descriptiveName + "-", ID);
                ID++;
                CreateUIRow(action, AxisRange.Positive, !string.IsNullOrEmpty(action.positiveDescriptiveName) ? action.positiveDescriptiveName : action.descriptiveName + "+", ID);
                ID++;                
            }
            else if (action.type == InputActionType.Button)
            {
                // Just create one positive field for Button-type Actions
                CreateUIRow(action, AxisRange.Positive, action.descriptiveName, ID);
                ID++;
            }            
        }
        RedrawUI();
    }

    //private void CreateUIRow(InputAction action, AxisRange actionRange, string label)
    /// <summary>Creates a row of elements and caches the data</summary>
    /// <param name="action">Action for this row</param>
    /// <param name="actionRange">Range for this row</param>
    /// <param name="label">Label for button</param>
    /// <param name="ID">Action ID</param>
    private void CreateUIRow(InputAction action, AxisRange actionRange, string label, byte ID)
    {
        //!@
        bool state = actionStates[ID];  //Get the current state of this action

        // Create the Action label
        //!@
        /*
        GameObject labelGo = Object.Instantiate<GameObject>(textPrefab);
        labelGo.transform.SetParent(actionGroupTransform);
        labelGo.transform.SetAsLastSibling();        
        labelGo.GetComponent<Text>().text = label;
        */

        //!@
        // Create the input field button
        /*
        GameObject buttonGo = Object.Instantiate<GameObject>(buttonPrefab);
        buttonGo.transform.SetParent(fieldGroupTransform);
        buttonGo.transform.SetAsLastSibling();
        */

        // Add the row to the rows list
        rows.Add(
            new Row()
            {
                action = action,
                actionRange = actionRange,

                //!@
                actionDescriptiveName = clrGreen + label + endColor,    //Set action name as label in green
                button = buttonGo[ID].GetComponent<Button>(),
                text = buttonGo[ID].GetComponentInChildren<Text>()
                //button = buttonGo.GetComponent<Button>(),
                //text = buttonGo.GetComponentInChildren<Text>()
            }
        );

        //!@
        //Toggle the enable state
        buttonGo[ID].SetActive(state);
    }

    /// <summary>Sets the selected main controller type</summary>
    /// <param name="controllerType">Type of controller</param>
    private void SetSelectedController(ControllerType controllerType)
    {
        bool changed = false;   //has the type changed? (Delta check)

        // Check if the controller type changed
        if (controllerType != selectedControllerType)
        { // controller type changed
            selectedControllerType = controllerType;
            changed = true;
        }

        // Check if the controller id changed
        int origId = selectedControllerId;
        if (selectedControllerType == ControllerType.Joystick)
        {
            if (player.controllers.joystickCount > 0)
            {
                //If the joystick count is non-0 (jsticks exist)

                //!@
                Controller newCont = (Controller)(player.controllers.Joysticks[0]); //Get the new controller ref for the 0th joystick
                bool[] types = new bool[1];                                         //Types for this new controller (ByRef return value of FindTypesOfGamePad)
                ControlsSetup.FindTypesOfGamePad(newCont, ref types);               //Return all types ByRef for this controller

                bool[] Found = new bool[1];                                         //Generic bool array var for result of ANDI bitmasks
                List<bool> found = new List<bool>();                                //List version of Found
                bool AnyFound = false;                                              //Were any true bits found in the ANDI List found result?

                //!@ LINQ: The Faces of Evil!
                //If porting game to XBone in the future (lol yeah right, fatchance),
                //Replace with non LINQ (reflection broken on evil XBone)

                //Check if there was a change in types btwn the controllers
                //https://stackoverflow.com/a/9223709
                bool isEqual = Enumerable.SequenceEqual(types, oldTypes);           //Check if the current types for this controller != oldTypes
                if (!isEqual)
                {
                    //If not equal (change detected), run the appropriate events
                    RunEvents(types);
                }
                oldTypes = types;   //Cache types into oldTypes for future delta checks

                //Check if type results match any from the exclusion list
                Found = new bool[1];                                         
                types.BoolArr_ANDI_Arr(exc_joyStickTypes, ref Found);    //Found = ANDI(exc_joyStickTypes availabe,types)
                found = new List<bool>(Found);
                AnyFound = found.Contains(true);                         //Were any invalid joyStickTypes found? (Check if found list has any true values from Bitwise ANDI op)
                if (AnyFound)
                {
                    //If any found, invalidate id 
                    selectedControllerId = -1;
                }
                else
                {
                    //If none from exclusion list were found, just see if any of the types match from the inclusion lists
                    Found = new bool[1];
                    types.BoolArr_ANDI_Arr(inc_joyStickTypes, ref Found);    //Found = ANDI(inc_joyStickTypes availabe,types)
                    found = new List<bool>(Found);
                    AnyFound = found.Contains(true);                         //Were any valid joyStickTypes found? (Check if found list has any true values from Bitwise ANDI op)

                    if (AnyFound)
                    {
                        //If any valid types were found, update the ID to this valid, 0th controller
                        selectedControllerId = player.controllers.Joysticks[0].id;
                    }
                    else
                    {
                        //If invalid, set to -1 to invalidate
                        selectedControllerId = -1;
                    }
                }
            }
            else
            {
                //If no joysticks founds, force to -1 to reset stuff using delta cmps
                selectedControllerId = -1;
            }
        }
        else
        {
            //Kbd should always use 0th controller (the kbd)
            selectedControllerId = 0;
        }

        //Flag changed if changed ID
        if (selectedControllerId != origId){changed = true;}

        // If the controller changed, stop the input mapper and update the UI
        if (changed)
        {
            inputMapper.Stop();
            RedrawUI();
        }
    }

    // Event Handlers

    /// <summary>Called by the controller UI Buttons when pressed</summary>
    /// <param name="controllerType">Type of controller</param>
    public void OnControllerSelected(int controllerType)
    {        
        //Try changing to selected controller
        SetSelectedController((ControllerType)controllerType);
        //!@ Wtf calls this and how?
        Debug.LogError("OnControllerSelected fired!");
    }
    
    //!@
    //private void OnInputFieldClicked(int index, int actionElementMapToReplaceId)
    /// <summary>Called by the input field UI Button when pressed to try handling remapping</summary>
    /// <param name="MC">MenuControls to disable</param>
    /// <param name="index">Index of thingy</param>
    /// <param name="actionElementMapToReplaceId">ID of ActionEMap to replace</param>
    private void OnInputFieldClicked(MenuControls MC, int index, int actionElementMapToReplaceId)
    {
        // index out of range
        if ((index < 0) || (index >= rows.Count)){return;}
        // there is no Controller selected
        if (controller == null){return;}

        //!@ If remapping joysticks, and the selected type is one of the readonly ones, skip remapping (just display mappings)
        /*
        if (selectedControllerType == ControllerType.Joystick)
        {
            Controller newCont = (Controller)(player.controllers.Joysticks[0]);                     //Get the controller ref for the 0th joystick
            bool[] types = new bool[1];                                                             //Types for this new controller (ByRef return value of FindTypesOfGamePad)
            ControlsSetup.FindTypesOfGamePad(newCont, ref types);                                   //Return all types ByRef for this controller

            bool[] Found = new bool[1];                                                             //Dummy bool[] ByRef param
            types.BoolArr_ANDI_Arr(joyStickTypes_R, ref Found);                                     //Found = ANDI(joyStickTypes_R readonly types,types)
            List<bool> found = new List<bool>(Found);                                               //Convert Found from bool[] to found List<bool>
            bool AnyFound = found.Contains(true);                                                   //Were any readonly joyStickTypes found? (Check if found list has any true values from Bitwise ANDI op)

            if (AnyFound)
            {
                //If any were found, then readonly; skip
                return;
            }
        }
        */

        MC.enabled = false;             //!@ Disable MC to prevent menu controls

        // Begin listening for input
        inputMapper.Start(
            new InputMapper.Context()
            {
                actionId = rows[index].action.id,
                controllerMap = controllerMap,
                actionRange = rows[index].actionRange,
                actionElementMapToReplace = controllerMap.GetElementMap(actionElementMapToReplaceId)
            }
        );
        
        //!@        
        //Set status as Remapping [action name] (localized)
        Localization.LangPack pack = new Localization.LangPack("Remapping ", "TRANSLATE ME ", "TRANSLATE ME ", "TRANSLATE ME ", "TRANSLATE ME ");
        string newLabel = pack.language + rows[index].actionDescriptiveName;    //Set new label as localized pack + action name
        statusUIText.text = newLabel;
        ToggleStatusBox(true);
    }

    /// <summary>Handles controller when it changes</summary>
    /// <param name="args">Stuff</param>
    private void OnControllerChanged(ControllerStatusChangedEventArgs args)
    {
        SetSelectedController(selectedControllerType);
    }

    /// <summary>Handleds events when the input has been remapped</summary>
    /// <param name="data">Data</param>
    private void OnInputMapped(InputMapper.InputMappedEventData data)
    {
        RedrawUI();
    }

    /// <summary>Handles stopping remapping</summary>
    /// <param name="data">Stuff</param>
    private void OnStopped(InputMapper.StoppedEventData data)
    {
        MC.enabled = true;                  //!@ Enable MC
        statusUIText.text = string.Empty;   //Clear status box
        ToggleStatusBox(false);             //Hide status box
    }

    /// <summary>Toggles the game's cursor</summary>
    /// <param name="state">Cursor state</param>
    public void ToggleCursor(bool state)
    {
        GameManager.instance.ToggleCursor(state);
    }

    /// <summary>Save the controls!</summary>
    public void SaveCtrls()
    {
        Debug.Log("Saving ControlRemapper controls...");
        ReInput.userDataStore.Save();
    }

    /// <summary>Moves this gameobject to different transform</summary>
    /// <param name="gotoNew">Goto newParent? Else orig</param>
    public void MoveGroup(bool gotoNew)
    {
        //Goto new parent; else goto old parent
        if (gotoNew)
        {this.gameObject.transform.SetParent(newParent, true);}
        else{this.gameObject.transform.SetParent(origParent, true);}
    }

    /// <summary>Toggels the enable state of the buttons</summary>
    /// <param name="state">Enabled?</param>
    public void ToggleState(bool state)
    {
        //Iterate through all rows, toggle interactive state
        foreach(Row r in rows)
        {r.button.interactable = state;}
    }

    /// <summary>Runs events</summary>
    /// <param name="types">Types</param>
    private void RunEvents(bool[] types)
    {
        //If ready for events...
        if (eventsReady)
        {
            //Check if this new gamepad matches any in the readonly array
            bool[] Found = new bool[1];                                         //Dummy bool[] ByRef param
            types.BoolArr_ANDI_Arr(joyStickTypes_R, ref Found);          //Found = ANDI(joyStickTypes_R availabe,types)
            List<bool> found = new List<bool>(Found);                               //Convert Found from bool[] to found List<bool>
            bool AnyFound = found.Contains(true);                             //Were any invalid readonly types found? (Check if found list has any true values from Bitwise ANDI op)

            byte index = 0;
            //If found, use event index 1; else 0
            if (AnyFound){index = 1;}
            //Invoke the appropriate events
            joyStickTypes_R_Events[index].Invoke();
        }
    }

    /// <summary>
    /// Runs joyStickTypes_R_Events (enable/disable version) as appropriate, based on the current subtypes of this joystick
    /// This overload used for UI events
    /// </summary>
    public void RunEvents()
    {
        if(selectedControllerType == ControllerType.Joystick)
        {
            //!@
            Controller newCont = (Controller)(player.controllers.Joysticks[0]); //Get the new controller ref for the 0th joystick
            bool[] types = new bool[1];                                         //Types for this new controller (ByRef return value of FindTypesOfGamePad)
            ControlsSetup.FindTypesOfGamePad(newCont, ref types);               //Return all types ByRef for this controller
            RunEvents(types);                                                   //Run the appropriate types of events
        }
    }

    /// <summary>
    /// Runs all position/scaleTweens located in children under assignmentsParent transform.
    /// Used to move/scale remap buttons to approrpiate places for readonly controllers (joyStickTypes_R), since they have a BG layout for them
    /// Used as an UI event
    /// </summary>
    /// <param name="forward">Run the tweens forward? (Go from rest to readonly pos/scales?)</param>
    public void RunPosScaleTweens(bool forward)
    {
        PositionTween[] PT = assignmentsParent.transform.GetComponentsInChildren<PositionTween>();  //Get all positionTweens in the children within assignmentsParent
        ScaleTween[] ST = assignmentsParent.transform.GetComponentsInChildren<ScaleTween>();        //Get all scaleTweens in the children within assignmentsParent

        if (PT != null && ST != null)
        {
            //If PT and ST found

            byte countPT = (byte)(PT.GetLength(0)); //Get the count of posTweens
            byte countST = (byte)(ST.GetLength(0)); //Get the count of scaleTweens

            //if not equal, skip func
            if (countPT != countST){return;}

            byte i = 0; //Generic iterator
            //Iterate through all pos/scale tweens in arrays
            for (i = 0; i < countST; i++)
            {
                if (forward)
                {
                    //If forward, play Scale/PosTweens forward
                    PT[i].PlayForward();
                    ST[i].PlayForward();
                }
                else
                {
                    //Else play Scale/PosTweens backwards
                    PT[i].PlayBackward();
                    ST[i].PlayBackward();
                }
            }
        }
    }

    /// <summary>Toggles the eventsReady flag, for running readonly events</summary>
    /// <param name="state">Ready?</param>
    public void ToggleReady(bool state)
    {
        eventsReady = state;
    }

    /// <summary>Toggles the status box gfx</summary>
    /// <param name="state">Enable?</param>
    public void ToggleStatusBox(bool state)
    {
        GameObject P = statusUIText.transform.parent.gameObject;    //Get the statuUIText's gameobject
        Button btn = P.GetComponent<Button>();                      //Get its button component
        Image img = P.GetComponent<Image>();                        //Get its image component

        //if button and image comps found
        //Toggle the img's enable state
        if (btn && img)
        {img.enabled = state;}
    }

        
    [System.Serializable]
    /// <summary>A small class to store information about the input field buttons</summary>
    public class Row
    {
        /// <summary>Action type to handle</summary>
        public InputAction action;
        /// <summary>!@ Name of the action</summary>
        public string actionDescriptiveName;
        /// <summary>Axis range of action</summary>
        public AxisRange actionRange;
        /// <summary>Button</summary>
        public Button button;
        /// <summary>Text</summary>
        public Text text;
    }
}