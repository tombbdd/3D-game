using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bloodBar_GUI : MonoBehaviour {
    private float curBlood = 0f;
    private float targetBlood = 0f;
    public Slider bloodBar;
    GameObject addBtn, minBtn;

    private void Start() {
        addBtn = GameObject.Find("addButton");
        Button a = addBtn.GetComponent<Button>();
        minBtn = GameObject.Find("minusButton");
        Button b = minBtn.GetComponent<Button>();
        a.onClick.AddListener(delegate () {
            this.OnClick(addBtn);
        });
        b.onClick.AddListener(delegate () {
            this.OnClick(minBtn);
        });
    }

    private void OnClick(GameObject sender) {
        if (sender.name == "addButton") addBlood();
        if (sender.name == "minusButton") desBlood();
    }

    public void addBlood() {
        if(targetBlood - 1f<0f){
            targetBlood=0f;
        }else{
            targetBlood=targetBlood - 1f;
        }
    }

    public void desBlood() {
        if(targetBlood + 1f > 10f){
            targetBlood=10f;
        }else{
            targetBlood=targetBlood + 1f;
        }
    }

    void Update() {
        curBlood = Mathf.Lerp(curBlood, targetBlood, 0.1f);
        bloodBar.value = curBlood;
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }
}