using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptVendedor : MonoBehaviour {
    static int intNumObjetos = 1;

    public GameObject prefabGeneralObjeto;
    GameObject goPlayer;

    public Sprite[] sprObjetos = new Sprite[intNumObjetos];
    
    List<int> arrListaPrecios = new List<int>(intNumObjetos){20,20,20,20,20,20};
    List<Sprite> arrListaSprites;


    List<GameObject> arrPaneles = new List<GameObject>();
    List<string> arrListaObjetos = new List<string>(intNumObjetos){"mira"};
    List<int> arrListaID = new List<int>(intNumObjetos){0,1,2,3,4,5};

    private void Start() {
        goPlayer = GameObject.FindGameObjectWithTag("Player");

        Debug.Log(intNumObjetos);
        arrListaSprites = new List<Sprite>(sprObjetos);

        foreach(Transform child in transform) {
            if (child.CompareTag("panelVendedor")) {
                arrPaneles.Add(child.gameObject);    
            }
        }

        crearObjetos();
        copiarListaEnPlayer();
    }

    private void Update() {
        
    }

    public void crearObjetos() {
        List<GameObject> copiaPaneles = new List<GameObject>(arrPaneles);
        
        while (copiaPaneles.Count > 0) {
            Debug.Log("Panel Nuevo");
            int randomPanel = Random.Range(0,copiaPaneles.Count);
            Debug.Log("Num paneles:"+copiaPaneles.Count);

            if (arrListaObjetos.Count > 0) {
                Debug.Log("Objeto nuevo");
                int intObjRandom = Random.Range(0,arrListaObjetos.Count);

                GameObject goObjTienda = Instantiate(prefabGeneralObjeto, copiaPaneles[randomPanel].transform.position, Quaternion.identity);
                goObjTienda.GetComponent<ScriptObjTienda>().transform.SetParent(copiaPaneles[randomPanel].transform);
                goObjTienda.GetComponent<ScriptObjTienda>().setSprite(arrListaSprites[intObjRandom]);
                goObjTienda.GetComponent<ScriptObjTienda>().setPrice(arrListaPrecios[intObjRandom]);
                goObjTienda.GetComponent<ScriptObjTienda>().setName(arrListaObjetos[intObjRandom]);
                goObjTienda.GetComponent<ScriptObjTienda>().setID(arrListaID[intObjRandom]);

                arrListaObjetos.RemoveAt(intObjRandom);
                arrListaPrecios.RemoveAt(intObjRandom);
                arrListaSprites.RemoveAt(intObjRandom);
                arrListaID.RemoveAt(intObjRandom);
                
            }

            Debug.Log("Borra panel");
            copiaPaneles.RemoveAt(randomPanel);
        }
    }

    public void copiarListaEnPlayer() {
        goPlayer.GetComponent<PlayerController>().setLista(arrListaObjetos);    
    }
}
