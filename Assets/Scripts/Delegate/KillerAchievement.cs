using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//委托是一个函数指针，能把“逻辑方法(method)”作为参数传递。
//事件是一个实例委托对象，是对委托的封装。

//巧妙地使用委托与事件，也正是实现了“观察者模式”，
//从而遵循了“面向对象编程”的核心原则之一——低耦合与封装。

//Func 所引用的方法接收一个或者多个入参并带有一个返回值，
//Action所引用的方法接收一个或者多个参数并且没有返回值

public class KillerAchievement
{
    private int killCount; //当前击杀计数
    private const int tenKill = 10; //击杀10个敌人的阈值检测
    private const int hundredKill = 100; //击杀100个敌人的阈值检测
    private List<Func<bool>> achievenmentList; //“委托”列表,Func<bool>指没有入参，返回值为bool的委托方法
    private int checkerPointer; //委托列表的index指针

    public KillerAchievement() //实例化构造函数
    {
        killCount = 0;
        achievenmentList = new List<Func<bool>>();
        achievenmentList.Add(TenKillChecker); //添加连续成就检测委托
        achievenmentList.Add(HundredKillChecker);//添加连续成就检测委托
        achievenmentList.Add(null);//Trick：添加空委托实现遍历终止
        checkerPointer = 0;
    }

    public void KillChecker(int count) //用于对接外部事件的委托方法
    {
        killCount += count; //击杀计数累计
        Debug.Log($"收到一次击杀事件，击杀了{count}个敌人，总计击杀：{killCount}");
        var flag = achievenmentList[checkerPointer]?.Invoke() ?? false; //检查成就满足条件
        if (flag)
        {
            checkerPointer++; //前置成就满足，成就列表index指针向后偏移一位
            KillChecker(0); //连续递归成就检测，比如一次击杀200人，应该同时满足10和100的成就提示
        }
    }

    private bool TenKillChecker() //累计10击杀成就逻辑
    {
        if (killCount >= tenKill)
        {
            Debug.Log("十人斩！");
            return true;
        }
        return false;
    }

    private bool HundredKillChecker() //累计100击杀成就逻辑
    {
        if (killCount >= hundredKill)
        {
            Debug.Log("百人斩！");
            return true;
        }
        return false;
    }
}
