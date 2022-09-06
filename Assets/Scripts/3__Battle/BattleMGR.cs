using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMGR : MonoBehaviour
{
    public MapMGR mapMGR;
    public InputMGR p1InputMGR;
    public InputMGR p2InputMGR;

    private void OnEnable()
    {
        EnableInit();
    }

    public void EnableInit()
    {
        Debug.Log($"BattleMGRのEnableInitを実行します");
        mapMGR.Init(0);
        p1InputMGR.Init(true);
        p2InputMGR.Init(false);
    }

}
