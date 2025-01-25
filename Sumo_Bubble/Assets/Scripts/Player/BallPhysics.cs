using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BallPhysics : MonoBehaviour
{
    [Range(0, 1)] public float currentSize;

    public Preset SmallestPreset;
    public Preset LargestPreset;
    public Preset CurrentPreset;

    public float maxSpeed = 45;

    public Rigidbody rigid;
    public PhysicMaterial PhysMaterial;
    public Vector2 direction;

    [ShowNonSerializedField]
    float currentSpeed;
    

    [System.Serializable]
    public class Preset
    {
        public float movementForce = 80;
        public float mass = 3;
        public float angularDrag = 0.05f;
        public float drag = 0;
        public float scale = 3;
        public float staticFriction;
        public float dynamicFriction;
    }


    //public bool isForward, isBackward, isLeft, isRight;

    [Button]
    public void DebugTestPreset()
    {
        rigid.mass = CurrentPreset.mass;
        rigid.angularDrag = CurrentPreset.angularDrag;
        rigid.drag = CurrentPreset.drag;
        this.transform.localScale = Vector3.one * CurrentPreset.scale;
        PhysMaterial.dynamicFriction = CurrentPreset.dynamicFriction;
        PhysMaterial.staticFriction = CurrentPreset.staticFriction;
    }

    public void Update()
    {
        updateSizeSettings();
    }

    public void updateSizeSettings()
    {
        rigid.mass = Mathf.Lerp(SmallestPreset.mass, LargestPreset.mass, currentSize);
        rigid.angularDrag = Mathf.Lerp(SmallestPreset.angularDrag, LargestPreset.angularDrag, currentSize);
        rigid.drag = Mathf.Lerp(SmallestPreset.drag, LargestPreset.drag, currentSize);
        this.transform.localScale = Vector3.one * Mathf.Lerp(SmallestPreset.scale, LargestPreset.scale, currentSize); ;
        PhysMaterial.dynamicFriction = Mathf.Lerp(SmallestPreset.dynamicFriction, LargestPreset.dynamicFriction, currentSize);
        PhysMaterial.staticFriction = Mathf.Lerp(SmallestPreset.staticFriction, LargestPreset.staticFriction, currentSize);
    }

    public void clampSpeed()
    {
        if (currentSpeed > maxSpeed)
            rigid.velocity = rigid.velocity / currentSpeed * maxSpeed;
    }

    public void FixedUpdate()
    {
        rigid.AddForce(new Vector3(direction.x, 0, direction.y) * CurrentPreset.movementForce);
        currentSpeed = rigid.velocity.magnitude;
        clampSpeed();
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
