using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonJugar : MonoBehaviour {

    #region Variables

    //Variables generales

    //Lista de sprites del botón
    public Sprite[] spritesBotonJugar = new Sprite[3];

    //Componentes
    public SpriteRenderer sRenderer;
    public SpriteRenderer sprFondoNegro;

    //Booleanas
    bool clicked = false;

    #endregion

    private void OnMouseOver() {
        if (!clicked) {
            sRenderer.sprite = spritesBotonJugar[1];
        }
    }

    private void OnMouseExit() {
        sRenderer.sprite = spritesBotonJugar[0];    
    }

    private void OnMouseDown() {
        clicked = true;
        sRenderer.sprite = spritesBotonJugar[2];
        HandleIt();
        
    }

    private void HandleIt() {
        SceneManager.LoadScene ("MainScene");
    }

    IEnumerator fadeToBlack() {
        for (int i = 0; i < 255; i++) {
            yield return null;
            //Color tmp Thugge
            //sprFondoNegro.color.a += i;
        }
    }
}
