using UnityEngine;

/// <summary>Transform extensions</summary>
public static class TransformExtensions
{
    /// <summary>Destroy all children underneath this parent</summary>
    /// <param name="self">This Transform</param>
    public static void DestroyAllChildren(this Transform self)
    {
        foreach (Transform child in self)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
