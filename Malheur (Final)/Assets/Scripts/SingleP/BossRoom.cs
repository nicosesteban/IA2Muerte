using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class BossRoom : Room {

    public List<GameObject> spawns = new List<GameObject>();
    public List<GameObject> buttons = new List<GameObject>();
    public List<GameObject> cores = new List<GameObject>();
    public float _timerWave;
    public float onTimeWave;
    public bool _onWave;
    public int _waves;
    public GameObject prefabTankie;
    private GameObject _heroe;
    private float _timeToWin;
    private float _onTimeToWin = 6;

	public override void Start ()
    {
        base.Start();
        _heroe = GameObject.Find("Heroe");
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "Spawn")
                spawns.Add(child);
            else if (child.tag == "Button")
            {
                buttons.Add(child);
                child.GetComponent<Animator>().StopPlayback();
            }
            else if (child.tag == "Core")
            {
                cores.Add(child);
            }
        }
	}
	
	public override void Update ()
    {
        if (buttons.Count == 0)
        {
            for (int i = 0; i < cores.Count; i++)
            {
                cores[i].GetComponent<Core>().readyToDie = true;
            }

            _timeToWin += Time.deltaTime;
            if (_timeToWin >= _onTimeToWin)
            {
                SceneManager.LoadScene("Victory");
                Analytics.CustomEvent("wonTheGame");
            }
        }

        //Si está recibiendo oleadas suma al timer entreoleada y oleada
        if (_onWave)
        {
            _timerWave += Time.deltaTime;
        }
        if (_timerWave >= onTimeWave)
        {
            _timerWave = 0;
            //Solo suma mientras sea menos que la cantidad de oleadas deseadas
            if (_waves < 3)
            {
                Spawn();
                _waves++;
            }
        }

        //Si cumplió todas las oleadas del turno, permite abrir las puertas
        if(_waves == 3 && enemies.Count == 0)
        {
            _waves = 0;
            _timerWave = 0;
            _onWave = false;
            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].GetComponent<Door>().canOpen = true;
            }
        }

        for (int i = 0; i < doors.Count; i++)
        {
            //Si una de las puertas se abre, evita que las otras se abran
            if (doors[i].GetComponent<Door>().isActive)
            {
                for (int h = 0; h < doors.Count; h++)
                {
                    doors[h].GetComponent<Door>().canOpen = false;
                }
            }
        }
	}

    public void Spawn()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            GameObject tankie = Instantiate(prefabTankie);
            tankie.transform.SetParent(this.transform);
            tankie.transform.position = spawns[i].transform.position;
            tankie.transform.forward = spawns[i].transform.forward;
            tankie.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
            enemies.Add(tankie);
            tankie.GetComponent<NavMeshAgent>().SetDestination(_heroe.transform.position);
        }
    }

    public void PressedButton(GameObject button)
    {
        if (buttons.Contains(button))
        {
            button.GetComponent<Animator>().Play("Pulled");
            if (!button.GetComponent<AudioSource>().isPlaying) button.GetComponent<AudioSource>().Play();
            buttons.Remove(button);
            if (buttons.Count > 0)
            {
                _onWave = true;
            }
        }
    }
}
