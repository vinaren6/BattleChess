using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] GameObject whiteTeamSpawner;
    [SerializeField] GameObject blackTeamSpawner;
    [SerializeField] GameObject whiteFighter;
    [SerializeField] GameObject blackFighter;
    // Start is called before the first frame update
    void Awake()
    {
        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    private void spawn()
    {
       
         
       GameObject white = Instantiate(whiteFighter, whiteTeamSpawner.transform.position, Quaternion.identity);
        white.GetComponent<PlayerCombat>().health = Chessboard.instance.chessPieces[Chessboard.instance.whiteTeamFighter.x, Chessboard.instance.whiteTeamFighter.y].health;
        white.GetComponent<Movement>().team = 0;
        white.gameObject.name = "white Team";
        white.gameObject.name = white.GetComponent<Movement>().team + " Team";
        

        GameObject black = Instantiate(blackFighter, blackTeamSpawner.transform.position, Quaternion.identity);
        black.GetComponent<PlayerCombat>().health = Chessboard.instance.chessPieces[Chessboard.instance.BlackTeamFighter.x, Chessboard.instance.BlackTeamFighter.y].health;
        black.GetComponent<Movement>().team = 1;
        black.gameObject.name = black.GetComponent<Movement>().team + " Team";


    }
}
