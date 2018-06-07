using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridEntity : MonoBehaviour {

    public event Action<GridEntity> OnMove = delegate { };

    protected void ExecuteOnMove()
    {
        OnMove(this);
    }
}
