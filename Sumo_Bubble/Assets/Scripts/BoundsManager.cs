using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsManager : MonoBehaviour
{
    public GameObject North;
    public GameObject South;
    public GameObject East;
    public GameObject West;

    private float timer = 0f;
    public float ShrinkSeconds = 45f;

    private Vector3 topStart, bottomStart, leftStart, rightStart;
    private Vector3 finalPosition;
    private bool ready = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PLAYER")
        {
            Debug.Log("Game Over!");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        topStart = North.transform.position;
        bottomStart = South.transform.position;
        leftStart = West.transform.position;
        rightStart = East.transform.position;
        ready = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ready)
        {
            timer += UnityEngine.Time.deltaTime;
            North.transform.position = Vector3.Lerp(topStart, finalPosition, timer / ShrinkSeconds);
            South.transform.position = Vector3.Lerp(bottomStart, finalPosition, timer / ShrinkSeconds);
            East.transform.position = Vector3.Lerp(rightStart, finalPosition, timer / ShrinkSeconds);
            West.transform.position = Vector3.Lerp(leftStart, finalPosition, timer / ShrinkSeconds);
        }
    }

    public void ToggleReady(bool state)
    {
        ready = state;
    }

    public float GetTimer_Countdown()
    {
        float result = (ShrinkSeconds - timer);
        return result;
    }

    public float GetTimer_Percent()
    {
        float result = (timer / ShrinkSeconds);
        return result;
    }
}
