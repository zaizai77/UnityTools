using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeTutorialMethod
{
    [MethodAttribute] //赋予特性
    public static void DelegateMethod() //这是一个静态函数
    {
        Debug.LogError("芜湖！我们是通过标签委托进行调用的！");
    }
}
