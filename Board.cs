using UnityEngine;

public struct BoardPiece
{
    public GameObject GameObject;
    public BoardPiece(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}

public class Board : MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    public BoardPiece[] board;

    public void GenerateBoard()
    {
        board = new BoardPiece[64];
        for (var rank = 0; rank < 8; rank++)
        {
            for (var file = 0; file < 8; file++)
            {
                var tile = new GameObject($"{file + rank * 8}");
                tile.transform.parent = transform;
                tile.AddComponent<SpriteRenderer>();
                tile.AddComponent<BoxCollider2D>();
                tile.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
                tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(
                    "Sprites/Square");
                tile.AddComponent<BoardMouseDetection>();
                tile.transform.position = new Vector3(file, 7 - rank, 0.1f);
                board[file + rank * 8] = new BoardPiece(tile);
            }
        }
    }

    public void UpdateColor(Color lightColor, Color darkColor)
    {
        var tileIndex = 1;
        foreach (var tile in board)
        {
            if (Mathf.Ceil(tileIndex / 8f) % 2 == 0)
            {
                tile.GameObject.GetComponent<SpriteRenderer>().color = tileIndex % 2 == 0 ? darkColor : lightColor;
            }
            else
            {
                tile.GameObject.GetComponent<SpriteRenderer>().color = tileIndex % 2 == 0 ? lightColor : darkColor;
            }

            tileIndex++;
        }
    }
}

