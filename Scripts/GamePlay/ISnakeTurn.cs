using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISnakeTurn
{
    public void EarlyTurn();
    public void ActiveTurn();
    public Transform GetTransform();
}
