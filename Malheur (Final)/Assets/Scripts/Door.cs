using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public bool canOpen;
    public bool isActive;
    public float speed;
    public Vector3 initialPos;
    public float rangeToDelete;
    public Light light;

	public virtual void Start ()
    {
        initialPos = transform.position;
        //Si no está en ninguna Room, puede abrirse sin necesidad de matar enemigos
        if(transform.parent == null)
            canOpen = true;
	}
	
	public virtual void Update ()
    {
        if (canOpen) light.color = Color.green;
        else light.color = Color.red;

        if (isActive)
        {
            transform.position += transform.right * speed * Time.deltaTime;
            if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        }
        if (Vector3.Distance(transform.position, initialPos) >= rangeToDelete)
        {
            if (transform.parent != null)
            {
                Destroy(this.gameObject);
                //Además de destruir el GameObject hay que removerlo de la lista de Room
                transform.parent.GetComponent<Room>().doors.Remove(this.gameObject);
            }
            else
                Destroy(this.gameObject);
        }
	}
}
