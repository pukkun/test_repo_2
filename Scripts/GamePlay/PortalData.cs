using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[System.Serializable]
public class PortalData
{
    public int Id;
    public int Turn = 99;
    public int CoveredTurn = -1;//turn cua nap day
    public BlockColor SelectedColor = BlockColor.None;
    public Color GetColor()
    {
        return GameUtils.GetColorFromOption(SelectedColor);
    }
    public List<Vector2Int> Coord = new List<Vector2Int>();

}
