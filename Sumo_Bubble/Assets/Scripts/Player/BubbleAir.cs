using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAir : MonoBehaviour
{
    [Header("Constants")]
    public float MaxAir = 100;
    public float MinAir;
    public float StartingAir = 25;
    public float pumpUpIncrement;
    public float airLossPerSecond = 10;

    [Header("Debug")]
    public float currentAir;

    [Button]
    public void PumpUp()
    {
        currentAir += pumpUpIncrement;
    }

    [Button]
    public void Blow()
    {
        currentAir -= airLossPerSecond;
    }
}
