using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class PhysicMoveAction : SSAction {
    Vector3 force;
    float startX;
    public SceneController sceneControler = (SceneController)Director.getInstance().sceneController;
    public static PhysicMoveAction GetSSAction()
    {
        PhysicMoveAction action = ScriptableObject.CreateInstance<PhysicMoveAction>();
        return action;
    }
    //随机产生一个飞碟飞出位置以及飞出时初始力，决定了飞出速度
    public override void Start()
    {
        force = new Vector3(2 * Random.Range(-1, 1), -2.5f * Random.Range(0.5f, 2), -1 + 2 * Random.Range(0.5f, 2));//力的大小  
    }
    public override void Update()
    {
        if (gameobject.activeSelf)
        {
            //不需要在制作预设时就添加component，在这里通过编程为游戏角色添加刚体属性
            Debug.Log(this.transform.position.y);
            if(gameobject.GetComponent<Rigidbody>() == null)
                gameobject.AddComponent<Rigidbody>();
            gameobject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameobject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            if (this.transform.position.y < -5){
                //检测飞碟位置，当飞碟落到地面以下则销毁游戏对象
                this.destroy = true;
                this.enable = false;
                this.callback.SSActionEvent(this);
            }
        }
    }
}
