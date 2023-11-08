using UnityEngine;
using System;

// ReSharper disable once InconsistentNaming


public class FEN
{
    public readonly char[] PieceTypes = { 'p', 'n', 'b', 'r', 'q', 'k' };

    public Vector4Int[] Read(string fen)
    {

        var info = new Vector4Int[68];
        var seperatedFen = fen.Split(' ');
        var boardDetails = seperatedFen[0].Split('/');

        var index = 0;

        foreach (var rank in boardDetails)
        {
            foreach (var type in rank)
            {
                // 48 - 57 ASCII is 0-9
                if (type <= 57)
                {
                    for (var i = 0; i < type - 48; i++)
                    {
                        var x = (index + i) % 8 + 1;
                        var y = Mathf.Ceil((64 - (index + i)) / 8f);
                        info[index + i] = new Vector4Int(-1, -1, x, (int)y);
                    }
                    index += type - 48;
                }
                else
                {
                    // b k n p q r
                    // bishop, king, knight, pawn, queen, rook
                    var pieceType = Array.IndexOf(PieceTypes, Lowercase(type));
                    var color = IsCapitalized(type) ? 0 : 1;
                    var x = index % 8;
                    var y = Mathf.Ceil((64 - index) / 8f - 1);
                    info[index] = new Vector4Int(pieceType, color, x, (int)y);
                    index++;
                }
            }
        }
        // color to move
        info[64] = seperatedFen[1] == "w" ? Vector4Int.zero : Vector4Int.one;
        // castling rights
        info[65] = new Vector4Int(seperatedFen[2].Contains("K") ? 1 : 0, seperatedFen[2].Contains("Q") ? 1 : 0,
            seperatedFen[2].Contains("k") ? 1 : 0, seperatedFen[2].Contains("q") ? 1 : 0);
        // en passant target
        if (seperatedFen[3][0] == '-')
            info[66] = -Vector4Int.one;
        else
            info[66] = new Vector4Int(seperatedFen[3][0] - 98, seperatedFen[3][1] - 48);
        // half moves / full moves
        info[67] = new Vector4Int(seperatedFen[4][0] - 48, seperatedFen[5][0] - 48);

        return info;
    }

    public string Construct(Vector4Int[] board)
    {
        var fenString = "";

        var emptyCount = 0;
        var index = 0;
        var previousW = 8;
        foreach (var tile in board)
        {
            switch (index)
            {
                case 64:
                    fenString += tile == Vector4Int.zero ? " w " : " b ";
                    index++;
                    continue;
                case 65:
                    fenString += tile.x == 1 ? "K" : "";
                    fenString += tile.y == 1 ? "Q" : "";
                    fenString += tile.z == 1 ? "k" : "";
                    fenString += tile.w == 1 ? "q " : " ";
                    index++;
                    continue;
                case 66:
                    if (tile == Vector4Int.zero)
                        fenString += "-";
                    else
                    {
                        fenString += (char)(tile.x + 98);
                        fenString += tile.y;
                    }
                    fenString += " ";
                    index++;
                    continue;
                case 67:
                    fenString += tile.x;
                    fenString += " ";
                    fenString += tile.y;
                    index++;
                    continue;
            }

            var newRank = tile.w != previousW;
            if (tile.x == -1 && tile.y == -1)
            {
                if (!newRank)
                    emptyCount++;
                else
                {
                    if (emptyCount > 0)
                        fenString += $"{emptyCount}/";
                    else
                        fenString += "/";
                    emptyCount = 1;
                }
            }
            else
            {
                if (emptyCount > 0)
                {
                    fenString += $"{emptyCount}";
                    emptyCount = 0;
                }

                if (newRank)
                    fenString += "/";

                var pieceType = PieceTypes[tile.x];
                fenString += tile.y == 0 ? Capitalize(pieceType) : pieceType;
            }
            previousW = tile.w;
            index++;
        }

        return fenString;
    }

    public static bool IsCapitalized(char c)
    {
        return c >= 65 && c <= 90;
    }

    public static char Capitalize(char c)
    {
        if (IsLowercase(c))
            return (char)(c - 32);
        return c;
    }

    public static bool IsLowercase(char c)
    {
        return c >= 97 && c <= 122;
    }

    public static char Lowercase(char c)
    {
        if (IsCapitalized(c))
            return (char)(c + 32);
        return c;
    }
}