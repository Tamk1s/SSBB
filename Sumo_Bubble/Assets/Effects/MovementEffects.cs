using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEffects : MonoBehaviour
{
    /// <summary>BallController comp</summary>
    public BallController ballController;
    /// <summary>BoostTrail object</summary>
    public GameObject boostTrail;
    /// <summary>TrailRenderer</summary>
    public TrailRenderer TR;
    
    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        UpdateTrail();
    }

    /// <summary>
    /// Updates the trail GO and color lerp, based on ball's current color AirLerp
    /// </summary>
    private void UpdateTrail()
    {
        //Toggle boostTrail gameObject, if ball is boosting
        boostTrail.SetActive(ballController.isBoosting);

        //Update colorLerp
        float cAir = ballController.air.currentAir;         //Get current air amount
        Color32 clr = ballController.air.ColorLerp(cAir);   //Get current color lerp
        //Set start/end of trail to lerp color
        TR.startColor = clr;
        TR.endColor = clr;
    }
}
