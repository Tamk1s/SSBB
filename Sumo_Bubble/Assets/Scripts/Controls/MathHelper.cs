using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class of random Mathf-like extensions, for various stuff
/// </summary>
public static class MathHelper {

    #region Mod
    public static int Mod(int v, int m) => (v % m + m) % m;//Because '%' is a remainder operator and NOT modulus.  This works with negative numbers.
    public static float Mod(float v, float m) => (v % m + m) % m;//Because '%' is a remainder operator and NOT modulus.  This works with negative numbers.
    #endregion


    #region Sign
    /// <summary>Returns a random sign scalar (±1f)</summary>
    /// <returns>±1f</returns>
    public static float GetRndSign()
    {
        byte s = (byte)(UnityEngine.Random.Range(0x00, 0x02));  //Get rnd byte between 0 and 1
        float sign = -1f;                                       //Set negative sign scalar (-1f)
        if (s != 0) { sign = 1f; }                              //If 0x01, get +1f
        return sign;
    }

    /// <summary>A sign function that returns 0 for input 0 as opposed to +1 for 0</summary>
    /// <param name="v">Float vlaue</param>
    /// <returns>Sign as int</returns>
    public static int Sign(float v)
    {
        const int ZERO = 0;                                     //0
        const int ONE = 1;                                        //1

        int result = ZERO;                                      //Sign result (int). Default ZERO if (v=0)
        if (v < 0f) { result = -ONE; }                            //If negative float, then return -ONE
        else if (v > 0f) { result = ONE; }                        //if positive float, then return ONE
        return result;
    }

    /// <summary>Checks if two floats are the same Sign()</summary>
    /// <param name="v1">A</param>
    /// <param name="v2">B</param>
    /// <returns>Same sign?</returns>
    public static bool SameSign(float v1, float v2)
    {
        bool result = (Sign(v1) == Sign(v2));
        return result;
    }
    #endregion

    #region Misc
    public static int Repeat(int v, int low, int high) => ((v - low) % (high - low)) + low;

    /// <summary>
    /// Like Mathf.Lerp, but lerp between two int a,b
    /// (Lerp between 2 ints by a float T)
    /// Modification of http://answers.unity.com/answers/1654686/view.html
    /// </summary>
    /// <param name="a">A (int)</param>
    /// <param name="b">B (int)</param>
    /// <param name="t">T</param>
    /// <returns>Int value</returns>
    public static int LerpInt(int a, int b, float t)
    {
        const float hi = 0.9999f;                       //High value limit
        int result = 0x00;                              //int value result to return

        if (t > hi) { result = b; }                       //If t is nearly 1f, then use B
        else
        {
            //a + (int)(((float)b - (float)a) * t);
            result = a;                                 //Set result to a
            float delta = (((float)b - (float)a) * t);  //Lerp delta factor
            int val = (int)(delta);                     //Cast delta as int
            result += val;                              //Add it to result
        }
        return result;                                  //Return result
    }
    #endregion

    #region RndRanges

    /// <summary>Returns a random bool state</summary>
    /// <returns>Random bool</returns>
    public static bool RndBool()
    {
        //Constants
        const byte bitFalse = 0x00;                                //False bit
        const byte bitTrue = 0x01;                                  //True bit
        const byte max = bitTrue + 0x01;                              //Max count

        byte bit = (byte)(UnityEngine.Random.Range(bitFalse, max)); //Get random bit
        bool result = (bit == bitTrue);                             //Conv to bool
        return result;                                              //Return
    }
    /// <summary>
    /// Gets a random Vtr2, using a positive range. Then applies a random sign scalar (±1f)
    /// This gets a quadrant piece
    /// </summary>
    /// <param name="rX">+Random x comp range (as vtr2)</param>
    /// <param name="rY">+Random y comp range (as vtr2)</param>
    /// <returns>±Rnd Vtr2</returns>
    public static Vector2 RndRange_VtrUnsigned(Vector2 rX, Vector2 rY)
    {
        float sx = GetRndSign();                            //Get rnd   xSign        
        float x = rX.RndRange();                            //Get rnd   x value from range        
        x *= sx;                                            //Change xSign

        float sy = GetRndSign();                            //Get rnd   y sign
        float y = rY.RndRange();                            //Get rnd   y ~
        y *= sy;                                            //Change ySign

        Vector2 result = new Vector2(x, y);                 //Create new vtr2
        return result;                                      //Return
    }

    /// <summary>Gets a random Vtr2</summary>
    /// <param name="rX">Random x comp range (as vtr2)</param>
    /// <param name="rY">Random y comp range (as vtr2)</param>
    /// <returns>Rnd Vtr2</returns>
    public static Vector2 RndRange_Vtr(Vector2 rX, Vector2 rY)
    {
        float x = rX.RndRange();                            //Get rnd   x value from range
        float y = rY.RndRange();                            //Get rnd   y ~
        Vector2 result = new Vector2(x, y);                 //Create new vtr2
        return result;                                      //Return
    }

    /// <summary>Get rnd value from range, represented within a Vtr2</summary>
    /// <param name="rng">vtr.x = min, vtr.y = max</param>
    /// <returns>Rnd Value</returns>
    public static float RndRange(this Vector2 rng)
    {
        float min = rng.x;                                  //Min value (x comp)
        float max = rng.y;                                  //Max value (y comp)
        float result = UnityEngine.Random.Range(min, max);  //Get rnd value
        return result;                                      //Return
    }

    /// <summary>Like RndRange(V2Int), but no max++ adjustment. (Normal UnityEngine.Random.Range for ints)</summary>
    /// <param name="rng"></param>
    /// <returns>Int</returns>
    public static int RndRange2(this Vector2Int rng)
    {
        int min = rng.x;                                    //Min value (x comp)
        int max = rng.y;                                    //Max value (y comp)
        int result = UnityEngine.Random.Range(min, max);    //Get rnd value	
        return result;                                      //Return
    }

    /// <summary>Get rnd value from range, represented within a Vtr2</summary>
    /// <param name="rng">vtr.x = min, vtr.y = max</param>
    /// <returns>Rnd Value</returns>
    public static int RndRange(this Vector2Int rng)
    {
        int min = rng.x;                                    //Min value (x comp)
        int max = rng.y; max++;                             //Max value (y comp), +=1
        int result = UnityEngine.Random.Range(min, max);    //Get rnd value	
        return result;                                      //Return
    }

    /// <summary>Given a Vector2Int as a range, display range as a string</summary>
    /// <param name="rng">Range (min/max)</param>
    /// <param name="digs">Digits to pad both sides (if any)</param>
    /// <returns>Range string</returns>
    public static string ToStringRng(this Vector2Int rng, int digs = 0x00)
    {
        const string digitCode = "D";   //Digit ToString code
        const string dash = "-";        //Dash separating min/max of range

        string a = "";                  //min value as string
        string b = "";                  //max value as string
        string code = "";               //ToString code
        string op = "";                 //String operand

        //if digits specified
        bool good = (digs != 0x00);
        if (good)
        {
            //Convert digits to string, generate code "Dx"
            op = digs.ToString();
            code = digitCode + op;
        }
        
        //Convert values min/max to string with code
        a = rng.x.ToString(code);
        b = rng.y.ToString(code);

        //Concenate both values with a dash in between
        string result = a + dash + b;
        return result;
    }
    #endregion
}