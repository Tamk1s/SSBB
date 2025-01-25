using UnityEngine;
using System.Collections.Generic;

/// <summary>Special tween which will lerp-move a RectTransform between various points. !@ For UI element repositioning IIRC</summary>
public class MoveTo : MonoBehaviour
{
    public List<Vector3> points;
    public int index = 0;               //Current point index
    public float moveSpeed = .5f;       //Movement speed for lerping
    public AnimationCurve curve;        //Anim curve

    private bool isMoving;              //Are we currently moving/lerping?
    private Vector3 origin;             //Orign pt at start of lerp
    private float timer = 0f;           //Timer T for lerps
    private RectTransform rectTransform;//RectTransform for UI movement
	
    /// <summary>Get the rectTransform on start</summary>
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>Handle lerping between points</summary>
    private void Update ()
    {
        //Only process if moving
        if (isMoving)
        {
            //Increment timer by deltaTime
            timer += (Time.deltaTime);
            float value = (timer / moveSpeed);  //Increase value percent by time/moveSpeed
            
            if(value <= 1f)
            {
                //If target not met, then just move to value
                Move(value);
            }
            else
            {
                //If target exceed, then cap at 1f, move, and reset moving flag
                value = 1f;
                Move(value);
                isMoving = false;
            }
        }
	}

    /// <summary>Goto next point</summary>
    public void Next()
    {
        //if next index exceed point count limit, then set to index = 0; else, just increment index
        if ((index + 1) == points.Count)
        {index = 0;}
        else{index++;}

        //Move to new point
        MoveToPoint();
    }

    /// <summary>Goto previous point</summary>
    public void Prev()
    {
        //if index is 0, then cap to pt limit - 1; else just decrement index
        if (index == 0)
        {index = (points.Count - 1);}
        else{index--;}

        //Move to new point
        MoveToPoint();
    }

    /// <summary>
    /// Scroll movement by value.
    /// Call this as a public function?
    /// </summary>
    /// <param name="value">Movement value</param>
    public void ScrollValue(float value)
    {
        //If value is positive, then goto previous point; else next pt
        if (value > 0f){Prev();}
        else if (value < 0f){Next();}
    }

    /// <summary>Set point index, then move to indexed point</summary>
    /// <param name="index">New index</param>
    public void AtIndex(int index)
    {
        this.index = index;
        MoveToPoint();
    }

    /// <summary>Actually moves object to point[index]</summary>
    public void MoveToPoint()
    {
        isMoving = true;                    //Set moving flag!
        origin = transform.localPosition;   //Set origin as current posn
        timer = 0f;                         //Reset timer
    }

    /// <summary>Handle movement between points</summary>
    /// <param name="value">Normal value T</param>
    private void Move(float value)
    {
        //If rectTransform exists, then lerp between origin and point[index], by value
        if (rectTransform)
        {rectTransform.anchoredPosition3D = Vector3.Lerp(origin, points[index], curve.Evaluate(value));}
        else{transform.localPosition = Vector3.Lerp(origin, points[index], curve.Evaluate(value));}
    }
}
