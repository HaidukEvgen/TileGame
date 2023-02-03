using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{ 
    public static bool firstOn = true;
    public static bool startPlay = false;
    public static bool stopPlay = false;
    public static bool isOn = true;

    private AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        sound = gameObject.GetComponent<AudioSource>(); 
        if(firstOn){
            DontDestroyOnLoad(gameObject);
            firstOn = false;
            sound.Play();
        }
        else{
            Destroy(gameObject);
        }
    }

    void Update(){
        if(startPlay){
            sound.Play();
            startPlay = false;
            isOn = true;
        }

        if(stopPlay){
            sound.Stop();
            stopPlay = false;
            isOn = false;
        }
    }
}
