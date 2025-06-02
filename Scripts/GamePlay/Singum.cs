using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singum : MonoBehaviour
{
    public List<Snake> ListSnakePairing = new List<Snake>();//0 = main, 1 = pair
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
}
