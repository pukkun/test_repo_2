using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private System.Action cbLoadSceneComplete;
    bool unloading = false;
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.sceneUnloaded += onSceneUnLoaded;
    }
    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.LogError("onSceneLoaded " + scene.name);
        if (scene.name == "GameScene")
        {
            
        }else if (scene.name == SceneConstant.SCENE_HOME)
        {
            this.Wait(0.5f, () =>
            {
                LoadingController.Instance.ShowCompleteFirstLoader(()=> {
                    LoadScene(SceneConstant.SCENE_GAME, LoadSceneMode.Single, ()=> {
                        LoadingController.Instance.HideMainSlash(false);
                        LoadingController.Instance.HideWaiting();
                    });
                });
            });
            //SceneController.Instance.LoadScene(SceneConstant.SCENE_GAME);
        }
        cbLoadSceneComplete?.Invoke();
    }
    private void onSceneUnLoaded(Scene scene)
    {
        Debug.LogError("onSceneUnLoaded " + scene.name);
    }
    public void LoadScene(string name, LoadSceneMode loadMode = LoadSceneMode.Single, System.Action cbLoadSceneComplete = null)
    {
        this.cbLoadSceneComplete = cbLoadSceneComplete;
        Debug.LogError("call load scene " + name);
        StartCoroutine(loadScene(name, loadMode));
    }
    private IEnumerator loadScene(string name, LoadSceneMode loadMode = LoadSceneMode.Single)
    {

        AsyncOperation operationMainScene = SceneManager.LoadSceneAsync(name, loadMode);
        while (!operationMainScene.isDone)
        {
            yield return null;
        }
        operationMainScene.allowSceneActivation = true;
        yield return null;
    }
    public void UnloadSceneLevel()
    {
        int count = SceneManager.sceneCount - 1;
        while (true)
        {
            Scene activeScene = SceneManager.GetSceneAt(count);
            if (activeScene != null)
            {
                if(activeScene.name.Contains("Level_") || activeScene.name.Contains("Event_"))
                {
                    SceneManager.UnloadSceneAsync(activeScene);
                }
            }
            count--;
            if (count == 0) break;
        }
        
    }
    public void UnloadScene(string name, System.Action onComplete)
    {
        if (unloading) return;
        Scene mainScene = SceneManager.GetSceneByName(name);
        if (mainScene == null) return;
        unloading = true;
        Debug.LogError("call UnloadScene " + name);
        StartCoroutine(unLoadScene(name, onComplete));
    }
    private IEnumerator unLoadScene(string name, System.Action onComplete)
    {
        //LoadingController.Instance.ShowWaiting();
        AsyncOperation operationMainScene = SceneManager.UnloadSceneAsync(name);
        yield return operationMainScene;
        while (!operationMainScene.isDone)
        {
            yield return null;
        }
        unloading = false;
        this.Wait(0.3f, () => { onComplete?.Invoke(); });
    }
    public Scene GetSceneAt(int index)
    {
        return SceneManager.GetSceneAt(index);
    }
}
