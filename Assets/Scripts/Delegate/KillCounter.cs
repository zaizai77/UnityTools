using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCounter
{
    public event Action<int> killEvent; //定义公开事件，事件定义关键字：“event”
                                        //Action<int>指没有返回值，有一个入参为int类型的委托方法

    //委托声明
    public delegate int MathOperation(int x, int y);

    //实例化委托

    public int AddNum(int one,int two)
    {
        Debug.Log(one + two);
        return one + two;
    }

    public void Awake()
    {
        MathOperation mathOperation = new MathOperation(AddNum);
        mathOperation(1, 2);
    }


    public KillCounter()
    {

    }
    public void DoKill(int count)
    {
        killEvent(count);
    }
}
