using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class LobbyController : MonoBehaviour {

    #region Variables
    //Información general
    int intLevel, intSubLevel;
    int intMaxRoomsPrincipales, intMaxRoomsSecundarias;
    int intContadorGenerador = 0;

    //Información de las rooms
    protected string strRoomType = "Lobby"; //Lobby, principal, secundaria, boss
    public bool bolUnlocked = true;

    //Lista de rooms principales
    List<GameObject> arrRoomsPrincipales = new List<GameObject>();

    //Lista de temas
    List<string> arrTemas = new List<string>();
    List<string> arrTemasBackup = new List<string>();
    List<string> arrPalabras = new List<string>();
    List<string> arrPalabrasPistas = new List<string>();

    //Pista room
    public string strPistaRoom = "¡Estamos en el Lobby!";

    //Array con el orden de creación de rooms
    string[] arrOrdenCreacion = {"ARRIBA","DERECHA","ABAJO","IZQUIERDA"};
    int[] arrPositionValues = {10,18,-10,-18};

    //GameObjects
    public GameObject goLobbyRoom;
    public GameObject goPrincipalRoom;

    public GameObject roomBackground;

    //Colliders
    public GameObject goUpCollider;
    public GameObject goRightCollider;
    public GameObject goDownCollider;
    public GameObject goLeftCollider;

    //Players
    GameObject goPlayer;

    //Animación FadeOut
    public GameObject prefabLevelFadeOut;

    //Vendedor
    public GameObject objVendedor;

    //Componentes 
    public AudioSource aSource;
    public TextMeshProUGUI txtLevel;

    //Audio
    public AudioClip openDoor;

    //Texto
    public TextAsset taTemas;
    public List<TextAsset> taPalabras;
    public List<TextAsset> taPistas;

    #endregion

    private void Start() {
        StartCoroutine(animacionFadeOut());
        leerListaTemas();
        if (arrTemas.Count > 0) {
            roomBackground.GetComponent<SpriteRenderer>().color = Color.green;
        }
        crearRoomsPrincipales(); //Se ejecuta el método para crear las rooms principales

        //Se desactivan todos los bordes que vienen activados por defecto, esto es solo en la sala de lobby
        goUpCollider.SetActive(false);
        goDownCollider.SetActive(false);
        goRightCollider.SetActive(false);
        goLeftCollider.SetActive(false);

        goPlayer = GameObject.FindGameObjectWithTag("Player");
        goPlayer.GetComponent<PlayerController>().setControlability(false);

        if (intSubLevel == 5) {
            objVendedor.SetActive(true);
        }
    }

    private void Update() {
        if (bolUnlocked) {
            foreach (Transform child in transform) {
                if (child.CompareTag("hDOOR") || child.CompareTag("vDOOR")) {
                    child.GetComponent<DesbloquearPuerta>().unlock(true);    
                }
            }
        }
    }

    void crearRoomsPrincipales() {
        //Se elige aleatoriamente cuantas rooms principales se van a crear
        //Se escogen los temas para cada room principal de una lista de temas
        //Se crean las rooms principales y se añaden al array de rooms principales  
        
        //Se establece la dirección en la que se crearán las salas principales (empieza por arriba)
        string strNextPosition = arrOrdenCreacion[intContadorGenerador];

        float xPos = transform.position.x, yPos = transform.position.y; //Se guarda en variables las posicione x e y del lobby para poder crear las salas principales.
        bool bolGenerating = true; //Booleano que establece si las salas se están creando o no.

        while (bolGenerating) {

            //Switch que establece las variaciones en x e y según la dirección en la que se tenga que crear la room.
            switch(strNextPosition) {
                case "ARRIBA": {
                    xPos = transform.position.x;
                    yPos = transform.position.y+10;

                }
                    break;

                case "DERECHA": {
                    xPos = transform.position.x+18;
                    yPos = transform.position.y;

                }
                    break; 

                case "ABAJO": {
                    xPos = transform.position.x;
                    yPos = transform.position.y-10;

                }
                    break; 

                case "IZQUIERDA": {
                    xPos = transform.position.x-18;
                    yPos = transform.position.y;

                }
                    break; 
            }

            int nRoomsPrincipales = Random.Range(1,intMaxRoomsPrincipales+1); //Número random que decide cuantas rooms se van a crear (por ala)

            for(int i = 0; i < nRoomsPrincipales; i++) {
                GameObject tempRoomPrincipal = Instantiate(goPrincipalRoom, new Vector2(xPos,yPos),Quaternion.identity); //Se instancia el prefab de la room principal y se guarda en una variable temporal.
                tempRoomPrincipal.transform.SetParent(transform); //Se establece su parent (el lobby).
                tempRoomPrincipal.transform.localScale = new Vector3(1,1,1); //Se escala para que tenga el tamaño adecuado.
                tempRoomPrincipal.tag = "Principal";
                arrRoomsPrincipales.Add(tempRoomPrincipal); //Se añade la room creada al array de rooms principales.

                //Comprueba si la room es la última del ala y se establece que es la última room o no (necesario para cosas posteriores)
                if (i == nRoomsPrincipales-1) tempRoomPrincipal.GetComponent<PrincipalRoomController>().setLastRoom(true);
                else tempRoomPrincipal.GetComponent<PrincipalRoomController>().setLastRoom(false);

                tempRoomPrincipal.GetComponent<PrincipalRoomController>().setRoomPosition(strNextPosition); //Se guarda la dirección de la room en su propia variable (dentro del script de la room creada)
                tempRoomPrincipal.GetComponent<PrincipalRoomController>().setRoomType("Principal"); //Se guarda el tipo de room (dentro del script de la room creada).
                tempRoomPrincipal.GetComponent<PrincipalRoomController>().setRoomsSecundarias(intMaxRoomsSecundarias);
                tempRoomPrincipal.GetComponent<PrincipalRoomController>().arrTemas = arrTemasBackup; //Se copia el array de temas en la nueva room.
                tempRoomPrincipal.GetComponent<PrincipalRoomController>().strPistaRoom = "¡Adivina el tema!";
                
                //SELECCIÓN DE TEMA PARA LA NUEVA ROOM
                int nChoosedTheme = Random.Range(0,arrTemas.Count); //Se crea un número aleatorio entre 0 y el total de temas.
                tempRoomPrincipal.GetComponent<PrincipalRoomController>().setTheme(arrTemas[nChoosedTheme]); //Se añade el tema seleccionado.
                tempRoomPrincipal.GetComponent<PrincipalRoomController>().setPalabras(leerPalabras(nChoosedTheme),leerPistas(nChoosedTheme)); //Se añade el tema seleccionado.
                arrTemas.RemoveAt(nChoosedTheme); //Se borra el tema de la lista para que no se pueda repetir.
                taPalabras.RemoveAt(nChoosedTheme);
                taPistas.RemoveAt(nChoosedTheme);

                //Se comprueba de nuevo la dirección de la room para poder sumar a las coordenadas y crear la siguiente.
                if (strNextPosition == "ARRIBA") {
                    xPos = transform.position.x;
                    yPos += 10;

                } else if (strNextPosition == "DERECHA") {
                    xPos += 18;
                    yPos = transform.position.y;

                } else if (strNextPosition == "ABAJO") {
                    xPos = transform.position.x;
                    yPos -= 10;

                } else if (strNextPosition == "IZQUIERDA") {
                    xPos -= 18;
                    yPos = transform.position.y;
                }
            }

            //Se copia el array de rooms principales en cada una de las rooms principales que se crean.
            foreach (GameObject rPrincipal in arrRoomsPrincipales) {
                rPrincipal.GetComponent<PrincipalRoomController>().arrRoomsPrincipales = arrRoomsPrincipales;
            }

            intContadorGenerador++; //Se suma al contador para saber cuando se han creado todas las rooms de ese ala y poder pasar a la siguiente dirección.
            if (intContadorGenerador <= 3) {
                strNextPosition = arrOrdenCreacion[intContadorGenerador];
            }

            //Cuando se han creado las salas en todas las direcciones se desactiva el booleano y dejan de crearse salas.
            if (intContadorGenerador > 3) {
                bolGenerating = false;
            }
        }
    }

    //Método para leer los temas de un archivo y guardarlos en un array.
    void leerListaTemas() { 
        arrTemas = textAssetToList(taTemas);
        arrTemasBackup = textAssetToList(taTemas);
        
    }

    List<string> leerPalabras(int tema) {
        Debug.Log("Tema: "+tema);
        var arrPalabrasProv = textAssetToList(taPalabras[tema]);
        Debug.Log(arrPalabrasProv);
        return arrPalabrasProv;
    }

    List<string> leerPistas(int tema) {
        Debug.Log("Pistas: "+tema);
        var arrPalabrasPistasProv = textAssetToList(taPistas[tema]);
        return arrPalabrasPistasProv;
    }

    private List<string> textAssetToList(TextAsset ta){
        return new List<string>(ta.text.Split('\n'));
    }

    IEnumerator animacionFadeOut() {
        txtLevel.text = "Nivel "+intLevel.ToString()+"-"+intSubLevel.ToString();
        prefabLevelFadeOut.SetActive(true);
        yield return new WaitForSeconds(2);
        prefabLevelFadeOut.SetActive(false);
        goPlayer.GetComponent<PlayerController>().setControlability(true);
        Debug.Log("done");
    }

    public void setLevel(int value) {
        intLevel = value;
    }

    public void setSubLevel(int value) {
        intSubLevel = value;
    }

    public void setRoomsPrincipales(int value) {
        intMaxRoomsPrincipales = value;
    }

    public void setRoomsSecundarias(int value) {
        intMaxRoomsSecundarias = value;
    }
}
