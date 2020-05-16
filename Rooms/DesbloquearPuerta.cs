using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesbloquearPuerta : MonoBehaviour {

    #region Variables

    //GameObjects
    public GameObject goColliderDoor1, goColliderDoor2;

    //Componentes
    public SpriteRenderer doorSprite;

    //Sprites
    public Sprite sprHDoorUnlocked, sprVDoorUnlocked, sprHDoorLocked, sprVDoorLocked;

    //Booleanas
    public bool bolUnlocked;

    #endregion

    private void Update() {
        if (bolUnlocked) {        
            goColliderDoor1.GetComponent<BoxCollider2D>().isTrigger = true;
            goColliderDoor2.GetComponent<BoxCollider2D>().isTrigger = true;

            if (transform.CompareTag("hDOOR")) {
                doorSprite.sprite = sprHDoorUnlocked;
            } else if (transform.CompareTag("vDOOR")) {
                doorSprite.sprite = sprVDoorUnlocked;    
            }
        } else {
            goColliderDoor1.GetComponent<BoxCollider2D>().isTrigger = false;
            goColliderDoor2.GetComponent<BoxCollider2D>().isTrigger = false;

            if (transform.CompareTag("hDOOR")) {
                doorSprite.sprite = sprHDoorLocked;
            } else if (transform.CompareTag("vDOOR")) {
                doorSprite.sprite = sprVDoorLocked;    
            }    
        }
    }

    public void unlock(bool value) {
        bolUnlocked = value;
    }
}
