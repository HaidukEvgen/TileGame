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
    public Text playWinnertxt;
    public Text finalScoretxt;
    public Text bombtxt;
    public Text rollertxt;
    public Text resizetxt;

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
    public GameObject pauseButton;
    public GameObject rotateButton;
    public GameObject endGamePanel;
    public GameObject tutorialUI;

    public AudioSource endSound;
    public AudioSource changeFigSound;
    public AudioSource wrongSound;
    
    public static Game gm;
    
    public static Tilemap tm;
    
    private bool notOpenRoundPanel = true;
    public static bool useResize = false;
    public static bool useBomb = false;
    public static bool IsendGamePanel = false;

    private int maxRound;

    public int minBonusCount = -10;

    void Start(){
        Application.targetFrameRate = 60;
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
            UITutorial.closePanel = true;
        }
    }

    public void PauseGame(){
        tilemap.SetActive(false);
        infPanel.SetActive(false);
        curFig.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0; 
        if(SoundManager.isOn == false){
            MusicOff(true);
        }
        tutorialUI.SetActive(false);
    }

    public void CountinueGame(){
        endGamePanel.SetActive(false);
        tilemap.SetActive(true);
        infPanel.SetActive(true);
        curFig.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1; 
        CloseBonusesPanel();
        tutorialUI.SetActive(true);
    }

    public void playRotateSound(){
        if(SoundManager.isSoundEffectsOn){
            rotateButton.GetComponent<AudioSource>().Play();
        }
    }

    public void GotoMenu(){
        SceneManager.LoadScene("StartScene");
    }

    public void openEndGamePanel(){
        IsendGamePanel = true;
        finalScoretxt.text = "Rounds score: " + gm.GetPlayer1().GetWinRounds().ToString() + " : " + gm.GetPlayer2().GetWinRounds().ToString();

        if(gm.GetPlayer1().GetWinRounds() > gm.GetPlayer2().GetWinRounds()){
            playWinnertxt.text = "First player won the game";
        }
        else if(gm.GetPlayer1().GetWinRounds() < gm.GetPlayer2().GetWinRounds()){
            playWinnertxt.text = "Second player won the game";
        }
        else{
            playWinnertxt.text = "Draw";
        }

        endGamePanel.SetActive(true);
        tilemap.SetActive(false);
        infPanel.SetActive(false);
        curFig.SetActive(false);

        if(SoundManager.isSoundEffectsOn){
            endGamePanel.GetComponent<AudioSource>().Play();
        }
    }

    public void roundEnd(int round){
        UITutorial.closePanel = true;
        IsendGamePanel = true;
        roundsScoretxt.text = "Rounds score: " + gm.GetPlayer1().GetWinRounds().ToString() + " : " + gm.GetPlayer2().GetWinRounds().ToString();

        if (round == maxRound){
            openEndGamePanel();
            return;
        }
        else if(gm.GetPlayer1().GetPoints() > gm.GetPlayer2().GetPoints()){
            winnertxt.text = "First player won this round";
        }
        else if (gm.GetPlayer1().GetPoints() < gm.GetPlayer2().GetPoints()){
            winnertxt.text = "Second player won this round";
        }
        else{
            winnertxt.text = "Draw";
        }

        curFig.SetActive(false);
        endRoundPanel.SetActive(true);
        if(SoundManager.isSoundEffectsOn){
            endSound.Play();
        }

        openBonusesButton.GetComponent<Button>().interactable = false;
        pauseButton.GetComponent<Button>().interactable = false;
        rotateButton.GetComponent<Button>().interactable = false;
    }

    public void CountinueGameRound(){
        IsendGamePanel = false;
        endRoundPanel.SetActive(false);
        curFig.SetActive(true);
        gm.inGame = true;
        gm.skipNum = 0;
        notOpenRoundPanel = true;
        gm.CleanOldVal(tm, maxRound);
        openBonusesButton.GetComponent<Button>().interactable = true;
        pauseButton.GetComponent<Button>().interactable = true;
        rotateButton.GetComponent<Button>().interactable = true;
    }

    public void OpenBonusPanel(){
        bombtxt.text = "Bomb (X" + gm.GetCurPlayer().GetBonusAmount(Game.Bonuses.bomb).ToString() + ")";
        rollertxt.text = "Roller (X" + gm.GetCurPlayer().GetBonusAmount(Game.Bonuses.painter).ToString() + ")";
        resizetxt.text = "Resizer (X" + gm.GetCurPlayer().GetBonusAmount(Game.Bonuses.resizer).ToString() + ")";
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
        SoundManager.isOn = true;
        SoundManager.isSoundEffectsOn = true;
    }

    public void MusicOff(bool effectsOn){
        musicOn.SetActive(false);
        musicOff.SetActive(true);
        SoundManager.stopPlay = true;
        SoundManager.isOn = false;
        if(!effectsOn){
            SoundManager.isSoundEffectsOn = false;
        }
    }

    public void changeFig(){
        if(gm.GetCurPlayer().GetBonusAmount(Game.Bonuses.resizer) > minBonusCount){
            gm.GetCurPlayer().ReduceBonusAmount(Game.Bonuses.resizer);
            resizetxt.text = "Resizer (X" + gm.GetCurPlayer().GetBonusAmount(Game.Bonuses.resizer).ToString() + ")";
            useResize = true;
            if(SoundManager.isSoundEffectsOn){
                changeFigSound.Play();
            }
            CloseBonusesPanel();
        }
        else{
            if(SoundManager.isSoundEffectsOn){
                wrongSound.Play();
            }
        }
    }

    public void selectBomb(){
        if(gm.GetCurPlayer().GetBonusAmount(Game.Bonuses.bomb) > minBonusCount){
            gm.GetCurPlayer().ReduceBonusAmount(Game.Bonuses.bomb);
            bombtxt.text = "Bomb (X" + gm.GetCurPlayer().GetBonusAmount(Game.Bonuses.bomb).ToString() + ")";
            useBomb = true;
            playRotateSound();
            CloseBonusesPanel();
        }
        else{
            if(SoundManager.isSoundEffectsOn){
                wrongSound.Play();
            }
        }
    }
}
