using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    public GameObject prefabExplosion;
    private float _onTimeToExplode = 1;
    private float _timeToExplode;
    public bool readyToDie;

	void Update ()
    {
        if (readyToDie)
        {
            _timeToExplode += Time.deltaTime;
            if(_timeToExplode >= _onTimeToExplode)
            {
                _timeToExplode = 0;
                GameObject explo = Instantiate(prefabExplosion);
                explo.transform.localScale *= 2;
                explo.transform.position = transform.position;
            }
        }
	}
}
