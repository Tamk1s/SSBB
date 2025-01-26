using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BallPhysics : MonoBehaviour
{
    //Components
    [Range(0, 1)] public float currentSize;

    public float boostMaxSpeedAddend = 15;

    public float extraGravity = 5;
    public LayerMask CollisionMask;

    public Preset SmallestPreset;
    public Preset LargestPreset;
    public float maxSpeed = 45;

    public BallController ballC;
    public Rigidbody rigid;
    public PhysicMaterial PhysMaterial;
    public Vector2 direction;

    public float boostForce = 60;
    public float boostStaticFriction = 0;
    public float boostDynamicFriction = 0;

    [ShowNonSerializedField]
    float currentSpeed;

    [System.Serializable]
    public class Preset
    {
        public float movementForce = 80;
        public float boostForce;
        public float mass = 3;
        public float angularDrag = 0.05f;
        public float drag = 0;
        public float scale = 3;
        public float staticFriction;
        public float dynamicFriction;        
    }

    //public bool isForward, isBackward, isLeft, isRight;
    private bool ready = false;

    public void Start()
    {
        ready = true;
    }

    public void Update()
    {
        if (ready){updateSizeSettings();}
    }

    public void ToggleReady(bool state)
    {
        ready = state;
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
        this.transform.localScale = Vector3.one * Mathf.Lerp(SmallestPreset.scale, LargestPreset.scale, currentSize);
        if (ballC.isBoosting)
        {
            PhysMaterial.dynamicFriction = boostDynamicFriction;
            PhysMaterial.staticFriction = boostStaticFriction;
        }
        else
        {
            PhysMaterial.dynamicFriction = Mathf.Lerp(SmallestPreset.dynamicFriction, LargestPreset.dynamicFriction, currentSize);
            PhysMaterial.staticFriction = Mathf.Lerp(SmallestPreset.staticFriction, LargestPreset.staticFriction, currentSize);
        }
    }

    public void clampSpeed()
    {
        float max;
        
        if (ballC.isBoosting)
        {
            max = maxSpeed + boostMaxSpeedAddend;
        }
        else
        {
            max = maxSpeed;
        }
        
        if (currentSpeed > max)
        {
            rigid.velocity = rigid.velocity / currentSpeed * maxSpeed;
        }
    }

    public void FixedUpdate()
    {
        rigid.isKinematic = !ready;
        if (ready)
        {
            Vector3 moveVector = new Vector3(direction.x, 0, direction.y);
            rigid.AddForce(moveVector * Mathf.Lerp(SmallestPreset.movementForce, LargestPreset.movementForce, currentSize)); // add basic force
            if (ballC.isBoosting)
            {
                rigid.AddForce(moveVector * boostForce, ForceMode.Acceleration); // add boost force
            }
            currentSpeed = rigid.velocity.magnitude;
            clampSpeed();
            additionalGravity();
        }
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
