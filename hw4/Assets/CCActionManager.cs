using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class CCActionManager : SSActionManager,ISSActionCallback, IActionManager
{
    public FirstController sceneController;
    public List<CCMoveAction> Fly;
    public int DiskNumber = 0;              //统计飞碟个数

    private List<SSAction> used = new List<SSAction>();     // used是用来保存正在使用的动作
    private List<SSAction> free = new List<SSAction>();     // free是用来保存还未被激活的动作
    protected new void Start()
    {
        sceneController = (FirstController)Director.getInstance().sceneController;
        if(sceneController.is_physics == false)
            sceneController.actionManager = this;
        Fly.Add(CCMoveAction.GetSSAction());
    }
    public SSAction GetSSAction()
    {
        SSAction action = null;
        if (free.Count > 0)
        {
            action = free[0];
            free.Remove(free[0]);
        }
        else
        {
            action = ScriptableObject.Instantiate<CCMoveAction>(Fly[0]);
        }
        used.Add(action);
        return action;
    }
    //free掉一个在used的动作
    public void FreeSSAction(SSAction action)
    {
        SSAction temp = null;
        foreach(SSAction i in used)
        {
            if (action.GetInstanceID() == i.GetInstanceID())
            {
                temp = i;
            }
        }
        if(temp != null)
        {
            temp.reset();
            free.Add(temp);
            used.Remove(temp);
        }
    }
    //实现接口ISSActionCallback
    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, Object objectParam = null)
    {
        if (source is CCMoveAction)
        {
            DiskNumber--;
            DiskFactory df = Singleton<DiskFactory>.Instance;
            df.FreeDisk(source.gameobject);
            FreeSSAction(source);
        }
    }
    
    //扔飞盘的动作
    public void Throw(Queue<GameObject> diskQueue)
    {
        foreach (GameObject tmp in diskQueue)
        {
            RunAction(tmp, GetSSAction(), this);
        }
    }
    public int getDiskNumber()
    {
        return DiskNumber;
    }
    public void setDiskNumber(int num)
    {
        DiskNumber = num;
    }
}