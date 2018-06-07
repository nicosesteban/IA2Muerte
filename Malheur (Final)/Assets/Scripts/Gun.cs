using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public List<AudioData> audios = new List<AudioData>();

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void PlaySound(string sound)
    {
        for (int i = 0; i < audios.Count; i++)
        {
            if (audios[i].audioName == sound)
                GetComponent<AudioSource>().PlayOneShot(audios[i].audioClip);
        }
    }
}
