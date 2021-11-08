using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] GameObject whiteTeamSpawner;
    [SerializeField] GameObject blackTeamSpawner;
    [SerializeField] GameObject Fighter;
    // Start is called before the first frame update
    void Start()
    {
        //print("White team " + Chessboard.instance.whiteTeamFighter);
        //print("Black team " + Chessboard.instance.BlackTeamFighter);
        //print("startAttack " + Chessboard.instance.StartedFight);
       // print("EnPassant " + Chessboard.instance.EnPassantPosition);
        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void spawn()
    {
       
         
       GameObject white = Instantiate(Fighter, whiteTeamSpawner.transform.position, Quaternion.identity);
        white.GetComponent<Movement>().health = Chessboard.instance.chessPieces[Chessboard.instance.whiteTeamFighter.x, Chessboard.instance.whiteTeamFighter.y].health;
        white.GetComponent<Movement>().team = 0;
        white.gameObject.name = "white Team";
        white.gameObject.name = white.GetComponent<Movement>().team + " Team";

        GameObject black = Instantiate(Fighter, blackTeamSpawner.transform.position, Quaternion.identity);
        black.GetComponent<Movement>().health = Chessboard.instance.chessPieces[Chessboard.instance.BlackTeamFighter.x, Chessboard.instance.BlackTeamFighter.y].health;
        black.GetComponent<Movement>().team = 1;
        black.gameObject.name = black.GetComponent<Movement>().team + " Team";
        //Instantiate(Fighter, blackTeamSpawner.transform.position, Quaternion.identity);


    }
}
