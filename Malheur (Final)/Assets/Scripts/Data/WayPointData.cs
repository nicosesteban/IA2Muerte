using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointData : MonoBehaviour {

    public GameObject[] wayPoints;

	void Start ()
    {
        wayPoints = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            wayPoints[i] = transform.GetChild(i).gameObject;
        }
	}
}
