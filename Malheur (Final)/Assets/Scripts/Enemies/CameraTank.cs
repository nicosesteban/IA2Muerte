using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Linq;

using IA2;

public class CameraTank : MonoBehaviour {

    private GameObject _heroe;
    private bool _lost;
    public WayPointData waySystem;
    public int wayIndex = 0;
    public int wayDirection = 1;
    public bool moving = true;
    public float speed;
    public float minDist;
    public float visionAngle;
    public GameObject cam;
    public GameObject arm;

    public enum CamaraTankInputs { IDLE, MOVE, PURSUIT, ALARM}
    private EventFSM<CamaraTankInputs> _myFsm;

    public List<Tankie> allEnemies;


    void Awake()
    {
        var idle = new State<CamaraTankInputs>("Idle");
        var move = new State<CamaraTankInputs>("Move");
        var pursuit = new State<CamaraTankInputs>("Pursuit");
        var alarm = new State<CamaraTankInputs>("Alarm");

        #region StateConfiguration
        StateConfigurer.Create(idle)
            .SetTransition(CamaraTankInputs.MOVE, move)
            .SetTransition(CamaraTankInputs.PURSUIT, pursuit)
            .SetTransition(CamaraTankInputs.ALARM, alarm)
            .Done();

        StateConfigurer.Create(move)
            .SetTransition(CamaraTankInputs.IDLE, idle)
            .SetTransition(CamaraTankInputs.PURSUIT, pursuit)
            .SetTransition(CamaraTankInputs.ALARM, alarm)
            .Done();

        StateConfigurer.Create(pursuit)
            .SetTransition(CamaraTankInputs.IDLE, idle)
            .SetTransition(CamaraTankInputs.MOVE, move)
            .SetTransition(CamaraTankInputs.ALARM, alarm)
            .Done();

        StateConfigurer.Create(alarm)
            .SetTransition(CamaraTankInputs.IDLE, idle)
            .SetTransition(CamaraTankInputs.MOVE, move)
            .SetTransition(CamaraTankInputs.PURSUIT, pursuit)
            .Done();

        #endregion
        move.OnUpdate += () =>
        {
            if (Vector3.Distance(transform.position, _heroe.transform.position) <= minDist)
            {
                if (Vector3.Angle(transform.forward, _heroe.transform.position - transform.position) <= visionAngle)
                {
                    Ray r = new Ray(transform.position, _heroe.transform.position - transform.position);
                    RaycastHit rayInfo = new RaycastHit();
                    if (Physics.Raycast(r, out rayInfo))
                    {
                        if (rayInfo.collider.tag == "Heroe")
                        {
                            SendInputToFSM(CamaraTankInputs.ALARM);
                        }
                    }
                }
            }
        };
        move.OnFixedUpdate += () =>
        {
            if (wayIndex >= waySystem.wayPoints.Length || wayIndex < 0)
            {
                wayDirection *= -1;
                wayIndex += wayDirection;
            }

            if (moving)
            {
                Vector3 dist = waySystem.wayPoints[wayIndex].transform.position - transform.position;

                //Mientras esté lejos del waypoint, me voy acercando
                if (dist.magnitude > speed * Time.deltaTime)
                {
                    transform.forward = Vector3.Lerp(transform.forward, dist.normalized, 0.25f);
                    transform.position += transform.forward * speed * Time.deltaTime;
                }
                //Si me acerco, me fijo en el waypoint y apunto al siguiente
                else
                {
                    transform.position = new Vector3(waySystem.wayPoints[wayIndex].transform.position.x, transform.position.y,
                                                     waySystem.wayPoints[wayIndex].transform.position.z);
                    wayIndex += wayDirection;
                }
            }
        };

        alarm.OnFixedUpdate += () =>
        {
            if (Vector3.Distance(transform.position, _heroe.transform.position) <= minDist)
            {
                if (Vector3.Angle(transform.forward, _heroe.transform.position - transform.position) <= visionAngle)
                {
                    Ray r = new Ray(transform.position, _heroe.transform.position - transform.position);
                    RaycastHit rayInfo = new RaycastHit();
                    if (Physics.Raycast(r, out rayInfo))
                    {
                        if (rayInfo.collider.tag == "Heroe")
                        {
                            cam.transform.up = Vector3.Lerp(cam.transform.up, cam.transform.position - _heroe.transform.position, Time.deltaTime / 2);
                            arm.transform.forward = Vector3.Lerp(arm.transform.forward, _heroe.transform.position - arm.transform.position, Time.deltaTime / 2);
                            arm.transform.eulerAngles = new Vector3(0, arm.transform.eulerAngles.y, 0);
                            if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
                            moving = false;
                            _heroe.GetComponent<Heroe>().isLocked = true;
                            AchievementData.spotted = true;
                            //_lost = true;
                            StartCoroutine(Lost());
                        }
                    }
                }
            }
        };

        _myFsm = new EventFSM<CamaraTankInputs>(move);
    }

