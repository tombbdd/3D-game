## 1.游戏对象的使用
游戏对象是游戏的重要构成部分，开发人员可以通过控制游戏对象的属性来操控游戏对象，如改变位置、大小、移动等等，也可以自行编写脚本挂载到游戏对象上，让第项拥有自主运动或者交互的能力。
游戏对象也有几种不同的分类，如空对象、3D对象（正方体、长方体、球体、地形）、灯光、音频、UI、摄像机等不同的几种，每种都有对应的功能。
灯光用于调整游戏中的光源方向、距离、位置等等
音频可以为游戏加入背景音乐
摄像机决定了游戏运行时的视角，可以新建多个摄像机获得多个视图
3D对象一般用于构建游戏角色或者游戏环境，也可用于挂载脚本控制整个游戏的进程，如空对象挂载脚本。也有类似天空盒和地形等用于构建游戏环境的对象，可以设置天空的贴图的地形。

游戏对象运动的本质是什么？
  游戏对象运动的本质是游戏对象的坐标、范围、姿态等属性的变化
请用三种方法以上方法，实现物体的抛物线运动。
1.直接修改position属性
```
public class Move : MonoBehaviour {
    public int a = 5;
    private void Update()
    {
        this.transform.position += Vector3.right * Time.deltaTime * 1f;
        this.transform.position += Vector3.down * Time.deltaTime * Time.deltaTime * a;
    }
}
```
2.利用transform中的translate函数来进行改变position
```
public class Move : MonoBehaviour {
    public float speed = 1;
	void Update () {
        Vector3 change = new Vector3(Time.deltaTime * 5, -Time.deltaTime * (speed / 10), 0);
        transform.Translate(change);
        speed++;
    }
}
```
3.初始设定一个vector3变量你，将position不断与变量相加
```
public class Move : MonoBehaviour {
    public float speed = 2;
	void Update () {
        Vector3 change = new Vector3( Time.deltaTime*5, -Time.deltaTime*(speed/10), 0);
        ;
        this.transform.position += change;
        speed++;
	}
}
```


## priest&devil
参考博客：https://blog.csdn.net/dickdick111/article/details/79874812?ops_request_misc=%257B%2522request%255Fid%2522%253A%2522163402874716780271558094%2522%252C%2522scm%2522%253A%252220140713.130102334.pc%255Fall.%2522%257D&request_id=163402874716780271558094&biz_id=0&utm_medium=distribute.pc_search_result.none-task-blog-2~all~first_rank_ecpm_v1~rank_v31_ecpm-1-79874812.pc_search_result_control_group&utm_term=%E4%B8%AD%E5%B1%B1%E5%A4%A7%E5%AD%A6%E6%B8%B8%E6%88%8Fnote3&spm=1018.2226.3001.4187

  
这次的作业要求是在上一次作业的基础上完成动作分离版，也需要设计一个裁判类，当游戏达到结束条件时，通知场景控制器游戏结束。其实也就是需要把判断游戏胜利和失败的逻辑代码分离出来组成一个单独的裁判类，而不是像之前的杂糅在整个游戏过程的控制代码中。

具体的实现分成两步，分离动作、分离裁判类
首先是分离裁判类。这一步是比较简单的，之前的程序实现中使用了一个check函数，所以只需要把原来的check函数分离出来成一个对象即可，这里需要注意新的check对象在游戏开始时需要初始化firstcontroller对象实例赋值过去，这样后面判断胜负时可以直接获取游戏中的角色状态和数据。
```
using UnityEngine;
using Com.Mygame;

namespace CheckApplication{
    public class Check : MonoBehaviour
    {
        public FirstController sceneController;

        void Start()
        {
            sceneController = (FirstController)Director.getInstance().sceneController;
            sceneController.gameStatusManager=this;
        }

        public int check_game_over()
        {   
            int left_priest = 0, left_devil = 0, right_priest = 0, right_devil = 0;
            int[] fromCount = sceneController.leftCoast.getCharacterNum();
            int[] toCount = sceneController.rightCoast.getCharacterNum();
            left_priest += fromCount[0];
            left_devil += fromCount[1];
            right_priest += toCount[0];
            right_devil += toCount[1];
            //获胜条件
            if (right_priest + right_devil == 6)      
                return 1;
            int[] boatCount = sceneController.boat.getCharacterNum();
            //统计左右两岸的牧师与恶魔的数量
            if (!sceneController.boat.get_is_left())
            {   
                right_priest += boatCount[0];
                right_devil += boatCount[1];
            }
            else
            {        
                left_priest += boatCount[0];
                left_devil += boatCount[1];
            }
            //游戏失败条件
            if ((left_priest < left_devil && left_priest > 0)|| (right_priest < right_devil && right_priest > 0))
            {       
                return -1;
            }
            return 0;           //游戏继续
        }
    }
        
}
```

另外一个部分是分离动作，需要完成一个负责动作管理的类，完成船、小人等游戏角色的运动脚本的内部细节实现，供游戏控制脚本直接调用
actionmanager:
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class CCActionManager : SSActionManager
{
    public CCMoveToAction ccmoveBoat;     
    public CCSequenceAction ccmoveCharacter;  
    public FirstController sceneController;

    //游戏开始时先获取游戏控制对象的实例，便于获取游戏对象数据
    protected new void Start(){
        sceneController = (FirstController)Director.getInstance().sceneController;
        sceneController.actionManager = this;    
    }


    //根据传入的游戏对象调用另外编写的角色运动的函数，动作管理者仅需调用下层提供的接口，不需要展示内部实现
    public void moveBoatAction(GameObject boat, Vector3 target, float speed){
        ccmoveBoat = CCMoveToAction.GetSSAction(target, speed);
        this.RunAction(boat, ccmoveBoat, this);
    }
    public void moveCharacterAction(GameObject character, Vector3 middle_pos, Vector3 end_pos, float speed){
        SSAction action1 = CCMoveToAction.GetSSAction(middle_pos, speed);
        SSAction action2 = CCMoveToAction.GetSSAction(end_pos, speed);
        ccmoveCharacter = CCSequenceAction.GetSSAction(1, 0, new List<SSAction> { action1, action2 });
        this.RunAction(character, ccmoveCharacter, this);
    }
}
```


运行游戏的步骤
基本上步骤跟前一次的作业相同，主要是新编写的一个工作管理脚本以及裁判脚本也需要挂载到空对象即可  
1.创建一个空的游戏对象  
2.将clickgui、usergui、firstcontroller、CCActionManager、check 五个脚本挂载到创建好的空对象上  
3.将resources文件夹复制到项目中  
4.运行游戏即可  

游戏演示视频网址：  
https://www.bilibili.com/video/BV1Xu411Z7z2?p=1&share_medium=ipad&share_plat=ios&share_source=QQ&share_tag=s_i&timestamp=1634038515&unique_k=kd11w9