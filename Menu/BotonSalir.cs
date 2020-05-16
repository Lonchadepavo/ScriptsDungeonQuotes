using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonSalir : MonoBehaviour {

    #region Variables
    //Variables generales

    //Lista de sprites del botón
    public Sprite[] spritesBotonSalir = new Sprite[3];

    //Componentes
    public SpriteRenderer sRenderer;

    //Booleanas
    bool clicked = false;

    #endregion

    private void OnMouseOver() {
        if (!clicked) {
            sRenderer.sprite = spritesBotonSalir[1];
        }
    }

    private void OnMouseExit() {
        sRenderer.sprite = spritesBotonSalir[0];    
    }

    private void OnMouseDown() {
        clicked = true;
        sRenderer.sprite = spritesBotonSalir[2];
        StartCoroutine(HandleIt());
        
    }

    private IEnumerator HandleIt() {
        // process pre-yield
        yield return new WaitForSeconds( 0.2f );
        Application.Quit();
    }
}
