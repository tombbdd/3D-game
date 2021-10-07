using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class FirstController : MonoBehaviour, SceneController, UserAction {
    Vector3 water_pos = new Vector3(0, 0.5f, 0);
    // Vector3 bac_pos = new Vector3(0, 0, 10);

    public CoastController leftCoast;
    public CoastController rightCoast;
    public BoatController boat;

    private MyCharacterController[] characters = null;
    private UserGUI userGUI = null;
    public bool flag_stop = false;

    void Awake()
    {
        Director director = Director.getInstance();
        director.sceneController = this;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        characters = new MyCharacterController[6];
        load();
        flag_stop = false;
    }

    public void load()
    {
        GameObject water = Instantiate(Resources.Load("Perfabs/water", typeof(GameObject)), water_pos, Quaternion.identity, null) as GameObject;
        // GameObject bac = Instantiate(Resources.Load("Perfabs/background", typeof(GameObject)), bac_pos, Quaternion.identity, null) as GameObject;
        // bac.name = "background";
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
    public void moveBoat()
    {
        if (boat.isEmpty())
            return;
        boat.boat_move();
        userGUI.status = check_game_over();
    }
    int check_game_over()
    {   
        int left_priest = 0, left_devil = 0, right_priest = 0, right_devil = 0;
        int[] fromCount = leftCoast.getCharacterNum();
        int[] toCount = rightCoast.getCharacterNum();
        left_priest += fromCount[0];
        left_devil += fromCount[1];
        right_priest += toCount[0];
        right_devil += toCount[1];
        //获胜条件
        if (right_priest + right_devil == 6)      
            return 1;
        int[] boatCount = boat.getCharacterNum();
        //统计左右两岸的牧师与恶魔的数量
        if (!boat.get_is_left())
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

    public void characterIsClicked(MyCharacterController character)
    {
        //角色要上岸
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
            character.movePosition(coast.getEmptyPosition());
            character.Oncoast(coast);
            coast.getOnCoast(character);
        }
        // 角色要上船
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
            character.movePosition(boat.getEmptyPos());
            character.Onboat(boat);
            boat.GetOnBoat(character);
        }
        userGUI.status = check_game_over();
    }
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
    public bool stop()
    {
        if(check_game_over() != 0)
            return true;
        return false;
    }
}