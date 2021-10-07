using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class UserGUI : MonoBehaviour {
    private UserAction action;
    public int status = 0; // -1表示失败，1表示成功
    GUIStyle style;
    GUIStyle style2;
    GUIStyle buttonStyle;
    public bool show = false;

    void Start()
    {
        action = Director.getInstance().sceneController as UserAction;
        style = new GUIStyle();
        style.fontSize = 15;
        style.alignment = TextAnchor.MiddleLeft;
        style2 = new GUIStyle();
        style2.fontSize = 30;
        style2.alignment = TextAnchor.MiddleCenter;
    }

    void OnGUI()
    {
        if (status == -1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 65, 100, 50), "Gameover!", style2);   
        }
        else if (status == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 65, 100, 50), "You win!", style2);  
        }
        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 15;
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 20, 100, 50), "Restart", buttonStyle))
        {
            status = 0;
            action.restart();
        }
    }
}