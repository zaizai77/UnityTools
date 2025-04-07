using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReflectionTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetClassInformation();
    }

    public void GetClassInformation()
    {
        Type type = typeof(MonoBehaviour);
        var methods = type.GetMethods();
        PrintInfo(methods, "methods");
        Debug.Log("---------------------------------------");
        var members = type.GetMembers();
        PrintInfo(members, "members");
        Debug.Log("---------------------------------------");
        var properties = type.GetProperties();
        PrintInfo(properties, "properties");
        Debug.Log("芜湖");
    }

    private void PrintInfo<T>(T[] values, string groupName)
    {
        Debug.Log($"{groupName}共有{values.Length}个");
        foreach (var value in values)
        {
            Debug.Log(value.ToString());
        }
    }
}
