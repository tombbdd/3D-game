1、智能巡逻兵

- 提交要求：
- 游戏设计要求：
  - 创建一个地图和若干巡逻兵(使用动画)；
  - 每个巡逻兵走一个3~5个边的凸多边型，位置数据是相对地址。即每次确定下一个目标位置，用自己当前位置为原点计算；
  - 巡逻兵碰撞到障碍物，则会自动选下一个点为目标；
  - 巡逻兵在设定范围内感知到玩家，会自动追击玩家；
  - 失去玩家目标后，继续巡逻；
  - 计分：玩家每次甩掉一个巡逻兵计一分，与巡逻兵碰撞游戏结束；



## 运行方式：

替换工程中的Assets文件，将场景替换为my scene，即可运行游戏



## 代码实现部分：

动作管理部分的代码可以复用之前作业的内容

FirstSceneController相当于整个游戏的进程控制，实现了订阅发布模式。进行游戏初始化时资源的导入，游戏的开始，游戏的进程推动，游戏结束的控制等等

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneController : MonoBehaviour, IUserAction, ISceneController
{
    public PropFactory patrol_factory;                          
    public ScoreRecorder recorder;                               
    public PatrolActionManager action_manager;                 
    public int wall_sign = -1;                                   
    public GameObject player;                                     
    public Camera main_camera;                                      
    public float player_speed = 5;                                   
    public float rotate_speed = 135f;                                
    private List<GameObject> patrols;                                
    private List<GameObject> crystals;                              
    private bool game_over = false;                                  

    void Update()
    {
        for (int i = 0; i < patrols.Count; i++)
        {
            patrols[i].gameObject.GetComponent<PatrolData>().wall_sign = wall_sign;
        }
        //水晶收集完毕
        if(recorder.GetCrystalNumber() == 0)
        {
            Gameover();
        }
    }
    void Start()
    {
        SSDirector director = SSDirector.GetInstance();
        director.CurrentScenceController = this;
        patrol_factory = Singleton<PropFactory>.Instance;
        action_manager = gameObject.AddComponent<PatrolActionManager>() as PatrolActionManager;
        LoadResources();
        main_camera.GetComponent<CameraFlow>().follow = player;
        recorder = Singleton<ScoreRecorder>.Instance;
    }

    public void LoadResources()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Plane"));
        player = Instantiate(Resources.Load("Prefabs/Player"), new Vector3(0, 9, 0), Quaternion.identity) as GameObject;
        crystals = patrol_factory.GetCrystal();
        patrols = patrol_factory.GetPatrols();
        //所有侦察兵移动
        for (int i = 0; i < patrols.Count; i++)
        {
            action_manager.GoPatrol(patrols[i]);
        }
    }
    public void MovePlayer(float translationX, float translationZ)
    {
        if(!game_over)
        {
            if (translationX != 0 || translationZ != 0)
            {
                player.GetComponent<Animator>().SetBool("run", true);
            }
            else
            {
                player.GetComponent<Animator>().SetBool("run", false);
            }
            //玩家移动和转向
            player.transform.Translate(0, 0, translationZ * player_speed * Time.deltaTime);
            player.transform.Rotate(0, translationX * rotate_speed * Time.deltaTime, 0);
            if (player.transform.localEulerAngles.x != 0 || player.transform.localEulerAngles.z != 0)
            {
                player.transform.localEulerAngles = new Vector3(0, player.transform.localEulerAngles.y, 0);
            }
            if (player.transform.position.y != 0)
            {
                player.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            }     
        }
    }

    public int GetScore()
    {
        return recorder.GetScore();
    }

    public int GetCrystalNumber()
    {
        return recorder.GetCrystalNumber();
    }
    public bool GetGameover()
    {
        return game_over;
    }
    //重新导入场景
    public void Restart()
    {
        SceneManager.LoadScene("Scenes/mySence");
    }

    void OnEnable()
    {
        GameEventManager.ScoreChange += AddScore;
        GameEventManager.GameoverChange += Gameover;
        GameEventManager.CrystalChange += ReduceCrystalNumber;
    }
    void OnDisable()
    {
        GameEventManager.ScoreChange -= AddScore;
        GameEventManager.GameoverChange -= Gameover;
        GameEventManager.CrystalChange -= ReduceCrystalNumber;
    }
    void ReduceCrystalNumber()
    {
        recorder.ReduceCrystal();
    }
    void AddScore()
    {
        recorder.AddScore();
    }
    void Gameover()
    {
        game_over = true;
        patrol_factory.StopPatrol();
        action_manager.DestroyAllAction();
    }
}

