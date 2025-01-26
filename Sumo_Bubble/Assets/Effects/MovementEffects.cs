using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEffects : MonoBehaviour
{
    public BallController ballController;
    public GameObject boostTrail;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        boostTrail.SetActive(ballController.isBoosting);
    }
}
