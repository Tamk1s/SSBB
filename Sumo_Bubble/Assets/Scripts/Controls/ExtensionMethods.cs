using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rewired;

/// <summary>Class of various extension methods</summary>
public static class ExtensionMethods
{

    #region LangPack
    /// <summary>prepend langPack onto a langPack (concatenate)</summary>
    /// <param name="p">This langPack</param>
    /// <param name="meow">Prepend LangPack</param>
    /// <returns>LangPack</returns>
    public static Localization.LangPack PackConcat_Pre(this Localization.LangPack p, Localization.LangPack meow)
    {
        string e = meow.english + p.english;
        string s = meow.spanish + p.spanish;
        string f = meow.french + p.french;
        string c = meow.chinese + p.chinese;
        string r = meow.russian + p.russian;
        Localization.LangPack result = new Localization.LangPack(e, s, f, c, r);
        return result;
    }

    /// <summary>postpend langPack onto a langPack (concatenate)</summary>
    /// <param name="p">This langPack</param>
    /// <param name="meow">Postpend LangPack</param>
    /// <returns>LangPack</returns>
    public static Localization.LangPack PackConcat_Post(this Localization.LangPack p, Localization.LangPack meow)
    {
        string e = p.english + meow.english;
        string s = p.spanish + meow.spanish;
        string f = p.french + meow.french;
        string c = p.chinese + meow.chinese;
        string r = p.russian + meow.russian;
        Localization.LangPack result = new Localization.LangPack(e, s, f, c, r);
        return result;
    }

    /// <summary>prepend langPack2 onto a langPack2 (concatenate)</summary>
    /// <param name="p">This langPack2</param>
    /// <param name="meow">Prepend LangPack2</param>
    /// <returns>LangPack2</returns>
    public static Localization.LangPack2 PackConcat_Pre(this Localization.LangPack2 p, Localization.LangPack2 meow)
    {
        string e = meow.english + p.english;
        string s = meow.spanish + p.spanish;
        string f = meow.french + p.french;
        string c = meow.chinese + p.chinese;
        string r = meow.russian + p.russian;
        Localization.LangPack2 result = new Localization.LangPack2(e, s, f, c, r);
        return result;
    }

    /// <summary>postpend langPack2 onto a langPack2 (concatenate)</summary>
    /// <param name="p">This langPack2</param>
    /// <param name="meow">Postpend LangPack2</param>
    /// <returns>LangPack</returns>
    public static Localization.LangPack2 PackConcat_Post(this Localization.LangPack2 p, Localization.LangPack2 meow)
    {
        string e = p.english + meow.english;
        string s = p.spanish + meow.spanish;
        string f = p.french + meow.french;
        string c = p.chinese + meow.chinese;
        string r = p.russian + meow.russian;
        Localization.LangPack2 result = new Localization.LangPack2(e, s, f, c, r);
        return result;
    }



    /// <summary>prepend string scalar onto a langPack2 (concatenate)</summary>
    /// <param name="p">pack</param>
    /// <param name="meow">prepend string</param>
    /// <returns>LangPack2</returns>
    public static Localization.LangPack2 PackConcat_Pre(this Localization.LangPack2 p, string meow)
    {
        string e = meow + p.english;
        string s = meow + p.spanish;
        string f = meow + p.french;
        string c = meow + p.chinese;
        string r = meow + p.russian;
        Localization.LangPack2 result = new Localization.LangPack2(e, s, f, c, r);
        return result;
    }

    /// <summary>postpend string scalar onto a langPack2 (concatenate)</summary>
    /// <param name="p">pack</param>
    /// <param name="meow">String</param>
    /// <returns>LangPack2</returns>
    public static Localization.LangPack2 PackConcat_Post(this Localization.LangPack2 p, string meow)
    {
        string e = p.english + meow;
        string s = p.spanish + meow;
        string f = p.french + meow;
        string c = p.chinese + meow;
        string r = p.russian + meow;
        Localization.LangPack2 result = new Localization.LangPack2(e, s, f, c, r);
        return result;
    }
    #endregion

    #region Audio.SFX[]


