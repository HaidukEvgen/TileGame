using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player{
    private int points;
    private int winRounds;
    private bool firstTurn;
    private Game.TileState tileState;
    private Dictionary<Game.Bonuses, int> bonuses;

    public Player(Game.TileState tileState){
        this.points = 0;
        this.winRounds = 0;
        this.tileState = tileState;
        this.firstTurn = true;
        this.bonuses = new Dictionary<Game.Bonuses, int>();
        this.bonuses[Game.Bonuses.bomb] = 0;
        this.bonuses[Game.Bonuses.painter] = 0;
        this.bonuses[Game.Bonuses.resizer] = 0;
    }

    public void SetFirstMove(){
        this.firstTurn = true;
    }

    public void SetTileState(Game.TileState tileState){
        this.tileState = tileState;
    }

    public void SetPoints(int points){
        this.points = points;
    }

    public void AddWin(){
        this.winRounds++;
    }

    public int GetWinRounds(){
        return this.winRounds;
    }

    public int GetBonusAmount(Game.Bonuses bonus){
        return this.bonuses[bonus];
    }

    public void ReduceBonusAmount(Game.Bonuses bonus){
        this.bonuses[bonus]--;
    }

    public void ResetWinRounds(){
        this.winRounds = 0;
    }

    public Game.TileState GetTileState(){
        return this.tileState;
    }

    public int GetPoints(){
        return this.points;
    }

    public void AddPoints(int points){
        this.points += points;
    }

    public void FirstTurnDone(){
        this.firstTurn = false;
    }

    public bool IsFirstTurn(){
        return this.firstTurn;
    }

    public bool IsUpperPlayer(){
        return this.GetTileState() == Game.TileState.firstPlayer;
    }
}
