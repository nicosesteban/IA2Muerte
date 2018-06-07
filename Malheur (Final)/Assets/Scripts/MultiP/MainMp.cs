using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainMp : NetworkManager {

    public static GameObject hero1;
    public Transform pos1;
    public static GameObject hero2;
    public Transform pos2;

	void Start ()
    {
        startPositions.Add(pos1);
        startPositions.Add(pos2);
	}
	
	void Update ()
    {
        
	}
}
