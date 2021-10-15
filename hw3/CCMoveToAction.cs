using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//仅仅执行单个移动函数，通过GetSSAction来得到目的地和速度
public class CCMoveToAction : SSAction
{
    public Vector3 target;
    public float speed;

    public static CCMoveToAction GetSSAction(Vector3 target, float speed)
    {
        CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
        action.target = target;
        action.speed = speed;
        return action;
    }
    // Use this for initialization
    public override void Start(){}
    // Update is called once per frame
    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, 10f * Time.deltaTime);
        if (this.transform.position == target)
        {
            //waiting for destroy;
            this.destory = true;
            this.callback.SSActionEvent(this);
        }
    }
}