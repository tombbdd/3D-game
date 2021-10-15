using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//仅仅执行多个移动函数，将动作存在队列中。通过GetSSAction来得到目的地和速度
public class CCSequenceAction : SSAction, ISSActionCallback
{
    public List<SSAction> sequence;
    public int repeat = -1; //repeat forever
    public int start = 0;

    public static CCSequenceAction GetSSAction(int repeat, int start, List<SSAction> sequence)
    {
        CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }

    //  执行动作前，为每个动作注入当前动作游戏对象，并将自己作为动作事件的接收者。
    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this;
            action.Start();
        }
    }

    // 执行当前动作
    public override void Update()
    {
        if (sequence.Count == 0) return;
        if (start < sequence.Count)
            sequence[start].Update();
    }
    // 收到当前动作执行完成，推下一个动作，如果完成一次循环，减次数。如完成，通知该动作的管理者。
    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, Object objectParam = null)
    {
        source.destory = false;
        this.start++;
        if (this.start >= sequence.Count)
        {
            this.start = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0)
            {
                this.destory = true;
                this.callback.SSActionEvent(this);
            }
            else
            {
                sequence[start].Start();
            }
        }
    }

    private void OnDestroy()
    {
        //destory
    }
}