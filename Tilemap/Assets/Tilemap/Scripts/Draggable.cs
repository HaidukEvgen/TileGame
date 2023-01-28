using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Draggable : MonoBehaviour
{
    private Vector3 mousePositionOffset;
    private static BoxCollider2D m_collider;
    public static bool throwBack = false;

    // set collider for the first figure
    void Awake() {
        m_collider = GetComponent<BoxCollider2D>();
    }

    //if figure was left incorrectly then throw it back without placing
    void Update(){
        if(throwBack){
            ThrowFigureBack();
            throwBack = false;
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
    }

    private void OnMouseUp(){
        Testing.position = transform.position;  
        Testing.processingTurn = true;
    }

    private void ThrowFigureBack(){
        transform.position = new Vector3(6, -4, 0);
    }

    //change figure's collider according to its size
    public static void ChangeCollider(int x, int y){
        m_collider.size = new Vector3(x, y, 0f);
        m_collider.offset = new Vector3(.5f * x, .5f * y, 0f);
    }
}
