using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AABB
{
    public Vector3 minCorner;
    public Vector3 maxCorner;

    public AABB(Vector3 min, Vector3 max)
    {
        this.minCorner = min;
        this.maxCorner = max;
    }

    public Vector3 size => maxCorner - minCorner;
    public Vector3 center => (maxCorner + minCorner) * 0.5f;

    public static AABB Reset()
    {
        return new AABB(Vector3.one * float.MaxValue, Vector3.one * float.MinValue);
    }

    /// <summary>
    /// 两个AABB取并集
    /// </summary>
    /// <param name="aabb"></param>
    /// <returns></returns>
    public AABB Union(AABB aabb)
    {
        minCorner.x = Mathf.Min(minCorner.x, aabb.minCorner.x);
        minCorner.y = Mathf.Min(minCorner.y, aabb.minCorner.y);
        minCorner.z = Mathf.Min(minCorner.z, aabb.minCorner.z);

        maxCorner.x = Mathf.Max(maxCorner.x, aabb.maxCorner.x);
        maxCorner.y = Mathf.Max(maxCorner.y, aabb.maxCorner.y);
        maxCorner.z = Mathf.Max(maxCorner.z, aabb.maxCorner.z);

        return new AABB(minCorner, maxCorner);
    }

    /// <summary>
    /// 表面积
    /// </summary>
    public float surfaceArea => (size.x * size.y + size.x * size.z + size.y * size.z) * 2.0f;
}
