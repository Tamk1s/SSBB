using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Class defines a CutScenePanel object</summary>
public class CutScenePanel : MonoBehaviour
{
    /// <summary>Icon image</summary>
    public Image byIcon;
    /// <summary>Name text</summary>
    public TextMeshProUGUI byName;
    /// <summary>Expression text</summary>
    public TextMeshProUGUI expressionLabel;
    /// <summary>Next gameobject indicator</summary>
    public GameObject nextMarker;

    /// <summary>IDs for CSP controls</summary>
    private enum CSP_Controls
    {
        CSPC_FF,
        CPSC_Next,
    };
    /// <summary>Max controls</summary>
    public const byte maxControls = (byte)(CSP_Controls.CPSC_Next + 0x01);

    [Header("Controls Stuff")]
    [NamedArrayAttribute(typeof(CSP_Controls))]
    /// <summary>List of UI controls</summary>
    public GameObject[] controls = new GameObject[maxControls];
    [NamedArrayAttribute(typeof(CSP_Controls))]
    /// <summary>Disabled sprites for controls</summary>
    public Sprite[] disableSprites = new Sprite[maxControls];
    [NamedArrayAttribute(typeof(CSP_Controls))]
    /// <summary>Disabled sprites for controls</summary>
    public Sprite[] enableSprites = new Sprite[maxControls];


    [Header("Options Stuff")]
    /// <summary>Option A screenItem</summary>
    public ScreenItem item_OptionA;
    /// <summary>Option B screenItem</summary>
    public ScreenItem item_OptionB;
    /// <summary>Option A text (for decisions)</summary>
    public TextMeshProUGUI txt_OptionA;
    /// <summary>Option B text (for decisions)</summary>
    public TextMeshProUGUI txt_OptionB;

    /// <summary>Cutscene Healthbars for the characters; aligned with the Characters enum</summary>
    //[NamedArrayAttribute(typeof(Characters))]
    //public GameObject[] CutSceneHealthBars = new GameObject[(byte)(Characters.MAX)+1];
}
