using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityMGR : MonoBehaviour
{
    //Getter
    public Vector2Int GetGridPos()
    {
        return TwoDIM.ToGridPos(transform.position);
    }
}