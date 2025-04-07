using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOctree : MonoBehaviour
{
    //生成物体数量
    [Range(0, 500)]
    public int genCount = 100;

    //Octree 构建最大深度
    [Range(1, 8)]
    public int buildDepth = 3;
    //Octree 的根节点
    public OctreeNode root;

    //物体生成范围
    [Range(1, 300)]
    public float range = 100;

    //记录生成的场景物体
    private List<GameObject> sceneObjects;
    private void Start()
    {
        GenSceneObjects();
        OctreePartion();
    }

    [Header("可视化相关")]
    //是否显示八叉树
    public bool showOctree = true;
    //可视化类型
    public OctreeDebugMode octreeDebugMode;
    //可视化深度
    [Range(0, 8)]
    public int displayDepth = 3;
    //不同深度的可视化颜色
    public Color[] displayColor;
     
    [Header("查询")]
    //是否显示场景查询的结果
    public bool showQueryResult = true;
    //检查点对象
    public GameObject checkTarget;
    //场景查询的结果，邻近物体列表
    private List<GameObject> queryObjects;
    //场景查询的结果，检查点所属节点
    private OctreeNode queryNode;


    //生成场景物体
    private void GenSceneObjects()
    {
        var genRange = range * 0.5f;
        sceneObjects = new List<GameObject>();

        for (int i = 0; i < genCount; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(Random.Range(-genRange, genRange),
                Random.Range(-genRange, genRange),
                Random.Range(-genRange, genRange));
            obj.hideFlags = HideFlags.HideInHierarchy;
            sceneObjects.Add(obj);
        }
    }

    //进行Octree的划分
    private void OctreePartion()
    {
        var initialOrigin = Vector3.zero;
        root = new OctreeNode(initialOrigin, range);
        root.areaObjects = sceneObjects;
        GenerateOctree(root, range, buildDepth);
    }

    /// <summary>
    /// 如果当前生成深度大于0， 则从【当前节点】创建8个【子节点】
    ///遍历【当前节点】中记录的场景对象，将对象分配到对应的【子节点】中
    ///当前深度减一，遍历【子节点】，尝试执行步骤1。直到【子节点】中的物体数量小于等于2，或者深度值为0
    /// </summary>
    /// <param name="root"></param>
    /// <param name="range"></param>
    /// <param name="depth"></param>
    private void GenerateOctree(OctreeNode root, float range, float depth)
    {
        if (depth <= 0) return;

        //计算grid的中心、尺寸
        var halfRange = range / 2.0f;
        var rootOffset = halfRange / 2.0f;
        var rootCenter = root.center;

        //1. 创建8个子节点
        var origin = rootCenter + new Vector3(-1, 1, -1) * rootOffset;
        root.top0 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, 1, -1) * rootOffset;
        root.top1 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, 1, 1) * rootOffset;
        root.top2 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(-1, 1, 1) * rootOffset;
        root.top3 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(-1, -1, -1) * rootOffset;
        root.bottom0 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, -1, -1) * rootOffset;
        root.bottom1 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, -1, 1) * rootOffset;
        root.bottom2 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(-1, -1, 1) * rootOffset;
        root.bottom3 = new OctreeNode(origin, halfRange);

        //2. 遍历当前空间对象，分配对象到子节点
        PartitionSceneObjects(root);

        //3. 判断子节点对象数量，如果过多，则继续递归划分。
        if (root.top0.objectCount >= 2)
            GenerateOctree(root.top0, halfRange, depth - 1);

        if (root.top1.objectCount >= 2)
            GenerateOctree(root.top1, halfRange, depth - 1);

        if (root.top2.objectCount >= 2)
            GenerateOctree(root.top2, halfRange, depth - 1);

        if (root.top3.objectCount >= 2)
            GenerateOctree(root.top3, halfRange, depth - 1);

        if (root.bottom0.objectCount >= 2)
            GenerateOctree(root.bottom0, halfRange, depth - 1);

        if (root.bottom1.objectCount >= 2)
            GenerateOctree(root.bottom1, halfRange, depth - 1);

        if (root.bottom2.objectCount >= 2)
            GenerateOctree(root.bottom2, halfRange, depth - 1);

        if (root.bottom3.objectCount >= 2)
            GenerateOctree(root.bottom3, halfRange, depth - 1);
    }

    //将空间中的物体划分到子节点
    private void PartitionSceneObjects(OctreeNode root)
    {
        var objcets = root.areaObjects;
        foreach (var obj in objcets)
        {
            if (root.top0.Contains(obj.transform.position))
            {
                root.top0.AddGameobject(obj);
            }
            else if (root.top1.Contains(obj.transform.position))
            {
                root.top1.AddGameobject(obj);
            }
            else if (root.top2.Contains(obj.transform.position))
            {
                root.top2.AddGameobject(obj);
            }
            else if (root.top3.Contains(obj.transform.position))
            {
                root.top3.AddGameobject(obj);
            }
            else if (root.bottom0.Contains(obj.transform.position))
            {
                root.bottom0.AddGameobject(obj);
            }
            else if (root.bottom1.Contains(obj.transform.position))
            {
                root.bottom1.AddGameobject(obj);
            }
            else if (root.bottom2.Contains(obj.transform.position))
            {
                root.bottom2.AddGameobject(obj);
            }
            else if (root.bottom3.Contains(obj.transform.position))
            {
                root.bottom3.AddGameobject(obj);
            }
        }
    }

    #region 可视化
    private void OnDrawGizmos()
    {
        if (root == null) return;

        if (showOctree && displayDepth <= buildDepth)
        {
            //显示所有深度的空间范围
            if (octreeDebugMode == OctreeDebugMode.AllDepth)
            {
                Gizmos.color = new Color(1, 1, 1, 0.2f);
                DrawNode(root, displayDepth);
            }
            //只显示指定深度的空间范围
            else if (octreeDebugMode == OctreeDebugMode.TargetDepth)
            {
                if (displayColor.Length > displayDepth)
                {
                    var color = displayColor[displayDepth];
                    color.a = 0.2f;
                    Gizmos.color = color;
                    DrawTargetDepth(root, displayDepth);
                }
            }
        }

        if (showQueryResult)
        {
            Gizmos.color = Color.green;
            queryNode?.DrawGizmos();

            if (queryObjects != null)
            {
                Gizmos.color = Color.red;

                foreach (var obj in queryObjects)
                {
                    Gizmos.DrawWireSphere(obj.transform.position, 0.5f);
                    Gizmos.DrawLine(checkTarget.transform.position, obj.transform.position);
                }
            }
        }
    }

    //绘制指定深度
    private void DrawTargetDepth(OctreeNode node, int depth)
    {
        if (node == null) return;

        if (depth <= 0)
        {
            node.DrawGizmos();
            return;
        }

        var nextDepth = depth - 1;
        var kid = node.top0;
        DrawTargetDepth(kid, nextDepth);

        kid = node.top1;
        DrawTargetDepth(kid, nextDepth);

        kid = node.top2;
        DrawTargetDepth(kid, nextDepth);

        kid = node.top3;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom0;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom1;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom2;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom3;
        DrawTargetDepth(kid, nextDepth);
    }

    //绘制所有深度
    private void DrawNode(OctreeNode node, int depth)
    {
        if (node == null) return;

        if (depth > 0 && depth < displayColor.Length)
        {
            var color = displayColor[depth];
            color.a = 0.5f;
            Gizmos.color = color;
            node.DrawGizmos();
        }

        var kid = node.top0;
        DrawNode(kid, depth - 1);

        kid = node.top1;
        DrawNode(kid, depth - 1);

        kid = node.top2;
        DrawNode(kid, depth - 1);

        kid = node.top3;
        DrawNode(kid, depth - 1);

        kid = node.bottom0;
        DrawNode(kid, depth - 1);

        kid = node.bottom1;
        DrawNode(kid, depth - 1);

        kid = node.bottom2;
        DrawNode(kid, depth - 1);

        kid = node.bottom3;
        DrawNode(kid, depth - 1);
    }

    #endregion

    #region 查询

    private void Update()
    {
        if (checkTarget != null)
        {
            var position = checkTarget.transform.position;
            if (root.Contains(position))
            {
                var node = QueryOctTree(position, root);
                if (node != null)
                {
                    queryObjects = node.areaObjects;
                    queryNode = node;
                }
            }
            else
            {
                queryObjects = null;
                queryNode = null;
            }
        }
    }

    //我们已知有一个检查点的坐标，想知道场景中的物体，哪些与这个检查点很接近，意味着其可能发生碰撞。
    //场景查询函数
    private OctreeNode QueryOctTree(Vector3 position, OctreeNode checkNode)
    {
        if (checkNode.top0?.Contains(position) ?? false) return QueryOctTree(position, checkNode.top0);
        if (checkNode.top1?.Contains(position) ?? false) return QueryOctTree(position, checkNode.top1);
        if (checkNode.top2?.Contains(position) ?? false) return QueryOctTree(position, checkNode.top2);
        if (checkNode.top3?.Contains(position) ?? false) return QueryOctTree(position, checkNode.top3);

        if (checkNode.bottom0?.Contains(position) ?? false) return QueryOctTree(position, checkNode.bottom0);
        if (checkNode.bottom1?.Contains(position) ?? false) return QueryOctTree(position, checkNode.bottom1);
        if (checkNode.bottom2?.Contains(position) ?? false) return QueryOctTree(position, checkNode.bottom2);
        if (checkNode.bottom3?.Contains(position) ?? false) return QueryOctTree(position, checkNode.bottom3);

        return checkNode;
    }

    #endregion

}

public enum OctreeDebugMode
{
    AllDepth,
    TargetDepth
}
