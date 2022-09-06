using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMGR : EntityMGR
{
    InputMGR inputMGR;

    public void Init(bool isP1)
    {
        EntityInit();

        if (isP1)
        {
            inputMGR = GameManager.instance.battleMGR.p1InputMGR;

        }
        else
        {
            inputMGR = GameManager.instance.battleMGR.p2InputMGR;

        }
    }

    public override void OnClicked( bool isP1)
    {
        Vector2 mousePos;
        Vector2Int mouseGridPos;


        if (inputMGR.GetStep() != InputMGR.Step.SelectBrickYard) return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseGridPos = TwoDIM.ToGridPos(mousePos);


        Debug.Log($"mouseGridPos:{mouseGridPos}, gridPos:{gridPos}");

        if (mouseGridPos.Equals(gridPos))
        {
            inputMGR.SelectTowerStep();

        }
        else
        {
            //何もしない
        }

        inputMGR.SetTowerPos(mouseGridPos);

    }
}
