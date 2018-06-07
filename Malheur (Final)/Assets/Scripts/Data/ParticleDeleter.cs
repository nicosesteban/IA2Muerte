using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDeleter : MonoBehaviour {

    private float _onTimeDestroy = 4;
    private float _timeToDestroy;
	
	void Update ()
    {
        _timeToDestroy += Time.deltaTime;
        if (_timeToDestroy >= _onTimeDestroy) Destroy(this.gameObject);
	}
}
