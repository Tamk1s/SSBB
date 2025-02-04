using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] Transform faceRoot;
    [SerializeField] float catchupSpeed = 5.0f;
    Quaternion desiredRotation;
    private void LateUpdate()
    {
        if(Camera.main == null)
        {
            return;
        }

        //This assumes that the negative side of a sprite is forward, will need to be updated if that's ever not the case
        //desiredRotation = Quaternion.loCamera.main.transform.rotation;
        faceRoot.forward = Camera.main.transform.forward;/*Quaternion.Slerp(faceRoot.rotation, desiredRotation, Time.deltaTime * catchupSpeed);*/
    }
}
