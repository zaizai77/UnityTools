using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MethodAttributeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AttTest();
    }

    private void AttTest()
    {
        Type type = typeof(AttributeTutorial); //获取类型
        var mems = type.GetMembers(); //获取类型中的成员信息
        foreach (var mem in mems) //遍历成员信息
        {
            var methodAtt = mem.GetCustomAttribute<MethodAttribute>(); //获取成员信息上的特性
            if (methodAtt != null) //如果特性不为空
            {
                var methodInfo = mem as MethodInfo; //把成员信息转换为方法信息（MethodInfo）
                var del = Delegate.CreateDelegate(typeof(Action), methodInfo) as Action; //进行委托创建
                del?.Invoke(); //调用委托
            }
        }
    }

    //输出
    //芜湖！我们是通过标签委托进行调用的！
    //UnityEngine.Debug:LogError(object)
}
