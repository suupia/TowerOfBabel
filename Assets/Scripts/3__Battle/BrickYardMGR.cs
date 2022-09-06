using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickYardMGR : EntityMGR
{
    InputMGR inputMGR;

    public void Init()
    {
        EntityInit();


        //とりあえず、プレイヤー1だけの操作を書く
        inputMGR = GameManager.instance.battleMGR.p1InputMGR;
    }

    public override void OnClicked(bool isP1)
    {
        Vector2 mousePos;
        Vector2Int mouseGridPos;

        //とりあえず、プレイヤー1だけの操作を書く

        if (inputMGR.GetStep() != InputMGR.Step.SelectSpawnPoint) return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseGridPos = TwoDIM.ToGridPos(mousePos);


        Debug.Log($"mouseGridPos:{mouseGridPos}, gridPos:{gridPos}");

        if (mouseGridPos.Equals(gridPos))
        {
            inputMGR.SelectBrickYardStep();

        }
        else
        {
            //何もしない
        }
    }
}
