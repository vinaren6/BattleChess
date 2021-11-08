using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class Chessboard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private float dragOffset = 0.1f;
    [SerializeField] private GameObject victoryScreen;
    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teaamMaterials;
    
    //Logic
    public ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isWhiteTurn;

    public Vector2Int whiteTeamFighter;
    public Vector2Int BlackTeamFighter;
    public Vector2Int StartedFight;
    public Vector2Int EnPassantPosition ;
    public Vector2Int loserCombat;
    public Vector2Int winnerCombat;
    public bool attack = false;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    private SpecialMove specialMove;

    public static Chessboard instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        isWhiteTurn = true;
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }
    private void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
        
            //Get index of tile hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            //If we hovering a tile after not hover any tile
            if(currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            //If we hovering a tile, change previous tile 
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsVailidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            //if mouse pressed down
            if (Input.GetMouseButtonDown(0))
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    // is our turn
                    if ((chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn))
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];
                        //Get list of moves
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        // special move list
                        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);
                        HiglightTiles();
                    }
                }
            }
            //realeasing mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
         
                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if (!validMove)
                {
                    print(specialMove);
                    if (specialMove == SpecialMove.None)
                    {
                        currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));  
                    }
                        
 
                }
                currentlyDragging = null;
                RemoveHiglightTiles();
                if (attack)
                {
                    print("loadBatlle");
                    StartCoroutine(sceneManager.instance.LoadBattle());
                }
                
                //print("White team " + whiteTeamFighter);
                //print("Black team " + BlackTeamFighter);
                //print("startAttack " + StartedFight);
                //print("EnPassant " + EnPassantPosition);
            }
        }
        else
        {

            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsVailidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHiglightTiles();
                
            }
        }
        // if dragging peice
        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset );
            float distance = 0.2f;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
            }
        }
        
    }

    

    //Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (TILE_COUNT_Y / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    //Spawn pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0;
        int blackTeam = 1;

        // White team
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam, 100f);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam, 100f);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam, 100f);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam, 100f);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam, 100f);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam, 100f);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam, 100f);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam, 100f);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam, 100f);
        }

        // Black team
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam, 100f);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam, 100f);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam, 100f);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam, 100f);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam, 100f);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam, 100f);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam, 100f);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam, 100f);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam, 100f);
        }

    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team, float health)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.health = health;
        cp.type = type;
        cp.team = team;
        int material;
        if (cp.team == 0)
        {
            material = 0;
        }
        else
        {
            material = 6;
        }
        cp.GetComponent<MeshRenderer>().material = teaamMaterials[material + (int)type - 1];

        return cp;
    }

    //Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x,y] != null)
                {
                    PositionSinglePiece(x, y, true);
                }
            }
        }
    }
    private void PositionSinglePiece(int x, int y, bool force = false )
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    //Places the piece on the center on the tile and the models for queen and rook is a litell diffrent in height
    private Vector3 GetTileCenter(int x, int y)
    {
      
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    //Highlight TIles
    private void HiglightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHiglightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }

    //Checkmate
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    public void OnResetButton()
    {
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        currentlyDragging = null;
        availableMoves.Clear();
        moveList.Clear();

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    Destroy(chessPieces[x, y].gameObject);
                }
                chessPieces[x, y] = null;
            }
        }
        for (int i = 0; i < deadWhites.Count; i++)
        {
            Destroy(deadWhites[i].gameObject);
        }
        for (int i = 0; i < deadBlacks.Count; i++)
        {
            Destroy(deadBlacks[i].gameObject);
        }
        deadWhites.Clear();
        deadBlacks.Clear();
        RemoveHiglightTiles();
        SpawnAllPieces();
        PositionAllPieces();
        isWhiteTurn = true;
    }
    public void OnExitButton()
    {
        Application.Quit();
    }

    //Special Moves
    private void ProcessSpecialMove()
    {
        if (specialMove != SpecialMove.EnPassant)
        {
            EnPassantPosition = new Vector2Int(0, 0);
        }

        if (specialMove == SpecialMove.EnPassant)
        {
            Vector2Int[] newMove = moveList[moveList.Count - 1];
            ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
            Vector2Int[] targetPawnPosition = moveList[moveList.Count - 2];

            
            ChessPiece enemyPawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

            EnPassantPosition = targetPawnPosition[1];
            if (myPawn.currentX == enemyPawn.currentX)
            {
                if (myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1)
                {
                    if (enemyPawn.team == 0)
                    {
                        print("test");
                        int direction = 1;
                        whiteTeamFighter = new Vector2Int(enemyPawn.currentX, enemyPawn.currentY);
                        BlackTeamFighter = new Vector2Int(myPawn.currentX, myPawn.currentY);
                        // deadWhites.Add(enemyPawn);
                        // enemyPawn.SetScale(Vector3.one * deathSize);
                        //enemyPawn.SetPosition(new Vector3(8 * tileSize, yOffset, (-1 * tileSize) + tileSize / 3) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.forward * deathSpacing) * deadWhites.Count);
                    }
                    else
                    {
                        int direction = -1;
                        BlackTeamFighter = new Vector2Int(enemyPawn.currentX, enemyPawn.currentY);
                        whiteTeamFighter = new Vector2Int(myPawn.currentX, myPawn.currentY);
                       // deadBlacks.Add(enemyPawn);
                        //enemyPawn.SetScale(Vector3.one * deathSize);
                        //enemyPawn.SetPosition(new Vector3(-1 * tileSize, yOffset, (8 * tileSize) - tileSize / 3) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.back * deathSpacing) * deadBlacks.Count);
                    }
                    //chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
                }
            }
            attack = true;

            

        }

        if (specialMove == SpecialMove.Promotion)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

            if (targetPawn.type == ChessPieceType.Pawn)
            {
                if (targetPawn.team == 0 && lastMove[1].y == 7)
                {
                    ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 0, targetPawn.health);
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);
                }
                if (targetPawn.team == 1 && lastMove[1].y == 0)
                {
                    ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 1, targetPawn.health);
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);
                }
            }
        }

        if (specialMove == SpecialMove.Castling)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            //Left
            if (lastMove[1].x == 2)
            {
                if (lastMove[1].y == 0) //White
                {
                    ChessPiece rook = chessPieces[0, 0];
                    chessPieces[3, 0] = rook;
                    PositionSinglePiece(3, 0);
                    chessPieces[0, 0]  = null;
                }
                else if(lastMove[1].y == 7) //Black
                {
                    ChessPiece rook = chessPieces[0, 7];
                    chessPieces[3, 7] = rook;
                    PositionSinglePiece(3, 7);
                    chessPieces[0, 7] = null;
                }
            }
            //Right
            else if (lastMove[1].x == 6)
            {
                if (lastMove[1].y == 0) //White
                {
                    ChessPiece rook = chessPieces[7, 0];
                    chessPieces[5, 0] = rook;
                    PositionSinglePiece(5, 0);
                    chessPieces[7, 0] = null;
                }
                else if (lastMove[1].y == 7) //Black
                {
                    ChessPiece rook = chessPieces[7, 7];
                    chessPieces[5, 7] = rook;
                    PositionSinglePiece(5, 7);
                    chessPieces[7, 7] = null;
                }
            }
        }
    }
    //Operation
    private bool ContainsVailidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }

    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x,y] == hitInfo)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one; //Invalid
    }
    public IEnumerable afterCombat()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        while (currentScene.name == "Chess")
        {
            currentScene = SceneManager.GetActiveScene();
        }
        //Enemy team
        ChessPiece winner = chessPieces[winnerCombat.x, winnerCombat.y];
        ChessPiece loser = chessPieces[loserCombat.x, loserCombat.y];
        print(loser.team);
        if (loser.team == 0)
        {
            if (loser.type == ChessPieceType.King)
            {
                CheckMate(1);
            }
            deadWhites.Add(loser);
            loser.SetScale(Vector3.one * deathSize);
            loser.SetPosition(new Vector3(8 * tileSize, yOffset, (-1 * tileSize) + tileSize / 3) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.forward * deathSpacing) * deadWhites.Count);
            chessPieces[loser.currentX, loser.currentY] = null;
        }
        else
        {
            if (loser.type == ChessPieceType.King)
            {
                CheckMate(0);
            }
            deadBlacks.Add(loser);
            loser.SetScale(Vector3.one * deathSize);
            loser.SetPosition(new Vector3(-1 * tileSize, yOffset, (8 * tileSize) - tileSize / 3) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.back * deathSpacing) * deadBlacks.Count);
            chessPieces[loser.currentX, loser.currentY] = null;
        }
        if (winnerCombat == StartedFight)
        {
            chessPieces[loserCombat.x, loserCombat.y] = winner;
            chessPieces[StartedFight.x, StartedFight.y] = null;

            PositionSinglePiece(loserCombat.x, loserCombat.y);
        }
        attack = false;
        return null;
    }
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        if (!ContainsVailidMove(ref availableMoves, new Vector2Int(x, y)))
        {
            return false;
        }
        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);
        // test
        if (cp.team == 0)
        {
            whiteTeamFighter = previousPosition;
            StartedFight = whiteTeamFighter;
        }
        else if (cp.team == 1)
        {
            BlackTeamFighter = previousPosition;
            StartedFight = BlackTeamFighter;
        }
        // IF antoher piece is on the target position
        if (chessPieces[x,y] != null)
        {
            attack = true;
            ChessPiece ocp = chessPieces[x, y];

            if (cp.team == ocp.team)
            {
                return false;
            }
            //Enemy team
            if (ocp.team == 0)
            {
                whiteTeamFighter = new Vector2Int(ocp.currentX, ocp.currentY);
              }
            else
            {
                BlackTeamFighter = new Vector2Int(ocp.currentX, ocp.currentY);
              }
        }
        if (chessPieces[x,y] == null)
        {
            chessPieces[x, y] = cp;
            chessPieces[previousPosition.x, previousPosition.y] = null;
        
                PositionSinglePiece(x, y);

            
        }
        //chessPieces[x, y] = cp;
        //chessPieces[previousPosition.x, previousPosition.y] = null;

        //PositionSinglePiece(x, y);

        isWhiteTurn = !isWhiteTurn;
        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });
        ProcessSpecialMove();
        return true;

    }
}
