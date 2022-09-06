using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMGR : MonoBehaviour
{
    MapMGR mapMGR;

    bool isP1;


    SpawnPointMGR spawnPointMGR;
    Vector2Int spawnPos;
    Vector2Int brickYardPos;
    Vector2Int towerPos;


    //仮置き
    bool isFristSpawn = true;

    public enum Step
    {
        Idle,
        SelectSpawnPoint,
        SelectBrickYard,
        SelectTower
    }

    [SerializeField] private Step step; //デバッグようにシリアライズ


    private void Update()
    {
        ClickEntity();

       
    }

    private void ClickEntity()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mouseGridPos = TwoDIM.ToGridPos(mousePos);


        if (mapMGR.GetMap().IsOutOfMap(mouseGridPos)) return;
        switch (step)
        {
            case Step.Idle:
                if (Input.GetMouseButtonDown(0))
                {
                    if (isP1 && mapMGR.GetMap().IDisExit(mouseGridPos, GameManager.instance.p1SpawnPointID))
                    {
                        spawnPointMGR =  mapMGR.GetMap().GetP1SpawnPointMGR(mouseGridPos);
                        spawnPointMGR.OnClicked(isP1);
                    }
                    else if (!isP1 && mapMGR.GetMap().IDisExit(mouseGridPos, GameManager.instance.p2SpawnPointID))
                    {
                        spawnPointMGR = mapMGR.GetMap().GetP2SpawnPointMGR(mouseGridPos);
                        spawnPointMGR.OnClicked(isP1);
                    }
                    else
                    {
                        //何もしない
                    }
                }
                break;
            case Step.SelectSpawnPoint:
                if (Input.GetMouseButtonDown(0))
                {
                    if (isP1 && mapMGR.GetMap().IDisExit(mouseGridPos, GameManager.instance.brickYardID))
                    {
                        mapMGR.GetMap().GetBrickYardMGR(mouseGridPos).OnClicked(isP1);
                    }
                    else
                    {
                        //何もしない
                    }
                }
                break;
            case Step.SelectBrickYard:
                if (Input.GetMouseButtonDown(0))
                {
                    if (isP1 && mapMGR.GetMap().IDisExit(mouseGridPos, GameManager.instance.p1TowerID))
                    {
                        mapMGR.GetMap().GetP1TowerMGR(mouseGridPos).OnClicked(isP1);
                    }
                    else if (!isP1 && mapMGR.GetMap().IDisExit(mouseGridPos, GameManager.instance.p2TowerID))
                    {
                        mapMGR.GetMap().GetP2TowerMGR(mouseGridPos).OnClicked(isP1);
                    }
                    else
                    {
                        //何もしない
                    }
                }
                break;
            case Step.SelectTower:
                SpawnUnit();
                break;
        }
    }

    public void Init(bool isP1)
    {
        mapMGR = GameManager.instance.battleMGR.mapMGR;

       this. isP1 = isP1;
    }

    private void SpawnUnit()
    {
        if (isFristSpawn)
        {
            spawnPointMGR.SpawnUnit(brickYardPos, towerPos);

            isFristSpawn = false;
        }
    }



    //Stepを変更する関数
    public void IdleStep()
    {
        step = Step.Idle;
    }
    public void SelectSpawnPointStep()
    {
        step = Step.SelectSpawnPoint;
    }
    public void SelectBrickYardStep()
    {
        step = Step.SelectBrickYard;
    }
    public void SelectTowerStep()
    {
        step=Step.SelectTower;
    }

    //Getter
    public Step GetStep()
    {
        return step;
    }

    //Setter
    public void SetSpawnPos(Vector2Int pos)
    {
        spawnPos = pos;
    }
    public void SetBrickYardPos(Vector2Int pos)
    {
        brickYardPos = pos;
    }
    public void SetTowerPos(Vector2Int pos)
    {
        towerPos = pos;
    }
}
