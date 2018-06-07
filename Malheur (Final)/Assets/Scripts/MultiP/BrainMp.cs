using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainMp : MonoBehaviour {

    private HeroeMp _heroe;

	void Start ()
    {
        _heroe = this.GetComponent<HeroeMp>();
	}

    void Update()
    {
        if (!_heroe.isLocked)
        {
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
                _heroe.Move();

            //Se iguala la velocity a 0 cuando se levanta el boton para que no se siga moviendo.
            if (Input.GetButtonUp("Vertical") || Input.GetButtonUp("Horizontal"))
                _heroe.rigidB.velocity = Vector3.zero;

            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                _heroe.Rotate();

            if (Input.GetButtonDown("Crouch"))
                _heroe.Crouch();

            if (Input.GetKey(KeyCode.E) || Input.GetKeyDown(KeyCode.E))
                _heroe.CmdUse();
        }
    }
}
