using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    KillCounter killCounter = new KillCounter(); //实例化killCounter
    KillerAchievement killerAchievement = new KillerAchievement(); //实例化killerAchievement 

    private void Start()
    {
        killCounter.killEvent += killerAchievement.KillChecker; //进行事件【注册】
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) //简单的按钮测试，每按一次A，随机一个击杀量
        {
            killCounter.DoKill(Random.Range(1, 10));
            //killCounter.DoKill(125); //为了测试边界条件的递归调用
        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            killCounter.Awake();
        }
    }
}
