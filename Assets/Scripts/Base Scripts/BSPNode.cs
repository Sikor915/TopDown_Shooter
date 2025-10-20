using UnityEngine;

public class BSPNode
{
    public RectInt rect;
    public BSPNode left;
    public BSPNode right;

    public BSPNode(RectInt rect)
    {
        this.rect = rect;
        left = null;
        right = null;
    }
}