using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
public class StartupScreen : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //GameStatic.StartFromInitScene = true;
        LoadingController.Instance.InitText();
        LoadingController.Instance.ShowMainSlash();
        LoadingController.Instance.UpdateProgress(0);
        yield return new WaitForSeconds(1.5f);
        float delay = 1;
        DOTween.To(_x =>
        {
            LoadingController.Instance.UpdateProgress((int)_x);
        }, 0, 80, delay);
        yield return new WaitForSeconds(delay);
        //yield return new WaitUntil(() => GameStatic.POPUP_CONSENT_COMPLETE == true);
        //Debug.LogError("GameStatic.ALLOW_CONSENT "+GameStatic.ALLOW_CONSENT);
        LoadingController.Instance.UpdateProgress(100);
        //Debug.LogError("start call load home scene ");
        int remain = 20;
        AsyncOperation operationMainScene = SceneManager.LoadSceneAsync(SceneConstant.SCENE_HOME, LoadSceneMode.Single);
        int lastPercent = 0;
        while (!operationMainScene.isDone)
        {
            yield return null;
        }
        LoadingController.Instance.UpdateProgress(100);
        operationMainScene.allowSceneActivation = true;
        yield return null;
        LoadingController.Instance.UpdateProgress(100);
        Debug.LogError("startup screen after call load home ");
        
    }

}