    void Start()
    {
        _heroe = GameObject.Find("Heroe");

        var hola = FindObjectsOfType<Tankie>();

        foreach (var gay in hola)
        {
            allEnemies.Add(gay.GetComponent<Tankie>());
        }
    }

    void Update ()
    {
        _myFsm.Update();

        var TorretasCercanas = allEnemies.Where(x =>
                        (Vector3.Distance(transform.position, x.transform.position) <= 20))
                        .DebugLogCount();

        foreach (var item in TorretasCercanas.Take(Random.Range(0, TorretasCercanas.Count())))
        {
            // Vengan putos
        }

        var TorretasCercanasReparables = allEnemies.Where(x =>
        (Vector3.Distance(transform.position, x.transform.position) <= 20))
        .Where(x => x.health < 40)
        .OrderByDescending(x => x)
        .DebugLogCount()
        .FirstOrDefault();

        /*
        #region Movement
        //Si estoy en el último o primero pego la vuelta
        if (wayIndex >= waySystem.wayPoints.Length || wayIndex < 0)
        {
            wayDirection *= -1;
            wayIndex += wayDirection;
        }

        if (moving)
        {
            Vector3 dist = waySystem.wayPoints[wayIndex].transform.position - transform.position;

            //Mientras esté lejos del waypoint, me voy acercando
            if(dist.magnitude > speed * Time.deltaTime)
            {
                transform.forward = Vector3.Lerp(transform.forward, dist.normalized, 0.25f);
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            //Si me acerco, me fijo en el waypoint y apunto al siguiente
            else
            {
                transform.position = new Vector3(waySystem.wayPoints[wayIndex].transform.position.x, transform.position.y,
                                                 waySystem.wayPoints[wayIndex].transform.position.z);
                wayIndex += wayDirection;
            }
        }
        #endregion

        #region Sight
        if (Vector3.Distance(transform.position, _heroe.transform.position) <= minDist)
        {
            if(Vector3.Angle(transform.forward, _heroe.transform.position - transform.position) <= visionAngle)
            {
                Ray r = new Ray(transform.position, _heroe.transform.position - transform.position);
                RaycastHit rayInfo = new RaycastHit();
                if(Physics.Raycast(r, out rayInfo))
                {
                    if(rayInfo.collider.tag == "Heroe")
                    {
                        cam.transform.up = Vector3.Lerp(cam.transform.up, cam.transform.position - _heroe.transform.position, Time.deltaTime / 2);
                        arm.transform.forward = Vector3.Lerp(arm.transform.forward, _heroe.transform.position - arm.transform.position, Time.deltaTime / 2);
                        arm.transform.eulerAngles = new Vector3(0, arm.transform.eulerAngles.y, 0);
                        if(!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
                        moving = false;
                        _heroe.GetComponent<Heroe>().isLocked = true;
                        AchievementData.spotted = true;
                        _lost = true;
                    }
                }
            }
        }
        #endregion

        if (_lost)
        {
            StartCoroutine(Lost());
        }*/
    }

    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
    }


    IEnumerator Lost()
    {
        yield return new WaitForSeconds(4);
        Analytics.CustomEvent("spottedByCamera");
        Menu.retryScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Defeat");
    }

    private void SendInputToFSM(CamaraTankInputs inp)
    {
        _myFsm.SendInput(inp);
    }
}
public static class Extensions
{
    public static IEnumerable<Tankie> DebugLogCount(this IEnumerable<Tankie> seed)
    {
        Debug.Log("" + seed.Count());
        return seed;
    }
}
