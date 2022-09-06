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
    [SerializeField] public TitleMGR titleMGR;
    [SerializeField] public SetupMGR setupMGR;
    [SerializeField] public BattleMGR battleMGR;
    [SerializeField] public ResultMGR resultMGR;

    [SerializeField] public int gameSpeed = 1;

    //ID
    public readonly int wallID_m1 = -1;
    public readonly int groundID_p1 = 1;
    public readonly int brickYardID = 5;
    public readonly int p1SpawnPointID = 11;
    public readonly int p2SpawnPointID = 13;
    public readonly int p1TowerID = 17;
    public readonly int p2TowerID = 19;
    public readonly int p1UnitID = 2;
    public readonly int p2UnitID = 3;




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
        BattleState();
        Test();
    }
    public void Test()
    {
        //string data = Resources.Load<TextAsset>(path + i).ToString(); //pathの例："JSON/posData" 
        //myMapDataArray[i] = JsonUtility.FromJson<MyMapJSONData>(data);

        //Debug.Log($"stageNum;{i}");
        //Debug.Log(string.Join(",", myMapDataArray[i].values));
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

    public void BattleState()
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
