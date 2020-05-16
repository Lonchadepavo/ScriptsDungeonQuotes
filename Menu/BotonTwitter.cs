using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonTwitter : MonoBehaviour {
    
    public string cuentaTwitter;

    private void OnMouseDown() {
        Application.OpenURL("https://twitter.com/"+cuentaTwitter);
    }
    
}