    /// <summary>sfx_play() a random sfx from audio pool. Returns Audio.SFX played</summary>
    /// <param name="sfx">SFX[] pool</param>
    /// <param name="audio">Output Audio script comp</param>
    /// <returns>Audio.SFX</returns>
    public static Audio.SFX sfx_play_rnd(this Audio.SFX[] sfx, Audio audio)
    {
        //Put min/max ranges into a Vector2Int, then fetch random int from range
        int min = 0x00;
        int max = sfx.Length;
        Vector2Int rng = new Vector2Int(min, max);
        int index = rng.RndRange2();
        Audio.SFX clip = sfx_play_ind(sfx, audio, index);
        return clip;
    }

    /// <summary>sfx_play() a sfx by Index from audio pool. Returns Audio.SFX played</summary>
    /// <param name="sfx">SFX[] pool</param>
    /// <param name="audio">Output Audio script comp</param>
    /// <param name="index">Index</param>
    /// <returns>Audio.SFX</returns>
    public static Audio.SFX sfx_play_ind(this Audio.SFX[] sfx, Audio audio, int index)
    {
        //Get audio clip by index
        Audio.SFX s = sfx[index];
        audio.sfx_play(s);
        return s;
    }

    /// <summary>sfx_play2() a random sfx from audio pool</summary>
    /// <param name="sfx">SFX[] pool</param>
    /// <param name="audio">Output Audio script comp</param>
    /// <param name="reverse">Reverse playback?</param>
    public static void sfx_play2_rnd(this Audio.SFX[] sfx, Audio audio, bool reverse)
    {
        //Put min/max ranges into a Vector2Int, then fetch random int from range
        int min = 0x00;
        int max = sfx.Length;
        Vector2Int rng = new Vector2Int(min, max);
        int index = rng.RndRange2();
        sfx_play2_ind(sfx, audio, index, reverse);
    }

    /// <summary>sfx_play2() a sfx by Index from audio pool</summary>
    /// <param name="sfx">SFX[] pool</param>
    /// <param name="audio">Output Audio script comp</param>
    /// <param name="index">Index</param>
    /// <param name="reverse">Reverse playback?</param>
    public static void sfx_play2_ind(this Audio.SFX[] sfx, Audio audio, int index, bool reverse)
    {
        //Get audio clip by index
        Audio.SFX s = sfx[index];

        //Playback in appropriate direction
        if (reverse)
        {audio.sfx_play2_rev(s);}
        else
        {audio.sfx_play2(s);}
    }
    #endregion

    #region Misc
    // https://forum.unity3d.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
    /// <summary>Sets the layer for an object and all of its children, except those with the name of exception string</summary>
    /// <param name="obj">this object</param>
    /// <param name="layer">layer to change to</param>
    /// <param name="exception">Child name to ignore</param>
    public static void SetLayerRecursively(this GameObject obj, int layer, string exception)
    {
        obj.layer = layer;                          //Get this object's layer
        //Iterate throguh all children in object
        foreach (Transform child in obj.transform)
        {
            //If it's name is not against the exception, then set layer recursively
            if (child.name != exception)
            {
                child.gameObject.SetLayerRecursively(layer, exception);
            }
        }
    }

    /// <summary>
    /// Normalizes any float to an arbitrary range by assuming the range wraps around when going below min or above max 
    /// https://stackoverflow.com/a/2021986
    /// </summary>
    /// <param name="value">this float value</param>
    /// <param name="start">Start float range</param>
    /// <param name="end">End float range</param>
    /// <returns>Normlized float value</returns>
    public static float normalise(this float value, float start, float end)
    {
        float width = end - start;   // 
        float offsetValue = value - start;   // value relative to 0
        float val = (offsetValue - (Mathf.Floor(offsetValue / width) * width)) + start;
        return val;
        // + start to reset back to start of original range
    }

    /// <summary>Replaces a character at a pos within string</summary>
    /// <param name="input">This string to modify </param>
    /// <param name="index">Index of char</param>
    /// <param name="newChar">New char at Index</param>
    /// <returns>New, modified string</returns>
    public static string ReplaceAt(this string input, int index, char newChar)
    {
        //If no input, throw error
        const string error = "input";
        if (input == null) { throw new ArgumentNullException(error); }
        char[] chars = input.ToCharArray();
        chars[index] = newChar;
        return new string(chars);
    }

