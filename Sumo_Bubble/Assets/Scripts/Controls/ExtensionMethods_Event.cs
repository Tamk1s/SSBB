using System;
using UnityEngine;

/// <summary>Dummy class script with Extension methods accessible via events (This removed)</summary>
public class ExtensionMethods_Event : MonoBehaviour
{
    public Material mat;                                               //this.material wrapper
    public Rewired.ComponentControls.Effects.RotateAroundAxis RAA;     //this.RotateAroundAxis wrapper

    /// <summary>Event wrapper for CopyPropertiesShaderFromMaterial in ExtensionMethods. Uses mat ref</summary>
    /// <param name="newMat">New material</param>
    public void mat_CopyPropertiesShaderFromMaterial(Material newMat)
    {
        mat.CopyPropertiesShaderFromMaterial(newMat);
    }

    /// <summary>Event wrapper for LerpSpeed in ExtensionMethods. Uses RAA ref</summary>
    /// <param name="param">Param</param>
    public void RAA_LerpSpeed(Vector3 param)
    {
        RAA.LerpSpeed(param);
    }
}