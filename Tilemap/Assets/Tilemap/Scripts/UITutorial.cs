using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorial : MonoBehaviour{

    public static bool isOn;

    void Start()
    {
        isOn = (PlayerPrefs.GetInt("Tutorial", 0) == 1)? true: false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
