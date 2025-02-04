using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JAM/Camera/Camera Configuration")]
public class CameraConfiguration : ScriptableObject
{
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float rotateSpeed = 5.0f;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 desiredRotation;
    [SerializeField] bool shouldSnap = false;

    public float MoveSpeed => moveSpeed;
    public float RotateSpeed => rotateSpeed;
    public bool ShouldSnap => shouldSnap;
    public Vector3 Offset => offset;

    public Vector3 Rotation => desiredRotation;
}