```



也有几个游戏内部角色之间的响应逻辑

巡逻兵追逐玩家的代码

当玩家距离巡逻兵一定距离时，巡逻兵会朝向玩家进行运动，距离大于一定距离，恢复正常的运动

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFollowAction : SSAction
{
    private float speed = 2f;          
    private GameObject player;          
    private PatrolData patrolDate;            

    private PatrolFollowAction() { }
    public static PatrolFollowAction GetSSAction(GameObject player)
    {
        PatrolFollowAction action = CreateInstance<PatrolFollowAction>();
        action.player = player;
        return action;
    }

    public override void Update()
    {
        if (transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0)
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
        if (transform.position.y != 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
         
        Follow();
        //如果侦察兵没有跟随对象，或者需要跟随的玩家不在侦查兵的区域内
        if (!patrolDate.follow_player || patrolDate.wall_sign != patrolDate.sign)
        {
            this.destroy = true;
            this.callback.SSActionEvent(this,1,this.gameobject);
        }
    }
    public override void Start()
    {
        patrolDate = this.gameobject.GetComponent<PatrolData>();
    }
    void Follow()
    {
        transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        this.transform.LookAt(player.transform.position);
    }
}

```

回复正常运动的代码

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoPatrolAction : SSAction
{
    private enum Dirction { EAST, NORTH, WEST, SOUTH };
    private float pos_x, pos_z;                 //移动前的初始x和z方向坐标
    private float move_length;                  //移动的长度
    private float move_speed = 1.2f;            //移动速度
    private bool move_sign = true;              //是否到达目的地
    private Dirction dirction = Dirction.EAST;  //移动的方向
    private PatrolData data;                    //侦察兵的数据
    

    private GoPatrolAction() { }
    public static GoPatrolAction GetSSAction(Vector3 location)
    {
        GoPatrolAction action = CreateInstance<GoPatrolAction>();
        action.pos_x = location.x;
        action.pos_z = location.z;
        //设定移动矩形的边长
        action.move_length = Random.Range(4, 7);
        return action;
    }
    public override void Update()
    {
        if (transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0)
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }            
        if (transform.position.y != 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        //侦察移动
        Gopatrol();
        //如果侦察兵需要跟随玩家并且玩家就在侦察兵所在的区域，侦查动作结束
        if (data.follow_player && data.wall_sign == data.sign)
        {
            this.destroy = true;
            this.callback.SSActionEvent(this,0,this.gameobject);
        }
    }
    public override void Start()
    {
        this.gameobject.GetComponent<Animator>().SetBool("run", true);
        data  = this.gameobject.GetComponent<PatrolData>();
    }

    void Gopatrol()
    {
        if (move_sign)
        {
            switch (dirction)
            {
                case Dirction.EAST:
                    pos_x -= move_length;
                    break;
                case Dirction.NORTH:
                    pos_z += move_length;
                    break;
                case Dirction.WEST:
                    pos_x += move_length;
                    break;
                case Dirction.SOUTH:
                    pos_z -= move_length;
                    break;
            }
            move_sign = false;
        }
        this.transform.LookAt(new Vector3(pos_x, 0, pos_z));
        float distance = Vector3.Distance(transform.position, new Vector3(pos_x, 0, pos_z));
        //当前位置与目的地距离浮点数的比较
        if (distance > 0.9)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(pos_x, 0, pos_z), move_speed * Time.deltaTime);
        }
        else
        {
            dirction = dirction + 1;
            if(dirction > Dirction.SOUTH)
            {
                dirction = Dirction.EAST;
            }
            move_sign = true;
        }
    }
}

```





负责发布事件的程序

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    //分数增加
    public delegate void ScoreEvent();
    public static event ScoreEvent ScoreChange;
    public delegate void GameoverEvent();
    public static event GameoverEvent GameoverChange;
    public delegate void CrystalEvent();
    public static event CrystalEvent CrystalChange;

    //分数变化
    public void PlayerEscape()
    {
        if (ScoreChange != null)
        {
            ScoreChange();
        }
    }
    //游戏结束
    public void PlayerGameover()
    {
        if (GameoverChange != null)
        {
            GameoverChange();
        }
    }
    public void ReduceCrystalNum()
    {
        if (CrystalChange != null)
        {
            CrystalChange();
        }
    }
}

```

