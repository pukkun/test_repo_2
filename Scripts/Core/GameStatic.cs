using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatic
{
    public static Vector3 POS_LEVEL = new Vector3(0, -3.7f, 0);
    public static int MAX_LEVEL = 60;
    public static bool AllowVirate = true;
    public static Dictionary<string, string> CachePref = new Dictionary<string, string>();
    public static Dictionary<string, object> ConfigLevel;
    public static Dictionary<string, object> DataLinks;
    public static List<int> LIST_LEVEL_HARD;
}
