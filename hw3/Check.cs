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