using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingBullet : MonoBehaviour {

    public GameObject key;
    private float _degrees;
    private float _rads;
    private Vector3 _mainMove;
    private Vector3 _oscilation;
    private Vector3 _initialPos;
    public float range;
    public float speed;
    public float oscilationSpeed;
    public float xOscilation;
    public float yOscilation;
    public float force;

	void Start ()
    {
        _mainMove = this.transform.position;
        _initialPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_initialPos.y - transform.position.y >= range) Destroy(this.gameObject);

        _mainMove -= transform.up * Time.deltaTime * speed;

        _degrees += Time.deltaTime * oscilationSpeed;
        _rads = _degrees * Mathf.Deg2Rad;

        _oscilation.x = Mathf.Cos(_rads) * xOscilation;
        _oscilation.y = Mathf.Sin(_rads) * yOscilation;

        transform.position = _mainMove + _oscilation;
	}

    public void OnCollisionEnter2D (Collision2D c)
    {
        if (c.gameObject == key)
        {
            key.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            key.GetComponent<Rigidbody2D>().AddForce((key.transform.position - this.transform.position).normalized * force, ForceMode2D.Impulse);
            Destroy(this.gameObject);
        }
    }
}
