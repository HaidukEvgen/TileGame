using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Draggable : MonoBehaviour
{
    private Vector3 mousePositionOffset;
    public static BoxCollider2D m_collider;
    public static bool throwBack = false;
    public static int height;
    public static int width;

    //set collider for the first figure
    void Awake() {
        m_collider = GetComponent<BoxCollider2D>();
    }


    //if figure was left incorrectly then throw it back without placing
    void Update(){
        if(throwBack){
            ThrowFigureBack();
            throwBack = false;
        } else {
            Testing.position = transform.position;
        }
    }

    private Vector3 GetMouseWorldPosition(){
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown(){
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag(){
        transform.position = GetMouseWorldPosition() + mousePositionOffset;
        UITutorial.closePanel = true;
    }

    private void OnMouseUp(){
        Testing.position = transform.position;  
        Testing.processingTurn = true;
    }

    private void ThrowFigureBack(){
        float x = 10.5f - width/2;
        float y = 0f - height/2;
        transform.position = new Vector3(x, y, 0);
        if(!UITutorial.gm.GetCurPlayer().IsUpperPlayer() || !UITutorial.singleMode)
            UITutorial.openPanel = true;
    }

    //change figure's collider according to its size
    public static void ChangeCollider(int x, int y){
        m_collider.offset = new Vector2(x / 2, y / 2);
    }
}
