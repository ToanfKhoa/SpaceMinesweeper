using UnityEngine;

public class Cell
{
    public enum Type
    {
        Empty,
        Mine,
        Number,
        NumEmpty,
        Block,
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool isRevealed;
    public bool isFlagged;
    public bool isExploded;
    public bool isNumberEmpty;
    public bool isBlock;
}
