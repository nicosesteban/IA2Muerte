using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class Teleport : MonoBehaviour {

    public string sceneToLoad;

    public void OnTriggerEnter (Collider c)
    {
        if (c.gameObject.tag == "Heroe")
        {
            SceneManager.LoadScene(sceneToLoad);
            Analytics.CustomEvent("levelComplete");
        }
    }
}
