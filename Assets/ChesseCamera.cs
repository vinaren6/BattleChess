using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChesseCamera : MonoBehaviour
{
    private Chessboard chesseboard;
    
    bool isWhiteTurn;

    // Start is called before the first frame update
    void Start()
    {
        chesseboard = GameObject.Find("ChessBoard").GetComponent<Chessboard>();
        isWhiteTurn = chesseboard.isWhiteTurn;
        if (isWhiteTurn)
        {
            transform.position = new Vector3(0f, 5f, -3.56f);
            transform.rotation = Quaternion.Euler(new Vector3(58f, 0, 0));
        }
        else
        {
            transform.position = new Vector3(0f, 5f, 3.56f);
            transform.rotation = Quaternion.Euler(new Vector3(121.7f, 0, 180));
        }
    }

    // Update is called once per frame
    void Update()
    {
        isWhiteTurn = chesseboard.isWhiteTurn;
        if (isWhiteTurn)
        {
            transform.position = new Vector3(0f, 5f, -3.56f);
            transform.rotation = Quaternion.Euler(new Vector3(58f, 0, 0));
        }
        else
        {
            transform.position = new Vector3(0f, 5f, 3.56f);
            transform.rotation = Quaternion.Euler(new Vector3(121.7f, 0, 180));
        }
    }
}
