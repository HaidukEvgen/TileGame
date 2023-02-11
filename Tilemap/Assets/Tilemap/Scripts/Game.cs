using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {
    private TileState[,] gameBoard;
    private int boardWidth;
    private int boardHeight;
    private int curWidth;
    private int curHeight;
    private bool singleMode;
    private Player player1;
    private Player player2; 
    public bool inGame;
    public int round;
    public int skipNum;
    public int gameScore;
    public bool isBomb;
    public enum Turns{firstPlTurn, secondPlTurn};
    public enum TileState{
        none,
        firstPlayer,
        secondPlayer,
        obstacle,
        bomb,
        painter,
        resizer
    };
    public enum Bonuses{
        bomb,
        painter,
        resizer
    }
    private Turns curTurn;
    private Tilemap tilemap;

    //initialize new game with borad's width and height, players as well as 0's in the cells
    public Game(int x, int y, Tilemap tilemap, bool singleMode){
        this.boardWidth = x;
        this.boardHeight = y;
        this.player1 = new Player(TileState.firstPlayer);
        this.player2 = new Player(TileState.secondPlayer);
        this.singleMode = singleMode;
        this.curTurn = GetNum(0, 2) == 0? Turns.firstPlTurn: Turns.secondPlTurn;
        this.round = 1;
        this.inGame = true;
        this.skipNum = 0;
        this.gameScore = 0;
        this.tilemap = tilemap;
        this.isBomb = false;
        gameBoard = new TileState[boardHeight, boardWidth];
        for(int i = 0; i < boardHeight; i++)
            for(int j = 0; j < boardWidth; j++)
                gameBoard[i, j] = TileState.none;
        SpawnObstacles(tilemap);
        SpawnBonus(tilemap);
    }

    public void SetBomb(){
        this.isBomb = true;
    }

    public void ResetBomb(){
        this.isBomb = false;
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

    public bool IsSingleMode(){
        return singleMode;
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
        SetObstacle(boardHeight / 2, boardWidth / 2, tilemap, Tilemap.TilemapObject.TilemapSprite.Obstacle);
        SetObstacle(boardHeight / 4, 3 * boardWidth / 4, tilemap, Tilemap.TilemapObject.TilemapSprite.Obstacle);
        SetObstacle(3 * boardHeight / 4, boardWidth / 4, tilemap, Tilemap.TilemapObject.TilemapSprite.Obstacle);      
        for(int i = 2; i < boardHeight - 2; i++)
            for(int j = 2; j < boardWidth - 2; j++){
                if(count > 6)
                    return;
                if(GetNum(0, 11) == 1 && i + j > (boardHeight + boardWidth - 8) / 2 && i + j < (boardHeight + boardWidth + 5) / 2){
                    count++;
                    SetObstacle(i, j, tilemap, Tilemap.TilemapObject.TilemapSprite.Obstacle);
                }  
            }
    }

    private void SpawnBonus(Tilemap tilemap){
        int a = GetNum(0, 4); 
        Bonuses curBonus;  
        if(a == 1)
            curBonus = Bonuses.bomb;
        else if(a == 2 )
            curBonus = Bonuses.painter;
        else
            curBonus = Bonuses.resizer;
        int i, j;
        while(true){
            i = GetNum(0, this.boardHeight);
            j = GetNum(0, this.boardWidth);
            if(gameBoard[i, j] == TileState.none)
                break;
        }
        gameBoard[i, j] = GetBonusTileState(curBonus);
        GetMapXY(ref i, ref j);
        tilemap.SetTilemapSprite(i, j, GetBonusSprite(curBonus));
    }

    public TileState GetBonusTileState(Bonuses bonus){
        if(bonus == Bonuses.bomb)
            return TileState.bomb;
        if(bonus == Bonuses.painter)
            return TileState.painter;
        return TileState.resizer;
    }  

    public Tilemap.TilemapObject.TilemapSprite GetBonusSprite(Bonuses bonus){
        if(bonus == Bonuses.bomb)
            return Tilemap.TilemapObject.TilemapSprite.Bomb;
        if(bonus == Bonuses.painter)
            return Tilemap.TilemapObject.TilemapSprite.Painter;
        return Tilemap.TilemapObject.TilemapSprite.Resizer;
    } 

    public Tilemap.TilemapObject.TilemapSprite GetBonusSprite(TileState tileState){
        if(tileState == TileState.bomb)
            return Tilemap.TilemapObject.TilemapSprite.Bomb;
        if(tileState == TileState.painter)
            return Tilemap.TilemapObject.TilemapSprite.Painter;
        if(tileState == TileState.resizer)    
            return Tilemap.TilemapObject.TilemapSprite.Resizer;
        return Tilemap.TilemapObject.TilemapSprite.None;
    } 

    public Tilemap.TilemapObject.TilemapSprite GetSpriteInCaseOfBonus(int x, int y){
        GetBoardXY(ref x, ref y);
        return this.GetBonusSprite(gameBoard[x, y]);
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

    public bool CanBePlaced(int width, int height, ref Stack<Testing.PCTurns> stack){
        bool canBePlaced = false;
        for(int x = 0; x < boardWidth; x++)
            for(int y = 0; y < boardHeight; y++)
                if(CheckFigure(this.GetCurPlayer(), x, y, width, height)){
                    canBePlaced = true;
                    stack.Push(new Testing.PCTurns(x, y, width, height));
                }
        return canBePlaced;
    }

    private bool IsBonusTile(TileState tileState){
        return (tileState == TileState.bomb || tileState == TileState.painter || tileState == TileState.resizer);
    }

    //check if figure can be placed there where it was left
    public bool CheckFigure(Player player, int x, int y, int width, int height){
        Game.TileState tileState = player.GetTileState(); 
        GetBoardXY(ref x, ref y);
        if(x < 0 || y < 0 || x + height > boardHeight || y + width > boardWidth)
            return false;
        
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

                if(isBomb){
                    if(gameBoard[i, j] == tileState){
                        return true;
                    }
                    else if(gameBoard[i, j] != tileState && gameBoard[i, j] != TileState.none){
                        return false;
                    }
                }
                //if placed on top of the non empty tile 
                if (!IsBonusTile(gameBoard[i, j]) && gameBoard[i, j] != TileState.none)
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

    private void addBonus(Player player, int a, int b){
        if(gameBoard[a, b] == Game.TileState.bomb){
            player.IncreaseBonusAmount(Game.Bonuses.bomb);
        }
        else if(gameBoard[a, b] == Game.TileState.painter){
            player.IncreaseBonusAmount(Game.Bonuses.painter);
        }
        else{
            player.IncreaseBonusAmount(Game.Bonuses.resizer);
        }
    }

    //mark the figure location in matrix
    public void AddFigure(Player player, int x, int y, int width, int height){
        GetBoardXY(ref x, ref y);
        for(int i = x; i < x + height; i++)
            for(int j = y; j < y + width; j++){
                if(IsBonusTile(gameBoard[i, j])){
                    addBonus(player, i, j);
                }
                gameBoard[i, j] = player.GetTileState();
            }

        TetrisCheck();
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
        tilemapSprite = this.IsFirstPlayerTurn()? Tilemap.TilemapObject.TilemapSprite.Blue: Tilemap.TilemapObject.TilemapSprite.Red;
    }

    public void MakeNewRound(Tilemap tilemap){
        this.inGame = false;
        
        if(this.player1.GetPoints() > this.player2.GetPoints()){
            this.gameScore++;
            this.player1.AddWin();
        }
        else if(this.player2.GetPoints() > this.player1.GetPoints()){
            this.gameScore--;
            this.player2.AddWin();
        }
    }

    public void CleanOldVal(Tilemap tilemap, int maxRound){
        this.player1.SetPoints(0);
        this.player1.SetFirstMove();

        this.player2.SetPoints(0);
        this.player2.SetFirstMove();

        tilemap.FillMap(Tilemap.TilemapObject.TilemapSprite.None);
        for(int i = 0; i < boardHeight; i++)
            for(int j = 0; j < boardWidth; j++)
                gameBoard[i, j] = TileState.none;

        if(this.round == maxRound){
            this.round = 1;
            this.player1.ResetWinRounds();
            this.player2.ResetWinRounds();
        }
        else{
            this.round++;
        }
        SpawnObstacles(tilemap);
        SpawnBonus(tilemap);
    }

    public Player GetPlayer1(){
        return this.player1;
    }

    public Player GetPlayer2(){
        return this.player2;
    }

    public void CleanRow(int i){
        tilemap.SetRowBlank(boardHeight - 1 - i, Tilemap.TilemapObject.TilemapSprite.None);

        for(int j = 0; j < boardWidth; j++)
            if(gameBoard[i, j] != TileState.obstacle){
                gameBoard[i, j] = TileState.none;
            }
            else{
                int x = i;
                int y = j;
                GetMapXY(ref x, ref y);
                tilemap.SetTilemapSprite(x, y, Tilemap.TilemapObject.TilemapSprite.Obstacle);
            }
    }

    public void CleanColmn(int i){
        tilemap.SetColmnBlank(i, Tilemap.TilemapObject.TilemapSprite.None);

        for(int j = 0; j < boardHeight; j++)
            if(gameBoard[j, i] != TileState.obstacle){
                gameBoard[j, i] = TileState.none;
            }
            else{
                int x = j;
                int y = i;
                GetMapXY(ref x, ref y);
                tilemap.SetTilemapSprite(x, y, Tilemap.TilemapObject.TilemapSprite.Obstacle);
            }
    }

    public void TetrisCheck(){
        for(int i = 0; i < boardHeight; i++){
            TetrisRowCheck(i);
        }

        for(int i = 0; i < boardWidth; i++){
            TetrisColmnCheck(i);
        }
    }

    private void TetrisColmnCheck(int i){
        if(gameBoard[0, i] != TileState.none){
            var curColor = gameBoard[0, i];
            for(int j = 1; j < boardHeight; j++){
                if(gameBoard[j, i] == curColor || gameBoard[j, i] == TileState.obstacle){
                    if(j == boardHeight - 1){
                        CleanColmn(i);
                        GetCurPlayer().AddPoints(boardHeight);
                    }
                }
                else{
                    break;
                }
            }
        }
    }

    private void TetrisRowCheck(int i){
        if(gameBoard[i, 0] != TileState.none){
            var curColor = gameBoard[i, 0];
            for(int j = 1; j < boardWidth; j++){
                if(gameBoard[i, j] == curColor || gameBoard[i, j] == TileState.obstacle){
                    TetrisColmnCheck(j);
                    if(j == boardWidth - 1){
                        CleanRow(i);
                        GetCurPlayer().AddPoints(boardWidth);
                    }
                }
                else{
                    break;
                }
            }
        }
    }
}