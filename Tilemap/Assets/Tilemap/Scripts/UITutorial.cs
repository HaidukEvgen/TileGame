using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITutorial : MonoBehaviour{
    public static bool isOn;
    public static bool closePanel = false;
    public static bool openPanel = false;
    public static bool changeColorPanel = false;
    public static bool useBonuses = false;
    public static bool singleMode;

    public Text tutorialText;

    public static Game gm;

    public Image tutorialPanel;
    public GameObject tutorialObject;
    public GameObject arrow;
    public GameObject click;
    public GameObject bombButton;
    public GameObject rollerButton;

    public GameObject startPanel;
    public GameObject bonusPanel;

    private Color32 red_player_1;
    private Color32 blue_player_2;

    private bool canClick = false;

    private int MAX_NUM_CALL = 5;

    public static int numInvoke = 0;
    public static bool openBonuses = false;
    
    void Start()
    {
        red_player_1 = new Color32(238, 159, 168, 255);
        blue_player_2 = new Color32(159, 186, 238, 255); 

        singleMode = PlayerPrefs.GetInt("GameMode", 1) == 1? true: false;

        isOn = PlayerPrefs.GetInt("Tutorial", 0) == 1? true: false;

        if(singleMode){
            tutorialPanel.color = blue_player_2;
        }
        else{
            if(gm.GetCurPlayer().GetTileState() == Game.TileState.firstPlayer){
                tutorialPanel.color = red_player_1;
            }
            else{
                tutorialPanel.color = blue_player_2;
            }
        }
        if(isOn)
            openTutorial(true);

        numInvoke = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(openPanel && isOn){
            openTutorial(false);
            openPanel = false;
        }

        else if(closePanel && isOn){
            closeTutorial();
            closePanel = false;
        }

        else if(changeColorPanel && isOn){
            ChangeColorPanel();
            changeColorPanel = false;
        }

        else if(useBonuses && isOn && !UIController.IsendGamePanel){
            UpdateTextBonus();
            openTutorial(true);
            useBonuses = false;
        }
        else{
            openPanel = false;
            closePanel = false;
            changeColorPanel = false;
            useBonuses = false;
        }

        if(gm.GetCurPlayer().IsUpperPlayer() && singleMode){
            closeTutorial();
        }

        if(openBonuses){
            openBonuses = false;
            if(!isOn){
                TurnButtons(true);
            }
        }
    }

    private void openTutorial(bool canOpen){
        if(numInvoke < MAX_NUM_CALL){
            tutorialObject.SetActive(true);
            bonusPanel.SetActive(false);
            if(canClick){
                click.SetActive(true);
            }
        }
        else if(canOpen){
            tutorialObject.SetActive(true);
            startPanel.SetActive(false);
        }
    }

    private void closeTutorial(){
        tutorialObject.SetActive(false);
        click.SetActive(false);
    }

    private void UpdateText(){
        tutorialText.text = "Put the figure near yours territory";
        click.SetActive(true);
        canClick = true;
    }

    private void UpdateTextBonus(){
        tutorialText.text = "Try to use some bonuses to increase your chances of winning";
        arrow.transform.position = new Vector3(0.5f, -5.7f, 0f);
        startPanel.SetActive(false);
        bonusPanel.SetActive(true);
    }

    private void ChangeColorPanel(){
        singleMode = PlayerPrefs.GetInt("GameMode", 1) == 1? true: false;
        if(!singleMode){
            tutorialPanel.color = tutorialPanel.color == red_player_1? blue_player_2: red_player_1;
            numInvoke++;
        }
        else{
            numInvoke = numInvoke + 2;
        }
        
        if(numInvoke == 2){
            UpdateText();
        }
        else if(numInvoke == 6){
            TurnButtons(true);
        }
    }

    private void TurnButtons(bool how){
        bombButton.GetComponent<Button>().interactable = how;
        rollerButton.GetComponent<Button>().interactable = how;
    }
}
