using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;


public class Bullet : MonoBehaviour {

    public float speed;
    public int damage;
    public float range;
    private Vector3 _initialPos;

	public virtual void Start ()
    {
        _initialPos = transform.position;
	}
	
	public virtual void Update ()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, _initialPos) > range)
            Destroy(this.gameObject);
	}

    public virtual void OnCollisionEnter (Collision c)
    {
        Destroy(this.gameObject);

        if (c.gameObject.tag == "Enemy")
        {
            c.gameObject.GetComponentInParent<AudioSource>().PlayOneShot(c.gameObject.GetComponentInParent<AudioSource>().clip);
            c.gameObject.GetComponentInParent<Turret>().health -= damage;
        }
        else if (c.gameObject.tag == "Heroe")
        {
            c.gameObject.GetComponent<Heroe>().PlaySound("Hit");

            if (SceneManager.GetActiveScene().name == "TestingMultiP")
                c.gameObject.GetComponent<HeroeMp>().RpcTakeDamange(damage);
            else
            {
                c.gameObject.GetComponent<Heroe>().health -= damage;
                Analytics.CustomEvent("gotHit");
            }
        }
    }
}
