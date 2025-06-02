using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnviroment : Singleton<GameEnviroment>
{
    public GameDataSO GameDataSO;
    public Enviroment Enviroment;
#if UNITY_EDITOR
    public LocalEditorSO LocalEditorData;
    [Range(0, 5)]
    public float timeScale = 1;
#endif
    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        DOTween.Init().SetCapacity(500, 500);
        Vibration.Init();
    }
    private void Start()
    {
        GameUtils.InitOffLineData();
        UserInfo.InitResource();
    }
    private void Update()
    {
#if UNITY_EDITOR
        Time.timeScale = timeScale;
#endif
    }
}
