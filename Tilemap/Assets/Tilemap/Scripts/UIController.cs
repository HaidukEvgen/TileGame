using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text scoretxt;
    public GameObject pausePanel;
    public GameObject curFig;
    public GameObject tilemap;
    public static Game gm;

    // Update is called once per frame
    void Update()
    {
        scoretxt.text = gm.GetScore();
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
}
