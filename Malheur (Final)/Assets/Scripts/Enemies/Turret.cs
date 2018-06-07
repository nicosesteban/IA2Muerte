using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Turret : NetworkBehaviour {

    public GameObject prefabBullet;
    public GameObject gun;
    public GameObject arm;
    public GameObject heroe;
    public float minDist;
    public float timeToAim;

    private float _timeToShoot;
    public float onTimeToShoot;

    public float health;
    public GameObject prefabExplosion;

    public bool isDisabled;

    public virtual void Start ()
    {
        if(SceneManager.GetActiveScene().name != "TestingMultiP") heroe = GameObject.Find("Heroe");
	}
	
	public virtual void Update ()
    {
        /*if (health <= 0)
        {
            //Además de destruir el GameObject, se remueve de la lista de Room
            if (transform.parent != null)
                transform.parent.GetComponent<Room>().enemies.Remove(this.gameObject);

            GameObject explo = Instantiate(prefabExplosion);
            explo.transform.position = transform.position;
            Destroy(this.gameObject);
        }

        //Dada una distancia mínima tira raycast
        if (Vector3.Distance(transform.position, heroe.transform.position) < minDist)
        {
            //Si no hay nada de por medio, mira al persoaje y dispara
            Ray ray = new Ray(gun.transform.position, heroe.transform.position - gun.transform.position);
            RaycastHit rayInfo = new RaycastHit();
            if (Physics.Raycast(ray, out rayInfo))
            {
                if (rayInfo.transform.gameObject == heroe)
                {
                    gun.transform.forward = Vector3.Lerp(gun.transform.forward, heroe.transform.position - gun.transform.position, Time.deltaTime / timeToAim);
                    //El brazo mira al personaje (se evita que rote), el arma también mira.
                    arm.transform.forward = Vector3.Lerp(arm.transform.forward, heroe.transform.position - arm.transform.position, Time.deltaTime / timeToAim);
                    arm.transform.eulerAngles = new Vector3(0, arm.transform.eulerAngles.y, 0);                   
                    Shoot();
                }
            }
        }*/
    }

    public virtual void Shoot()
    {
        _timeToShoot += Time.deltaTime;
        if (_timeToShoot >= onTimeToShoot)
        {
            GameObject bullet = Instantiate(prefabBullet);
            bullet.transform.position = gun.transform.position;
            bullet.transform.up = heroe.transform.position - bullet.transform.position;
            _timeToShoot = 0;
            if (SceneManager.GetActiveScene().name == "TestingMultiP") NetworkServer.Spawn(bullet);
        }
    }

    [ClientRpc]
    public void RpcShoot()
    {
        Shoot();
    }
}
