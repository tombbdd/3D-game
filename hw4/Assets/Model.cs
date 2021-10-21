using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

namespace Com.Mygame
{
    //导演类，单例模式
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

    //负责界面与场景控制器通信的接口
    public enum GameState { ROUND_START, ROUND_FINISH, RUNNING, PAUSE, START ,GAME_OVER, WIN}

    public interface IUserAction
    {
        void GameOver();
        GameState getGameState();
        void setGameState(GameState gs);
        int GetScore();
        void hit(Vector3 pos);
    }

    //动作与动作管理器之间的接口
    public enum SSActionEventType : int { Started, Competeted }

    public interface ISSActionCallback
    {
        void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
            int intParam = 0, string strParam = null, Object objectParam = null);
    }

    //飞碟的一些基本属性，用于调整以及判断得分
    public class DiskComponent : MonoBehaviour
    {
        public Vector3 size;
        public int score;
        public float speed;
        public Vector3 direction;
    }

}


