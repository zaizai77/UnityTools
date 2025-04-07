using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    //空间内包含的物体
    public List<GameObject> areaObjects;
    //空间中心
    public Vector3 center;
    //空间尺寸
    public float size;

    //必定有8个子节点
    private const int kidCount = 8;
    private OctreeNode[] kids;
    public OctreeNode(Vector3 center, float size)
    {
        kids = new OctreeNode[kidCount];
        this.center = center;
        this.size = size;

        areaObjects = new List<GameObject>();
    }

    #region 为了访问子节点，创建一系列属性
    public OctreeNode top0
    {
        get
        {
            return kids[0];
        }
        set
        {
            kids[0] = value;
        }
    }

    public OctreeNode top1
    {
        get
        {
            return kids[1];
        }
        set
        {
            kids[1] = value;
        }
    }

    public OctreeNode top2
    {
        get
        {
            return kids[2];
        }
        set
        {
            kids[2] = value;
        }
    }

    public OctreeNode top3
    {
        get
        {
            return kids[3];
        }
        set
        {
            kids[3] = value;
        }
    }

    public OctreeNode bottom0
    {
        get
        {
            return kids[4];
        }
        set
        {
            kids[4] = value;
        }
    }

    public OctreeNode bottom1
    {
        get
        {
            return kids[5];
        }
        set
        {
            kids[5] = value;
        }
    }

    public OctreeNode bottom2
    {
        get
        {
            return kids[6];
        }
        set
        {
            kids[6] = value;
        }
    }

    public OctreeNode bottom3
    {
        get
        {
            return kids[7];
        }
        set
        {
            kids[7] = value;
        }
    }
    #endregion

    #region 数学逻辑判断，物体的记录，可视化代码

    //获取当前空间内记录的物体数量
    public int objectCount => areaObjects.Count;

    //unity gizmos可视化代码
    public void DrawGizmos()
    {
        Gizmos.DrawWireCube(center, Vector3.one * size);
    }

    //判断空间是否包含某个点
    public bool Contains(Vector3 position)
    {
        var halfSize = size * 0.5f;
        return Mathf.Abs(position.x - center.x) <= halfSize &&
            Mathf.Abs(position.y - center.y) <= halfSize &&
            Mathf.Abs(position.z - center.z) <= halfSize;
    }

    //清理当前空间内物体
    public void ClearArea()
    {
        areaObjects.Clear();
    }

    //记录物体
    public void AddGameobject(GameObject obj)
    {
        areaObjects.Add(obj);
    }

    #endregion

}
