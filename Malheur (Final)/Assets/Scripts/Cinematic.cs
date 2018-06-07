using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour {

    public MovieTexture cinematic;
    public float currentTime;

	void Start ()
    {
        cinematic.Play();
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= cinematic.duration) SceneManager.LoadScene("MainMenu");
	}
}
