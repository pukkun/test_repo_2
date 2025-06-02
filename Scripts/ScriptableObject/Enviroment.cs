using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enviroment", menuName = "ScriptableObjects/Enviroment", order = 1)]
public class Enviroment : ScriptableObject
{
    public Enums.BuildGame BuildGame;
    public int VersionAndroid;
    public int VersionIOS;

}
