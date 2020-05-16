using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class PrincipalRoomController : MonoBehaviour {
    #region Variables
    //Información de las rooms
    public int intIdRoom; //ID de la room
    protected string strRoomType; //Lobby, principal, secundaria, boss
    protected string strRoomPosition; //ARRIBA, DERECHA, ABAJO, IZQUIERDA
    int intMaxRoomsSecundarias;

    //Booleanas
    protected bool bolLastRoom;
    public bool bolUnlocked = false;
    public bool bolRoomTerminada = false;
    public bool bolAlaFinalizada = false;
    bool bolBucle1 = true;

    //Lista de rooms secundarias
    public List<GameObject> arrRoomsPrincipales = new List<GameObject>();
    public List<GameObject> arrRoomsSecundarias = new List<GameObject>();

    //Lista de displays y plataformas
    public List<GameObject> arrDisplay = new List<GameObject>();
    public List<GameObject> arrPlatforms = new List<GameObject>();
    public List<GameObject> arrUsedPlatforms = new List<GameObject>();
    public List<char> arrAlfabeto = new List<char>(){'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Ñ','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

    //Tema de la room
    public string strTemaRoom;
    public string strPalabraRoom;
    public string strPistaRoom;
    List<string> arrPalabras = new List<string>();
    List<string> arrPistasPalabras = new List<string>();
    public List<string> arrTemas = new List<string>();

    public List<char> arrRespuestaEnChar = new List<char>();
    
    
    //Array con el orden de creación de rooms
    string[] arrOrdenCreacion = {"ARRIBA","DERECHA","ABAJO","IZQUIERDA"};
    int[] arrPositionValues = {10,18,-10,-18};

    int intContadorGenerador;

    //Prefabs de rooms
    public GameObject goPrincipalRoom;
    public GameObject goSecondaryRoom;

    //Partes de la room
    public GameObject goUpDoor;
    public GameObject goRightDoor;
    public GameObject goDownDoor;
    public GameObject goLeftDoor;

    public GameObject goUpCollider;
    public GameObject goRightCollider;
    public GameObject goDownCollider;
    public GameObject goLeftCollider;

    public GameObject goUpDoorCollider1;
    public GameObject goUpDoorCollider2;

    public GameObject goRightDoorCollider1;
    public GameObject goRightDoorCollider2;

    public GameObject goDownDoorCollider1;
    public GameObject goDownDoorCollider2;

    public GameObject goLeftDoorCollider1;
    public GameObject goLeftDoorCollider2;

    public GameObject roomBackground;
    public GameObject objVitrina;

    //Componentes
    public AudioSource aSource;
    public TextMeshProUGUI txtTema;

    //Audio
    public AudioClip openDoor;

    #endregion

    private void Start() {
        roomBackground.GetComponent<SpriteRenderer>().color = Color.blue;
        Debug.Log(arrPalabras.Count);
        Debug.Log(arrPistasPalabras.Count);
        txtTema.enabled = false;

        foreach(Transform child in transform) {
            if (child.CompareTag("Display")) {
                arrDisplay.Add(child.gameObject);
            }    
        }

        int randomDisplay = Random.Range(0,arrDisplay.Count);
        arrDisplay[randomDisplay].SetActive(true);

        foreach(GameObject display in arrDisplay) {
            if (display.activeSelf) {
                foreach (Transform child in display.transform) {
                    arrPlatforms.Add(child.gameObject);
                }
            }
        }

        //Switch que comprueba la dirección de la room para activar/desactivar las puertas y paredes que toque en cada caso.
        switch(strRoomPosition) {
            case "ARRIBA": {
                if (!bolLastRoom) {
                    goRightDoor.SetActive(false);
                    goDownDoor.SetActive(false);
                    goLeftDoor.SetActive(false);

                    goUpCollider.SetActive(false);
                    goLeftCollider.SetActive(false);
                    goDownCollider.SetActive(false);
                } else {
                    goRightDoor.SetActive(false);
                    goDownDoor.SetActive(false);
                    goLeftDoor.SetActive(false);
                    goUpDoor.SetActive(false);

                    goLeftCollider.SetActive(false);
                    goDownCollider.SetActive(false);
                }

            }
                break;

            case "DERECHA": {
                if (!bolLastRoom) {
                    goUpDoor.SetActive(false);
                    goDownDoor.SetActive(false);
                    goLeftDoor.SetActive(false);

                    goUpCollider.SetActive(false);
                    goLeftCollider.SetActive(false);
                    goRightCollider.SetActive(false);
                } else {
                    goRightDoor.SetActive(false);
                    goDownDoor.SetActive(false);
                    goLeftDoor.SetActive(false);
                    goUpDoor.SetActive(false);

                    goUpCollider.SetActive(false);
                    goLeftCollider.SetActive(false);
                }

            }
                break; 

            case "ABAJO": {
                if (!bolLastRoom) {
                    goRightDoor.SetActive(false);
                    goUpDoor.SetActive(false);
                    goLeftDoor.SetActive(false);

                    goUpCollider.SetActive(false);
                    goDownCollider.SetActive(false);
                    goRightCollider.SetActive(false);
                } else {
                    goRightDoor.SetActive(false);
                    goDownDoor.SetActive(false);
                    goLeftDoor.SetActive(false);
                    goUpDoor.SetActive(false);

                    goRightCollider.SetActive(false);
                    goUpCollider.SetActive(false);
                }

            }
                break; 

            case "IZQUIERDA": {
                if (!bolLastRoom) {
                    goRightDoor.SetActive(false);
                    goDownDoor.SetActive(false);
                    goUpDoor.SetActive(false);

                    goDownCollider.SetActive(false);
                    goLeftCollider.SetActive(false);
                    goRightCollider.SetActive(false);
                } else {
                    goRightDoor.SetActive(false);
                    goDownDoor.SetActive(false);
                    goLeftDoor.SetActive(false);
                    goUpDoor.SetActive(false);

                    goDownCollider.SetActive(false);
                    goRightCollider.SetActive(false);
                }

            }
                break; 
        }
        
        crearRoomsSecundarias();

        strPalabraRoom = strTemaRoom.ToUpper();

        char[] palabraEnChar = strPalabraRoom.ToCharArray();
        arrRespuestaEnChar = new List<char>(palabraEnChar);
        if (System.Char.IsWhiteSpace(arrRespuestaEnChar[arrRespuestaEnChar.Count-1])) {
            arrRespuestaEnChar.RemoveAt(arrRespuestaEnChar.Count-1);
        }
        
        fillPlatforms();
    }

    private void Update() {

        //Comprueba si la room está desbloqueada, si lo está busca las puertas de la room y las abre.
        if (bolUnlocked) {
            foreach (Transform child in transform) {
                if (child.CompareTag("hDOOR") || child.CompareTag("vDOOR")) {
                    child.GetComponent<DesbloquearPuerta>().unlock(true);    
                }
            }
        }

        if (bolLastRoom) objVitrina.SetActive(true);

        if (bolRoomTerminada) {
            arrRoomsSecundarias[0].GetComponent<SecondaryRoomController>().bolUnlocked = false;
            strPistaRoom = "¡Habitación terminada!";

            foreach(GameObject platform in arrPlatforms) {
                platform.transform.GetComponent<PlatformController>().bolActive = false;    
            }

            foreach(GameObject platform in arrUsedPlatforms) {
                platform.transform.GetComponent<PlatformController>().bolActive = false;    
            }

            if (bolLastRoom) {
                objVitrina.GetComponent<ScriptVitrina>().SendMessage("abrirVitrina");
            }
        }

        if (arrRoomsSecundarias[arrRoomsSecundarias.Count-1].GetComponent<SecondaryRoomController>().bolRoomTerminada) {
            if (bolLastRoom) { 
                bolAlaFinalizada = true;
            }

            txtTema.enabled = true;
            txtTema.GetComponent<TextMeshProUGUI>().enabled = true;
        }

        if (arrRespuestaEnChar.Count == 0) {
            bolRoomTerminada = true;
            bolUnlocked = true;
            if (bolBucle1) {
                bolBucle1 = false;
                aSource.PlayOneShot(openDoor);
            }
        }

        txtTema.text = strTemaRoom;
    }

    void crearRoomsSecundarias() {
        //Se elige aleatoriamente cuantas rooms secundarias se van a crear
        //Se escogen las palabras para cada room secundaria de una lista de palabras
        //Se crean las rooms secundarias y se añaden al array de rooms secundarias

        //Se establece la dirección en la que se crearán las salas principales (empieza por arriba)
        string strNextPosition = strRoomPosition;

        float xPos = transform.position.x, yPos = transform.position.y; //Se guarda en variables las posicione x e y del lobby para poder crear las salas principales.

        //Switch que establece las variaciones en x e y según la dirección en la que se tenga que crear la room.
        switch(strNextPosition) {
            case "ARRIBA": {
                xPos = transform.position.x-18;
                yPos = transform.position.y;

            }
                break;

            case "DERECHA": {
                xPos = transform.position.x;
                yPos = transform.position.y+10;

            }
                break; 

            case "ABAJO": {
                xPos = transform.position.x+18;
                yPos = transform.position.y;

            }
                break; 

            case "IZQUIERDA": {
                xPos = transform.position.x;
                yPos = transform.position.y-10;

            }
                break; 
        }

        int nRoomsPrincipales = Random.Range(1,intMaxRoomsSecundarias+1); //Número random que decide cuantas rooms se van a crear (por cada room principal).
        int intContadorSalas = 0;

        for(int i = 0; i < nRoomsPrincipales; i++) {
            GameObject tempRoomSecundaria = Instantiate(goSecondaryRoom, new Vector2(xPos,yPos),Quaternion.identity); //Se instancia el prefab de la room secundaria y se guarda en una variable temporal.
            tempRoomSecundaria.transform.SetParent(transform); //Se establece su parent (la room principal).
            tempRoomSecundaria.transform.localScale = new Vector3(1,1,1); //Se escala para que tenga el tamaño adecuado.
            tempRoomSecundaria.tag = "Secundaria";
            arrRoomsSecundarias.Add(tempRoomSecundaria); //Se añade la room creada al array de rooms secundarias.

            //Comprueba si la room es la última del ala y se establece que es la última room o no (necesario para cosas posteriores)
            if (i == nRoomsPrincipales-1) tempRoomSecundaria.GetComponent<SecondaryRoomController>().setLastRoom(true);
            else tempRoomSecundaria.GetComponent<SecondaryRoomController>().setLastRoom(false);

            tempRoomSecundaria.GetComponent<SecondaryRoomController>().setRoomPosition(strNextPosition); //Se guarda la dirección de la room en su propia variable (dentro del script de la room creada)
            tempRoomSecundaria.GetComponent<SecondaryRoomController>().setRoomType("Secundaria"); //Se guarda el tipo de room (dentro del script de la room creada)
            tempRoomSecundaria.GetComponent<SecondaryRoomController>().setID(intContadorSalas);
            intContadorSalas++;

            int nChoosedWord = Random.Range(0,arrPalabras.Count); //Se crea un número aleatorio entre 0 y el total de temas.
            tempRoomSecundaria.GetComponent<SecondaryRoomController>().setWord(arrPalabras[nChoosedWord].ToUpper()); //Se añade el tema seleccionado.
            tempRoomSecundaria.GetComponent<SecondaryRoomController>().setPista(arrPistasPalabras[nChoosedWord]); //Se añade el tema seleccionado.
            //Debug.Log("Room: "+ intIdRoom+" Tema:"+strTemaRoom+ " Palabra: "+ arrPalabras[nChoosedWord]+ " Pista: "+arrPistasPalabras[nChoosedWord]+ " Numero: "+nChoosedWord);
            arrPalabras.RemoveAt(nChoosedWord); //Se borra el tema de la lista para que no se pueda repetir.
            arrPistasPalabras.RemoveAt(nChoosedWord);

            //Se comprueba de nuevo la dirección de la room para poder sumar a las coordenadas y crear la siguiente.
            if (strNextPosition == "ARRIBA") {
                xPos -= 18;
                yPos = transform.position.y;

            } else if (strNextPosition == "DERECHA") {
                xPos = transform.position.x;
                yPos += 10;

            } else if (strNextPosition == "ABAJO") {
                xPos += 18;
                yPos = transform.position.y;

            } else if (strNextPosition == "IZQUIERDA") {
                xPos = transform.position.x;
                yPos -= 10;
            }
        }

        //Se copia el array de rooms secundarias en cada una de las rooms secundarias que se crean.
        foreach (GameObject rSecundaria in arrRoomsSecundarias) {
            rSecundaria.GetComponent<SecondaryRoomController>().arrRoomsSecundarias = arrRoomsSecundarias;
        }

        arrRoomsSecundarias[0].GetComponent<SecondaryRoomController>().bolUnlocked = true; //Desbloquea por defecto todas las rooms secundarias que están al principio.
    }

    void fillPlatforms() {
        //Se establecen aleatoriamente las letras de la palabra a adivinar
        foreach(char letra in arrRespuestaEnChar) {
            Debug.Log(letra);
        }
        for (int i = 0; i < arrRespuestaEnChar.Count; i++) {
            int rNumero = Random.Range(0,arrPlatforms.Count);

            arrPlatforms[rNumero].GetComponent<PlatformController>().fillWithLetters(arrRespuestaEnChar[i]);
            arrUsedPlatforms.Add(arrPlatforms[rNumero]);
            arrPlatforms.RemoveAt(rNumero);
        }

        for (int i = 0; i < arrPlatforms.Count; i++) {
            int rNumero = Random.Range(0,arrAlfabeto.Count); 
            arrPlatforms[i].GetComponent<PlatformController>().fillWithLetters(arrAlfabeto[rNumero]);
        }
    }

    public int checkLetra(char letra) {
        bool check = false;
        for (int i = 0; i < arrRespuestaEnChar.Count; i++) {
            if (letra.Equals(arrRespuestaEnChar[i])) {
                check = true;
                Debug.Log("Esta");

                if (letra.Equals(arrRespuestaEnChar[0])) {
                    arrRespuestaEnChar.RemoveAt(0);
                    return 1;
                }
            }
        }

        if (!check) {
            return 2;
        }

        return 0;

    }

    public void setLastRoom(bool value) {
        bolLastRoom = value;
    }

    public void setRoomPosition(string value) {
        strRoomPosition = value;
    }

    public void setRoomType(string value) {
        strRoomType = value;
    }

    public void setTheme(string value) {
        strTemaRoom = value;
    }

    public void setRoomsSecundarias(int value) {
        intMaxRoomsSecundarias = value;
    }

    public void setPalabras(List<string> palabras, List<string> pistas) {
        arrPalabras = palabras;
        arrPistasPalabras = pistas;
    }
}
