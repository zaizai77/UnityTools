using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BvhSpace
{
    public BvhNode root { get; private set; }
    public void BuildBvh(List<GameObject> sceneObjects, int depth, int type = 0)
    {
        root = new BvhNode("root", null);
        foreach (var obj in sceneObjects)
        {
            var aabb = BvhNode.ComputeWorldAABB(obj);
            root.UnionAABB(aabb);
        }

        if (type == 0)
            BinaryPartition(root, sceneObjects, 0, sceneObjects.Count, depth);
        else
            AxisPartition(root, sceneObjects, depth);
    }

    //最大方差轴分割
    private void AxisPartition(BvhNode node, List<GameObject> sceneObjects, int depth)
    {
        if (depth <= 0) return;

        var leftNode = new BvhNode(node.name + "_leftKid_" + depth.ToString(), null);
        var rightNode = new BvhNode(node.name + "_rightKid_" + depth.ToString(), null);

        var leftNodeSceneObjects = new List<GameObject>();
        var rightNodeSceneObjects = new List<GameObject>();

        var mode = PickVariance(sceneObjects);
        switch (mode)
        {
            case 0:
                //以当前AABB的中心点二分
                var middleX = node.aabb.center.x;
                foreach (var obj in sceneObjects)
                {
                    var aabb = BvhNode.ComputeWorldAABB(obj);
                    if (obj.transform.position.x <= middleX)
                    {
                        leftNodeSceneObjects.Add(obj);
                        leftNode.UnionAABB(aabb);
                    }
                    else
                    {
                        rightNodeSceneObjects.Add(obj);
                        rightNode.UnionAABB(aabb);
                    }
                }
                break;
            case 1:
                var middleY = node.aabb.center.y;
                foreach (var obj in sceneObjects)
                {
                    var aabb = BvhNode.ComputeWorldAABB(obj);
                    if (obj.transform.position.y <= middleY)
                    {
                        leftNodeSceneObjects.Add(obj);
                        leftNode.UnionAABB(aabb);
                    }
                    else
                    {
                        rightNodeSceneObjects.Add(obj);
                        rightNode.UnionAABB(aabb);
                    }
                }
                break;
            case 2:
                var middleZ = node.aabb.center.z;
                foreach (var obj in sceneObjects)
                {
                    var aabb = BvhNode.ComputeWorldAABB(obj);
                    if (obj.transform.position.z <= middleZ)
                    {
                        leftNodeSceneObjects.Add(obj);
                        leftNode.UnionAABB(aabb);
                    }
                    else
                    {
                        rightNodeSceneObjects.Add(obj);
                        rightNode.UnionAABB(aabb);
                    }
                }
                break;
        }

        node.SetLeaf(leftNode, rightNode);
        AxisPartition(leftNode, leftNodeSceneObjects, depth - 1);
        AxisPartition(rightNode, rightNodeSceneObjects, depth - 1);
    }

    //寻找最大方差轴
    private int PickVariance(List<GameObject> sceneObjects)
    {
        var mean_x = 0.0f;
        var mean_y = 0.0f;
        var mean_z = 0.0f;

        //统计期望
        foreach (var obj in sceneObjects)
        {
            var position = obj.transform.position;
            mean_x += position.x;
            mean_y += position.y;
            mean_z += position.z;
        }

        mean_x /= (float)sceneObjects.Count;
        mean_y /= (float)sceneObjects.Count;
        mean_z /= (float)sceneObjects.Count;

        var variance_x = 0.0f;
        var variance_y = 0.0f;
        var variance_z = 0.0f;

        //统计方差
        foreach (var obj in sceneObjects)
        {
            var position = obj.transform.position;
            variance_x += Mathf.Pow(position.x - mean_x, 2);
            variance_y += Mathf.Pow(position.y - mean_y, 2);
            variance_z += Mathf.Pow(position.z - mean_z, 2);
        }

        variance_x /= (float)(sceneObjects.Count - 1);
        variance_y /= (float)(sceneObjects.Count - 1);
        variance_z /= (float)(sceneObjects.Count - 1);

        //x轴
        if (variance_x > variance_y && variance_x > variance_z)
            return 0;

        //y轴
        if (variance_y > variance_z && variance_y > variance_x)
            return 1;

        //z轴
        if (variance_z > variance_x && variance_z > variance_y)
            return 2;

        return 0;
    }
    //二分分割
    //递归算法，对一个List<GameObject>进行二分划分。
    private void BinaryPartition(BvhNode node, List<GameObject> objs, int startIndex, int endIndex, int depth)
    {
        if (depth <= 0) return;

        //计算二分下标
        var halfIndex = (endIndex + startIndex) / 2;
        var leftNode = new BvhNode(node.name + "_leftKid_" + depth.ToString(), null);
        var rightNode = new BvhNode(node.name + "_rightKid_" + depth.ToString(), null);

        //前半部分，统计AABB
        for (int i = startIndex; i < halfIndex; i++)
        {
            var obj = objs[i];
            var aabb = BvhNode.ComputeWorldAABB(obj);
            leftNode.UnionAABB(aabb);
        }

        //后半部分，统计AABB
        for (int i = halfIndex; i < endIndex; i++)
        {
            var obj = objs[i];
            var aabb = BvhNode.ComputeWorldAABB(obj);
            rightNode.UnionAABB(aabb);
        }

        node.SetLeaf(leftNode, rightNode);

        //前半部分递归
        BinaryPartition(leftNode, objs, startIndex, halfIndex, depth - 1);
        //后半部分递归
        BinaryPartition(rightNode, objs, halfIndex, endIndex, depth - 1);
    }
}
