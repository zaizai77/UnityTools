using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBvhSpace
{
    public BvhNode root { get; private set; }

    //维护一个当前bvh的所有叶子结点列表
    private List<BvhNode> leafs;
    private int generateCount = 0;

    //游戏对象与节点的映射
    public Dictionary<GameObject, BvhNode> gameObjectToNode;

    public DynamicBvhSpace()
    {
        leafs = new List<BvhNode>();
        gameObjectToNode = new Dictionary<GameObject, BvhNode>();
    }

    //更新、记录游戏对象与节点的映射关系
    private void RecordGameobject(BvhNode node)
    {
        var obj = node.sceneObject;
        if (obj != null)
        {
            if (gameObjectToNode.ContainsKey(obj))
            {
                gameObjectToNode[obj] = node;
            }
            else
            {
                gameObjectToNode.Add(obj, node);
            }
        }
    }


    //添加一个节点
    public BvhNode AddNode(GameObject go)
    {
        //创建叶子节点
        BvhNode leaf = new BvhNode("node_" + generateCount.ToString(), go);
        Debug.Log(leaf);
        //记录映射
        RecordGameobject(leaf);
        //进行bvh的构建
        BuildBvh(leaf);

        generateCount++;
        return leaf;
    }

    //删除一个节点
    public bool RemoveNode(GameObject go)
    {
        if (gameObjectToNode.TryGetValue(go, out var node))
        {
            leafs.Remove(node);
            leafs.Remove(node.GetSibling());

            var subtree = BvhNode.SeparateNodes(node);
            if (subtree.isLeaf)
            {
                leafs.Add(subtree);
                RecordGameobject(subtree);
            }

            return true;
        }

        return false;
    }

    public void UpdateNode(GameObject go)
    {
        if (RemoveNode(go))
        {
            AddNode(go);
            //Debug.Log("true");
        }
    }

    //构建bvh
    public void BuildBvh(BvhNode leaf)
    {
        if (root == null)
        {
            root = leaf;
            leafs.Add(leaf);
        }
        else
        {
            //选取合适的点 -- 选取构建之后总体表面积最小的点
            var targetNode = SAH(leaf);
            if (targetNode != null)
            {
                leafs.Remove(targetNode);
                var newNode = BvhNode.CombineNodes(targetNode, leaf);

                leafs.Add(leaf);
                leafs.Add(newNode);

                RecordGameobject(newNode);

                root = newNode.FindRoot();
            }
            else
            {
                Debug.LogError("SAH 未找到合适的node");
            }
        }
    }

    //近似估计，所得到的AABB划分并不一定两两不相交。它只是尽可能地让AABB的表面积最小。
    private BvhNode SAH(BvhNode newLeaf)
    {
        var minCost = float.MaxValue;
        BvhNode minCostNode = null;

        //遍历所有叶子节点
        foreach (var leaf in leafs)
        {
            var newBranchAABB = leaf.aabb.Union(newLeaf.aabb);
            //新增的分支节点表面积
            var deltaCost = newBranchAABB.surfaceArea;
            var wholeCost = deltaCost;
            var parent = leaf.parent;
            //统计所有祖先节点的表面积差
            while (parent != null)
            {
                var s2 = parent.surfaceArea;
                var unionAABB = parent.aabb.Union(newLeaf.aabb);
                var s3 = unionAABB.surfaceArea;
                deltaCost = s3 - s2;
                wholeCost += deltaCost;
                parent = parent.parent;
            }

            //返回最小的目标
            if (wholeCost < minCost)
            {
                minCostNode = leaf;
                minCost = wholeCost;
            }
        }

        return minCostNode;
    }
}
