### 血条（Health Bar）的预制设计。

- 分别使用 IMGUI 和 UGUI 实现
- 使用 UGUI，血条是游戏对象的一个子元素，任何时候需要面对主摄像机
- 分析两种实现的优缺点
- 给出预制的使用方法

### IMGUI

```c#
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
```

### 

### UGUI

```c#
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
```

### 优缺点

IMGUI：

优点：实现起来难度较低，代码结构简单

缺点：不利于布局，对象之间的位置难以协调。每次调用函数会全部重新渲染，性能较低

UGUI：

优点：性能较高

缺点：代码结构难度较大

### 预制使用方法

1.将assets文件夹加入到项目当中

2.使用提前准备好的Display场景即可运行

