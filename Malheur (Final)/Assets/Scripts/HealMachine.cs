using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealMachine : MonoBehaviour {

    public int healCap;
    public GameObject heroe;

	void Start ()
    {
    }
	
	void Update ()
    {
        if (healCap <= 0) GetComponent<AudioSource>().volume--;
        GetComponent<AudioSource>().volume = Mathf.Clamp01(GetComponent<AudioSource>().volume);
    }

    public void Heal()
    {
        if(healCap > 0)
        {
            if(SceneManager.GetActiveScene().name == "TestingMultiP") heroe.GetComponent<HeroeMp>().health++;
            else heroe.GetComponent<Heroe>().health++;
            healCap--;
        }

        if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
    }
}
