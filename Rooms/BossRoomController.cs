using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomController : MonoBehaviour {

    #region Variables

    //Información de las rooms
    public int intIdRoom; //ID de la room
    protected string strRoomType; //Lobby, principal, secundaria, boss
    protected int intBossMechanic; //Mecánica de boss que tendrá la room

    public string strTemaRoom;
    public string strPalabraRoom;
    public string strPistaRoom;

    public List<char> arrAlfabeto = new List<char>(){'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Ñ','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
    string[] arrRoomTypes = {"BossBattle","BossFinal"};

    //Booleanas
    public bool bolUnlocked = false;
    public bool bolRoomTerminada = false;
    public bool bolInBossBattle = false;

    //Array de boss rooms
    public List<GameObject> arrBossRoom = new List<GameObject>();
    public bool[] arrBolMecanicas = {false};

    //Lista de displays y plataformas
    public List<GameObject> arrDisplay = new List<GameObject>();
    public List<GameObject> arrPlatforms = new List<GameObject>();

    //Creación de las rooms
    int xPos, yPos;

    //GameObjects
    public GameObject goBossRoom;
    public GameObject bossDoor;
    public GameObject goLibroPortal;
    public GameObject roomBackground;

    public GameObject platMaxPos;
    public GameObject platMinPos;

    //Arrays para gestionar las palabras/pistas  
    List<string> arrPalabras = new List<string>();
    List<string> arrPistasPalabras = new List<string>();
    public List<string> arrTemas = new List<string>();
    public List<char> arrRespuestaEnChar = new List<char>();

    #endregion

    private void Start() {
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.y-10;

        if (strRoomType == "Boss") {
            crearBossRoom();     
        } else {
            if (intIdRoom == arrBossRoom.Count) bossDoor.SetActive(false);
        }

        switch(intIdRoom) {
            case 0:

                bolUnlocked = true;
                break;

            case 1:
                //Se elige un display de plataformas y la mecánica de boss

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

                bolUnlocked = true;

                break;

            case 2:

                goLibroPortal.SetActive(true);
                break;
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

    void crearBossRoom() {
        for (int i = 0; i < 2; i++) {
            GameObject goBoss = Instantiate(goBossRoom, new Vector2(xPos,yPos),Quaternion.identity);

            //Se establece el id y el tipo de la room.
            goBoss.transform.GetComponent<BossRoomController>().intIdRoom = i+1;
            goBoss.transform.GetComponent<BossRoomController>().setRoomType(arrRoomTypes[i]);
            
            //Se establece el parent y el tamaño de la room.
            goBoss.transform.SetParent(transform);
            goBoss.transform.localScale = new Vector3(1,1,1);

            //Se añade la room a la lista de rooms.
            arrBossRoom.Add(goBoss);

            //Se disminuye la variable yPos para que la siguiente room se cree mas abajo.
            yPos -= 10;
        }

        foreach(GameObject bossRoom in arrBossRoom) {
            bossRoom.transform.GetComponent<BossRoomController>().arrBossRoom = arrBossRoom;
        }
    }

    public void setRoomType(string value) {
        strRoomType = value;
    }

    public void setTheme(string value) {
        strTemaRoom = value;
    }    
}
