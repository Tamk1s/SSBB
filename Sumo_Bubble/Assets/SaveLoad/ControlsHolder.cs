using System;
using System.Collections.Generic;

/// <summary>Holds controls (duh)</summary>
[Serializable]
public class ControlsHolder
{
    /// <summary>
    /// List of controlsetups
    /// </summary>
    public List<ControlsSetup> controls;                            

    /// <summary>Creates a controls holder from an array of ControlsSetup</summary>
    /// <param name="initialControls">ControlsSetup[]</param>
    public ControlsHolder(params ControlsSetup[] initialControls)
    {
        //Constructor
        controls = new List<ControlsSetup>();

        //If array has size, then add the array of controls to the constructor
        if (initialControls.Length > 0){controls.AddRange(initialControls);}
    }
}
