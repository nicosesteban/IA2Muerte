using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> doors = new List<GameObject>();

	public virtual void Start ()
    {
        //Entre todos los child busca los enemigos y las puertas, guardandolos en sus respectivas lsitas
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "Enemy")
                enemies.Add(child);
            else if (child.tag == "Door")
                doors.Add(child);
        }
	}
	
	public virtual void Update ()
    {
        if (doors.Count != 0)
        {
            //Cuando no hay más enemigos en la habitación, permite que se abran las puertas
            if (enemies.Count == 0)
            {
                for (int i = 0; i < doors.Count; i++)
                {
                    doors[i].GetComponent<Door>().canOpen = true;
                }
            }
        }

        //Desetruye el gameobject si no tiene nada dentro
        if (transform.childCount == 0)
            Destroy(this.gameObject);
	}
}
