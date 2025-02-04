using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BubbleAirBar : MonoBehaviour
{
    public Camera mainCamera;           // Reference to the main camera    
    public BallController ball;
    public GameObject Sphere = null;
    public GameObject targetObject;     // The object the image should follow
    public Vector3 offset;              //Offset from target
    public float divFactor = 30f;

    public TextMeshProUGUI fillText;
    public Image fillBar;

    private bool ready = false;

    public void Start()
    {
        ready = true;
    }

    public void Update()
    {
        if (ready)
        {
            Follow();
            Update_fillValue_Text();
        }
    }

    /// <summary>
    /// Updates the position of fakeBar in World space if not exploded yet
    /// </summary>
    private void Follow()
    {
        // Get the target object's world position
        Vector3 targetPosition = targetObject.transform.position;
        targetPosition += (offset * (Sphere.transform.localScale.x / divFactor));
        // Convert the world position to screen space
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetPosition);
        // Update the image's position based on the converted screen position
        this.transform.position = screenPosition;
    }    

    private void Update_fillValue_Text()
    {
        const string formatter = "P2";
        const byte maxByte = 0xFF;
        //const byte alpha = 0xC0;
        Color32 clr = new Color32(maxByte, maxByte, maxByte, maxByte);

        float val = ball.air.currentAir;
        clr = ball.air.ColorLerp(val);
        //ball.animator.Change_ModelColor(clr);
        //clr = clr.changeAlpha(alpha);
        val /= 100f;
        fillText.text = val.ToString(formatter);
        //fillText.color = clr;
        Update_fillValue_Bar(clr, val);
    }

    private void Update_fillValue_Bar(Color32 clr, float t)
    {
        fillBar.color = clr;
        fillBar.fillAmount = t;
    }
}