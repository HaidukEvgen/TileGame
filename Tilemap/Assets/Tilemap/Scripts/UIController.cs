using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Text scoretxt;
    public Text roundtxt;
    public Text winnertxt;
    public GameObject pausePanel;
    public GameObject curFig;
    public GameObject tilemap;
    public GameObject endRoundPanel;
    public static Game gm;
    public static Tilemap tm;
    private bool notOpenRoundPanel = true;
    
    // Update is called once per frame
    void Update()
    {
        scoretxt.text = "Score: "  + gm.GetScore();
        roundtxt.text = "Round: " + gm.round.ToString();

        if(!gm.inGame && notOpenRoundPanel){
            roundEnd(gm.round);
            notOpenRoundPanel = false;
        }
    }

    public void PauseGame(){
        tilemap.SetActive(false);
        curFig.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0; 
    }

    public void CountinueGame(){
        tilemap.SetActive(true);
        curFig.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1; 
    }

    public void GotoMenu(){
        SceneManager.LoadScene("StartScene");
    }

    public void roundEnd(int round){
        if (round == 3){
            if(gm.gameScore > 0){
                winnertxt.text = "The first player wins the game";
            }
            else if (gm.gameScore < 0){
                winnertxt.text = "The second player wins the game";
            }
            else{
                winnertxt.text = "Draw";
            }
            
        }
        else if(gm.curWinner == 1){
            winnertxt.text = "First player wins this round";
        }
        else if (gm.curWinner == 2){
            winnertxt.text = "Second player wins this round";
        }
        else{
            winnertxt.text = "Draw";
        }

        curFig.SetActive(false);
        endRoundPanel.SetActive(true);
    }

    public void CountinueGameRound(){
        endRoundPanel.SetActive(false);
        curFig.SetActive(true);
        gm.inGame = true;
        gm.skipNum = 0;
        notOpenRoundPanel = true;
        gm.CleanOldVal(tm);
    }
}
