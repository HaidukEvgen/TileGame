using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject starterPanel;
    public GameObject settingsPanel;
    public GameObject modePanel;
    
    public Toggle musicOn;
    public Toggle effectsOn;
    public Toggle tutorialOn;

    public Slider roundSlider;
    public Text roundtxt;
    public static int round;

    public Slider mapSlider;
    public Text maptxt;

    void Start(){
        round = PlayerPrefs.GetInt("Rounds", 3);
        PlayerPrefs.SetInt("Tutorial", 1);
        roundtxt.text = "Rounds: " + round.ToString();
        roundSlider.value = (float)round / 10f  - 0.1f;
        mapSlider.value = PlayerPrefs.GetFloat("MapSliderVal" , 0.75f);
    }

    public void ChangeValRounds(){
        round = (int)(roundSlider.value * 10f)%10  + 1;
        if(roundSlider.value == 1){
            round = 10;
        }
        roundtxt.text = "Rounds: " + round.ToString();
        PlayerPrefs.SetInt("Rounds", round);
    }

    public void ChangeValMap(){
        int val = 0;
        if(mapSlider.value >= 0.5f){
            val = 1;
            maptxt.text = "Map size: L";
        }
        else{
            val = 0;
            maptxt.text = "Map size: M";
        }
        PlayerPrefs.SetInt("Map", val);
        PlayerPrefs.SetFloat("MapSliderVal", mapSlider.value);
    }

    public void StartGameTwoPlayers(){
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.SetInt("GameMode", 2);
    }

    public void StartGameOnePlayer(){
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.SetInt("GameMode", 1);
        UITutorial.numInvoke  = 0;
    }

    public void OpenSettings(){
        starterPanel.SetActive(false);
        settingsPanel.SetActive(true);
        if(SoundManager.isOn != true){
            musicOn.isOn = false; 
        }
        if(SoundManager.isSoundEffectsOn != true){
            effectsOn.isOn = false; 
        }
        if(PlayerPrefs.GetInt("Tutorial", 1) == 1){
            tutorialOn.isOn = true;
        }
    }

    public void CloseSettings(){
        starterPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void openModePanel(){
        starterPanel.SetActive(false);
        modePanel.SetActive(true);
    }

    public void EffectsOff(){
        if(effectsOn.isOn){
            SoundManager.isSoundEffectsOn = true;
        }
        else{
            SoundManager.isSoundEffectsOn = false;
        }
    }

    public void MusicOn(){
        if(musicOn.isOn){
            SoundManager.startPlay = true;
        }
        else{
            SoundManager.stopPlay = true;
        }
    }

    public void ChangeTutorialMode(){
        if(tutorialOn.isOn){
            PlayerPrefs.SetInt("Tutorial", 1);
        }
        else{
            PlayerPrefs.SetInt("Tutorial", 0);
        }
    }
}