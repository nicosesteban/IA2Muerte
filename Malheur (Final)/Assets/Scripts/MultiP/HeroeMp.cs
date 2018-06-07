using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HeroeMp : NetworkBehaviour {

    public Vector3 initialPos;
    public Vector3 crouchHeight;
    public float crouchSpeed;
    private bool _isCrouching;
    private Vector3 _yVector;
    public float speed;
    public float runningSpeed;
    public float rotationSpeed;

    public GameObject capsule;
    public GameObject gun;
    public Rigidbody rigidB;
    public GameObject cameraGO;
    public bool isLocked;

    public GameObject enemy;
    public NetworkManager Nm;
    public List<AudioData> audios = new List<AudioData>();
    public float health;
    public Image healthBar;

    void Awake()
    {
        Nm = GameObject.Find("Network Manager").GetComponent<MainMp>();      
    }

    void Start ()
    {
        Cursor.visible = false;
        health = 100;
        rigidB = this.GetComponent<Rigidbody>();
        gun = this.transform.Find("Gun").gameObject;

        if (isLocalPlayer) this.gameObject.AddComponent<BrainMp>();
        if (Nm.numPlayers == 1)
        {
            this.transform.position = Nm.startPositions[0].position;
            MainMp.hero1 = this.gameObject;
        }
        else
        {
            //El segundo jugador entra, setea a su propio enemigo, y setea la del otro player
            this.transform.position = Nm.startPositions[1].position;
            MainMp.hero2 = this.gameObject;
            enemy = MainMp.hero1;
            enemy.GetComponent<HeroeMp>().enemy = this.gameObject;
        }
    }
	
	void Update ()
    {
        if (health <= 0)
        {
            rigidB.constraints = RigidbodyConstraints.None;
            rigidB.constraints = RigidbodyConstraints.FreezePositionZ;
            rigidB.constraints = RigidbodyConstraints.FreezeRotationZ;
            isLocked = true;
        }

        healthBar.fillAmount = health / 100;

        _yVector.y = rigidB.velocity.y;

        gun.transform.forward = cameraGO.transform.forward;
    }

    public void Move()
    {
        if (_isCrouching)
            rigidB.velocity = (transform.forward * Input.GetAxis("Vertical") + _yVector + transform.right * Input.GetAxis("Horizontal")).normalized * crouchSpeed;

        else if (Input.GetButton("Run") && !_isCrouching)
            rigidB.velocity = (transform.forward * Input.GetAxis("Vertical") + _yVector + transform.right * Input.GetAxis("Horizontal")).normalized * runningSpeed;

        else
            rigidB.velocity = (transform.forward * Input.GetAxis("Vertical") + _yVector + transform.right * Input.GetAxis("Horizontal")).normalized * speed;

        PlaySound("Steps");
    }

    public void Rotate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed, 0));
        cameraGO.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0));
        //Clamp de la camara para que no pegue una vuelta entera en X
        cameraGO.transform.localRotation = (new Quaternion(Mathf.Clamp(cameraGO.transform.localRotation.x, -0.6f, 0.6f),
        cameraGO.transform.localRotation.y, cameraGO.transform.localRotation.z, cameraGO.transform.localRotation.w));
    }

    public void Crouch()
    {
        if (!_isCrouching)
        {
            cameraGO.transform.position += crouchHeight;
            capsule.transform.localScale += crouchHeight;
            _isCrouching = true;
        }
        else
        {
            cameraGO.transform.position -= crouchHeight;
            capsule.transform.localScale -= crouchHeight;
            _isCrouching = false;
        }
    }

    [Command]
    public void CmdUse()
    {
        RpcUse();
    }

    [ClientRpc]
    public void RpcUse()
    {
        Ray myRay = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
        RaycastHit rayInfo = new RaycastHit();
        if (Physics.Raycast(myRay, out rayInfo, 20))
        {
            switch (rayInfo.collider.tag)
            {
                case "Door":
                    if (rayInfo.collider.GetComponent<Door>().canOpen) rayInfo.collider.GetComponent<Door>().isActive = true;
                    break;

                case "Healing":
                    if (health < 100)
                    {
                        rayInfo.collider.transform.GetComponent<HealMachine>().heroe = this.gameObject;
                        rayInfo.collider.transform.GetComponent<HealMachine>().Heal();
                    }
                    break;

                case "Hacking":
                    rayInfo.collider.transform.GetComponent<Hacking>().heroe = this.gameObject;
                    rayInfo.collider.transform.GetComponent<Hacking>().CmdHack();
                    break;
            }
        }
    }

    [ClientRpc]
    public void RpcTakeDamange(int d)
    {
        health -= d;
    }

    public void PlaySound(string sound)
    {
        for (int i = 0; i < audios.Count; i++)
        {
            if (audios[i].audioName == sound && !GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().PlayOneShot(audios[i].audioClip);
        }
    }
}
