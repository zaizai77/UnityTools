using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBvh : MonoBehaviour
{
    private BvhSpace binarySpace;

    [Range(1, 100)]
    public int generateCount = 10;
    private List<GameObject> seneObjects;

    [Range(0, 10)]
    public int partionDepth = 4;
    [Range(0, 10)]
    public int displayDepth;

    public enum GenerateType
    {
        Ordered,
        Random
    }

    public GenerateType generateType;


    void Start()
    {
        seneObjects = new List<GameObject>();
        binarySpace = new BvhSpace();

        CreateScene();
    }

    private void CreateScene()
    {
        var halfCount = generateCount * 0.5f;
        for (int i = 0; i < generateCount; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var positionX = i - halfCount;
            var randomPos = Random.insideUnitSphere * 10;
            if (generateType == GenerateType.Ordered)
            {
                go.transform.position = new Vector3(positionX, randomPos.y, randomPos.z);
            }
            else
            {
                go.transform.position = randomPos;
            }
            go.name = "Sphere_" + i.ToString();

            seneObjects.Add(go);
        }

        binarySpace.BuildBvh(seneObjects, partionDepth,1);
    }

    private void OnDrawGizmos()
    {
        binarySpace?.root?.DrawTargetDepth(displayDepth);
    }
}
