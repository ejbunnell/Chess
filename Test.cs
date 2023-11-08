using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    private FEN fen;

    public string fenString;
    private string oldFenString;

    private void Awake()
    {
        fen = new FEN();
        fenString = null;
        oldFenString = null;
    }

    private void Update()
    {
        if (fenString == oldFenString || fenString is "" or null) return;
        var test = fen.Read(fenString);
        // foreach (var v in test)
        // {
        //     Debug.Log(v);
        // }
        var testFen = fen.Construct(test);
        if (fenString != testFen)
        {
            throw new Exception("FEN strings are not the same");
        }

        Debug.Log(fenString);
        oldFenString = fenString;
    }
}
