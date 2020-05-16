using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {
    #region Variables

    //Información general
    public int intGameState = 0; //0 = En juego, 1 = En boss
    public int intLevel = 1, intSubLevel = 1;
    int intMaxRoomsPrincipales, intMaxRoomsSecundarias;

    //GameObjects

    //Rooms
    public GameObject LobbyPrefab;
    public GameObject BossPrefab;
    GameObject goCurrentRoom;

    //Player
    public GameObject prefabPlayer;
    GameObject goPlayer;

    //Lista de rooms
    GameObject[] arrRooms;
    string[] arrRoomTags = {"Lobby", "Boss"};

    //Componentes
    public AudioSource aSource;

    //Audio
    public AudioClip bgMusic;

    #endregion

    private void Awake() {
        goPlayer = Instantiate(prefabPlayer, new Vector3(0,0,0),Quaternion.identity);
        goPlayer.transform.SetParent(transform);
        goPlayer.transform.localScale = new Vector3(161.1171f,161.1171f,0);
        goPlayer.GetComponent<PlayerController>().principalPosition = goPlayer.transform.position;
    }

    private void Start() {
        arrRooms = new GameObject[]{LobbyPrefab, BossPrefab};
        aSource.PlayOneShot(bgMusic);
        crearRoom(intGameState);
    }

    public void crearRoom(int value) {
        switch(intLevel) {
            case 1:
                intMaxRoomsPrincipales = 1;
                intMaxRoomsSecundarias = 1;
                break;

            case 2:
                intMaxRoomsPrincipales = 1;
                intMaxRoomsSecundarias = 2;
                break;
            case 3:
                intMaxRoomsPrincipales = 2;
                intMaxRoomsSecundarias = 3;
                break;

            case 4:
                intMaxRoomsPrincipales = 3;
                intMaxRoomsSecundarias = 10;
                break;

            case 5:
                intMaxRoomsPrincipales = 3;
                intMaxRoomsSecundarias = 20;
                break;
        }

        GameObject goRoom = Instantiate(arrRooms[value], new Vector3(0,0,0),Quaternion.identity);
        goRoom.transform.localScale = new Vector3(1,1,1);
        goRoom.transform.tag = arrRoomTags[intGameState];

        if (intGameState == 0) {
            goRoom.transform.GetComponent<LobbyController>().setRoomsPrincipales(intMaxRoomsPrincipales);
            goRoom.transform.GetComponent<LobbyController>().setRoomsSecundarias(intMaxRoomsSecundarias);
            goRoom.transform.GetComponent<LobbyController>().setLevel(intLevel);
            goRoom.transform.GetComponent<LobbyController>().setSubLevel(intSubLevel);
        }
        else if (intGameState == 1) {
            goRoom.transform.GetComponent<BossRoomController>().goBossRoom = BossPrefab;
            goRoom.transform.GetComponent<BossRoomController>().setRoomType("Boss");
        }

        goCurrentRoom = goRoom;
        goRoom.transform.SetParent(transform);
    }

    public void destruirRoom(int value) {
        intGameState = value;
        Destroy(goCurrentRoom);
        goCurrentRoom = null;
        crearRoom(intGameState);
    }

}
