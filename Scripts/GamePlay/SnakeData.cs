using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[System.Serializable]
public class SnakeData
{
    public int Id;
    public string name = "Snake";
    public int Turn;
    public BlockColor SelectedColor = BlockColor.None;
    public SnakeObtacle Obtacle = SnakeObtacle.None;
    public bool BomdDefused = false;
    public Color GetColor()
    {
        return GameUtils.GetColorFromOption(SelectedColor);
    }
    public List<Vector2Int> cells = new List<Vector2Int>();
    public List<Transform> segments = new List<Transform>();
    public Enums.Direction KeyDirection;

    public bool IsParing;
    public int PairingId;

#if UNITY_EDITOR
    public int CurrentIndexPair;
#endif
}
