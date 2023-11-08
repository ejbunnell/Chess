using UnityEngine;

public class Piece
{
    private GameObject piece;
    private char pieceType;
    private char pieceColor;
    private int pieceColorInt;

    public Piece(char pieceType, char pieceColor)
    {
        this.pieceType = pieceType;
        this.pieceColor = pieceColor;
        pieceColorInt = pieceColor == 'w' ? 0 : 1;
        piece = new GameObject($"{pieceType}.{pieceColor}");
        piece.AddComponent<SpriteRenderer>();
        piece.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(
            $"Sprites/{pieceType}.{pieceColor}");
        piece.transform.localScale += new Vector3(0.5f, 0.5f, 0f);
    }

    public Vector2Int[] GetLegalMoves(Vector4Int[] boardInfo)
    {
        var moves = pieceType switch
        {
            'p' => GetPawnMoves(boardInfo),
            'n' => GetKnightMoves(boardInfo),
            'b' => GetBishopMoves(boardInfo),
            'r' => GetRookMoves(boardInfo),
            'q' => GetQueenMoves(boardInfo),
            'k' => GetKingMoves(boardInfo),
            _ => null
        };

        return moves;
    }

    private Vector2Int[] GetPawnMoves(Vector4Int[] boardInfo)
    {
        var position = piece.transform.position;
        var currentPosition = new Vector2Int((int)position.x, (int)position.y);
        var file = currentPosition.x;
        var rank = currentPosition.y;
        var isDoubleMove = (pieceColor == 'w' && rank == 1) ||
                           (pieceColor == 'b' && rank == 6);
        var moves = GetNegativeArray(4);
        var direction = pieceColor == 'w' ? 1 : -1;

        var forwardOne = boardInfo[file + (7 - (rank + direction)) * 8];
        if (forwardOne.x == -1)
        {
            moves[0] = new Vector2Int(file, rank + direction);
            var forwardTwo = boardInfo[file + (7 - (rank + direction * 2)) * 8];
            if (forwardTwo.x == -1 && isDoubleMove)
                moves[1] = new Vector2Int(file, rank + direction * 2);
        }

        var enPassantTarget = boardInfo[66];
        if (file - 1 != -1)
        {
            var diagonalLeft = boardInfo[file - 1 + (7 - (rank + direction)) * 8];
            if ((diagonalLeft.x != -1 || (enPassantTarget.x == file - 1 && enPassantTarget.y == rank + direction)) && diagonalLeft.y != pieceColorInt)
                moves[2] = new Vector2Int(file - 1, rank + direction);
        }
        if (file + 1 != 8)
        {
            var diagonalRight = boardInfo[file + 1 + (7 - (rank + direction)) * 8];
            if ((diagonalRight.x != -1 || (enPassantTarget.x == file + 1 && enPassantTarget.y == rank + direction)) && diagonalRight.y != pieceColorInt)
                moves[3] = new Vector2Int(file + 1, rank + direction);
        }

        return moves;
    }

    private Vector2Int[] GetKnightMoves(Vector4Int[] boardInfo)
    {
        var position = piece.transform.position;
        var currentPosition = new Vector2Int((int)position.x, (int)position.y);
        var file = currentPosition.x;
        var rank = currentPosition.y;

        var moves = GetNegativeArray(8);
        var directions = new Vector2Int[] { new(1, 2), new (2, 1), new (2, -1), new (1, -2),
            new (-1, -2), new (-2, -1), new (-2, 1), new (-1, 2)};

        var index = 0;
        foreach (var direction in directions)
        {
            var move = currentPosition + direction;
            if (!IsMoveInBoard(move))
            {
                moves[index] = -Vector2Int.one;
                index++;
                continue;
            }
            var tile = boardInfo[file + direction.x + (7 - (rank + direction.y)) * 8];
            if (tile.y != pieceColorInt)
                moves[index] = move;
            else
                moves[index] = -Vector2Int.one;

            index++;
        }

        return moves;
    }

    private Vector2Int[] GetBishopMoves(Vector4Int[] boardInfo)
    {
        var position = piece.transform.position;
        var currentPosition = new Vector2Int((int)position.x, (int)position.y);
        var file = currentPosition.x;
        var rank = currentPosition.y;

        var moves = GetNegativeArray(13);
        var directions = new Vector2Int[] { new(-1, 1), new(1, 1), new(1, -1), new(-1, -1) };
        var index = 0;
        foreach (var direction in directions)
        {
            for (var i = 1; i < 8; i++)
            {
                var move = currentPosition + direction * i;
                if (!IsMoveInBoard(move)) break;
                var tile = boardInfo[file + direction.x * i + (7 - (rank + direction.y * i)) * 8];
                if (tile.x != -1)
                {
                    if (tile.y == pieceColorInt) break;

                    moves[index] = move;
                    index++;
                    break;
                }

                moves[index] = move;
                index++;
            }
        }

        return moves;
    }

