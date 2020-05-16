using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlatformController : MonoBehaviour {

    #region Variables

    //Información general
    public float platformSpeed = 0.1f;
    char strLetter = 'A';

    //Booleanas
    public bool bolActive = true;

    //Componentes
    public Canvas canvas;
    public TextMeshProUGUI txtLetra;

    #endregion

    private void Start() {
        canvas.overrideSorting = true;
    }

    private void Update() {
        txtLetra.text = strLetter.ToString().ToUpper();
    }
    
    public void activarCheckLetter() {
        int respuesta = 0;

        if (transform.parent.transform.parent.CompareTag("Principal")) {
            respuesta = transform.GetComponentInParent<PrincipalRoomController>().checkLetra(strLetter);
            Debug.Log("Padre1");
        } else if (transform.parent.transform.parent.CompareTag("Secundaria")) {
            respuesta = transform.GetComponentInParent<SecondaryRoomController>().checkLetra(strLetter);
            Debug.Log("Padre2");
        }

        if (bolActive) {
            if (respuesta == 1) {
                bolActive = false;
                GetComponentInChildren<SpriteRenderer>().color = Color.green;    
            }
        
            else if (respuesta == 2) {
                bolActive = false;
                GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }   
        }

    }

    public void fillWithLetters(char letra) {
        strLetter = letra;    
    }
}
