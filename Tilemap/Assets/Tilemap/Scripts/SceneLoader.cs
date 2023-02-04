using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject starterPanel;
    public GameObject settingsPanel;
    
    public Toggle musicOn;
    public Toggle musicOff;

    public Slider roundSlider;
    public Text roundtxt;
    public int round;

    public Slider mapSlider;
    public Text maptxt;

    void Start(){
        round = PlayerPrefs.GetInt("Rounds", round);
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

    public void StartGame(){
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings(){
        starterPanel.SetActive(false);
        settingsPanel.SetActive(true);
        if(SoundManager.isOn != true){
            MusicOff();
        }
    }

    public void CloseSettings(){
        starterPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void MusicOff(){
        if(musicOff.isOn){
            musicOn.isOn = false; 
            SoundManager.stopPlay = true;
        }
        else{
            musicOn.isOn = true;
            SoundManager.startPlay = true;
        }
    }

    public void MusicOn(){
        if(musicOn.isOn){
            musicOff.isOn = false; 
            SoundManager.startPlay = true;
        }
        else{
            musicOff.isOn = true;
            SoundManager.stopPlay = true;
        }
    }
}