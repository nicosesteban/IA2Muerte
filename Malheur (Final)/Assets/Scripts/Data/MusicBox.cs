using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicBox : MonoBehaviour {

    public List<AudioData> songs = new List<AudioData>();

	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
	}
	
	void Update ()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlaySong("Menu Theme");
        }
        else if (SceneManager.GetActiveScene().name == "Lvl1")
        {
            PlaySong("Lvl1 Theme");
            GetComponent<AudioSource>().loop = true;
        }
        else if (SceneManager.GetActiveScene().name == "Lvl2"||
                 SceneManager.GetActiveScene().name == "Lvl3")
        {
            PlaySong("Lvl Theme");
            GetComponent<AudioSource>().loop = true;
        }
        else if (SceneManager.GetActiveScene().name == "LvlBoss")
        {
            PlaySong("Boss Theme");
        }
	}

    public void PlaySong(string s)
    {
        for (int i = 0; i < songs.Count; i++)
        {
            if (songs[i].audioName == s && GetComponent<AudioSource>().clip != songs[i].audioClip)
            {
                GetComponent<AudioSource>().clip = songs[i].audioClip;
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
