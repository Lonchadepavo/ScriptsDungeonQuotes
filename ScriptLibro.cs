using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptLibro : MonoBehaviour {

    #region Variables
    //Información general
    
    //Booleanas
    bool bolColisionando = false;

    //Componentes
    public SpriteRenderer sRenderer;

    //Sprites
    public Sprite sprLibroAbierto, sprLibroCerrado;
    

    #endregion

    private void Update() {
        if (bolColisionando) sRenderer.sprite = sprLibroAbierto;
        else sRenderer.sprite = sprLibroCerrado;
    }

    public void colisionPortal(bool value) {
        bolColisionando = value;
    }
}
