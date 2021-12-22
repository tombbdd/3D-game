using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloodBar_script : MonoBehaviour {
    public float curBlood = 10f;
    private float targetBlood = 10f;
    private Rect bloodBarArea;
    private Rect addBtn;
    private Rect minBtn;

	void Start () {
        bloodBarArea = new Rect(Screen.width - 220, 20, 200, 50);
        addBtn = new Rect(Screen.width - 220, 50, 40, 20);
        minBtn = new Rect(Screen.width - 60, 50, 40, 20);
	}

    public void addBlood(float num) {
        if(targetBlood + num > 10f){
            targetBlood =10f;
            //超过满血时显示为满血
        }else{
            targetBlood =targetBlood + num;
        }
        
    }

    public void desBlood(float num) {
        if(targetBlood - num < 0f){
            targetBlood=0f;
            //血量低于0时显示为0
        }else{
            targetBlood = targetBlood - num;
        }
        
    }

    private void OnGUI() {
        if (GUI.Button(addBtn, "加血")) addBlood(1);
        if (GUI.Button(minBtn, "扣血")) desBlood(1);
        curBlood = Mathf.Lerp(curBlood, targetBlood, 0.1f);
        GUI.HorizontalScrollbar(bloodBarArea, 0f, curBlood, 0f, 10f);
    }
}