    /// <summary>
    /// Check if this animator has a particular param byName
    /// http://answers.unity3d.com/questions/571414/is-there-a-way-to-check-if-an-animatorcontroller-h.html
    /// </summary>
    /// <param name="animator">this Animator</param>
    /// <param name="paramName">Param name to check</param>
    /// <returns>HasParam?</returns>
    public static bool HasParameter(this Animator animator, string paramName)
    {
        bool has = false;                                                   //Has param?
        //Iterate through all params in animator
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            //Check if name match; if so, break and return true; else false if all fail
            has = (param.name == paramName);
            if(has){break;}
        }
        return has;
    }

    /// <summary>Copies properties from a shader for a particular material to a new material</summary>
    /// <param name="mat">This dest material</param>
    /// <param name="newMat">Src material</param>
    public static void CopyPropertiesShaderFromMaterial(this Material mat, Material newMat)
    {
        mat.shader = newMat.shader;                 //Copy shaders
        mat.CopyPropertiesFromMaterial(newMat);     //Copy properties from material into mat
    }

    //http://answers.unity.com/answers/1080062/view.html
    /*
    public static void AddResource(this ICollection<Material> materials, string resource)
        {
            Material material = Resources.Load(resource, typeof(Material)) as Material;
            if (material)
                materials.Add(material);
            else
                Debug.LogWarning("Material Resource '" + resource + "' could not be loaded.");
        }
     */

    /// <summary>Extension function to lerp a slow/fast speed value on a Rewired.ComponentControls.Effects.RotateAroundAxis component</summary>
    /// <param name="RAA">this Rewired.ComponentControls.Effects.RotateAroundAxis</param>
    /// <param name="param">Lerp params. param.x & param.y = min/max speedLerp value, .z integer = speed type to modify (part: negative = slow, positive = fast, 0 = currentSpeed being played, .z float part = time division factor for UnityEngine.Time.deltaTime lerpT value</param>
    public static void LerpSpeed(this Rewired.ComponentControls.Effects.RotateAroundAxis RAA, Vector3 param)
    {
        float z = param.z;                          //full z component
        float zInt = (int)(param.z);                //Int part of z
        float zFrac = (Mathf.Abs(z) - zInt);        //Frac part of z
        bool good = float.TryParse(Mathf.Abs(zFrac).ToString().Replace("0.", ""),out zFrac);   //Conv zFrac to string, remove 0. to get string portion, get the new value
        //Debug.Log("zInt, zFrac: " + zInt.ToString() + ", " + zFrac.ToString());
        if (good)
        {
            Vector4 v4param = new Vector4(param.x, param.y, zInt, zFrac);           //Create new Vector4, set .z and .w params appropriately
            GlobalMonoBehaviour.instance.StartCoroutine(_LerpSpeed(RAA, v4param));  //Run lerpSpeed coroutine with hacky GlobalMonoBehaviour
        }
    }

    /// <summary>Actually does the speed lerping for LerpSpeed extension function</summary>
    /// <param name="RAA">Rewired.ComponentControls.Effects.RotateAroundAxis</param>
    /// <param name="param">Lerp params. param.x & param.y = min/max speedLerp value, .z integer = speed type to modify (part: negative = slow, positive = fast, 0 = currentSpeed being played, .z float part = time division factor for UnityEngine.Time.deltaTime lerpT value</param>
    /// <returns>Speed lerping/yield</returns>
    private static IEnumerator _LerpSpeed(Rewired.ComponentControls.Effects.RotateAroundAxis RAA, Vector4 param)
    {
        Vector2 lerp = new Vector2(param.x, param.y);   //Get min/max lerp values as .x and .y of param
        sbyte spd = (sbyte)(Mathf.Sign(param.z));       //Get speedType to change as Mathf.Sign(param.z)
        float DivFactor = param.w;                      //Get division factor as param.w
        
        float timer = 0f;                               //LerpT timer value
        float speed = 0f;                               //New speed to apply

        //While timer is not maxed out
        while (timer <= 1f)
        {
            timer += (UnityEngine.Time.deltaTime / DivFactor);  //Increment timer by (deltaTime/DivFactor)
            speed = Mathf.Lerp(lerp.x, lerp.y, timer);          //Get new speed from the Lerp

            //Apply the new speed to the appropriate speed type
            if (spd == -1)
            {
                //If negative, apply to slow speed
                RAA.slowRotationSpeed = speed;
            }
            else if (spd == 1)
            {
                //If positive, apply to fast speed
                RAA.fastRotationSpeed = speed;
            }
            else
            {
                //If 0, apply to current speed being played
                if (RAA.speed == Rewired.ComponentControls.Effects.RotateAroundAxis.Speed.Slow)
                {
                    //Apply to slow if slow
                    RAA.slowRotationSpeed = speed;
                }
                else if (RAA.speed == Rewired.ComponentControls.Effects.RotateAroundAxis.Speed.Fast)
                {
                    //Apply to fast if fast
                    RAA.fastRotationSpeed = speed;
                }
            }
            yield return new WaitForSeconds(.1f);   //Safety NOP
        }
        yield break;
    }

    /// <summary>
    /// Determines if a object's renderer is within a camera's view
    /// https://wiki.unity3d.com/index.php?title=IsVisibleFrom
    /// </summary>
    /// <param name="renderer">This obj renderer</param>
    /// <param name="camera">Camera to check against</param>
    /// <returns>Visible?</returns>
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
    public static V GetValueOrDefault<V, K>(this Dictionary<K, V> dict, K key) where V : class
    {
        return dict.TryGetValue(key, out V entry) ? entry : null;
    }
    #endregion

    #region Bitwise
    /// <summary>For a bool array, ANDI it by an AND bool mask array, and return ByRef</summary>
    /// <param name="bvar">this bool array</param>
    /// <param name="ANDI_mask">ANDI Mask bool array</param>
    /// <param name="result">Resultant bool array. Returns null if both arrays are not of equal size</param>
    public static void BoolArr_ANDI_Arr(this bool[] bvar, bool[] ANDI_mask, ref bool[] result)
    {
        byte length_bvar = (byte)(bvar.GetLength(0));       //Get lenght of bvar
        byte length_Mask = (byte)(ANDI_mask.GetLength(0));  //Get length of mask

        //If both don't equal, set result to null and then return
        if (length_bvar != length_Mask)
        {
            result = null;
            return;
        }

        //Redim result to length
        result = new bool[length_bvar];
        byte i = 0; //Iterator
        //Iterate through all bytes, do bvar AND Mask and set into result
        for (i = 0; i < length_Mask; i++)
        {
            result[i] = (bvar[i] & ANDI_mask[i]);
        }
    }

    /// <summary>For a bool array, ORI it by an OR bool mask array, and return ByRef</summary>
    /// <param name="bvar">this bool array</param>
    /// <param name="ORI_mask">ORI Mask bool array</param>
    /// <param name="result">Resultant bool array. Returns null if both arrays are not of equal size</param>
    public static void BoolArr_ORI_Arr(this bool[] bvar, bool[] ORI_mask, ref bool[] result)
    {
        byte length_bvar = (byte)(bvar.GetLength(0));       //Get lenght of bvar
        byte length_Mask = (byte)(ORI_mask.GetLength(0));  //Get length of mask

        //If both don't equal, set result to null and then return
        if (length_bvar != length_Mask)
        {
            result = null;
            return;
        }

        //Redim result to length
        result = new bool[length_bvar];
        byte i = 0; //Iterator
        //Iterate through all bytes, do bvar AND Mask and set into result
        for (i = 0; i < length_Mask; i++)
        {
            result[i] = (bvar[i] | ORI_mask[i]);
        }
    }
    #endregion

    #region Int_Exts
    /// <summary>Converts an int to bool bit</summary>
    /// <param name="value">Int value</param>
    /// <returns>Bool</returns>
    public static bool ToBit(this int value)
    {
        value = Mathf.Abs(value);       //Get the absVal of value
        bool result = (value > 0);      //Conv to bit
        return result;
    }

    /// <summary>Converts a byte to bool bit</summary>
    /// <param name="value">Byte value</param>
    /// <returns>Bool</returns>
    public static bool ToBit(this byte value)
    {
        int v = (int)(value);
        bool result = v.ToBit();
        return result;
    }
    #endregion

    #region String_Exts
    /// <summary>Gets a random string from an array</summary>
    /// <param name="arr">Array of strings</param>
    /// <param name="forceValid">Required to not have badString?</param>
    /// <param name="badString">badString to ignore</param>
    /// <returns>Random string from index</returns>
    public static string SelectRndString(this string[] arr, bool forceValid = false, string badString = "")
    {
        string result = badString;                          //Initz result to strBadAirBlock
                                                            //Loop while bad string
        while (result == badString)
        {
            byte max = (byte)(arr.Length);                  //Get size of array
            Vector2Int rnd = new Vector2Int(0x00, max);     //Random range from 0x00 to max
            byte index = (byte)(rnd.RndRange2());           //Get index
            result = arr[index];                            //Fetch string
            if (!forceValid) { break; }                     //If no flag, then just use result
        }
        return result;                                      //Return
    }

    /// <summary>Combine a bunch of strings as a path</summary>
    /// <param name="arr">String array of params</param>
    /// <returns>Final path</returns>
    public static string PathCombine(this string[] arr)
    {
        string path = "";                                       //Result
        if (arr != null) { path = System.IO.Path.Combine(arr); }    //Combine them all
        return path;
    }
    #endregion

    #region float_exts
    // when you want a + or - minus in front a float's string
    public static string ToSignedString(this short value, float afterDecimalPlaces = 0)
    {
        if (value > 0)
        {
            return "+" + value.ToString("F" + afterDecimalPlaces.ToString());
        }
        else
        {
            return value.ToString("F" + afterDecimalPlaces.ToString());
        }
    }

    #endregion

    #region Vector3_Exts
    /// <summary>Converts a Vector3 into a Vector2 on XZ plane</summary>
    /// <param name="vtr">Vector3</param>
    /// <returns>Vector2 XZ</returns>
    public static Vector2 ToVector2_XZ(this Vector3 vtr)
    {
        Vector2 result = new Vector2(vtr.x, vtr.z);
        return result;
    }

    #endregion

    #region Vector2_Exts
    /// <summary>Converts a Vector2Int to Vector2</summary>
    /// <param name="v">this Vector2Int</param>
    /// <returns>Vector2 </returns>
    public static Vector2 toVector2(this Vector2Int v) => new Vector2(v.x, v.y);
    /// <summary>Converts a Vector2 to Vector2Int</summary>
    /// <param name="v">this Vector2</param>
    /// <returns>Vector2Int</returns>
    public static Vector2Int FloorToVector2Int(this Vector2 v) => new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

    /// <summary>Convert vector to Vector2Int</summary>
    /// <param name="v">Vector2</param>
    /// <returns>Vector2Int</returns>
    public static Vector2Int toVector2Int(this Vector2 v)
    {
        int x = (int)(v.x);                             //Get	x	comp, cast to int
        int y = (int)(v.y);                             //~		y	~
        Vector2Int result = new Vector2Int(x, y);       //Create new v2i
        return result;                                  //Return
    }

    /// <summary>Rotates a vector by degress</summary>
    /// <param name="v">This vector2</param>
    /// <param name="degrees">Angle degrees</param>
    /// <returns>Rotated vector2</returns>
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    #endregion

    #region RectInt_Exts
    public static RectInt Int(this Rect rect) => new RectInt((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

    public static RectInt Union(this RectInt r1, RectInt r2)
    {
        return new RectInt
        {
            xMin = Mathf.Min(r1.xMin, r2.xMin),
            yMin = Mathf.Min(r1.yMin, r2.yMin),
            xMax = Mathf.Max(r1.xMax, r2.xMax),
            yMax = Mathf.Max(r1.yMax, r2.yMax)
        };
    }

    public static RectInt Union(this RectInt r, Vector2Int v)
    {
        return new RectInt
        {
            xMin = Mathf.Min(r.xMin, v.x),
            yMin = Mathf.Min(r.yMin, v.y),
            xMax = Mathf.Max(r.xMax, v.x + 1),
            yMax = Mathf.Max(r.yMax, v.y + 1)
        };
    }

    /// <summary>
    /// Get a random posn within a RectInt
    /// !@ Needs adjustments for UnityEngine.Random.Range max exclusion stuff (0/1-based stuff)
    /// </summary>
    /// <param name="r">RectInt area</param>
    /// <returns>Rnd Posn</returns>
    public static Vector2Int RndPos(this RectInt r)
    {
        const float neg = -1f;                  //Mathf.Sign neg value
        Vector2Int result = r.position;         //Initz rndPt result to posn
        Vector2Int offset = Vector2Int.zero;    //Offset from size area	
        Vector2Int size = r.size;               //Size area

        Vector2Int xRng;                        //Rnd x range
        Vector2Int yRng;                        //Rnd y range
        int x = 0x00;                           //Rnd x value
        int y = 0x00;                           //Rnd y value

        //Get the sign of the x component
        float sgn = Mathf.Sign(size.x);
        //Setup range appropriate based on sign
        if (sgn == neg) { xRng = new Vector2Int(size.x, 0x00); }
        else { xRng = new Vector2Int(0x00, size.x); }

        //Ditto for y value
        sgn = Mathf.Sign(size.y);
        if (sgn == neg) { yRng = new Vector2Int(size.y, 0x00); }
        else { yRng = new Vector2Int(0x00, size.y); }

        //Get rnd x and y values, setup offset, add it to result/og posn
        x = xRng.RndRange2();
        y = yRng.RndRange2();
        offset = new Vector2Int(x, y);
        result += offset;
        return result;
    }
    #endregion

    #region IENUMERABLE

    /// Determines whether the collection is null or contains no elements.
    /// </summary>
    /// <typeparam name="T">The IEnumerable type.</typeparam>
    /// <param name="enumerable">The enumerable, which may be null or empty.</param>
    /// <returns>
    ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null)
        {
            return true;
        }
        /* If this is a list, use the Count property for efficiency. 
         * The Count property is O(1) while IEnumerable.Count() is O(N). */
        var collection = enumerable as ICollection<T>;
        if (collection != null)
        {
            return collection.Count < 1;
        }
        return !enumerable.Any();
    }

    public static T RandomElementOrDefault<T>(this IEnumerable<T> enumerable)
    {
        int randomIndex;

        if (!enumerable.Any())
        {
            return default(T);
        }


        randomIndex = UnityEngine.Random.Range(0, enumerable.Count());
        return enumerable.ElementAt(randomIndex);
    }

    public static int IndexOf<T>(this IEnumerable<T> enumerable, System.Func<T, bool> predicate)
    {
        int index = -1;
        foreach (T item in enumerable)
        {
            index++;

            if (predicate(item))
            {
                return index;
            }
        }

        return index;
    }

    #endregion

    #region List
    /// <summary>Move index within List[T]. https://stackoverflow.com/a/28597288 </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">List to affect</param>
    /// <param name="oldIndex">oldIndex</param>
    /// <param name="newIndex">newIndex</param>
    public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        // exit if positions are equal or outside array
        if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= list.Count) || (0 > newIndex) ||
            (newIndex >= list.Count)) return;
        // local variables
        var i = 0;
        T tmp = list[oldIndex];
        // move element down and shift other elements up
        if (oldIndex < newIndex)
        {
            for (i = oldIndex; i < newIndex; i++)
            {
                list[i] = list[i + 1];
            }
        }
        // move element up and shift other elements down
        else
        {
            for (i = oldIndex; i > newIndex; i--)
            {
                list[i] = list[i - 1];
            }
        }
        // put element from position 1 to destination
        list[newIndex] = tmp;
    }
    public static T RandomElementOrDefault<T>(this List<T> list)
    {
        return ((IEnumerable<T>)list).RandomElementOrDefault();
    }

    // Fisher-Yates shuffle algorithm
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    #endregion
}