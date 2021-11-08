using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public static bool battleOver;
    public int team;
    public float health;
    public Movement(int team, float health)
    {
        this.team = team;
        this.health = health;
    }
    void Start()
    {
        battleOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!battleOver)
        {
            if (team == 0)
            {
                if (Input.GetKeyDown("e"))
                {
                    health -= 20;
                }
                if (health <= 0)
                {
                    battleOver = true;
                    Chessboard.instance.winnerCombat = Chessboard.instance.BlackTeamFighter;
                    Chessboard.instance.loserCombat = Chessboard.instance.whiteTeamFighter;
                    Chessboard.instance.chessPieces[Chessboard.instance.winnerCombat.x, Chessboard.instance.winnerCombat.y].health = GameObject.Find("1 Team").GetComponent<Movement>().health;
                    print("white " + GameObject.Find("0 Team").GetComponent<Movement>().health);
                    print("black " + GameObject.Find("1 Team").GetComponent<Movement>().health);
                    StartCoroutine(sceneManager.instance.LoadChess());
                }
            }
            else if (team == 1)
            {
                if (Input.GetKeyDown("e"))
                {
                    health -= 10;
                }

                    if (health <= 0)
                    {
                        battleOver = true;
                        Chessboard.instance.winnerCombat = Chessboard.instance.whiteTeamFighter;
                        Chessboard.instance.loserCombat = Chessboard.instance.BlackTeamFighter;
                        Chessboard.instance.chessPieces[Chessboard.instance.winnerCombat.x, Chessboard.instance.winnerCombat.y].health = GameObject.Find("0 Team").GetComponent<Movement>().health;
                        print("white " + GameObject.Find("0 Team").GetComponent<Movement>().health);
                        print("black " + GameObject.Find("1 Team").GetComponent<Movement>().health);
                        StartCoroutine(sceneManager.instance.LoadChess());
                    }
                
            }
        }
    }
}
