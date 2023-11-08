using System;
using TMPro;
using UnityEngine;


public class Controller : MonoBehaviour
{
    private Board board;
    private BoardMouseDetection[] boardMouseDetections;
    private Piece[] pieces;

    private Piece selectedPiece;
    //temp, create something for easier color changing
    public Color lightColor = new (229f / 255f, 202f / 255f, 202f / 255f, 1f);
    public Color darkColor = new (75f / 255f, 46f / 255f, 13f / 255f, 1f);

    public string fenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private FEN fen;
    private GameObject fenInput;

    private GameObject[] highlightedSquares;
    private Vector2Int[] possibleMoves;

    public char currentPlayer;
    public Vector2Int enPassantTarget;
    public int halfMoves;
    public int fullMoves;

    private bool whiteKingCastle;
    private bool whiteQueenCastle;
    private bool blackKingCastle;
    private bool blackQueenCastle;

    private void OnEnable()
    {
        board = gameObject.AddComponent<Board>();
        board.GenerateBoard();
        board.UpdateColor(lightColor, darkColor);

        pieces = new Piece[64];
        GeneratePieces();

        fen = new FEN();
        fenInput = GameObject.Find("FENInput");
        //fenInput.GetComponent<TMP_Text>().text;
    }

    private void Update()
    {
        MoveController();
    }

    private void MoveController()
    {
        var selectedTile = DetectMouseClick();

        if (Input.GetMouseButtonDown(0) && highlightedSquares != null)
        {
            foreach (var square in highlightedSquares)
            {
                if (square == null) break;
                var position = square.transform.position;
                var squarePos = new Vector2Int((int)position.x, (int)position.y);

                if (selectedTile != squarePos.x + (7 - squarePos.y) * 8) continue;

                MovePiece(squarePos);
                if (selectedPiece.GetPieceType() == 'p')
                    enPassantTarget = possibleMoves[1] == squarePos ? possibleMoves[0] : -Vector2Int.one;

                // if halfmove = 100, end game in tie
                if (currentPlayer == 'w')
                {
                    currentPlayer = 'b';
                }
                else
                {
                    currentPlayer = 'w';
                    fullMoves++;
                }

                break;
            }

            DeleteLegalMoves();
        }

        if (selectedTile == -1) return;

        selectedPiece = pieces[selectedTile];
        if (selectedPiece == null) return;

        if (selectedPiece.GetPieceColor() != currentPlayer) return;
        possibleMoves = selectedPiece.GetLegalMoves(GenerateFenVectors(pieces));
        foreach (var move in possibleMoves)
        {

        }
        highlightedSquares = selectedPiece.HighlightLegalMoves(possibleMoves);
    }

    private void GeneratePieces()
    {
        var boardInfo = fen.Read(fenString);
        for (var i = 0; i < 64; i++)
        {
            var tile = boardInfo[i];
            if (tile.x == -1) continue;

            CreatePiece(fen.PieceTypes[tile.x], tile.y == 0 ? 'w' : 'b', new Vector2Int(tile.z, tile.w));
        }

        currentPlayer = boardInfo[64] == Vector4Int.zero ? 'w' : 'b';
        enPassantTarget = boardInfo[66];
        halfMoves = boardInfo[67].x;
        fullMoves = boardInfo[67].y;

    }

    private void CreatePiece(char type, char color, Vector2Int position)
    {
        var index = position.x + (7 - position.y) * 8;
        pieces[index] = new Piece(type, color);
        pieces[index].SetCurrentPosition(position);
    }

