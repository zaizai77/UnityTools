using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuItemTest : MonoBehaviour
{
    [MenuItem("CustomTools/MyRefTest")] //字符串定义了这个委托在Editor的面板路径
    public static void SomeLog() //静态函数
    {
        Debug.Log("这是通过unity按钮调用的！");
    }

    //输出
    //这是通过unity按钮调用的！
    //UnityEngine.Debug:Log(object)
}
