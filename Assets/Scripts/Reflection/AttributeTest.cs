using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class AttributeTest : MonoBehaviour
{
    private void Start()
    {
        AttTest();
    }

    private void AttTest()
    {
        Type type = typeof(AttributeTutorial); //获取类型
        var mems = type.GetMembers(); //获取类型中的成员信息
        foreach (var mem in mems)  //遍历成员信息
        {
            var attribute = mem.GetCustomAttribute<MyAttribute>(); //获取成员信息上的特性
            if (attribute != null) //如果特性不为空
            {
                Debug.Log($"发现一个att,其信息是： {attribute.someTagInfo}"); //打印特性信息
            }
        }
    }

    //输出：
    //发现一个att,其信息是： a Method
    //发现一个att,其信息是： a property
    //发现一个att,其信息是： a string
}
