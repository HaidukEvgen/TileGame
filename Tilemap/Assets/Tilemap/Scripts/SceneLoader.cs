using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject starterPanel;
    public GameObject settingsPanel;

    
    [SerializeField] public Slider roundSlider;
    public Text roundtxt;
    public int round;

    void Start(){
        round = PlayerPrefs.GetInt("Rounds", round);
        roundtxt.text = "Rounds: " + round.ToString();
        roundSlider.value = (float)round / 10f  - 0.1f;
    }

    public void ChangeValRounds(){
        round = (int)(roundSlider.value * 10f)%10  + 1;
        if(roundSlider.value == 1){
            round = 10;
        }
        roundtxt.text = "Rounds: " + round.ToString();
        PlayerPrefs.SetInt("Rounds", round);
    }

    public void StartGame(){
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings(){
        starterPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings(){
        starterPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}
