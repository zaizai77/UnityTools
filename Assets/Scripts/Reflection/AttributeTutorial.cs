using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTutorial
{
    [My("a string")]
    public string oneString;

    [My("a property")]
    public bool oneProperty { get; set; }

    [My("a Method")]
    public void OneMethod()
    {

    }

    [MethodAttribute] //赋予特性
    public static void DelegateMethod() //这是一个静态函数
    {
        Debug.LogError("芜湖！我们是通过标签委托进行调用的！");
    }

    //My后面的括号，就相当于一个class的构造函数，如果我们定义的构造函数有参数，则需要指定参数值。
}
