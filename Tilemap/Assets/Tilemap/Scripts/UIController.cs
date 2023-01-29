using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text scoretxt;
    public GameObject pausePanel;
    public static Game gm;

    // Update is called once per frame
    void Update()
    {
        scoretxt.text = gm.GetScore();
    }

    public void PauseGame(){
        pausePanel.SetActive(true);
        Time.timeScale = 0; 
    }

    public void CountinueGame(){
        pausePanel.SetActive(false);
        Time.timeScale = 1; 
    }
}
