这次作业完成打飞碟游戏，我感觉主要的难点在几个部分：使用工厂模式产生飞碟游戏对象，生成飞碟对象时随机分配飞碟的种类（分值、速度），动作分离，编写用户鼠标点击飞盘的事件。
动作分离因为在上一次游戏已经做过了，所以可以复用一些代码，也可以仿照着写，然后MVC模式也可以仿照上一次作业。
工厂模式代码实现：
基本思路就是先获取到飞盘的预制，利用预制生成游戏对象，然后提供一个getdisk函数提供给上层调用，每次调用getdisk函数时，实例化一个游戏对象，接着要根据游戏的当前关卡，随机分配一个种类给飞盘，对应不同的分数、速度、颜色，接着再随机生成一个再游戏中出现的位置。同时也提供一个销毁函数给上层调用，用于销毁使用完毕的飞盘或者响应用户点击事件。
```
//用于制造飞碟的 类
public class DiskFactory : MonoBehaviour {
    public GameObject diskPrefab;
    //保存飞碟
    private List<DiskComponent> used = new List<DiskComponent>();    
    private List<DiskComponent> free = new List<DiskComponent>();    
    private int num = 1;
    //根据游戏的当前轮数，随机的为飞碟分配一个身份（对应不同分值），前面的关卡没有高分的飞碟
    private int getCharacter(int round)
    {
        int character = 0;
        if (round == 0)
        {
            character = 0;
        }
        else if (round == 1)
        {
            character = Random.Range(-2f, 1f) > 0 ? 1 : 0;
        }
        else
        {
            character = Random.Range(-3f, 2f) > 0 ? 1 : 2;
        }
        return character;
    }
    //随机初始化一个丢出飞碟的位置
    private float getStartX(int round)
    {
        if (round == 0)
        {
            return UnityEngine.Random.Range(-0.5f, 0.5f);
        }
        else if (round == 1)
        {
            return UnityEngine.Random.Range(-1.5f, 1.5f);
        }
        else
        {
            return UnityEngine.Random.Range(-2.5f, 2.5f);
        }
    }
    //利用提前制作好的预制生成游戏对象
    private void Awake()
    {
        diskPrefab = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Disk"), Vector3.zero, Quaternion.identity);
        diskPrefab.SetActive(false);
    }
    //产生一个飞碟，并为其初始化相关的数据
    public GameObject GetDisk(int round)
    {
        GameObject newdisk = null;
        if(free.Count > 0)
        {
            newdisk = free[0].gameObject;
            free.Remove(free[0]);
        }
        else
        {
            newdisk = GameObject.Instantiate<GameObject>(diskPrefab, Vector3.zero, Quaternion.identity);
            newdisk.AddComponent<DiskComponent>();
        }
        //随机生成飞碟的种类、初始位置
        int disk_character = getCharacter(round);
        float RanX = getStartX(round);
        //按照飞碟的种类，初始化各项参数，分值、速度、颜色、位置等
        switch (disk_character)
        {
            case 0:
                {
                    newdisk.GetComponent<DiskComponent>().score = 1;
                    newdisk.GetComponent<DiskComponent>().speed = 5.0f;
                    newdisk.GetComponent<DiskComponent>().direction = new Vector3(RanX, 1.5f, 0);
                    newdisk.GetComponent<Renderer>().material.color = Color.white;
                    break;
                }
            case 1:
                {
                    newdisk.GetComponent<DiskComponent>().score = 2;
                    newdisk.GetComponent<DiskComponent>().speed = 6.0f;
                    newdisk.GetComponent<DiskComponent>().direction = new Vector3(RanX, 1, 0);
                    newdisk.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                }
            case 2:
                {
                    newdisk.GetComponent<DiskComponent>().score = 3;
                    newdisk.GetComponent<DiskComponent>().speed = 7.0f;
                    newdisk.GetComponent<DiskComponent>().direction = new Vector3(RanX, 0.5f, 0);
                    newdisk.GetComponent<Renderer>().material.color = Color.red;
                    break;
                }
        }
        used.Add(newdisk.GetComponent<DiskComponent>());
        newdisk.name = "disk_" + num;
        num++;
        return newdisk;
    }
    //销毁使用完毕的飞碟
    public void FreeDisk(GameObject disk)
    {
        DiskComponent temp = null;
        foreach (DiskComponent i in used)
        {
            if (disk.GetInstanceID() == i.gameObject.GetInstanceID())
            {
                temp = i;
            }
        }
        if(temp != null)
        {
            temp.gameObject.SetActive(false);
            free.Add(temp);
            used.Remove(temp);
        }
    }

}
```

