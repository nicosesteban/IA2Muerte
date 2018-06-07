using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Hacking : NetworkBehaviour {

    public GameObject heroe;
    public GameObject hackingData;
    private GameObject _key;
    private Vector3 _initialKeyPos;
    private GameObject _hole;
    public float keySpeed;
    private GameObject _shooter;
    private bool _shooting;
    public GameObject prefabBullet;
    public float timeToShoot;
    public GameObject spawn;
    public GameObject prefabTankie;

	void Start ()
    {
       
    }
	
	void Update ()
    {
        if (hackingData != null && hackingData.activeSelf)
        {
            if (_key.GetComponent<BoxCollider2D>().IsTouching(_hole.GetComponent<BoxCollider2D>()))
            {
                CmdSpawn();
                _key.transform.position = _initialKeyPos;
                hackingData.SetActive(false);
                heroe.GetComponent<HeroeMp>().isLocked = false;
            }

            _key.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            MoveKey();
            if(!_shooting) StartCoroutine(Shoot());
        }
	}

    [Command]
    public void CmdHack()
    {
        RpcHack();
    }

    [ClientRpc]
    public void RpcHack()
    {
        hackingData = heroe.transform.Find("TotalCanvasMultiP").Find("Canvas").Find("Hacking Data").gameObject;
        _key = hackingData.transform.Find("Key").gameObject;
        _initialKeyPos = _key.transform.position;
        _hole = hackingData.transform.Find("Hole").gameObject;
        _shooter = hackingData.transform.Find("Shooter").gameObject;
        hackingData.SetActive(true);
        heroe.GetComponent<HeroeMp>().isLocked = true;
    }

    public void MoveKey()
    {
        _key.GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * keySpeed;
        //_key.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * keySpeed;
    }

    void Oscilation(HackingBullet bul)
    {
        bul.xOscilation = Random.Range(-80, 80);
        bul.yOscilation = Random.Range(-80, 80);
    }

    [Command]
    public void CmdSpawn()
    {
        print("La re concha de la lora");
        GameObject tankie = GameObject.Instantiate(prefabTankie);
        tankie.transform.position = spawn.transform.position;
        tankie.transform.forward = spawn.transform.forward;
        tankie.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        tankie.GetComponent<Tankie>().heroe = heroe.GetComponent<HeroeMp>().enemy;
        tankie.GetComponent<NavMeshAgent>().SetDestination(tankie.GetComponent<Tankie>().heroe.transform.position);
        NetworkServer.Spawn(tankie);
    }

    IEnumerator Shoot()
    {
        _shooting = true;
        GameObject bullet = Instantiate(prefabBullet);
        bullet.transform.SetParent(_shooter.transform.parent, false);
        bullet.GetComponent<RectTransform>().position = _shooter.transform.position;
        bullet.GetComponent<HackingBullet>().key = _key;
        Oscilation(bullet.GetComponent<HackingBullet>());
        yield return new WaitForSeconds(timeToShoot);
        _shooting = false;
        yield return null;
    }
}
