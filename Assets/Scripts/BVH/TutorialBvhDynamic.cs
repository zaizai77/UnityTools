using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBvhDynamic : MonoBehaviour
{
    private DynamicBvhSpace sahSpace;
    private List<GameObject> seneObjects;
    [Range(0, 10)]
    public int displayDepth;

    public GameObject removeObj;

    void Start()
    {
        seneObjects = new List<GameObject>();
        sahSpace = new DynamicBvhSpace();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var randomPos = Random.insideUnitSphere * 10;
            go.transform.position = randomPos;
            sahSpace.AddNode(go);
            seneObjects.Add(go);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (removeObj != null)
            {
                sahSpace.RemoveNode(removeObj);
                Destroy(removeObj);
                seneObjects.Remove(removeObj);
            }
        }
        if (removeObj != null)
        {
            sahSpace.UpdateNode(removeObj);
        }
    }

    private void OnDrawGizmos()
    {
        sahSpace?.root?.DrawDepth(displayDepth);
    }
}
