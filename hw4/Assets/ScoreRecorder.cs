using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;


//得分记录者
public class ScoreRecorder : MonoBehaviour {
    private int score;

    private void Start()
    {
        score = 0;
    }

    public int getScore() { return score; }

    public void setScore(GameObject disk)
    {
        score += disk.GetComponent<DiskComponent>().score;
    }

    public void Reset()
    {
        score = 0;
    }
}
