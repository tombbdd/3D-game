using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

//创建 MonoBehaiviour 管理一个动作集合，动作做完自动回收动作。
public class SSActionManager : MonoBehaviour
{
    public Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();

    // 开始动作
    protected void Start()
    {

    }

    // 该类演示了复杂集合对象的使用。
    protected void Update()
    {
        foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
                waitingDelete.Add(ac.GetInstanceID());
            else if (ac.enable)
                ac.Update();
        }

        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }
// 提供了运行一个新动作的方法。该方法把游戏对象与动作绑定，并绑定该动作事件的消息接收者。
    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        action.destroy = false;
        action.enable = true;
        waitingAdd.Add(action);
        action.Start();
    }

}