响应用户鼠标点击事件：
```
public void hit(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.gameObject.GetComponent<DiskComponent>() != null)
            {
                //用户点击飞碟时，获得飞碟对应的分数，调用下层提供的函数
                scoreRecorder.setScore(hit.collider.gameObject);
                //为了方便直接将飞碟位置移到地面以下，会自动调用销毁函数
                hit.collider.gameObject.transform.position = new Vector3(0, -6, 0);
            }
        }
    }
```

游戏进程的整体控制：
整体上就是根据玩家的分数，判断游戏胜负，关卡数，同时在游戏过程中根据时间间隔丢出飞碟。
```
private void Update()
    {
        if (scoreRecorder.getScore() <= point[num] && gameState == GameState.ROUND_FINISH)
        {
            currentRound = -1;
            round = 3;
            time = 0;
            num = 0;
            gameState = GameState.GAME_OVER;
            scoreRecorder.Reset();
        }
        if (scoreRecorder.getScore() >= 30 && gameState == GameState.ROUND_FINISH)
        {       
            currentRound = -1;
            round = 3;
            time = 0;
            num = 0;
            scoreRecorder.Reset();
            gameState = GameState.WIN;
        }
        if (actionManager.DiskNumber == 0 && gameState == GameState.RUNNING)
        {
            gameState = GameState.ROUND_FINISH;
        }
        if (actionManager.DiskNumber == 0 && gameState == GameState.ROUND_START)
        {
            currentRound = (currentRound + 1) % round;
            num++;
            DiskFactory df = Singleton<DiskFactory>.Instance;
            for (int i = 0; i < diskNumber; i++)
            {
                diskQueue.Enqueue(df.GetDisk(currentRound));
            }
            actionManager.Throw(diskQueue);
            actionManager.DiskNumber = 10;
            gameState = GameState.RUNNING;
        }
        if (time > 0.5)
        {
            ThrowDisk();
            time = 0;
        }
        else
        {
            time += Time.deltaTime;
        }
        Debug.Log(num);
    }
```

除此之外还封装了移动函数，不过具体实现和魔鬼与牧师的上船部分相似，除了新加入了竖直方向上的移动，所以为了契合现实生活，按照重力加速度设置飞碟的移动速度
```
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
```

上面部分是基础版的实现，因为第二节课老师说直接将两次作业一起完成，所以可以利用unity自带的物理引擎来帮助实现，同时还需要按照adapt模式修改
不过大部分代码都没有进行改动，增加了几个脚本。

这个脚本就是为预制的游戏角色添加刚体属性，同时使用了物理引擎来实现运动，即在飞碟飞出的时候给一个力，这样就会让每个飞碟有个随机的加速度
```
public class PhysicMoveAction : SSAction {
    Vector3 force;
    float startX;
    public SceneController sceneControler = (SceneController)Director.getInstance().sceneController;
    //随机产生一个飞碟飞出位置以及飞出时初始力，决定了飞出速度
    public override void Start()
    {
        force = new Vector3(2 * Random.Range(-1, 1), -2.5f * Random.Range(0.5f, 2), -1 + 2 * Random.Range(0.5f, 2));//力的大小  
    }
    public static PhysicMoveAction GetSSAction()
    {
        PhysicMoveAction action = ScriptableObject.CreateInstance<PhysicMoveAction>();
        return action;
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


```
适配器接口实现
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IActionManager {
    void Throw(Queue<GameObject> diskQueue);
    int getDiskNumber();
    void setDiskNumber(int num);
}

```

没有删除原来有的CCAction，可以自行选择想使用的动作管理器进行游戏

游戏运行方法：
1.将Asset文件夹替换原来项目中的文件夹，其中包含了提前制作好的预制以及所有的c#脚本
2.创建一个空对象，将脚本CCActionManager、USERGui、Diskfactory、Scorerecorder、FirstController、PhysicMoveActionManager挂载到空对象上
3.点击运行即可

基础版演示视频地址：https://www.bilibili.com/video/BV1mT4y1o7pb?p=1&share_medium=ipad&share_plat=ios&share_source=QQ&share_tag=s_i&timestamp=1634715770&unique_k=L1u7tu

使用物理引擎：https://www.bilibili.com/video/BV1mT4y1o75S?p=1&share_medium=ipad&share_plat=ios&share_source=QQ&share_tag=s_i&timestamp=1634806035&unique_k=XsNvNl
