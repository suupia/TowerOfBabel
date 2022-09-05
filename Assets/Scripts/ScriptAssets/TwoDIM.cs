using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TwoDIM
{
    public static Vector2Int ToGridPos(Vector3 transformPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(transformPosition.x), Mathf.FloorToInt(transformPosition.y));
    }
}
