using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptVitrina : MonoBehaviour {

    #region Variables

    //Información general    
    public BoxCollider2D vitrinaCollider;

    //GameObjects
    public GameObject objPagina;

    //Componentes
    public SpriteRenderer sRenderer;

    //Sprites
    public Sprite sprVitrina1, sprVitrina2, sprPagina1, sprPagina2;

    #endregion

    
    public void abrirVitrina() {
        sRenderer.sprite = sprVitrina2;
        vitrinaCollider.isTrigger = true;
        objPagina.GetComponent<SpriteRenderer>().sprite = sprPagina2;
    }
}
