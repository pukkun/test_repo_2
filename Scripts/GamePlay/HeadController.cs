using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeadController : MonoBehaviour
{
    [SerializeField] private Transform transformHead = null;
    [SerializeField] private Transform transformMount = null;
    [SerializeField] private Transform transformMountTarget = null;
    [SerializeField] private EyeAnimations eyeAnimations = null;

    private IEnumerator enumeratorDelayHide;
    public Transform TransformMount => transformMount;
    public Transform TransformMountTarget => transformMountTarget;
    public Transform TransformHead => transformHead;

    private Transform transformStun;

    static WaitForSeconds wait = new WaitForSeconds(0.8f);
    private void Awake()
    {
        if (transformMountTarget == null) transformMountTarget = transformMount.Find("mieng/Target");
    }
    public void Init(GameDataSO gameDataSO, Enums.BlockColor blockColor)
    {
        Transform mieng = TransformMount.transform.Find("mieng");
        if (mieng != null)
        {
            mieng.GetComponent<Renderer>().material = gameDataSO.GetMaterial(blockColor);
        }
    }
    public void HideMount()
    {
        TransformMount.gameObject.SetActive(false);
    }
    public void ShowMount()
    {
        StartCoroutine(delayShow());
        if (enumeratorDelayHide != null) StopCoroutine(enumeratorDelayHide);
        enumeratorDelayHide = delayHide();
        StartCoroutine(enumeratorDelayHide);
    }
    private IEnumerator delayShow()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        TransformMount.gameObject.SetActive(true);
    }
    private IEnumerator delayHide()
    {
        yield return wait;
        HideMount();
        transform.DORotate(new Vector3(0, 0, 180), 0.2f);
    }

    public void ShowStun()
    {
        if(transformStun == null) transformStun = eyeAnimations.transform.Find("FxStun");
        transformStun.gameObject.SetActive(true);
        eyeAnimations.ForceRunStun2();
        this.Wait(3, () =>
        {
            eyeAnimations.ForceNormal();
            transformStun.gameObject.SetActive(false);
        });
    }
}
