using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMira : MonoBehaviour {

    #region Variables

    //Información general
    int intContador;

    //GameObjects
    GameObject goPlayer;
    GameObject currentPlatform;

    //Componentes

    //Booleanas
    bool collidingPlatform = false;
    
    #endregion

    private void Update() {
        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pz.z = 0;
        transform.position = pz;

        if (collidingPlatform) {
            if (Input.GetMouseButtonDown(0)) {
                if (currentPlatform.transform.GetComponent<PlatformController>().bolActive) {
                    Debug.Log("Espacio");
                    GameObject go = currentPlatform.transform.gameObject;
                    go.GetComponent<PlatformController>().activarCheckLetter();
                }
            }    
        }
    }

    public void setPlayer(GameObject value) {
        goPlayer = value;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Platform")) {
            Debug.Log("platform");
            collidingPlatform = true;
            currentPlatform = collision.gameObject;

            if (collision.transform.GetComponent<PlatformController>().bolActive) {
                collision.transform.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.magenta,Color.blue,0.1f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Platform")) {
            collidingPlatform = false;
            currentPlatform = null;

            if (collision.transform.GetComponent<PlatformController>().bolActive) {
                collision.transform.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
