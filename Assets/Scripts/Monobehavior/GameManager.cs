using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject __Title;
    [SerializeField] private GameObject __SettingUp;
    [SerializeField] private GameObject __Battling;
    [SerializeField] private GameObject __Result;

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
        SettingUpState();
    }

    public void TitleState()
    {

    }

    public void SettingUpState()
    {
        Debug.Log($"_stateをSettingUpに移行します");
        __SettingUp.SetActive(true);
        _state = State.SettingUp;
    }

    public void BattlingState()
    {
        Debug.Log($"_stateをBattlingに移行します");

        __Battling.SetActive(true);
        _state = State.Battling;
    }

    public void ResultState()
    {
        Debug.Log($"_stateをResultに移行します");

        __Result.SetActive(true);
        _state = State.Result;
    }


}
