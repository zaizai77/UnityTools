using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
// AttributeTargets 是一个枚举值，其中使用【|】符号连接，表示【或】运算。
//这个就支持字段，属性，函数了

//[AttributeUsage(AttributeTargets.Field)]
public class MyAttribute : Attribute
{
    public string someTagInfo; //自定义字段，这里是一个“string”类型的变量
    public MyAttribute(string value) //定义构造函数，定义入参
    {
        someTagInfo = value; //变量赋值
    }
}

//上述就是一个自定义特性【MyAttribute】的使用方法，
//其中【AttributeUsage】这个“特性”指定了我们的自定义特性的“适用范围”，
//他的参数“AttributeTargets.Field”表示我们的自定义特性只对字段生效。

