using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour {

	private int [,] chessBoard = new int[3,3];
	private int step = 9;
	void Init()
	{
		step=9;
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				chessBoard [i,j] = 0;
			}
		}

	}

	void Start () {
		Init ();

	}

	void Update () {
		
	}
	int game_ret(){
		if(chessBoard[0,0] == chessBoard[1,1] && chessBoard[0,0] == chessBoard[2,2]&&chessBoard[0,0] != 0) 
			return chessBoard[0,0];
		if(chessBoard[0,2] == chessBoard[1,1] && chessBoard[0,2] == chessBoard[2,0]&&chessBoard[0,2] != 0) 
			return chessBoard[1,1];
		for (int i = 0; i < 3; i++) {
			if (chessBoard[i,0] == chessBoard[i,1] && chessBoard[i,0] == chessBoard[i,2] && chessBoard[i,0] != 0) {
				return chessBoard[i,0]; 
			}
			if (chessBoard[0,i] == chessBoard[1,i] && chessBoard[0,i] == chessBoard[2,i] && chessBoard[0,i] != 0) {
				return chessBoard[0,i]; 
			}
		}
		if (step == 0) {
			return 3;
		}
		return -1;
	}



	void OnGUI(){
		GUI.contentColor = Color.black;
		GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 25;
		int ret = game_ret ();
		if(ret==1){
			GUI.Label (new Rect (300, 70, 100, 70), "O获胜!");
		}else if(ret==2){
			GUI.Label (new Rect (300, 70, 100, 70), "X获胜!");
		}else if(ret==3){
			GUI.Label (new Rect (300, 70, 100, 70), "平局!");
		}
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (GUI.Button (new Rect (250 + 70 * i, 100 + 70 * j, 70, 70), "")) {
					if(chessBoard[i,j]==0)
					{	
						if (ret == -1) {
							if (step%2==1) {
								chessBoard [i, j] = 1;
								step--;
							}
							else if (step%2==0) {
								chessBoard [i, j] = 2;
								step--;
							}
						}
					}
				}
			}
		}
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (chessBoard [i, j] == 1) {
					GUI.Button (new Rect (250 + 70 * i, 100 + 70 * j, 70, 70), "O");
				} else if (chessBoard [i, j] == 2) {
					GUI.Button (new Rect (250 + 70 * i, 100 + 70 * j, 70, 70), "X");
				}

			}
		}
		if(GUI.Button(new Rect(252,320,200,40), "重新开始"))
		{
			Init ();
		}
	}
}


