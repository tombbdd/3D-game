using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class ClickGUI : MonoBehaviour {
    UserAction action;
    MyCharacterController character;
	// 得到唯一的实例
	void Start () {
        action = Director.getInstance().sceneController as UserAction;
    }
    //鼠标点击触发不同事件
    void OnMouseDown()
    {
        if (action.stop())
            return;
        if (gameObject.name == "boat")
        {
            action.moveBoat();
        }
        else
        {
            action.characterIsClicked(character);
        }
    }
    //设置角色控制器
    public void setController(MyCharacterController characterCtrl)
    {
        character = characterCtrl;
    }
}