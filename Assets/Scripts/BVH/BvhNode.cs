using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BvhNode
{
    //左子节点
    public BvhNode leftNode { get; private set; }
    //右子节点
    public BvhNode rightNode { get; private set; }
    //父节点
    public BvhNode parent;

    //节点所包含的对象
    public GameObject sceneObject { get; private set; }

    //节点所代表的AABB
    public AABB aabb { get; private set; }

    #region Debug
    //节点名称
    public string name { get; private set; }
    //节点颜色
    private Color spaceColor;
    #endregion

    /// <summary>
    /// 表面积
    /// </summary>
    public float surfaceArea => aabb.surfaceArea;

    public BvhNode(string name, GameObject obj)
    {
        BindSceneObject(obj);
        InitialAABB();

        this.name = name;
        spaceColor = new Color(Random.value, Random.value, Random.value, 0.9f);
    }

    //绑定场景对象
    public void BindSceneObject(GameObject sceneObject)
    {
        this.sceneObject = sceneObject;
    }

    //设置子节点
    public void SetLeaf(BvhNode left, BvhNode right)
    {
        this.leftNode = left;
        this.rightNode = right;
        if (left != null)
        {
            left.parent = this;
        }
        if (right != null)
        {
            right.parent = this;
        }

        this.sceneObject = null;
    }

    //初始化AABB
    public void InitialAABB()
    {
        ResetAABB();

        if (sceneObject == null) return;
        var objectAABB = ComputeWorldAABB(sceneObject);
        UnionAABB(objectAABB);
    }

    //计算物体网格的在世界空间的AABB
    public static AABB ComputeWorldAABB(GameObject obj)
    {
        var localMin = obj.GetComponent<MeshFilter>().sharedMesh.bounds.min;
        var localMax = obj.GetComponent<MeshFilter>().sharedMesh.bounds.max;
        var worldMin = obj.transform.TransformPoint(localMin);
        var worldMax = obj.transform.TransformPoint(localMax);

        return new AABB(worldMin, worldMax);
    }

    //重置AABB
    public void ResetAABB()
    {
        this.aabb = AABB.Reset();
    }

    //AABB求并集
    public void UnionAABB(AABB another)
    {
        this.aabb = this.aabb.Union(another);
    }

    public void DrawDepth(int depth)
    {
        if (depth != 0)
        {
            DrawGizmos();
            this.rightNode?.DrawDepth(depth - 1);
            this.leftNode?.DrawDepth(depth - 1);
        }
    }

    public void DrawTargetDepth(int depth)
    {
        if (depth <= 0)
        {
            DrawGizmos();
        }
        else
        {
            this.rightNode?.DrawTargetDepth(depth - 1);
            this.leftNode?.DrawTargetDepth(depth - 1);
        } 
    }

    public void DrawGizmos()
    {
        Gizmos.color = spaceColor;
        Gizmos.DrawWireCube(this.aabb.center, this.aabb.size);
        Gizmos.DrawSphere(this.aabb.minCorner, 0.1f);
        Gizmos.DrawSphere(this.aabb.maxCorner, 0.1f);
    }

    #region 更新节点

    //判断一下这个node是否是叶子结点
    public bool isLeaf => this.sceneObject != null;

    //构造函数，从另外一个"复制“生成
    public BvhNode(BvhNode source)
    {
        this.aabb = source.aabb;
        this.leftNode = source.leftNode;
        this.rightNode = source.rightNode;
        this.sceneObject = source.sceneObject;

        spaceColor = new Color(Random.value, Random.value, Random.value, 0.9f);
        this.name = source.name + "copied";
    }

    //获取兄弟节点
    public BvhNode GetSibling()
    {
        return this.parent?.GetTheOhterNode(this);
    }

    //获取另一个节点
    public BvhNode GetTheOhterNode(BvhNode separatetNode)
    {
        if (this.leftNode == separatetNode) return this.rightNode;
        if (this.rightNode == separatetNode) return this.leftNode;
        return null;
    }

    //查找根节点
    public BvhNode FindRoot()
    {
        if (this.parent != null)
        {
            return parent.FindRoot();
        }
        return this;
    }

    //合并两个node
    public static BvhNode CombineNodes(BvhNode targetNode, BvhNode insertNode)
    {
        //复制目标node信息
        var newNode = new BvhNode(targetNode);
        //合并 aabb
        targetNode.UnionAABB(insertNode.aabb);
        targetNode.AABBBroadCast();
        //重新设置左右子节点
        targetNode.SetLeaf(newNode, insertNode);

        return newNode;
    }

    //分离一个节点
    public static BvhNode SeparateNodes(BvhNode separatetNode)
    {
        var parent = separatetNode.parent;
        if (parent != null && parent.Contains(separatetNode))
        {
            var siblingNode = separatetNode.GetSibling();
            var siblingAABB = siblingNode.aabb;

            //把兄弟节点的子树丢给父节点
            parent.SetLeaf(siblingNode.leftNode, siblingNode.rightNode);
            //设置AABB
            parent.SetAABB(siblingAABB);
            //向上传播
            parent.AABBBroadCast();
            //绑定场景物体
            parent.BindSceneObject(siblingNode.sceneObject);

            return parent;
        }
        else
        {
            Debug.Log(parent);
            Debug.LogError("分离节点失败，目标节点父级为null或者父级不含有目标节点");
        }
        return null;
    }

    //检查节点是否为子节点
    public bool Contains(BvhNode node)
    {
        return this.leftNode == node || this.rightNode == node;
    }

    //设置aabb
    public void SetAABB(AABB aabb)
    {
        this.aabb = aabb;
    }

    //更新aabb
    public void UpdateAABB()
    {
        ResetAABB();
        if (leftNode != null)
        {
            UnionAABB(leftNode.aabb);
        }

        if (rightNode != null)
        {
            UnionAABB(rightNode.aabb);
        }
    }

    //aabb的向上广播
    public void AABBBroadCast()
    {
        if (this.parent != null)
        {
            this.parent.UpdateAABB();
            this.parent.AABBBroadCast();
        }
    }

    #endregion
}
