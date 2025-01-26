using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BallPhysics : MonoBehaviour
{
    //Components
    [Range(0, 1)] public float currentSize;

    public float extraGravity = 5;
    public LayerMask CollisionMask;

    public Preset SmallestPreset;
    public Preset LargestPreset;
    public Preset CurrentPreset;
    public float maxSpeed = 45;

    public BallController ballC;
    public Rigidbody rigid;
    public PhysicMaterial PhysMaterial;
    public Vector2 direction;

    [ShowNonSerializedField]
    float currentSpeed;

    [System.Serializable]
    public class Preset
    {
        public float movementForce = 80;
        public float boostForce;
        public float boostSpeed;
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

    public void changeSize(float size)
    {
        currentSize = size;
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
        float max = (maxSpeed + CurrentPreset.boostSpeed);
        if (currentSpeed > max)
        {
            rigid.velocity = rigid.velocity / currentSpeed * maxSpeed;
        }
    }

    public void FixedUpdate()
    {
        float boostForce = 0f;
        if (ballC.isBoosting){boostForce = CurrentPreset.boostForce;}
        float magnitude = CurrentPreset.movementForce + boostForce;
        Vector3 dir = new Vector3(direction.x, 0, direction.y);

        rigid.AddForce(dir * magnitude);
        currentSpeed = rigid.velocity.magnitude;
        clampSpeed();
        additionalGravity();
    }

    [Button]
    public void SetDirectionalInput(Vector2 dir)
    {
        direction = dir;
    }

    void additionalGravity()
    {
        if (!Physics.Raycast(this.transform.position, Vector3.up * -1, Mathf.Lerp(SmallestPreset.scale, LargestPreset.scale, currentSize) / 2 + 0.25f, CollisionMask))
        {
            //Debug.Log("Airborn");
            rigid.AddForce(Vector3.up * -1 * extraGravity, ForceMode.Acceleration);
        }
        else
        {
            //Debug.Log("Grounded");
        }
    }
}
