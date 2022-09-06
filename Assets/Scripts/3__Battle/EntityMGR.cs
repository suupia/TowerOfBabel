using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityMGR : MonoBehaviour
{
    protected Vector2Int gridPos;

    protected void EntityInit()
    {
        gridPos = TwoDIM.ToGridPos(transform.position); 
    }

    public abstract void OnClicked(bool isP1);
}