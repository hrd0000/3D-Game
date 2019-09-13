using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OXO : MonoBehaviour
{
	private int[,] board = new int[3,3];
    private int result = 0;

    // Start is called before the first frame update
    void Start()
    {
        Reset();    
    }

    void Reset() {
        result = 0;
         for (int i = 0; i < 3; ++i) {
             for (int j = 0; j < 3; ++j) {
                 board[i, j] = 0;
             }
         }
    }

    void OnGUI() {
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;
        fontStyle.normal.textColor = new Color(255,255,255);
        fontStyle.fontSize = 20;

        if (GUI.Button (new Rect (250, 60, 70, 50), "RESET")) Reset ();  

        for (int i=0; i<3; ++i) {    
            for (int j=0; j<3; ++j) {    
                if (board [i, j] == 1)    
                    GUI.Button (new Rect (210 + i * 50, 140 + j * 50, 50, 50), "O");  
                if (board [i, j] == 2)  
                    GUI.Button (new Rect (210 + i * 50, 140 + j * 50, 50, 50), "X");  
                if (GUI.Button (new Rect (210 + i * 50, 140 + j * 50, 50, 50), "") && result == 0) {      
                    board [i, j] = 1;
                    if (result == 0) {  
                        int ri = (int)Random.Range (0, 20);
                        ri %= 3;
                        int rj = (int)Random.Range (0, 20);
                        rj %= 3;
                        int cnt = 0;
                        while (board [ri, rj] != 0) {
                            ri = ri + 1;
                            ri %= 3;
                            cnt++;
                            if (cnt == 3)
                                break;
                        }
                        cnt = 0;
                        while (board [ri, rj] != 0) {
                            rj = rj + 1;
                            rj %= 3;
                            if (cnt == 3)
                                break;
                        }
                        int flag = 0;
                        if (board [ri, rj] == 0)
                            flag = 1;
                        if (flag == 1)
                            board [ri, rj] = 2;    
                    }
                }    
            }    
        }  

        GUI.Label (new Rect (50, 185, 100, 50), "Result:",fontStyle);
        result = check ();    
        if (result == 1) {    
            GUI.Label (new Rect (105, 185, 100, 50), "You win!", fontStyle);    
        } else if (result == 2) {    
            GUI.Label (new Rect (105, 185, 100, 50), "You lose!", fontStyle);    
        } else if (result == 3) {
            GUI.Label (new Rect (105, 185, 100, 50), "No one wins", fontStyle); 
        } else {
            GUI.Label (new Rect (105, 185, 100, 50), "Playing...", fontStyle);
        } 
    }

    int check() {    
        int count = 0;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (board [i, j] != 0)
                    count++;
            }
        }
        for (int i=0; i<3; ++i) {    
            if (board[i,0]!=0 && board[i,0]==board[i,1] && board[i,1]==board[i,2]) {    
                return board[i,0];    
            }    
        }    
        for (int j=0; j<3; ++j) {    
            if (board[0,j]!=0 && board[0,j]==board[1,j] && board[1,j]==board[2,j]) {    
                return board[0,j];    
            }    
        }    
        if (board[1,1]!=0 &&    
            board[0,0]==board[1,1] && board[1,1]==board[2,2] ||    
            board[0,2]==board[1,1] && board[1,1]==board[2,0]) {    
            return board[1,1];    
        }    
        if (count == 9)
            return 3;
        return 0; 
    }
}
