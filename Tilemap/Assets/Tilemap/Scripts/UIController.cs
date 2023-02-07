using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Text scoretxt;
    public Text roundtxt;
    public Text roundsScoretxt;
    public Text winnertxt;

    public GameObject pausePanel;
    public GameObject curFig;
    public GameObject tilemap;
    public GameObject endRoundPanel;
    public GameObject infPanel;
    public GameObject musicOn;
    public GameObject musicOff;
    public GameObject bonusesPanel;
    public GameObject openBonusesButton;
    public GameObject closeBonusesButton;

    public AudioSource endSound;
    
    public static Game gm;
    
    public static Tilemap tm;
    
    private bool notOpenRoundPanel = true;

    private int maxRound;

    void Start(){
        maxRound = PlayerPrefs.GetInt("Rounds", 3);
        PauseGame();
        CountinueGame();
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
        roundsScoretxt.text = "Rounds score: " + gm.GetPlayer1().GetWinRounds().ToString() + " : " + gm.GetPlayer2().GetWinRounds().ToString();

        if (round == maxRound){
            if(gm.gameScore > 0){
                winnertxt.text = "The first player won this game";
            }
            else if (gm.gameScore < 0){
                winnertxt.text = "The second player won this game";
            }
            else{
                winnertxt.text = "Draw";
            }
            
        }
        else if(gm.curWinner == 1){
            winnertxt.text = "First player won this round";
        }
        else if (gm.curWinner == 2){
            winnertxt.text = "Second player won this round";
        }
        else{
            winnertxt.text = "Draw";
        }

        curFig.SetActive(false);
        endRoundPanel.SetActive(true);
        if(SoundManager.isOn){
            endSound.Play();
        }
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

    public void OpenBonusPanel(){
        closeBonusesButton.SetActive(true);
        openBonusesButton.SetActive(false);
        curFig.SetActive(false);
        bonusesPanel.SetActive(true);
    }

    public void CloseBonusesPanel(){
        closeBonusesButton.SetActive(false);
        openBonusesButton.SetActive(true);
        curFig.SetActive(true);
        bonusesPanel.SetActive(false);
    }

    public void MusicOn(){
        musicOn.SetActive(true);
        musicOff.SetActive(false);
        SoundManager.startPlay = true;
    }
}
