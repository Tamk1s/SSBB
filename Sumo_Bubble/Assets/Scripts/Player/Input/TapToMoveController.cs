using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapToMoveController : MonoBehaviour
{
    /*
    public const string TTMC_GOName = "TapToMoveController";

    public static TapToMoveController Instance {
        get
        {
            bool valid = (instance != null);
            if(!valid)
            {
                GameObject gO = new GameObject(TTMC_GOName);
                if (gO)
                {
                    instance = gO.AddComponent<TapToMoveController>();
                    //!@ DEMO ONLY. Reload village scene
                    //DontDestroyOnLoad(gO);
                }
            }
            return instance;
        }
    }
    private static TapToMoveController instance;
    [SerializeField] PlayerController controller;
    private Vector2 touchPosition;
    public Vector3 navPosition { get; private set; }

    private void Update()
    {
        GetTouchPoint();
    }

    private void GetTouchPoint()
    {
        const CameraFollow.Layer ground = CameraFollow.Layer.LYR_GROUND;        //Ground layer value
        int mask = CameraFollow.GetLayerBit_Value(ground);                            //Get ground layer bit

        bool isTouched = false;
        GetTouches(out isTouched, out touchPosition);
        if (isTouched)
        {
            RaycastHit hit;
            Ray navRay = Camera.main.ScreenPointToRay(touchPosition);
            isTouched = Physics.Raycast(navRay, out hit, Mathf.Infinity, mask);
            if (isTouched)
            {
                //if valid ground pos, then set the new navPos
                bool valid = false;
                Vector3 newPos = GetValidPosition(hit, ref valid);
                if (valid){SetNavPosition(newPos);}
            }
        }
    }

    /// <summary>Sets a new NavPosition</summary>
    /// <param name="pos">newPos</param>
    public void SetNavPosition(Vector3 pos)
    {
        navPosition = pos;
    }

    private void GetTouches(out bool isTouched, out Vector2 touchScreenPos)
    {
        //!This code needs optimized/cleaned with less embedded literals :(
        if(EventSystem.current.currentSelectedGameObject != null)
        {
            if(EventSystem.current.currentSelectedGameObject.GetComponent<Graphic>() != null)
            {
                isTouched = false;
                touchScreenPos = Vector2.zero;
                return;
            }
        }
        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            isTouched = Input.GetMouseButton(0);
            if (isTouched)
            {
                touchScreenPos = Input.mousePosition;
            }
            else
            {
                touchScreenPos = Vector2.zero;
            }
        }
        else
        {
            isTouched = Input.touchCount > 0;
            if (isTouched)
            {
                touchScreenPos = Input.touches[0].position;
            }
            else {
                touchScreenPos = Vector2.zero;
            }
        }
    }

    /// <summary>Gets a valid position to move to</summary>
    /// Inputs:
    /// <param name="hit">Raycast hit</param>
    /// Outputs (ByRef):
    /// <param name="valid">Was hit pt valid?</param>
    /// Outputs (ByVal):
    /// <returns>Hit Pt</returns>
    private Vector3 GetValidPosition(RaycastHit hit, ref bool valid)
    {
        const CameraFollow.Layer ground = CameraFollow.Layer.LYR_GROUND;        //Ground layer value
        int mask = CameraFollow.GetLayerBit(ground);                            //Get ground layer bit

        //const CameraFollow.Layer ground = CameraFollow.Layer.LYR_GROUND;    //Ground layer value
        Vector3 result = Vector3.zero;                                      //V3 pt result
        //int mask = CameraFollow.GetLayerBit(ground);                        //Get ground layer bit
        //valid = (hit.transform.gameObject.layer == mask);                   //Valid if hit obj layer is Ground
        //If valid, then set reuslt pt to hitpt
        //if (valid){result = hit.point;}

        valid = (hit.transform.gameObject.layer == mask);                   //Valid if hit obj layer is Ground
        if (valid){result = hit.point;}
        return result;
    }

    private void OnDrawGizmos()
    {
        bool validPt = (touchPosition != Vector2.zero);
        if(validPt)
        {
            //if validTouch pt (pos not V2.zero)
            float z = Camera.main.nearClipPlane;                            //Get nearClipPlane z val
            Vector3 pt = new Vector3(touchPosition.x, touchPosition.y, z);  //Get new pt at touch pos, but z overriden at nearClipPlane
            pt = Camera.main.ScreenToWorldPoint(pt);                        //Conv screen to world pt
            Gizmos.DrawCube(pt, Vector3.one);                               //DrawCube at pt, unit size
        }

        //If validNav pt (pos not V3.zero, then drawCube at navPos with unit size)
        validPt = (navPosition != Vector3.zero);
        if(validPt) {Gizmos.DrawCube(navPosition, Vector3.one);}
    }
    */
}
