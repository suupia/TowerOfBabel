using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMGR : MonoBehaviour
{
    Vector2Int gridPos;

    private void Start()
    {
        Init();
        DecideRoute();
    }
    private void Init()
    {
        gridPos = TwoDIM.ToGridPos(transform.position);
    }

    private void DecideRoute()
    {
        Vector2Int startPos;
        Vector2Int endPos;

        startPos = gridPos;
        endPos = new Vector2Int (1,1);

        int mapWidth = GameManager.instance.battleMGR.mapMGR.GetMapWidth();
        int mapHeight = GameManager.instance.battleMGR.mapMGR.GetMapHeight();


      List<Vector2Int> searchRoute =  WaveletSearch.DiagonalSearchShortestRoute(startPos, endPos, WaveletSearch.OrderInDirection.LeftDown, mapWidth, mapHeight, 2, (vector, wallID) => { return GameManager.instance.battleMGR.mapMGR.GetMapValue(vector) < 0 ? true : false; });
    }

    //Getter
    public Vector2Int GetGridPos()
    {
        return gridPos;
    }
}
