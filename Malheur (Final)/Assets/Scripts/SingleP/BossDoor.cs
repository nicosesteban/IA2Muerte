using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : Door {

    private Heroe _heroe;
    public GameObject teleport;

	public override void Start ()
    {
        initialPos = transform.position;
        _heroe = GameObject.Find("Heroe").GetComponent<Heroe>();
	}
	
	public override void Update ()
    {
        if (_heroe.key1 && _heroe.key2)
            canOpen = true;

        if (isActive)
        {
            transform.position += this.transform.forward * speed * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, initialPos) > rangeToDelete)
        {
            teleport.SetActive(true);
        }
	}
}
