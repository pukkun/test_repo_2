using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LocalizedKey : MonoBehaviour , ILocalized
{

    [SerializeField]
    private string key;

    private string[] options;
    //private string[] values;

    [SerializeField] private Enums.FormatLocalized formatLocalized;

    [SerializeField] private Sprite[] sprites;

    private Text_ML text_ML;
    Vector3 position;

    private int _fontSize = -1;//default

    private void Awake()
    {
        position = transform.localPosition;
        position.z = 0;
        transform.localPosition = position;
        text_ML = GetComponent<Text_ML>();
        if (_fontSize != -1)
        {
            SetSize(_fontSize);
        }
        initText();
    }
    private void initText()
    {
        showLayoutText();
        LocalizedManager.Instance?.Add(this);
    }

    public void ChangeStyle(Enums.FormatLocalized formatLocalized)
    {
        this.formatLocalized = formatLocalized;
    }

    public void ChangeKey(string newKey, params string[] options)
    {
        key = newKey;
        this.options = options;
        showLayoutText();
    }

    public void SetSize(int newSize)
    {
        _fontSize = newSize;
        if (text_ML != null)
        {
            text_ML.fontSize = newSize;
        }
    }

    //public void UpdateValue(string newKey, params string[] values)
    //{
    //    key = newKey;
    //    this.values = values;
    //    showLayoutText();
    //}


    private void showLayoutText()
    {
        if (options.IsNullOrEmpty())
        {
            if (text_ML != null)
            {
                if (key.IsNullOrEmpty())
                {
                    
                }
                else
                {
                    text_ML.WrapperSetText(LanguageHelper.GetTextByKey(key, formatLocalized));
                }
            }
        }
        else
        {
           
            if (text_ML != null)
            {
                if (key.IsNullOrEmpty())
                {

                }
                else
                {
                    // text_ML.text = string.Format(LanguageHelper.GetTextByKey(key, formatLocalized), options);
                    text_ML.WrapperSetText(string.Format(LanguageHelper.GetTextByKey(key, formatLocalized), options));
                }
                    
            }
        }
        if (!sprites.IsNullOrEmpty())
        {
            //string textValue = text_ML.text;
            //string tag = "<sprite>";
            //while (true)
            //{
            //    int indexBegin = textValue.SmartIndexOf(tag);
            //    if (indexBegin == -1) break;
            //    int indexEnd = textValue.SmartIndexOf("</sprite>");
            //    string key = textValue.Substring(indexBegin + tag.Length, indexEnd - indexBegin - tag.Length);
            //    TextGenerator textGen = text_ML.cachedTextGenerator;
            //}
            //IEnumerable<int> listBegin = textValue.IndexOfAll(tag);
            //IEnumerable<int> listEnd = textValue.IndexOfAll("</sprite>");
            //int[] arrayBegin = listBegin.Cast<int>().ToArray();
            //int[] arrayEnd = listEnd.Cast<int>().ToArray();
            //for (int i = 0; i < arrayBegin.Length; i++)
            //{
            //    int indexBegin = arrayBegin[i];
            //    int indexEnd = arrayEnd[i];
            //    string key = textValue.Substring(indexBegin + tag.Length, indexEnd - indexBegin - tag.Length);
            //    print("key " + key);
            //    TextGenerator textGen = text_ML.cachedTextGenerator;
            //}
        }
    }

    private void OnDestroy()
    {
        LocalizedManager.Instance?.Remove(this);
    }

    public void ShowTextChange()
    {
        showLayoutText();
    }

    public void UpdateLanguage()
    {
        ShowTextChange();
    }

    public Text_ML GetText
    {
        get { return text_ML; }
    }
    public string GetKey => key;

}