    private Vector2Int[] GetRookMoves(Vector4Int[] boardInfo)
    {
        var position = piece.transform.position;
        var currentPosition = new Vector2Int((int)position.x, (int)position.y);
        var file = currentPosition.x;
        var rank = currentPosition.y;

        var moves = GetNegativeArray(14);
        var directions = new Vector2Int[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
        var index = 0;
        foreach (var direction in directions)
        {
            for (var i = 1; i < 8; i++)
            {
                var move = currentPosition + direction * i;
                if (!IsMoveInBoard(move)) break;
                var tile = boardInfo[file + direction.x * i + (7 - (rank + direction.y * i)) * 8];
                if (tile.x != -1)
                {
                    if (tile.y == pieceColorInt) break;

                    moves[index] = move;
                    index++;
                    break;
                }

                moves[index] = move;
                index++;
            }
        }

        return moves;
    }

    private Vector2Int[] GetQueenMoves(Vector4Int[] boardInfo)
    {
        var position = piece.transform.position;
        var currentPosition = new Vector2Int((int)position.x, (int)position.y);
        var file = currentPosition.x;
        var rank = currentPosition.y;

        var moves = GetNegativeArray(27);
        var directions = new Vector2Int[] { new(-1, 1), new(1, 1), new(1, -1), new(-1, -1),
            new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
        var index = 0;
        foreach (var direction in directions)
        {
            for (var i = 1; i < 8; i++)
            {
                var move = currentPosition + direction * i;
                if (!IsMoveInBoard(move)) break;
                var tile = boardInfo[file + direction.x * i + (7 - (rank + direction.y * i)) * 8];
                if (tile.x != -1)
                {
                    if (tile.y == pieceColorInt) break;

                    moves[index] = move;
                    index++;
                    break;
                }

                moves[index] = move;
                index++;
            }
        }

        return moves;
    }

    private Vector2Int[] GetKingMoves(Vector4Int[] boardInfo)
    {
        var position = piece.transform.position;
        var currentPosition = new Vector2Int((int)position.x, (int)position.y);
        var file = currentPosition.x;
        var rank = currentPosition.y;

        var moves = GetNegativeArray(8);
        var directions = new Vector2Int[] { new(-1, 1), new(1, 1), new(1, -1), new(-1, -1),
            new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };
        var index = 0;
        foreach (var direction in directions)
        {

            var move = currentPosition + direction;
            if (!IsMoveInBoard(move)) continue;
            var tile = boardInfo[file + direction.x + (7 - (rank + direction.y)) * 8];
            if (tile.x != -1)
            {
                if (tile.y == pieceColorInt) continue;

                moves[index] = move;
                index++;
                continue;
            }

            moves[index] = move;
            index++;

        }

        return moves;
    }

    private bool IsMoveInBoard(Vector2Int v)
    {
        return v.x is <= 7 and >= 0 && v.y is <= 7 and >= 0;
    }

    public GameObject[] HighlightLegalMoves(Vector2Int[] moves)
    {
        if (moves == null) return null;

        var circles = new GameObject[moves.Length];
        var index = 0;
        foreach (var move in moves)
        {
            if (move == -Vector2Int.one) continue;

            var circle = new GameObject("legalMoveCircle");
            circle.transform.position = new Vector3(move.x, move.y, -0.1f);
            circle.transform.localScale -= new Vector3(0.7f, 0.7f, 0f);
            var sPr = circle.AddComponent<SpriteRenderer>();
            sPr.sprite = Resources.Load<Sprite>("Sprites/Circle");
            sPr.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            circles[index] = circle;
            index++;
        }

        return circles;
    }

    public void SetCurrentPosition(Vector2Int position)
    {
        piece.transform.position = (Vector2) position;
    }

    public Vector2Int GetCurrentPosition()
    {
        var pos = piece.transform.position;
        return new Vector2Int((int)pos.x, (int)pos.y);
    }

    public char GetPieceType()
    {
        return pieceType;
    }

    public char GetPieceColor()
    {
        return pieceColor;
    }

    private static Vector2Int[] GetNegativeArray(int n)
    {
        var array = new Vector2Int[n];
        for (var i = 0; i < n; i++)
        {
            array[i] = -Vector2Int.one;
        }

        return array;
    }

    public void Destroy()
    {
        Object.Destroy(piece);
    }
}