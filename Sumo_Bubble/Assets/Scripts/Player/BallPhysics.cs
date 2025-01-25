using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BallPhysics : MonoBehaviour
{
    public Rigidbody rigid;
    public Vector2 direction;
    public float baseMovementForce;


    //public bool isForward, isBackward, isLeft, isRight;

    public void FixedUpdate()
    {
        rigid.AddForce(new Vector3(direction.x, 0, direction.y) * baseMovementForce);
    }

    [Button]
    public void SetDirectionalInput(Vector2 dir)
    {
        direction = dir;
    }

    //public void ForwardPressed()
    //{
    //    isForward = true;
    //}

    //public void ForwardReleased()
    //{
    //    isForward = false;
    //}

    //public void BackwardPressed()
    //{
    //    isBackward = true;
    //}

    //public void BackwardReleased()
    //{
    //    isBackward = false;
    //}

    //public void LeftPressed()
    //{
    //    isLeft = true;
    //}

    //public void LeftReleased()
    //{
    //    isLeft = false;
    //}

    //public void RightPressed()
    //{
    //    isRight = true;
    //}

    //public void RightReleased()
    //{
    //    isRight = false;
    //}
}
