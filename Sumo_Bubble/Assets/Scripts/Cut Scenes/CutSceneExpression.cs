using ByteSheep.Events;
using System;
using UnityEngine;

[Serializable]
/// <summary>Defines a cutscene expression</summary>
public class CutSceneExpression
{
    /// <summary>Localized Expression</summary>
    public Localization.LangPack2 expression;

    //Options stuff
    /// <summary>Use options?</summary>
    public bool useOptions = false;
    /// <summary>Localized Expression (for OptionA)</summary>
    public Localization.LangPack2 expressionOptA;
    /// <summary>Localized Expression (for OptionA)</summary>
    public Localization.LangPack2 expressionOptB;
    /// <summary>Action for OptionA</summary>
    public AdvancedEvent onActivate_OptA;
    /// <summary>Action for OptionB</summary>
    public AdvancedEvent onActivate_OptB;

    /// <summary>Vocal audio clip to play</summary>
    public Audio.VOX speech;
    /// <summary>Actor info</summary>
    public CutSceneActorInfo.ActorID byActor;
    /// <summary>Expression to run onStart</summary>
    public AdvancedEvent onExpressionStart;

    /// <summary>Dynamic String.Format text?</summary>
    public bool dynamic = false;       
    /// <summary>Variable objects to use for dynamic text</summary>
    public object[] vars = new object[0x01];

    /// <summary>Setup the vars</summary>
    /// <param name="Vars">Vars</param>
    public void SetupVars(object[] Vars)
    {
        //Set flag. Then initz array size
        dynamic = true;
        byte len = (byte)(Vars.GetLength(0));
        vars = new object[len];

        //Iterate through all indices, initz a copy from param
        byte index = 0x00;
        for (index = 0x00; index < len; index++)
        {
            vars[index] = Vars[index];
        }
    }
}
