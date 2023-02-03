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
    public GameObject infPanel;
    public GameObject musicOn;
    public GameObject musicOff;
    public static Game gm;
    public static Tilemap tm;
    private bool notOpenRoundPanel = true;

    private int maxRound;

    void Start(){
        maxRound = PlayerPrefs.GetInt("Rounds", 3);
    }
    
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
        infPanel.SetActive(false);
        curFig.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0; 
        if(SoundManager.isOn == false){
            MusicOff();
        }
    }

    public void CountinueGame(){
        tilemap.SetActive(true);
        infPanel.SetActive(true);
        curFig.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1; 
    }

    public void GotoMenu(){
        SceneManager.LoadScene("StartScene");
    }

    public void roundEnd(int round){
        if (round == maxRound){
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
        gm.CleanOldVal(tm, maxRound);
    }

    public void MusicOff(){
        musicOn.SetActive(false);
        musicOff.SetActive(true);
        SoundManager.stopPlay = true;
    }

    public void MusicOn(){
        musicOn.SetActive(true);
        musicOff.SetActive(false);
        SoundManager.startPlay = true;
    }
}
