using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraFollow : MonoBehaviour
{
    #region Setup
    /// <summary>LayerMask layers</summary>
    public enum Layer : byte
    {
        //Built in
        LYR_DEFAULT,
        LYR_TRANS_FX,
        LYR_IGNORE_RAY,
        LYR_USER3,
        LYR_WATER,
        LYR_UI,

        //User-defined
        LYR_GROUND,
        LYR_BALL,
        LYR_DEATHWALL,
        LYR_HURT,
        LYR_USER10,
        LYR_USER11,
        LYR_USER12,
        LYR_USER13,
        LYR_USER14,
        LYR_USER15,
        LYR_USER16,
        LYR_USER17,
        LYR_USER18,
        LYR_USER19,
        LYR_USER20,
        LYR_USER21,
        LYR_USER22,
        LYR_USER23,
        LYR_USER24,
        LYR_USER25,
        LYR_USER26,
        LYR_USER27,
        LYR_USER28,
        LYR_USER29,
        LYR_USER30,
        LYR_USER31,
        MAX = LYR_USER31
    }
    /// <summary>Max amount of layers</summary>
    public const byte maxLayer = (byte)(Layer.MAX) + 0x01;
    /// <summary>Value for NO layers (nothing)</summary>
    public const int layerNothing = 0x00000000;

    [NamedArrayAttribute(typeof(CameraFollow.Layer))]
    /// <summary>Corresponding string names for the Layers. Aligned with the CameraFollow.Layer enum</summary>
    public static string[] layerNames = new string[maxLayer]
    {
        //Built in
        "Default",
        "TransparentFX",
        "Ignore Raycast",
        "",
        "Water",
        "UI",

        //User-defined
        "Ground",
        "Ball",
        "DeathWall",
        "Hurt",
        "", //10
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "", //20
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",//30
        "",//31
    };

    /// <summary>Gets a layer string name, by Layer</summary>
    /// <param name="L">Layer</param>
    /// <returns>layer string name</returns>
    public static string GetLayerName(CameraFollow.Layer L)
    {
        byte index = (byte)(L);                 //Convert layer to byte index
        string result = layerNames[index];      //Fetch name
        return result;
    }

    /// <summary>Get the layer bit index</summary>
    /// <param name="L">Layer</param>
    /// <returns>Bit index</returns>
    public static int GetLayerBit(CameraFollow.Layer L)
    {
        //Convert layer to int
        int result = (int)(L);  
        return result;
    }

    /// <summary>Get the layer bit, as dec value</summary>
    /// <param name="L">Layer</param>
    /// <returns>Value</returns>
    public static int GetLayerBit_Value(CameraFollow.Layer L)
    {
        int result = GetLayerBit(L);            //Get layer bit
        result = (int)(Mathf.Pow(2, result));   //Convert bit to byte (2^x)
        return result;
    }

    /// <summary>Get the LayerMask from layers</summary>
    /// <param name="layers">Layers</param>
    /// <returns>LayerMask int value</returns>
    public static int GetLayerMask(CameraFollow.Layer[] layers)
    {
        int result = layerNothing;                  //Result to return

        //Iterate through all layers
        foreach (CameraFollow.Layer l in layers)
        {
            //Get bit value, reuslt = (result || value)
            int op = GetLayerBit_Value(l);
            result |= op;
        }
        return result;
    }
    #endregion

    #region Variables
    private static CameraFollow instance;
    public static CameraFollow Instance => instance;

    /// <summary>The PostProcessLayer</summary>
    public PostProcessLayer PP_Layer;
    [SerializeField] Transform _target;
    [SerializeField] CameraConfiguration defaultConfiguration;

    //[HideInInspector]
    [SerializeField] public CameraConfiguration currentConfiguration;
    private Vector3 targetPosition;
    #endregion

    #region StdUnityEvents
    private void Awake()
    {
        currentConfiguration = defaultConfiguration;
        instance = this;
    }

    private void LateUpdate()
    {
        if (_target == null || currentConfiguration == null)
        {
            return;
        }
        targetPosition = _target.position + currentConfiguration.Offset;
        if (!currentConfiguration.ShouldSnap)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, currentConfiguration.MoveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(currentConfiguration.Rotation), currentConfiguration.RotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(currentConfiguration.Rotation);
        }
    }
    #endregion

    #region Methods
    /// <summary>Consume camera configuration</summary>
    /// <param name="config">CameraConfiguration</param>
    public void ConsumeConfiguration(CameraConfiguration config)
    {
        currentConfiguration = config;
    }

    /// <summary>Consume/change target</summary>
    /// <param name="newTarget">newTarget</param>
    public void ConsumeTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    /// <summary>Consume both target and camConfigm</summary>
    /// <param name="config">CameraConfiguration</param>
    /// <param name="newTarget">NewTarget</param>
    /// <param name="targetFirst">Set target first; else camConfig</param>
    public void ConsumeTargetAndConfiguration(CameraConfiguration config, Transform newTarget, bool targetFirst)
    {
        if (targetFirst)
        {
            ConsumeTarget(newTarget);
            ConsumeConfiguration(config);
        }
        else
        {
            ConsumeConfiguration(config);
            ConsumeTarget(newTarget);
        }
    }

    /// <summary>Set the PPLayer's layer</summary>
    /// <param name="mask">LayerMask</param>
    public void PPLayer_SetLayer(int mask)
    {
        PP_Layer.volumeLayer = mask;
    }

    /// <summary>Reset currentConfig to default</summary>
    public void Reset()
    {
        currentConfiguration = defaultConfiguration;
    }
    #endregion
}