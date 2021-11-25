using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    //分数增加
    public delegate void ScoreEvent();
    public static event ScoreEvent ScoreChange;
    public delegate void GameoverEvent();
    public static event GameoverEvent GameoverChange;
    public delegate void CrystalEvent();
    public static event CrystalEvent CrystalChange;

    //分数变化
    public void PlayerEscape()
    {
        if (ScoreChange != null)
        {
            ScoreChange();
        }
    }
    //游戏结束
    public void PlayerGameover()
    {
        if (GameoverChange != null)
        {
            GameoverChange();
        }
    }
    public void ReduceCrystalNum()
    {
        if (CrystalChange != null)
        {
            CrystalChange();
        }
    }
}
