﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneController
{
    void LoadResources();
}

public interface IUserAction                          
{
    //玩家移动
    void MovePlayer(float translationX, float translationZ);
    //得到分数
    int GetScore();
    int GetCrystalNumber();
    //游戏结束标志
    bool GetGameover();
    //重新开始
    void Restart();
}

public interface ISSActionCallback
{
    void SSActionEvent(SSAction source,int intParam = 0,GameObject objectParam = null);
}

public interface IGameStatusOp
{
    void PlayerEscape();
    void PlayerGameover();
}
