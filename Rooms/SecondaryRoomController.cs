using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SecondaryRoomController : MonoBehaviour {
    #region Variables
    //Información de las rooms
    int intIdRoom; //ID de la room
    protected string strRoomType; //Lobby, principal, secundaria, boss
    protected string strRoomPosition; //ARRIBA, DERECHA, ABAJO, IZQUIERDA 
    int intContadorGenerador;

    //Booleanas
    protected bool bolLastRoom;
    public bool bolUnlocked = false;
    public bool bolRoomTerminada = false;
    public bool bolRoomEnCurso = false;
    bool bolBucle1 = true;

    //Variables del timer
    public float intTimer = 30;
    public bool timerOn = false;

    //Lista de rooms secundarias
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

    public List<char> arrRespuestaEnChar = new List<char>();

    //Array con el orden de creación de rooms
    string[] arrOrdenCreacion = {"ARRIBA","DERECHA","ABAJO","IZQUIERDA"};

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

    GameObject goPlayer;

    //Componentes
    public AudioSource aSource;

    public TextMeshProUGUI txtPalabra;
    public TextMeshProUGUI txtPista;

    //Timer
    public TextMeshProUGUI txtTimer;

    //Audio
    public AudioClip openDoor;

    #endregion
    
    private void Start() {
        roomBackground.GetComponent<SpriteRenderer>().color = Color.yellow;
        goPlayer = GameObject.FindGameObjectWithTag("Player");

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

                goUpDoor.SetActive(false);
                goDownDoor.SetActive(false);
                goLeftDoor.SetActive(false);

                goRightCollider.SetActive(false);

                if (!bolLastRoom) goLeftCollider.SetActive(false);
            }
                break;

            case "DERECHA": {
                goUpDoor.SetActive(false);
                goRightDoor.SetActive(false);
                goLeftDoor.SetActive(false);

                goDownCollider.SetActive(false);

                if (!bolLastRoom) goUpCollider.SetActive(false);
            }
                break; 

            case "ABAJO": {
                goRightDoor.SetActive(false);
                goDownDoor.SetActive(false);
                goUpDoor.SetActive(false);

                goLeftCollider.SetActive(false);
                
                if (!bolLastRoom) goRightCollider.SetActive(false);
            }
                break; 

            case "IZQUIERDA": {
                goLeftDoor.SetActive(false);
                goDownDoor.SetActive(false);
                goRightDoor.SetActive(false);

                goUpCollider.SetActive(false);

                if (!bolLastRoom) goDownCollider.SetActive(false);
            }
                break; 
        }

        txtTimer.text = intTimer.ToString();

        strPalabraRoom.ToUpper();

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
        } else {
            
            foreach (Transform child in transform) {
                if (child.CompareTag("hDOOR") || child.CompareTag("vDOOR")) {
                    child.GetComponent<DesbloquearPuerta>().unlock(false);    
                }
            }
        }

        if (bolRoomTerminada) {
            timerOn = false;
            strPistaRoom = "¡Habitación terminada!";

            if (intIdRoom+1 < arrRoomsSecundarias.Count) {
                if (!arrRoomsSecundarias[intIdRoom+1].GetComponent<SecondaryRoomController>().bolRoomEnCurso) {
                    arrRoomsSecundarias[intIdRoom+1].GetComponent<SecondaryRoomController>().bolUnlocked = true;
                }
            }
        }

        if (timerOn) {
            startTimer();
        }

        if (arrRespuestaEnChar.Count == 0) {
            bolRoomTerminada = true;
            bolUnlocked = true;
            if (bolBucle1) {
                bolBucle1 = false;
                aSource.PlayOneShot(openDoor);

                goPlayer.GetComponent<PlayerController>().addPoints((int)(10*intTimer));
            }
        }

        txtPalabra.text = strPalabraRoom;
        txtPista.text = strPistaRoom;
    }

    void fillPlatforms() {
        //Se establecen aleatoriamente las letras de la palabra a adivinar
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

    void startTimer() {
        if (intTimer >= 0.0f) {
            intTimer -= Time.deltaTime;
            txtTimer.text = intTimer.ToString("0");

            bolUnlocked = false;
        }
            
        else if (intTimer <= 0.0f) {
            timerOn = false;
            intTimer = 0.0f;
            txtTimer.text = "0";

            foreach(GameObject platform in arrPlatforms) {
                platform.GetComponent<SpriteRenderer>().color = Color.gray;
                platform.GetComponent<PlatformController>().bolActive = false;
            }

            foreach (GameObject platform in arrUsedPlatforms) {
                platform.GetComponent<PlatformController>().bolActive = false;
            }
                
            if (intIdRoom+1 < arrRoomsSecundarias.Count) {
                arrRoomsSecundarias[intIdRoom+1].GetComponent<SecondaryRoomController>().bolUnlocked = true;
                aSource.PlayOneShot(openDoor);
            }

            bolUnlocked = true;
            bolRoomTerminada = true;
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

    public void setWord(string value) {
        strPalabraRoom = value;
    }

    public void setPista(string value) {
        strPistaRoom = value;
    }

    public void setID(int value) {
        intIdRoom = value;
    }
}
