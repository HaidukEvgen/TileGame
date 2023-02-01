using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {
    private TileState[,] gameBoard;
    private int boardWidth;
    private int boardHeight;
    private int curWidth;
    private int curHeight;
    private Player player1;
    private Player player2; 
    public bool inGame;
    public int round;
    public int skipNum;
    public int gameScore;
    public int curWinner;
    public enum Turns{firstPlTurn, secondPlTurn};
    public enum TileState{
        none,
        firstPlayer,
        secondPlayer,
        obstacle
    };
    private Turns curTurn;

    //initialize new game with borad's width and height, players as well as 0's in the cells
    public Game(int x, int y, Tilemap tilemap){
        this.boardWidth = x;
        this.boardHeight = y;
        this.player1 = new Player(TileState.firstPlayer);
        this.player2 = new Player(TileState.secondPlayer);
        this.curTurn = GetNum(0, 2) == 0? Turns.firstPlTurn: Turns.secondPlTurn;
        this.round = 1;
        this.inGame = true;
        this.skipNum = 0;
        this.gameScore = 0;
        this.curWinner = 0;
        gameBoard = new TileState[boardHeight, boardWidth];
        for(int i = 0; i < boardHeight; i++)
            for(int j = 0; j < boardWidth; j++)
                gameBoard[i, j] = TileState.none;
        SpawnObstacles(tilemap);
    }

    public void SetCurWidth(int x){
        this.curWidth = x;
    }

    public void SetCurHeight(int y){
        this.curHeight = y;
    }

    public int GetCurWidth(){
        return this.curWidth;
    }

    public int GetCurHeight(){
        return this.curHeight;
    }

    public Turns GetCurTurn(){
        return this.curTurn;
    }

    public Player GetCurPlayer(){
        return IsFirstPlayerTurn()? player1: player2;
    }

    public bool IsFirstPlayerTurn(){
            return this.curTurn == Turns.firstPlTurn;
    }

    public string GetScore(){
        return player1.GetPoints().ToString() + " : " + player2.GetPoints().ToString();
    }

    // get random num (dice throw result)
    public int GetNum(int min, int max){
        System.Random random = new System.Random();
            return random.Next(min, max);
    }

    private void SpawnObstacles(Tilemap tilemap){
        int count = 3;
        SetObstacle(boardHeight / 2, boardWidth / 2, tilemap, Tilemap.TilemapObject.TilemapSprite.None);
        SetObstacle(boardHeight / 4, 3 * boardWidth / 4, tilemap, Tilemap.TilemapObject.TilemapSprite.None);
        SetObstacle(3 * boardHeight / 4, boardWidth / 4, tilemap, Tilemap.TilemapObject.TilemapSprite.None);      
        for(int i = 2; i < boardHeight - 2; i++)
            for(int j = 2; j < boardWidth - 2; j++){
                if(count > 6)
                    return;
                if(GetNum(0, 11) == 1 && i + j > (boardHeight + boardWidth - 8) / 2 && i + j < (boardHeight + boardWidth + 5) / 2){
                    count++;
                    SetObstacle(i, j, tilemap, Tilemap.TilemapObject.TilemapSprite.None);
                }  
            }
    }

    private void SetObstacle(int x, int y, Tilemap tilemap, Tilemap.TilemapObject.TilemapSprite tilemapSprite){
        gameBoard[x, y] = TileState.obstacle;
        GetMapXY(ref x, ref y);
        tilemap.SetTilemapSprite(x, y, tilemapSprite);
    }

    public bool CanBePlaced(int width, int height){
        for(int x = 0; x < boardWidth; x++)
            for(int y = 0; y < boardHeight; y++)
                if(CheckFigure(this.GetCurPlayer(), x, y, width, height))
                    return true;
        return false;
    }

    //check if figure can be placed there where it was left
    public bool CheckFigure(Player player, int x, int y, int width, int height){
        Game.TileState tileState = player.GetTileState(); 
        GetBoardXY(ref x, ref y);
        if(x < 0 || y < 0 || x + height > boardHeight || y + width > boardWidth)
            return false;
        //Debug.Log(player.GetTileState() == TileState.firstPlayer? "first": "second");
        //check if figure is placed in the corner during fitst turn
        if(player.IsFirstTurn()){
            if(player.IsUpperPlayer() && (x != 0 || y != 0))
                return false;
            else if(!player.IsUpperPlayer() && (x + height - boardHeight != 0 || y + width - boardWidth != 0))
                return false; 
            return true;
        }
        bool flag = false;
        for(int i = x; i < x + height; i++)
            for(int j = y; j < y + width; j++){
                //if placed on top of the non empty tile 
                if (gameBoard[i, j] != TileState.none)
                    return false;
                if(!flag){
                    //if there are same tiles above the figure
                    if(i == x && i != 0 && gameBoard[i - 1, j] == tileState)
                        flag = true;
                    //if there are same tiles on the left of the figure
                    if(j == y && j != 0 && gameBoard[i, j - 1] == tileState)
                        flag = true;
                    //if there are same tiles on the right of the figure
                    if(j == y + width - 1 && j != boardWidth - 1 && gameBoard[i, j + 1] == tileState)
                        flag = true;
                    //if there are same tiles below the figure
                    if(i == x + height - 1 && i != boardHeight - 1 && gameBoard[i + 1, j] == tileState)
                        flag = true;
                }
            }
        if(!flag)
            return false;
        return true;
    }

    //mark the figure location in matrix
    public void AddFigure(Player player, int x, int y, int width, int height){
        GetBoardXY(ref x, ref y);
        for(int i = x; i < x + height; i++)
            for(int j = y; j < y + width; j++)
                gameBoard[i, j] = player.GetTileState();
    }

    //transform grid's coordinates to board's ones
    private void GetBoardXY(ref int x, ref int y){
        y = boardHeight - y - 1;
        int temp = x;
        x = y;
        y = temp; 
    }

    private void GetMapXY(ref int x, ref int y){
        int temp = y;
        y = boardHeight - x - 1;
        x = temp; 
    }

    //change current player's turn and his tile texture
    public void ChangeTurn(ref Tilemap.TilemapObject.TilemapSprite tilemapSprite){
        curTurn = this.IsFirstPlayerTurn()? Turns.secondPlTurn: Turns.firstPlTurn;
        tilemapSprite = this.IsFirstPlayerTurn()? Tilemap.TilemapObject.TilemapSprite.Ground: Tilemap.TilemapObject.TilemapSprite.Dirt;
    }

    public void MakeNewRound(Tilemap tilemap){
        this.inGame = false;
        
        if(this.player1.GetPoints() > this.player2.GetPoints()){
            this.gameScore++;
            this.curWinner = 1;
        }
        else if(this.player2.GetPoints() > this.player1.GetPoints()){
            this.gameScore--;
            this.curWinner = 2;
        }
        else{
            this.curWinner = 0;
        }
    }

    public void CleanOldVal(Tilemap tilemap){
        this.player1.SetPoints(0);
        this.player1.SetFirstMove();

        this.player2.SetPoints(0);
        this.player2.SetFirstMove();

        tilemap.FillMap(Tilemap.TilemapObject.TilemapSprite.Path);
        for(int i = 0; i < boardHeight; i++)
            for(int j = 0; j < boardWidth; j++)
                gameBoard[i, j] = TileState.none;

        if(this.round == 3){
            this.round = 0;
        }
        else{
            this.round++;
        }
        SpawnObstacles(tilemap);
    }
}
