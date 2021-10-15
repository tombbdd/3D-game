using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;
using CheckApplication;

public class FirstController : MonoBehaviour, SceneController, UserAction {
    Vector3 water_pos = new Vector3(0, 0.5f, 0);

    public CoastController leftCoast;
    public CoastController rightCoast;
    public BoatController boat;
    public Check gameStatusManager;
    private MyCharacterController[] characters = null;
    private UserGUI userGUI = null;
    public bool flag_stop = false;
    
    Vector3 target;
    public float speed = 2.0f;

    public CCActionManager actionManager;
    //得到唯一的实例
    void Awake()
    {
        Director director = Director.getInstance();
        director.sceneController = this;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        characters = new MyCharacterController[6];
        load();
        flag_stop = false;
    }
    //初始化游戏资源，如角色，船等等
    public void load()
    {
        GameObject water = Instantiate(Resources.Load("Perfabs/water", typeof(GameObject)), water_pos, Quaternion.identity, null) as GameObject;
        water.name = "water";
        leftCoast = new CoastController("left");
        rightCoast = new CoastController("right");
        boat = new BoatController();
        for (int i = 0; i < 3; i++)
        {
            MyCharacterController character = new MyCharacterController("priest");
            character.setPosition(leftCoast.getEmptyPosition());
            character.Oncoast(leftCoast);
            leftCoast.getOnCoast(character);
            characters[i] = character;
            character.setName("priest" + i);
        }
        for (int i = 0; i < 3; i++)
        {
            MyCharacterController character = new MyCharacterController("devil");
            character.setPosition(leftCoast.getEmptyPosition());
            character.Oncoast(leftCoast);
            leftCoast.getOnCoast(character);
            characters[i + 3] = character;
            character.setName("devil" + i);
        }
    }
    //判断游戏胜负
    // int check_game_over()
    // {   
    //     int left_priest = 0, left_devil = 0, right_priest = 0, right_devil = 0;
    //     int[] fromCount = leftCoast.getCharacterNum();
    //     int[] toCount = rightCoast.getCharacterNum();
    //     left_priest += fromCount[0];
    //     left_devil += fromCount[1];
    //     right_priest += toCount[0];
    //     right_devil += toCount[1];
    //     //获胜条件
    //     if (right_priest + right_devil == 6)      
    //         return 1;
    //     int[] boatCount = boat.getCharacterNum();
    //     //统计左右两岸的牧师与恶魔的数量
    //     if (!boat.get_is_left())
    //     {   
    //         right_priest += boatCount[0];
    //         right_devil += boatCount[1];
    //     }
    //     else
    //     {        
    //         left_priest += boatCount[0];
    //         left_devil += boatCount[1];
    //     }
    //     //游戏失败条件
    //     if ((left_priest < left_devil && left_priest > 0)|| (right_priest < right_devil && right_priest > 0))
    //     {       
    //         return -1;
    //     }
    //     return 0;           //游戏继续
    // }
    //重置函数
    public void restart()
    {
        boat.reset();
        leftCoast.reset();
        rightCoast.reset();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].reset();
        }
    }
    //游戏结束后，不能再点击产生交互信息
    public bool stop()
    {
        if(gameStatusManager.check_game_over()!= 0)
            return true;
        return false;
    }
    //动作
    public void moveBoat()                  //移动船
    {
        if (boat.isEmpty()) return;
        actionManager.moveBoatAction(boat.getBoat(), boat.BoatMoveToPosition(), speed);   //调用动作管理器中的移动船函数，原先为调用moveable函数
        userGUI.status = gameStatusManager.check_game_over();
    }

    public void characterIsClicked(MyCharacterController character)    //移动角色
    {
        if (userGUI.status != 0) return;
        if (character.getis_onboat())
        {
            CoastController coast;
            if (!boat.get_is_left())
            {
                coast = rightCoast;
            }
            else
            {
                coast = leftCoast;
            }
            boat.GetOffBoat(character.getName());
            Vector3 end_pos = coast.getEmptyPosition();                                         
            Vector3 middle_pos = new Vector3(character.getGameObject().transform.position.x, end_pos.y, end_pos.z);  
            actionManager.moveCharacterAction(character.getGameObject(), middle_pos, end_pos, speed);  //调用动作管理器中的移动角色函数，原先为调用moveable函数
            character.Oncoast(coast);
            coast.getOnCoast(character);
        }
        else
        {
            CoastController coast = character.getcoastController();
            // 船上已有两人
            if (boat.getEmptyIndex() == -1)
            {
                return;
            }
            // 船与角色并不在同一边岸
            if (coast.get_is_right() == boat.get_is_left())
                return;

            coast.getOffCoast(character.getName());
            Vector3 end_pos = boat.getEmptyPos();                                             
            Vector3 middle_pos = new Vector3(end_pos.x, character.getGameObject().transform.position.y, end_pos.z); 
            actionManager.moveCharacterAction(character.getGameObject(), middle_pos, end_pos, character.speed);  //调用动作管理器中的移动角色函数，原先为调用moveable函数
            character.Onboat(boat);
            boat.GetOnBoat(character);
        }
        userGUI.status = gameStatusManager.check_game_over();
    }
}