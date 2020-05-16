using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScriptObjTienda : MonoBehaviour {

    #region Variables
    //Información general
    int intIdentificador;
    string strObjNombre;
    int intObjPrecio;

    //GameObjects
    GameObject goPanelParent;
    GameObject goPlayer;
    
    //Componentes
    public SpriteRenderer sRenderer;
    public TextMeshProUGUI txtPrice;

    //Sprites
    Sprite sprObjeto;

    private void Start() {
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        sRenderer.sprite = sprObjeto;
        txtPrice.text = intObjPrecio.ToString();
        Debug.Log("Nombre: "+strObjNombre+" ID: "+intIdentificador);
    }

    private void Update() {
        
    }

    public void setSprite(Sprite value) {
        sprObjeto = value;
    }

    public void setParent(GameObject value) {
        goPanelParent = value;
    }

    public void setPrice(int value) {
        intObjPrecio = value;
    }

    public void setName(string value) {
        strObjNombre = value;
    }

    public void setID(int value) {
        intIdentificador = value;
    }

    public void sendDataToPlayer() {
        goPlayer.SendMessage("activarObjeto",intIdentificador);    
        Destroy(transform.parent.transform.parent.gameObject);
    }

    #endregion

}
