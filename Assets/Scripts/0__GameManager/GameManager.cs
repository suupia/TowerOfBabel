using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //GameObject
    [SerializeField] private GameObject __Title;
    [SerializeField] private GameObject __Setup;
    [SerializeField] private GameObject __Battle;
    [SerializeField] private GameObject __Result;

    //スクリプト
    [SerializeField] private TitleMGR titleMGR;
    [SerializeField] private SetupMGR setupMGR;
    [SerializeField] private BattleMGR battleMGR;
    [SerializeField] private ResultMGR resultMGR;

    //ID
    public readonly int wallID;
    public readonly int groundID;
    public readonly int p1SpawnPoint;
    public readonly int p2SpawnPoint;
    public readonly int p1Tower;
    public readonly int p2Tower;
    public readonly int brickYard;



    public enum State
    {
        Title,
        SettingUp,
        Battling,
        Result
    }
    [SerializeField] private State _state; //デバッグ用
    public State state
    {
        get { return _state; }
    }

    private void Awake()
    {
        Singletonization();
    }

    public void Singletonization() //重複して呼んでも問題ない
    {
        //シングルトン化
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //デバッグ
        SetupState();
    }

    public void TitleState()
    {

    }

    public void SetupState()
    {
        Debug.Log($"_stateをSettingUpに移行します");
        __Setup.SetActive(true);
        _state = State.SettingUp;
    }

    public void BattlingState()
    {
        Debug.Log($"_stateをBattlingに移行します");

        __Battle.SetActive(true);
        _state = State.Battling;
    }

    public void ResultState()
    {
        Debug.Log($"_stateをResultに移行します");

        __Result.SetActive(true);
        _state = State.Result;
    }


}
