using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EyeAnimations : MonoBehaviour
{
    [SerializeField] private Transform transformLook = null;
    [SerializeField] private Transform[] eyes = null;
    [SerializeField] private Animator animator = null;


    float moveSpeed = 0.01f;

    Vector3 minLocalPosition = new Vector3(-0.0003f, -0.00118f);
    Vector3 maxLocalPosition = new Vector3(0.00024f, 0f);

    public float minIdleTime = 2f;
    public float maxIdleTime = 7;
    private float timer;
    string[] animations = new string[] {"EyesIdle"};// , "EyesIdle_3" , "EyesIdle_2"

    bool looking;
    Vector3 lookAtPosition;
    void Start()
    {
        ResetTimer();
    }
    void ResetTimer()
    {
        timer = GameUtils.RandomRange(minIdleTime, maxIdleTime);
    }
    // Update is called once per frame
    void Update()
    {
        
        if (transformLook != null)
        {
            foreach (var item in eyes)
            {

                float z = item.transform.localPosition.z;
                // Convert target's world position to local space
                Vector3 targetLocalPos = item.transform.parent.InverseTransformPoint(transformLook.position);

                // Move toward the target in local space
                Vector3 newLocalPosition = Vector3.MoveTowards(item.transform.localPosition, targetLocalPos, moveSpeed * Time.deltaTime);

                // Clamp movement within local space limits
                newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, minLocalPosition.x, maxLocalPosition.x);
                newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, minLocalPosition.y, maxLocalPosition.y);
                newLocalPosition.z = z;


                // Apply limited local position
                item.transform.localPosition = newLocalPosition;
            }


        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                string anim = animations[GameUtils.RandomRange(0, animations.Length - 1)];
                //animator.enabled = false;
                animator.enabled = true;
                animator.Play(anim);
                ResetTimer();
            }
        }

        if (looking)
        {
            foreach (var item in eyes)
            {

                float z = item.transform.localPosition.z;
                // Convert target's world position to local space
                Vector3 targetLocalPos = item.transform.parent.InverseTransformPoint(lookAtPosition);

                // Move toward the target in local space
                Vector3 newLocalPosition = Vector3.MoveTowards(item.transform.localPosition, targetLocalPos, moveSpeed * Time.deltaTime);

                // Clamp movement within local space limits
                newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, minLocalPosition.x, maxLocalPosition.x);
                newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, minLocalPosition.y, maxLocalPosition.y);
                newLocalPosition.z = z;


                // Apply limited local position
                item.transform.localPosition = newLocalPosition;
            }


        }
    }
    public void LookPosition(Transform transformLook)
    {
        animator.enabled = false;
        this.transformLook = transformLook;
        DOVirtual.DelayedCall(1.2f, returnNormal);
    }
    public void LookPosition(Vector3 pos)
    {
        lookAtPosition = pos;
        looking = true;
        DOVirtual.DelayedCall(1.2f, returnNormal);
    }
    private void returnNormal()
    {
        transformLook = null;
        looking = false;
        foreach (var item in eyes)
        {
            item.transform.DOLocalMove(Vector2.zero, 0.2f);
        }
    }

    public void ForceRunStun()
    {
        ResetTimer();
        animator.Play("EyesIdle_3");
    }
    public void ForceRunStun2()
    {
        ResetTimer();
        timer = maxIdleTime;
        animator.enabled = true;
        animator.Play("EyesIdle_3");
    }
    public void ForceNormal()
    {
        animator.Play("EyesIdle");
    }
}
