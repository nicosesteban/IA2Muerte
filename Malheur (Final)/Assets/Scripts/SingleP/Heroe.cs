using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class Heroe : MonoBehaviour {

    public Vector3 crouchHeight;
    public float crouchSpeed;
    private bool _isCrouching;
    public bool godMode;
    private Vector3 _yVector;
    public float speed;
    public float runningSpeed;
    public float rotationSpeed;

    public GameObject capsule;
    private Rigidbody _rigidB;
    public GameObject cameraGO;
    public GameObject gun;
    public bool isLocked;

    public Image gunImage;
    public Text heatText;
    public int heat;
    public int cool;
    private float _timeToCool;
    public float onTimeToCool;
    private float _timeToSuperCool;
    public float onTimeToSuperCool;
    private bool _superCool;
    public GameObject prefabBullet;
    public int bulletHeat;
    private float _timeShoot;
    public float onTimeShoot;
    private bool _isShooting;
    public int specialBulletHeat;
    public GameObject prefabSpecialBullet;
    public int _specialCharge;
    public int specialChargeCap;
    public Image specialChargeIndicator;
    public Image aim;
    private Vector2 _initialAimSize;

    public List<AudioData> audios = new List<AudioData>();
    public float health;
    public Image healthBar;
    public bool key1;
    public bool key2;
    public Image key1Image;
    public Image key2Image;
    public GameObject achievement;

	void Start ()
    {
        Cursor.visible = false;
        health = 100;
        _rigidB = GetComponent<Rigidbody>();
        _initialAimSize = aim.rectTransform.sizeDelta;
	}

    void Update()
    {
        if (health <= 0)
        {
            _rigidB.constraints = RigidbodyConstraints.None;
            _rigidB.constraints = RigidbodyConstraints.FreezePositionZ;
            _rigidB.constraints = RigidbodyConstraints.FreezeRotationZ;
            isLocked = true;
            StartCoroutine(Die());
        }
        healthBar.fillAmount = health / 100;

        _yVector.y = _rigidB.velocity.y;

        if (godMode)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponentInChildren<CapsuleCollider>().enabled = false;
        }
        else
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponentInChildren<CapsuleCollider>().enabled = true;
        }

        if (gun != null)
        {
            heatText.text = heat.ToString() + "%";
            gun.transform.forward = cameraGO.transform.forward;
        }


        #region Inputs
        if (!isLocked)
        {
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
                Move();

            //Se iguala la velocity a 0 cuando se levanta el boton para que no se siga moviendo.
            if (Input.GetButtonUp("Vertical") || Input.GetButtonUp("Horizontal"))
                _rigidB.velocity = Vector3.zero;

            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                Rotate();

            if (Input.GetButtonDown("Crouch"))
                Crouch();

            if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("MainMenu");

            if ((Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) && heat <= 100 - bulletHeat && !Input.GetButton("Fire2"))
                Shoot();

            //Al mantener click derecho va cargando el tiro especial
            if (Input.GetButton("Fire2") && !Input.GetButton("Fire1"))
            {
                if (gun != null)
                {
                    if (!gun.GetComponent<AudioSource>().isPlaying) gun.GetComponent<Gun>().PlaySound("Special Shoot Charge");
                    aim.rectTransform.sizeDelta += new Vector2(2, 2);
                    aim.rectTransform.sizeDelta = new Vector2(Mathf.Clamp(aim.rectTransform.sizeDelta.x, _initialAimSize.x, specialChargeIndicator.rectTransform.sizeDelta.x),
                                                              Mathf.Clamp(aim.rectTransform.sizeDelta.x, _initialAimSize.x, specialChargeIndicator.rectTransform.sizeDelta.x));

                    _superCool = false;
                    _isShooting = true;
                    _specialCharge++;
                    _specialCharge = Mathf.Clamp(_specialCharge, 0, specialChargeCap);
                }
            }

            if (Input.GetButtonUp("Fire2"))
            {
                if (gun != null)
                {
                    aim.rectTransform.sizeDelta = _initialAimSize;
                    _isShooting = false;
                    if (heat <= 100 - specialBulletHeat && !Input.GetButton("Fire1"))
                    {
                        SpecialShoot();
                    }
                }
            }

            if (Input.GetButtonUp("Fire1"))
                _isShooting = false;

            if (Input.GetKey(KeyCode.E) || Input.GetKeyDown(KeyCode.E))
                Use();

        }
        #endregion

        //Solo se reduce el calor del arma cuando no está disparando
        if (!_isShooting && heat > 0)
        {
            //Mientras corre el timer normal que enfría el arma de manera normal(_timeToCool)
            //corre otro más largo para que después de un tiempo el arma enfríe más rápido(_timeToSuperCool)
            _timeToSuperCool += Time.deltaTime;
            if (_timeToSuperCool > onTimeToSuperCool)
            {
                _superCool = true;
                _timeToSuperCool = 0;
            }
            //Si pasó el tiempo suficiente empieza a enfriar más rápido
            if (_superCool)
                _timeToCool += Time.deltaTime * 2;
            //Sino, lo normal
            else
                _timeToCool += Time.deltaTime;

            if (_timeToCool >= onTimeToCool)
            {
                heat -= cool;
                heat = Mathf.Clamp(heat, 0, 100);
                _timeToCool = 0;
            }
        }
    }

    public void Move()
    {
        if (_isCrouching)
            _rigidB.velocity = (transform.forward * Input.GetAxis("Vertical") + _yVector + transform.right * Input.GetAxis("Horizontal")).normalized * crouchSpeed;

        else if (Input.GetButton("Run") && !_isCrouching)
            _rigidB.velocity = (transform.forward * Input.GetAxis("Vertical") + _yVector + transform.right * Input.GetAxis("Horizontal")).normalized * runningSpeed;

        else if (godMode)
            transform.position += cameraGO.transform.forward * Time.deltaTime * runningSpeed;

        else
            _rigidB.velocity = (transform.forward * Input.GetAxis("Vertical") + _yVector + transform.right * Input.GetAxis("Horizontal")).normalized * speed;

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

    public void Shoot()
    {
        if (gun != null)
        {
            _isShooting = true;
            _superCool = false;
            _timeShoot += Time.deltaTime;

            if (_timeShoot >= onTimeShoot)
            {
                GameObject bullet = Instantiate(prefabBullet);
                bullet.transform.position = gun.transform.position;
                bullet.transform.up = gun.transform.forward;
                heat += bulletHeat;
                heat = Mathf.Clamp(heat, 0, 100);
                _timeShoot = 0;

                gun.GetComponent<Gun>().PlaySound("Shoot");
            }
        }
    }

    public void SpecialShoot()
    {
        gun.GetComponent<Gun>().PlaySound("Special Shoot");

        _isShooting = false;
        GameObject specialBullet = Instantiate(prefabSpecialBullet);
        //Hace la mitad de daño o todo entero según cuanto cargó el tiro (También afecta el tamaño)
        if (_specialCharge > 0 && _specialCharge <= specialChargeCap / 2)
        {
            specialBullet.GetComponent<SpecialBullet>().damage /= 2;
            specialBullet.transform.localScale *= specialBullet.transform.localScale.x;
        }
        specialBullet.transform.position = gun.transform.position;
        specialBullet.transform.up = gun.transform.forward;
        heat += specialBulletHeat;
        heat = Mathf.Clamp(heat, 0, 100);
        _specialCharge = 0;
    }

    public void Use()
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

                case "Button":
                    rayInfo.collider.transform.GetComponentInParent<BossRoom>().PressedButton(rayInfo.transform.gameObject);
                    break;
            }
        }
    }

    public void PlaySound(string sound)
    {
        for (int i = 0; i < audios.Count; i++)
        {
            if (audios[i].audioName == sound && !GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().PlayOneShot(audios[i].audioClip);
        }
    }

    public void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Key")
        {
            PlaySound("Pick Up");
            if (!key1)
            {
                key1 = true;
                key1Image.gameObject.SetActive(true);
                Destroy(c.gameObject);
            }
            else
            {
                key2 = true;
                key2Image.gameObject.SetActive(true);
                Destroy(c.gameObject);
            }
        }
        
        if(c.gameObject.tag == "RayGun")
        {
            PlaySound("Pick Up");
            Destroy(c.gameObject);
            gun = transform.FindChild("BRR").gameObject;
            gun.SetActive(true);
            heatText.gameObject.SetActive(true);
            gunImage.gameObject.SetActive(true);
            //Activo la puerta
            GameObject.Find("nivel encuentro arma").transform.FindChild("GreatDoor003").GetComponent<BossDoor>().canOpen = true;
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(3);
        Menu.retryScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Defeat");
        Analytics.CustomEvent("deathIn", transform.position);
    }
}
