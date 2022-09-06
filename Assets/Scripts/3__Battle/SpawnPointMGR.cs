using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointMGR : EntityMGR
{

    GameObject unitParent;
    [SerializeField] GameObject unitPrefab;

    InputMGR inputMGR;
    bool isP1;

    void Start()
    {
        Init(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnUnit();
        }

        if (Input.GetMouseButtonDown(0))
        {
         //   OnClicked();
        }
    }

    public void Init(bool isP1)
    {
        EntityInit();
        unitParent =  transform.parent.transform.parent.transform.Find("UnitParent").gameObject; //親の親オブジェクト（BattleMGR）の子オブジェクト（UnitParent）を取得する
        this.isP1 = isP1;
        if (isP1)
        {
            inputMGR = GameManager.instance.battleMGR.p1InputMGR;
        }
        else
        {
            inputMGR = GameManager.instance.battleMGR.p2InputMGR;
        }
    }

    private void SpawnUnit()
    {
        Vector2Int spawnPos = TwoDIM.ToGridPos(transform.position);
        GameObject unitGO = Instantiate(unitPrefab, transform.position, Quaternion.identity, unitParent.transform);
        UnitMGR unitMGR = unitGO.GetComponent<UnitMGR>();

        //数値データ
        GameManager.instance.battleMGR.mapMGR.GetMap().MultiplySetValue(spawnPos,GameManager.instance.p1UnitID);
        //スクリプト
        GameManager.instance.battleMGR.mapMGR.GetMap().AddP1UnitMGR(spawnPos, unitMGR);

        //初期化
        unitMGR.Init();

    }

    public override  void OnClicked(bool isP1)
    {
        Vector2 mousePos;
        Vector2Int mouseGridPos;

        if (inputMGR.GetStep() != InputMGR.Step.Idle) return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseGridPos = TwoDIM.ToGridPos(mousePos);


        Debug.Log($"mouseGridPos:{mouseGridPos}, gridPos:{gridPos}");

        if (mouseGridPos.Equals(gridPos))
        {

            inputMGR.SelectSpawnPointStep();

        }
        else
        {
            //何もしない
        }
    }

}

