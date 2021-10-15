using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

namespace Com.Mygame
{
    //Director 控制唯一一个实例
    public class Director : System.Object
    {
        private static Director D_instance;
        public SceneController sceneController { get; set; }

        public static Director getInstance()
        {
            if (D_instance == null)
                D_instance = new Director();
            return D_instance;
        }
    }

    //场景控制器，被FirstController类所继承来加载资源预设
    public interface SceneController
    {
        void load();
    }

    //门面模式，FirstController实现该接口来与用户交互
    public interface UserAction
    {
        void moveBoat();
        void characterIsClicked(MyCharacterController c_controller);
        void restart();
        bool stop();
    }
    //角色的类
    public class MyCharacterController
    {
        //只读数据，不希望通过Inspector中改变角色
        readonly GameObject character;
        //readonly Moveable moveable;
        readonly ClickGUI clickGUI;
        readonly bool is_priest;
        public float speed = 200;
        //可改变
        bool is_onboat;
        CoastController coastController;
        //通过字符串构造角色函数
        public MyCharacterController(string c_str)
        {
            if(c_str == "priest")
            {
                is_priest = true;
                character = Object.Instantiate(Resources.Load("Perfabs/priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            }
            else if(c_str == "devil")
            {
                is_priest = false;
                character = Object.Instantiate(Resources.Load("Perfabs/devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            }
            //moveable = character.AddComponent(typeof(Moveable)) as Moveable;
            clickGUI = character.AddComponent(typeof(ClickGUI)) as ClickGUI;
            clickGUI.setController(this);
        }
        //角色上船函数
        public void Onboat(BoatController boatController)
        {
            coastController = null; //离开岸边
            character.transform.parent = boatController.getBoat().transform;
            is_onboat = true;
        }
        //角色上岸函数
        public void Oncoast(CoastController temp)
        {
            coastController = temp;
            character.transform.parent = null;
            is_onboat = false;
        }
        //重置函数，恢复现场
        public void reset()
        {
            //moveable.reset();
            coastController = (Director.getInstance().sceneController as FirstController).leftCoast;
            Oncoast(coastController);
            setPosition(coastController.getEmptyPosition());
            coastController.getOnCoast(this);
        }
        //各种get，set函数
        public void setName(string name)
        {
            character.name = name;
        }
        public GameObject getGameObject()
        {
            return character;
        }
        public string getName()
        {
            return character.name;
        }
        public void setPosition(Vector3 position)
        {
            character.transform.position = position;
        }
        public Vector3 getPosition()
        {
            return character.transform.position;
        }
        /*public void movePosition(Vector3 position)
        {
            moveable.setDestination(position);
        }*/
        public bool getType() //true -> priest; false -> devil
        {
            return is_priest;
        }
        public bool getis_onboat()
        {
            return is_onboat;
        }
        public CoastController getcoastController()
        {
            return coastController;
        }
    }
    //船的类
    public class BoatController
    {
        //只读数据，不希望通过Inspector中改变船的位置
        readonly GameObject boat;
        public float speed = 200;
        //readonly Moveable moveable;
        readonly Vector3 right_pos = new Vector3(4, 1, 0);
        readonly Vector3 left_pos = new Vector3(-4, 1, 0);
        readonly Vector3[] start_pos;
        readonly Vector3[] end_pos;
        //判断船是否向左岸走
        bool is_left;
        //船上的角色最多只有两个
        MyCharacterController[] passenger = new MyCharacterController[2];
        //船的构造函数
        public BoatController()
        {
            is_left = true;
            end_pos =  new Vector3[] { new Vector3(3F, 2F, 0), new Vector3(4.5F, 2F, 0) };
            start_pos = new Vector3[] { new Vector3(-4.5F, 2F, 0), new Vector3(-3F, 2F, 0) };
            boat = Object.Instantiate(Resources.Load("Perfabs/boat", typeof(GameObject)), left_pos, Quaternion.identity, null) as GameObject;
            boat.name = "boat";
            //moveable = boat.AddComponent(typeof(Moveable)) as Moveable;
            boat.AddComponent(typeof(ClickGUI));
        }
        //判断船是否为空
        public bool isEmpty()
        {
            for (int i = 0; i < passenger.Length; i++)
            {
                if(passenger[i] != null)
                    return false;
            }
            return true;
        }
        //查找船上的空位
        public int getEmptyIndex()
        {
            for (int i = 0; i < passenger.Length; i++)
            {
                if (passenger[i] == null) return i;
            }
            return -1;
        }
        //查找船上空位的位置
        public Vector3 getEmptyPos()
        {
            int index = getEmptyIndex();
            if (is_left)
                return start_pos[index];
            else
                return end_pos[index];
        }
        //船的移动函数，动作分离版改变,获得目的地
        public Vector3 BoatMoveToPosition()                   
        {
            is_left = !is_left;
            if (is_left == true)
            {
                return left_pos;
            }
            else
            {
                return right_pos;
            }
        }
        //上船函数
        public void GetOnBoat(MyCharacterController charactercontroller)
        {
            int index = getEmptyIndex();
            if(index != -1)
                passenger[index] = charactercontroller;
        }
        //上岸函数
        public MyCharacterController GetOffBoat(string name)
        {
            for(int i = 0; i < passenger.Length; i++)
            {
                if(passenger[i] != null && passenger[i].getName() == name)
                {
                    MyCharacterController mycharacter = passenger[i];
                    passenger[i] = null;
                    return mycharacter;
                }
            }
            return null;
        }
        //重置函数
        public void reset()
        {
            //moveable.reset();
            is_left = true;
            boat.transform.position = left_pos;
            passenger = new MyCharacterController[2];
        }
        //各种get和set函数
        public bool get_is_left()
        {
            return is_left;
        }
        public GameObject getBoat()
        {
            return boat;
        }
        public int[] getCharacterNum()
        {
            int[] count = { 0, 0 };
            for(int i = 0; i < passenger.Length; i++)
            {
                if(passenger[i] != null)
                {
                    if(passenger[i].getType() == true)
                    {
                        count[0]++;
                    }
                    else
                    {
                        count[1]++;
                    }
                }
            }
            return count;
        }
    }
    //岸的类
    public class CoastController
    {
        //只读数据，不希望通过Inspector中改变左右岸的位置
        readonly GameObject coast;
        readonly Vector3 right_pos = new Vector3(10, 1, 0);
        readonly Vector3 left_pos = new Vector3(-10, 1, 0);
        //角色在岸上的位置
        readonly Vector3[] positions;
        //岸是否在右边
        readonly bool is_right;

        MyCharacterController[] passenger;

        public CoastController(string pos)
        {
            positions = new Vector3[] {new Vector3(6.5F,2.6F,0), new Vector3(7.7F,2.6F,0), new Vector3(8.9F,2.6F,0),
                new Vector3(10.1F,2.6F,0), new Vector3(11.3F,2.6F,0), new Vector3(12.5F,2.6F,0)};
            passenger = new MyCharacterController[6];
            if (pos == "right")
            {
                coast = Object.Instantiate(Resources.Load("Perfabs/coast", typeof(GameObject)), right_pos, Quaternion.identity, null) as GameObject;
                coast.name = "right";
                is_right = true;
            }
            else if (pos == "left")
            {
                coast = Object.Instantiate(Resources.Load("Perfabs/coast", typeof(GameObject)), left_pos, Quaternion.identity, null) as GameObject;
                coast.name = "left";
                is_right = false;
            }
        }
        //获得空位函数
        public int getEmptyIndex()
        {
            for (int i = 0; i < passenger.Length; i++)
            {
                if (passenger[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
        //获得空位位置函数
        public Vector3 getEmptyPosition()
        {
            Vector3 pos = positions[getEmptyIndex()];
            if (is_right == false)
                pos.x *= -1;
            return pos;
        }
        //上岸函数
        public void getOnCoast (MyCharacterController mycharacter)
        {
            passenger[getEmptyIndex()] = mycharacter;
        }
        //离岸函数
        public MyCharacterController getOffCoast(string name)
        {
            for (int i = 0; i < passenger.Length; i++)
            {
                if(passenger[i] != null && passenger[i].getName() == name)
                {
                    MyCharacterController mycharacter = passenger[i];
                    passenger[i] = null;
                    return mycharacter;
                }
            }
            return null;
        }
        //重置函数
        public void reset()
        {
            passenger = new MyCharacterController[6];
        }
        //各种get和set函数
        public bool get_is_right()
        {
            return is_right;
        }
        public int[] getCharacterNum()
        {
            int[] count = { 0, 0 };
            for(int i = 0; i < passenger.Length; i++)
            {
                if(passenger[i] != null)
                {
                    if(passenger[i].getType() == true)
                    {
                        count[0]++;
                    }
                    else
                    {
                        count[1]++;
                    }
                }
            }
            return count;
        }
    }
}