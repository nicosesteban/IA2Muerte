using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using IA2;

public class Tankie : Turret {

    public NavMeshAgent navMeshA;

    public List<AudioData> audios = new List<AudioData>();

    public enum TankieInputs { IDLE, PURSUIT, SHOOT, DISABLE }
    private EventFSM<TankieInputs> _myFsm;

    public bool alarmTrigger;

    void Awake()
    {
        var idle = new State<TankieInputs>("Idle");
        var pursuit = new State<TankieInputs>("Pursuit");
        var shoot = new State<TankieInputs>("Shoot");
        var disable = new State<TankieInputs>("Disable");

        #region StateConfiguration
        StateConfigurer.Create(idle)
            .SetTransition(TankieInputs.PURSUIT, pursuit)
            .SetTransition(TankieInputs.SHOOT, shoot)
            .SetTransition(TankieInputs.DISABLE, disable)
            .Done();

        StateConfigurer.Create(pursuit)
            .SetTransition(TankieInputs.IDLE, idle)
            .SetTransition(TankieInputs.SHOOT, shoot)
            .SetTransition(TankieInputs.DISABLE, disable)
            .Done();

        StateConfigurer.Create(shoot)
            .SetTransition(TankieInputs.IDLE, idle)
            .SetTransition(TankieInputs.PURSUIT, pursuit)
            .SetTransition(TankieInputs.DISABLE, disable)
            .Done();

        StateConfigurer.Create(disable)
            .SetTransition(TankieInputs.IDLE, idle)
            .SetTransition(TankieInputs.PURSUIT, pursuit)
            .SetTransition(TankieInputs.SHOOT, shoot)
            .Done();
        #endregion

        #region IdleSettings

        idle.OnUpdate += () =>
        {
            if (Vector3.Distance(transform.position, heroe.transform.position) < minDist)
            {
                Ray ray = new Ray(gun.transform.position, heroe.transform.position - gun.transform.position);
                RaycastHit rayInfo = new RaycastHit();
                if (Physics.Raycast(ray, out rayInfo, minDist))
                {
                    if (rayInfo.transform.gameObject == heroe)
                    {
                        SendInputToFSM(TankieInputs.PURSUIT);
                    }
                }
            }
            if (health <= 0)
                SendInputToFSM(TankieInputs.DISABLE);
            if (alarmTrigger)
                SendInputToFSM(TankieInputs.PURSUIT);
        };
        /*idle.GetTransition(TankieInputs.DISABLE).OnTransition += x =>
        {
            //Activar Particula
        };*/
        idle.GetTransition(TankieInputs.PURSUIT).OnTransition += x =>
        {
            //Hacer Ruido
            PlaySound("StartPursuit");
        };

        #endregion

        #region PursuitSettings

        pursuit.OnUpdate += () =>
        {
            if (navMeshA.velocity.x <= 0 && navMeshA.velocity.z == 0)
                SendInputToFSM(TankieInputs.IDLE);

            if (Vector3.Distance(transform.position, heroe.transform.position) > minDist && !alarmTrigger)
                SendInputToFSM(TankieInputs.IDLE);

            if (Vector3.Distance(transform.position, heroe.transform.position) < minDist)
            {
                Ray ray = new Ray(gun.transform.position, heroe.transform.position - gun.transform.position);
                RaycastHit rayInfo = new RaycastHit();
                if (Physics.Raycast(ray, out rayInfo))
                {
                    if (rayInfo.transform.gameObject == heroe)
                    {
                        SendInputToFSM(TankieInputs.SHOOT);
                    }
                }
            }
            if (health <= 0)
                SendInputToFSM(TankieInputs.DISABLE);
        };
        pursuit.OnFixedUpdate += () =>
        {
            if (Vector3.Distance(transform.position, heroe.transform.position) < minDist)
            {
                Ray ray = new Ray(gun.transform.position, heroe.transform.position - gun.transform.position);
                RaycastHit rayInfo = new RaycastHit();
                if (Physics.Raycast(ray, out rayInfo, minDist))
                {
                    if (rayInfo.transform.gameObject == heroe)
                    {
                        navMeshA.SetDestination(heroe.transform.position);
                    }
                }
            }
        };
        pursuit.OnExit += x =>
        {
            alarmTrigger = false;
        };
        /*pursuit.GetTransition(TankieInputs.IDLE).OnTransition += x =>
        {
            PlaySound("EndPursuit");
        };*/

        #endregion

        #region ShootSettings

        shoot.OnFixedUpdate += () =>
        {
            gun.transform.forward = Vector3.Lerp(gun.transform.forward, heroe.transform.position - gun.transform.position, Time.deltaTime / timeToAim);
            arm.transform.forward = Vector3.Lerp(arm.transform.forward, heroe.transform.position - arm.transform.position, Time.deltaTime / timeToAim);
            arm.transform.eulerAngles = new Vector3(0, arm.transform.eulerAngles.y, 0);
            Shoot();

            if (navMeshA.velocity.x <= 0 && navMeshA.velocity.z == 0)
                SendInputToFSM(TankieInputs.IDLE);

            if (Vector3.Distance(transform.position, heroe.transform.position) > minDist)
                SendInputToFSM(TankieInputs.IDLE);

            if (health <= 0)
                SendInputToFSM(TankieInputs.DISABLE);

            if (Vector3.Distance(transform.position, heroe.transform.position) < minDist)
                SendInputToFSM(TankieInputs.PURSUIT);
        };
        #endregion

        #region DisableSettings
        disable.OnFixedUpdate += () =>
        {
            isDisabled = true;


            //si no se destruye,falta hacer que si isDisable es true haga algo o que directamente haga algo.

            //si se destruye.

            /*if (transform.parent != null)
                transform.parent.GetComponent<Room>().enemies.Remove(this.gameObject);

            GameObject explo = Instantiate(prefabExplosion);
            explo.transform.position = transform.position;
            Destroy(this.gameObject);*/
        };
        #endregion

        _myFsm = new EventFSM<TankieInputs>(idle);
    }

    public override void Start ()
    {
        navMeshA = this.GetComponent<NavMeshAgent>();
        base.Start();
	}
	
	public override void Update ()
    {
        /*if (Vector3.Distance(transform.position, heroe.transform.position) < minDist)
        {
            Ray ray = new Ray(gun.transform.position, heroe.transform.position - gun.transform.position);
            RaycastHit rayInfo = new RaycastHit();
            if (Physics.Raycast(ray, out rayInfo, minDist))
            {
                if (rayInfo.transform.gameObject == heroe)
                {
                    navMeshA.SetDestination(heroe.transform.position);
                }
            }
        }*/

        base.Update();

        _myFsm.Update();
    }

    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
    }

    private void SendInputToFSM(TankieInputs inp)
    {
        _myFsm.SendInput(inp);
    }

    public override void Shoot()
    {
        if (SceneManager.GetActiveScene().name == "TestingMultiP") RpcShoot();
        else base.Shoot();
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
