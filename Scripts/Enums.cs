using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    public enum TypeLoseGame
    {
        None,
        TimeUp,
        NoSpace,
        Bomb
    }
    public enum UserInfoElement
    {
        Gold
    }
    public enum BuildGame
    {
        Dev,
        Production
    }
    public enum FormatLocalized
    {
        None,
        UpperFirst,
        UpperWord,
        UpperAll
    }
    public enum Direction
    {
        None, Up, Down, Left, Right
    }

    public enum SnakeObtacle
    {
        None,
        Xich,
        Ken,
        Bomb
    }
    [System.Serializable]
    public enum BlockColor
    {
        None,
        Blue, // #236BFE
        Red, // #FE2639
        Green, // #3EE350
        Yellow, // #FEE32B
        Orange, // #FFA800
        Purple,  // #D03CFE
        Pink,// #FC67DF
        DarkGreen, // #1E6624
        Cyan, // #2DC2ED
        Brown_Removed, // 82553B
        DarkRed // 400114
    }
    public enum GridValue
    {
        Wrong = -1,
        None = 0,
        Available = 1,
        Portal = 100,
        Removed = 500
    }
    public enum BorderType
    {
        None,
        End,
        Straight,
        Corner,
        U,
        Single,
        CornerPoint,
        CornerPointIn,
        StraightML,
        StraightMR,
        StraightMLR,
        DiagonalJoined
    }
}
