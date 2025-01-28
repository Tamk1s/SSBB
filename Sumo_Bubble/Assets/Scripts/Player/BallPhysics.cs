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
    //public ScaleTween ST;
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
        public float playerscale = 25;
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

        //Scale up 3D sphere
        Vector3 ID = Vector3.one;
        float scalar = GetSizeScalar(SmallestPreset.scale, LargestPreset.scale, currentSize);
        Vector3 scale = GetScaleVtr(ID, scalar);
        this.transform.localScale = scale;

        //Scale up player
        ID = Vector3.one;
        scalar = GetSizeScalar(SmallestPreset.playerscale, LargestPreset.playerscale, currentSize);
        scale = GetScaleVtr(ID, scalar);
        ballC.animator.Spr_Player.transform.localScale = scale;

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

    /*
    public void DoPop_Scaling()
    {
        //Constants        
        const float time = 1f;
        const float minScale = 0.001f;
        Vector3 ID = Vector3.one;
        Vector3 minScaleVtr = new Vector3(minScale, minScale, minScale);

        float scalar = GetSizeScalar(SmallestPreset.scale, LargestPreset.scale, currentSize);
        Vector3 scale = GetScaleVtr(ID, scalar);

        ST.from = scale;
        ST.to = minScaleVtr;
        ST.playbackTime = time;
        ST.PlayForward();
    }
    */

    private float GetSizeScalar(float scaleA, float scaleB, float t)
    {
        float scalar = Mathf.Lerp(scaleA, scaleB, t);
        return scalar;
    }

    private Vector3 GetScaleVtr(Vector3 ID, float scalar)
    {
        Vector3 scale = ID * scalar;
        return scale;
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

    private void additionalGravity()
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
