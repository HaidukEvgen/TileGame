using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text scoretxt;
    public static Game gm;

    // Update is called once per frame
    void Update()
    {
        scoretxt.text = gm.GetScore();
    }
}