    private void MovePiece(Vector2Int move)
    {
        if (selectedPiece == null) return;

        pieces[selectedPiece.GetCurrentPosition().x + (7 - selectedPiece.GetCurrentPosition().y) * 8] = null;
        selectedPiece.SetCurrentPosition(move);
        var moveIndex = move.x + (7 - move.y) * 8;
        if (pieces[moveIndex] != null)
        {
            pieces[moveIndex].Destroy();
            halfMoves = 0;
        }
        else if (selectedPiece.GetPieceType() == 'p')
        {
            halfMoves = 0;
        }
        else
        {
            halfMoves++;
        }

        if (selectedPiece.GetPieceType() == 'p' && move == enPassantTarget)
        {
            var enPassantMoveIndex = selectedPiece.GetPieceColor() == 'w' ? moveIndex + 8 : moveIndex - 8;
            pieces[enPassantMoveIndex].Destroy();
            pieces[enPassantMoveIndex] = null;
        }

        pieces[moveIndex] = selectedPiece;
    }

    private bool IsInCheck(Piece[] info)
    {
        var kingPos = -Vector2Int.one;
        foreach (var piece in info)
        {
            if (piece.GetPieceColor() == currentPlayer && piece.GetPieceType() == 'k')
                kingPos = piece.GetCurrentPosition();
        }

        foreach (var piece in info)
        {
            if (piece.GetPieceColor() == currentPlayer) continue;

            var fenVectors = GenerateFenVectors(info);
            foreach (var move in piece.GetLegalMoves(fenVectors))
            {
                if (move == kingPos) return true;
            }
        }

        return false;
    }

    private Vector4Int[] GenerateFenVectors(Piece[] info)
    {
        var boardInfo = new Vector4Int[68];
        var index = 0;
        foreach (var piece in info)
        {
            var pieceType = -1;
            var color = -1;
            if (piece != null)
            {
                pieceType = Array.IndexOf(fen.PieceTypes, FEN.Lowercase(piece.GetPieceType()));
                color = piece.GetPieceColor() == 'w' ? 0 : 1;
            }

            var x = index % 8;
            var y = (int)Mathf.Ceil((64 - index) / 8f - 1);
            boardInfo[index] = new Vector4Int(pieceType, color, x, y);
            index++;
        }
        
        boardInfo[64] = currentPlayer == 'w' ? Vector4Int.zero : Vector4Int.one;
        var rookIndex = Array.IndexOf(fen.PieceTypes, 'r');
        var kingIndex = Array.IndexOf(fen.PieceTypes, 'k');
        boardInfo[65] = new Vector4Int(boardInfo[0].x == rookIndex && boardInfo[4].x == kingIndex ? 1 : 0,
            boardInfo[7].x == rookIndex && boardInfo[4].x == kingIndex ? 1 : 0,
            boardInfo[56].x == rookIndex && boardInfo[60].x == kingIndex ? 1 : 0,
            boardInfo[63].x == rookIndex && boardInfo[60].x == kingIndex ? 1 : 0);
        if (enPassantTarget == -Vector2.one)
            boardInfo[66] = -Vector4Int.one;
        else
            boardInfo[66] = enPassantTarget;
        boardInfo[67] = new Vector4Int(halfMoves, fullMoves);

        return boardInfo;
    }

    private int DetectMouseClick()
    {
        if (boardMouseDetections == null)
        {
            boardMouseDetections = new BoardMouseDetection[64];
            var count = 0;
            foreach (var boardPiece in board.board)
            {
                boardMouseDetections[count] = boardPiece.GameObject.GetComponent<BoardMouseDetection>();
                count += 1;
            }
        }
        for (var boardNumber = 0; boardNumber < 63; boardNumber++)
        {
            var boardMouseDetection = boardMouseDetections[boardNumber];

            if (!boardMouseDetection.isClicked) continue;

            boardMouseDetection.Reset();
            return boardNumber;
        }

        return -1;
    }

    // Called by a button
    // ReSharper disable once UnusedMember.Global
    public void LoadFEN()
    {
        DeleteBoard();

        fenString = fenInput.GetComponent<TMP_Text>().text;
        GeneratePieces();
    }

    private void DeleteBoard()
    {
        foreach (var piece in pieces)
        {
            piece?.Destroy();
        }

        pieces = new Piece[64];
    }

    private void DeleteLegalMoves()
    {
        foreach (var highlightedSquare in highlightedSquares)
        {
            if (highlightedSquare == null) break;
            Destroy(highlightedSquare);
        }
    }
}
