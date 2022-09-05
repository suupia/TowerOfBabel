using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointMGR : EntityMGR
{

    GameObject unitParent;
    [SerializeField] GameObject unitPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Init();
            SpawnUnit();
        }
    }

    private void Init()
    {
        unitParent =  transform.parent.transform.parent.transform.Find("UnitParent").gameObject; //親の親オブジェクト（BattleMGR）の子オブジェクト（UnitParent）を取得する
    }

    private void SpawnUnit()
    {
        Vector3 spawnPos = transform.position;
        GameObject unitGO = Instantiate(unitPrefab, spawnPos, Quaternion.identity, unitParent.transform);
        UnitMGR unitMGR = unitGO.GetComponent<UnitMGR>();

    }

}

