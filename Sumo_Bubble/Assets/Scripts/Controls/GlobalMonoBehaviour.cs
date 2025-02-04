﻿using System;
using UnityEngine;

/// <summary>
/// Dummy DontDestroyOnLoad monobehaviour on GameManager object, used to run coroutines in extension methods etc
/// See https://stackoverflow.com/a/32992514
/// </summary>
public class GlobalMonoBehaviour : MonoBehaviour
{
    public static GlobalMonoBehaviour instance;
    void Start()
    {
        //If instance exists, then self-destruct this duplicate gameobject and skip
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }
}