using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevMode : MonoBehaviour
{
    [SerializeField] Button btnDown = null;
    [SerializeField] Button btnUp = null;
    [SerializeField] Button btnGo = null;
    [SerializeField] Button btnWin = null;
    [SerializeField] InputField inputFieldLevel = null;


    public System.Action OnGo;
    public System.Action OnWin;
    void Awake()
    {
        btnDown.onClick.AddListener(onDown);
        btnUp.onClick.AddListener(onUp);
        btnWin.onClick.AddListener(()=> { OnWin?.Invoke(); });
        btnGo.onClick.AddListener(()=> {
            OnGo?.Invoke();
        });
    }

    public void SetLevel(int level)
    {
        inputFieldLevel.text = level.ToString();
    }
    public int GetLevel()
    {
        return inputFieldLevel.text.ToInt();
    }
    private void onDown()
    {
        int level = inputFieldLevel.text.ToInt();
        level--;
        inputFieldLevel.text = level.ToString();
    }
    private void onUp()
    {
        int level = inputFieldLevel.text.ToInt();
        level++;
        inputFieldLevel.text = level.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
