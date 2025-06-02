using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [System.Serializable]
    public class IntArray
    {
        public int[] Widths;
    }
    [System.Serializable]
    public class CellArray
    {
        public Cell[] Widths;
    }
    [System.Serializable]
    public class CellDataArray
    {
        public CellData[] Widths;
    }
    public float SizeW;
    public float SizeH;
    //public IntArray[] Grids;
    //public CellArray[] CellGrids;
    public CellDataArray[] Grids;
    public System.Action OnChangeSize;
    private void Start()
    {
        //SpriteRenderer spr = GetComponent<SpriteRenderer>();
        //spr.sortingOrder = 4;
        //transform.localPosition = new Vector3(0, 0.02f, 0.15f);
        Transform border = transform.Find("Borders");
        if (border != null)
        {
            border.localPosition = new Vector3(0, 0, 0.15f);
        }
        transform.localPosition += Vector3.forward * 0.9f;
        initBorder();
        return;
        int minSize = 6;
        int maxSize = 8;
        
        SizeW = Grids[0].Widths.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE);
        SizeH = Grids.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE);
        float bigger = SizeH > SizeW ? SizeH : SizeW;
        float smaller = SizeH > SizeW ? SizeW : SizeH;
        float newFactor;
        if (SizeH > maxSize || SizeW > maxSize)
        {
            newFactor = maxSize / bigger;
            transform.parent.localScale = Vector3.one * newFactor;
        }
        else if (SizeH < minSize || SizeW < minSize)
        {
            newFactor = minSize / smaller;
            transform.parent.localScale = Vector3.one * newFactor;
        }
        Vector3 scale = transform.parent.localScale;
        SizeW = Grids[0].Widths.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE) * scale.x;
        SizeH = Grids.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE) * scale.y;
        OnChangeSize?.Invoke();
    }

    public void InitGrid(int w, int h)
    {
        Grids = new CellDataArray[h];
        for (int i = 0; i < h; i++)
        {
            Grids[i] = new CellDataArray();
        }
        foreach (var item in Grids)
        {
            item.Widths = new CellData[w];
        }
    }
    private void initBorder()
    {
        return;
        if (MainSO.Instance == null) return;
        GameObject go = Instantiate(MainSO.Instance.PrefabBorder, transform);
        Vector3 pos = Vector3.zero;
        pos.z = 0;
        go.transform.localPosition = pos;
    }
    public void Set(int x, int y, Enums.GridValue value)
    {
        try
        {
            Grids[y].Widths[x].GridValue = value;
        }
        catch (System.Exception)
        {
        }
        
    }
    public void SetCell(int x, int y, Cell cell)
    {
        try
        {
            Grids[y].Widths[x].Cell = cell;
        }
        catch (System.Exception)
        {
        }

    }
    public CellData Get(int x, int y)
    {
        try
        {
            return Grids[y].Widths[x];
        }
        catch (System.Exception)
        {
            return null;
        }
    }
    public Enums.GridValue GetGridValue(int x, int y)
    {
        try
        {
            return Grids[y].Widths[x].GridValue;
        }
        catch (System.Exception)
        {
            return 0;
        }
    }
    public Cell GetCell(int x, int y)
    {
        try
        {
            return Grids[y].Widths[x].Cell;
        }
        catch (System.Exception)
        {
            return null;
        }
    }
}
