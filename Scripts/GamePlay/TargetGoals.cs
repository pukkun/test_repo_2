using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class TargetGoals : MonoBehaviour
{
#if UNITY_EDITOR
    public List<ColorArray> GridColors;
    public List<Color> ListRootColor = new List<Color>();
    public List<BlockColor> ListRootBlockColor = new List<BlockColor>();
#endif
    public List<Color> ListTargetColor = new List<Color>();
    public List<BlockColor> ListTargetBlockColor = new List<BlockColor>();
    public int Width;
    public int Height;
    public int Layer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
