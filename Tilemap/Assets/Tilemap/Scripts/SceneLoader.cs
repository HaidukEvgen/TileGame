using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject starterPanel;
    public GameObject settingsPanel;
    
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
