using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

//定义飞碟运动的动作，继承了SSAction的动作基类
public class CCMoveAction : SSAction {
    public float move_t;     
    public float g;        
    public float vx;      
    public Vector3 dir;

    public override void Start () {
        enable = true;
        g = 9.8f;
        move_t = 0;
        vx = gameobject.GetComponent<DiskComponent>().speed;
        dir = gameobject.GetComponent<DiskComponent>().direction;
    }
    public override void Update () {
        if (gameobject.activeSelf)
        {
            move_t += Time.deltaTime;
            //竖直运动 v_y = g * move_t
            transform.Translate(Vector3.down * g * move_t * Time.deltaTime);
            //水平运动 vx = vx
            transform.Translate(dir * vx * Time.deltaTime);
            //飞碟落地情况，将信息回调
            if (this.transform.position.y < -5)
            {
                this.destroy = true;
                this.enable = false;
                this.callback.SSActionEvent(this);
            }
        }
    }
    public static CCMoveAction GetSSAction()
    {
        CCMoveAction action = ScriptableObject.CreateInstance<CCMoveAction>();
        return action;
    }
}
