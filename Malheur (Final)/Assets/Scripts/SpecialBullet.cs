using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class SpecialBullet : Bullet {

    private float _targetTime;
    public float timeToTarget;

	public override void Start ()
    {
        base.Start();
        Analytics.CustomEvent("specialShots");
	}
	
	public override void Update ()
    {
        base.Update();
	}

    public void OnTriggerStay(Collider c)
    {
        if (c.tag == "Enemy")
        {
            //Esto hace que vaya dirigiendose al enemigo lentamente
            _targetTime += Time.deltaTime;
            transform.up = Vector3.Lerp(transform.up, c.transform.position - transform.position, _targetTime/timeToTarget);
        }
    }
}
