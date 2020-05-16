using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour {

    #region Variables

    //Información general
    public float movementSpeed;
    public int intPuntuacion = 0;
    public int intPaginasRecogidas = 0;

    public Vector2 movementDirection;
    public Vector3 principalPosition;
    Vector3 lastCameraPosition;
    float vDirection;
    float hDirection;

    string strFraseGato = "¡Yo voy a darte las pistas!";

    //GameObjects
    public GameObject GameCanvas;
    public GameObject goPagina1, goPagina2, goPagina3, goPagina4;
    public GameObject prefabMira;
    GameObject goMira;
    GameObject currentPlatform;
    GameObject currentObject;

    //Componentes
    public Rigidbody2D rb;
    Camera MainCamera;
    public TextMeshProUGUI txtGato;

    //Booleanas
    bool bolCollisionActive = true;
    bool bolMovingCamera = false;  
    bool collidingPlatform = false;
    bool collidingBook = false;
    bool collidingObject = false;
    bool playerControlable = false;

    //Array de objetos (0 = mira, 1 = libro, 2 = hoja, 3 = gato, 4 = librocerrado, 5 = flecha)
    public bool[] arrObjetosUsados = {false, false, false, false, false, false};

    //Objetos de la tienda
    List<string> listaObjetos;

    #endregion

    private void Start() {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
    }

    private void Update() {
        if (playerControlable) {
            processInputs();
            move();
        
            if (intPaginasRecogidas == 1) {
                goPagina1.SetActive(true);
            } else if (intPaginasRecogidas == 2) {
                goPagina2.SetActive(true);    
            } else if (intPaginasRecogidas == 3) {
                goPagina3.SetActive(true);    
            } else if (intPaginasRecogidas == 4) {
                goPagina4.SetActive(true);    
            }

            if (collidingPlatform) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    if (currentPlatform.transform.GetComponent<PlatformController>().bolActive) {
                        Debug.Log("Espacio");
                        GameObject go = currentPlatform.transform.gameObject;
                        go.GetComponent<PlatformController>().activarCheckLetter();
                    }
                }    
            }

            if (collidingBook) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    if (intPaginasRecogidas == 4) {
                        if (GameCanvas.GetComponent<MainController>().intSubLevel == 5) {
                            Debug.Log("Paso de nivel");
                            GameCanvas.GetComponent<MainController>().intSubLevel = 0; 
                            transform.GetComponentInParent<MainController>().destruirRoom(1);
                        
                        } else if (GameCanvas.GetComponent<MainController>().intSubLevel != 5) {
                            Debug.Log("Mismo nivel");
                            GameCanvas.GetComponent<MainController>().intSubLevel++;
                            transform.GetComponentInParent<MainController>().destruirRoom(0);
                        
                        }
                        resetPlayer();
                    } else if (intPaginasRecogidas == 0) {
                        if (transform.GetComponentInParent<MainController>().intGameState == 1) {
                            GameCanvas.GetComponent<MainController>().intSubLevel++;
                            GameCanvas.GetComponent<MainController>().intLevel++;
                            transform.GetComponentInParent<MainController>().destruirRoom(0);
                            resetPlayer();
                        }
                    }  
                }
            }

            if (collidingObject) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    currentObject.GetComponent<ScriptObjTienda>().sendDataToPlayer();    
                }
            }

        }

        checkObjetos();

        if (bolMovingCamera) {
            MainCamera.transform.Translate(hDirection,vDirection,0);       
        }

        if (Input.GetKey(KeyCode.F)) {
            txtGato.text = "Letras: " + intPuntuacion.ToString();

        } else {
            txtGato.text = strFraseGato;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (bolCollisionActive) {

            if (collision.CompareTag("toDown")) {
                Debug.Log("DOWN");
                StartCoroutine( moveCamera("DOWN"));

            } else if (collision.CompareTag("toUp")) {
                Debug.Log("UP");
                StartCoroutine( moveCamera("UP"));
                
            } else if (collision.CompareTag("toRight")) {
                Debug.Log("RIGHT");
                StartCoroutine( moveCamera("RIGHT"));

            } else if (collision.CompareTag("toLeft")) {
                Debug.Log("LEFT");
                StartCoroutine( moveCamera("LEFT"));

            }

            if (collision.CompareTag("Platform")) {
                Debug.Log("platform");
                collidingPlatform = true;
                currentPlatform = collision.gameObject;

                if (collision.transform.GetComponent<PlatformController>().bolActive) {
                    collision.transform.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.magenta,Color.blue,0.1f);
                }
            }
        }

        if (collision.CompareTag("Pagina")) {
            collision.transform.position = new Vector3(100,100,100);
            intPaginasRecogidas++;
        }

        if (collision.CompareTag("LibroPortal")) {
            collision.SendMessage("colisionPortal",true);
            collidingBook = true;
        }

        if (collision.CompareTag("ObjetoTienda")) {
            currentObject = collision.gameObject;
            collidingObject = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
              
        if (collision.CompareTag("triggerTimer")) {
            collision.transform.GetComponentInParent<SecondaryRoomController>().timerOn = true;
            collision.transform.GetComponentInParent<SecondaryRoomController>().bolRoomEnCurso = true;

            if (collision.transform.parent.tag == "Lobby") {
                setFraseGato(collision.transform.parent.GetComponentInParent<LobbyController>().strPistaRoom);
            }
            else if (collision.transform.parent.tag == "Principal") {
                setFraseGato(collision.transform.parent.GetComponentInParent<PrincipalRoomController>().strPistaRoom);
            }
            else if (collision.transform.parent.tag == "Secundaria") {
                setFraseGato(collision.transform.parent.GetComponentInParent<SecondaryRoomController>().strPistaRoom);
            }
        }

        if (collision.CompareTag("ColliderPistaSetter")) {
            if (collision.transform.parent.tag == "Lobby") {
                setFraseGato(collision.transform.parent.GetComponentInParent<LobbyController>().strPistaRoom);
            }
            else if (collision.transform.parent.tag == "Principal") {
                setFraseGato(collision.transform.parent.GetComponentInParent<PrincipalRoomController>().strPistaRoom);
            }
            else if (collision.transform.parent.tag == "Secundaria") {
                setFraseGato(collision.transform.parent.GetComponentInParent<SecondaryRoomController>().strPistaRoom);
            }
            else if (collision.transform.parent.tag == "Boss") {
                setFraseGato(collision.transform.parent.GetComponentInParent<BossRoomController>().strPistaRoom);    
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("LibroPortal")) {
            collision.SendMessage("colisionPortal",false);
            collidingBook = false;
        }

        if (collision.CompareTag("ObjetoTienda")) {
            currentObject = null;
            collidingObject = false;
        }

        if (collision.CompareTag("Platform")) {
            collidingPlatform = false;
            currentPlatform = null;

            if (collision.transform.GetComponent<PlatformController>().bolActive) {
                collision.transform.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }
    }

    private IEnumerator moveCamera(string direction) {
        bolCollisionActive = false;
        playerControlable = false;
        transform.GetComponent<CircleCollider2D>().enabled = false;
        lastCameraPosition = MainCamera.transform.position;

        bolMovingCamera = true;

        switch(direction) {
            case "DOWN":                
                vDirection = -0.318f;
                hDirection = 0;

                yield return new WaitForSecondsRealtime(0.5f);

                MainCamera.transform.position = new Vector3(lastCameraPosition.x,lastCameraPosition.y-10,lastCameraPosition.z);

                break;

            case "UP":
                vDirection = 0.318f;
                hDirection = 0;

                yield return new WaitForSecondsRealtime(0.5f);

                MainCamera.transform.position = new Vector3(lastCameraPosition.x,lastCameraPosition.y+10,lastCameraPosition.z);

                break;

            case "RIGHT":
                vDirection = 0;
                hDirection = 0.378f;

                yield return new WaitForSecondsRealtime(0.8f);

                MainCamera.transform.position = new Vector3(lastCameraPosition.x+18,lastCameraPosition.y,lastCameraPosition.z);

                break;

            case "LEFT":
                vDirection = 0;
                hDirection = -0.378f;

                yield return new WaitForSecondsRealtime(0.8f);

                MainCamera.transform.position = new Vector3(lastCameraPosition.x-18,lastCameraPosition.y,lastCameraPosition.z);

                break;

        }

        bolMovingCamera = false;

        playerControlable = true;
        bolCollisionActive = true;
        transform.GetComponent<CircleCollider2D>().enabled = true;
    }

    void processInputs() {
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDirection.Normalize();
    }

    void move(){
        rb.velocity = movementDirection * movementSpeed;
    }

    public void setFraseGato(string frase) {
        strFraseGato = frase;
    }

    public void setControlability(bool value) {
        playerControlable = value;
        Debug.Log(playerControlable);
    }

    public void resetPlayer() {
        intPaginasRecogidas = 0;
        transform.position = new Vector3(0,0,0);
        MainCamera.transform.position = new Vector3(0,0,-10);
        goPagina1.SetActive(false);
        goPagina2.SetActive(false);
        goPagina3.SetActive(false);
        goPagina4.SetActive(false);
    }

    public void addPoints(int value) {
        intPuntuacion += value;
    }

    public void subsPoints(int value) {
        intPuntuacion -= value;
    }

    public void checkObjetos() {
        if (arrObjetosUsados[0]) {
            if (goMira == null) {
                goMira = Instantiate(prefabMira, new Vector3(0,0,0),Quaternion.identity);
                goMira.transform.SetParent(transform);
                goMira.transform.localScale = new Vector3(0.3f,0.3f,0);
            }
        } else {
            if (goMira != null) {
                Destroy(goMira);
            }
        }    
    }

    public void setLista(List<string> value) {
        listaObjetos = value;
    }
    
    public void activarObjeto(int value) {
        arrObjetosUsados[value] = true; 
        Debug.Log("Se ha activado el objeto: "+value.ToString());
    }

    public void desactivarObjeto(int value) {
        arrObjetosUsados[value] = false;
        Debug.Log("Se ha desactivado el objeto: "+value.ToString());
    }

}
