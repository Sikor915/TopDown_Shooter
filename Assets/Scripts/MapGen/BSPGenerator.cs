using UnityEngine;

public class BSPGenerator
{
    public BSPNode GenerateBSP(RectInt space, int minRoomSize = 10)
    {
        BSPNode root = new BSPNode(space);
        SplitNode(root, minRoomSize);
        return root;
    }

    private void SplitNode(BSPNode node, int minRoomSize = 10)
    {
        if (node.rect.width < minRoomSize || node.rect.height < minRoomSize)
        {
            return;
        }

        bool splitH = Random.Range(0f, 1f) > 0.5f;
        int splitPoint = splitH ? Random.Range(minRoomSize, node.rect.width - minRoomSize) : Random.Range(minRoomSize, node.rect.height - minRoomSize);

        if (splitH)
        {
            node.left = new BSPNode(new RectInt(node.rect.x, node.rect.y, splitPoint, node.rect.height));
            node.right = new BSPNode(new RectInt(node.rect.x + splitPoint, node.rect.y, node.rect.width - splitPoint, node.rect.height));
        }
        else
        {
            node.left = new BSPNode(new RectInt(node.rect.x, node.rect.y, node.rect.width, splitPoint));
            node.right = new BSPNode(new RectInt(node.rect.x, node.rect.y + splitPoint, node.rect.width, node.rect.height - splitPoint));
        }

        SplitNode(node.left);
        SplitNode(node.right);
    }
}