using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrungeParallax : MonoBehaviour
{
    float mousePosX;
    float screenX;
    float maxX;
    float mousePosY;
    float screenY;
    float maxY;
    Vector3 setThisPos;

    void Start()
    {
        screenY = Screen.height /2;
        screenX = Screen.width / 2;
        maxY = screenY / 6;
        maxX = screenX / 6;
    }
	
	void Update ()
    {
        mousePosX = -Input.mousePosition.x;
        mousePosY = -Input.mousePosition.y;
        setThisPos = new Vector3(mousePosX * maxX / screenX + screenX, mousePosY * maxY / screenY + screenY, 0f);
        var lerpPos = Vector3.Lerp(setThisPos , transform.position , 0.9f);
        transform.position = lerpPos;
	}
}
