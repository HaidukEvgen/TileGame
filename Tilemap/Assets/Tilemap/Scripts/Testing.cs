﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;
using System.Threading;

public class Testing : MonoBehaviour {

    private AudioSource wrongSound;

    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private TilemapVisual curFigureVisual;
    private Tilemap curFigure;
    private Tilemap tilemap;
    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;
    private Game game;

    public static bool processingTurn = false;
    public static Vector3 position;
    private int lastShadowX = -1;
    private int lastShadowY = -1;
    private bool isShadowDrawn = false;
    private int curTurnX;
    private int curTurnY;

    public const int GAME_POS_X = -16;
    public const int GAME_POS_Y = -7;

    public readonly int[] GAME_HEIGHT_ARRAY = {12 , 14};
    public int GAME_HEIGHT; 

    public readonly int[] GAME_WIDTH_ARRAY = {17 , 20};
    public int GAME_WIDTH;   // 20

    public const int FIGURE_SIZE = 6;

    public readonly float[] CELL_SIZE_ARRAY = {1.15f , 1f};
    public float CELL_SIZE;

    private void Start() {
        Input.multiTouchEnabled = false; 

        int numSize = PlayerPrefs.GetInt("Map", 1);

        //gameMode 1 or 2 players
        bool singleMode = PlayerPrefs.GetInt("GameMode", 1) == 1? true: false;

        GAME_HEIGHT = GAME_HEIGHT_ARRAY[numSize];
        GAME_WIDTH = GAME_WIDTH_ARRAY[numSize];
        CELL_SIZE = CELL_SIZE_ARRAY[numSize];

        //create tilemap of the game board and fill with gray sprites
        tilemap = new Tilemap(GAME_WIDTH, GAME_HEIGHT, CELL_SIZE, new Vector3(GAME_POS_X, GAME_POS_Y));
        UIController.tm = tilemap;
        tilemapSprite = Tilemap.TilemapObject.TilemapSprite.None;
        tilemap.FillMap(tilemapSprite);
        tilemap.SetTilemapVisual(tilemapVisual);

        //create gameboard matrix 
        game = new Game(GAME_WIDTH, GAME_HEIGHT, tilemap, singleMode);
        UIController.gm = game;

        //create tilemap for current figure
        curFigure = new Tilemap(FIGURE_SIZE, FIGURE_SIZE, CELL_SIZE, new Vector3(0, 0));

        int x = game.GetNum(1, 7);
        int y = game.GetNum(1, 7);

        CreateNextFigure(x, y);

        wrongSound = gameObject.GetComponent<AudioSource>();
    }
    private void Update() {
        if(!game.IsSingleMode()){
            //if tile was left correctly then put it on the board
            if(processingTurn){
                PrepareTurn();
                processingTurn = false;
            } else {
                DrawShadow();
            }
        } else {
            if (game.IsFirstPlayerTurn()){
                MakeTurnPC();
            }
            else{
                if(processingTurn){
                    PrepareTurn();
                    processingTurn = false;
                } else {
                    DrawShadow();
                }
            }
        }

        if(UIController.useResize){
            UIController.useResize = false;
            CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
        }

        if(UIController.useBomb){
            UIController.useBomb = false;
            setBomb();
        }
        /*
        if (Input.GetKeyDown(KeyCode.P)) {
            tilemap.Save();
            CMDebug.TextPopupMouse("Saved!");
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            tilemap.Load();
            CMDebug.TextPopupMouse("Loaded!");
        }*/
    } 

    private void PrepareTurn(){
        ProcessTurn(curTurnX, curTurnY);
    }

    //create next figure: get its size, draw it and change its collider
    private void CreateNextFigure(int x, int y){
        Draggable.throwBack = true;
        game.SetCurWidth(x);
        game.SetCurHeight(y);
        Draggable.width = x;
        Draggable.height = y;

        tilemapSprite = game.IsFirstPlayerTurn()? Tilemap.TilemapObject.TilemapSprite.Blue: Tilemap.TilemapObject.TilemapSprite.Red;
        
        curFigure.DrawFigure(x, y, tilemapSprite);
        curFigure.SetTilemapVisual(curFigureVisual);

        Draggable.ChangeCollider(x, y);
    }

    public void setBomb(){
        Draggable.throwBack = true;
        game.SetCurWidth(1);
        game.SetCurHeight(1);
        Draggable.width = 1;
        Draggable.height = 1;

        tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Bomb;
        
        curFigure.DrawFigure(1, 1, tilemapSprite);
        curFigure.SetTilemapVisual(curFigureVisual);

        Draggable.ChangeCollider(1, 1);

        game.SetBomb();
    }

    //change rotate figure: change its size, draw it and change its collider
    public void ChangeRotateFigure(){
        int x = game.GetCurHeight();
        int y = game.GetCurWidth();
        CreateNextFigure(x, y);
    }

    public void DrawShadow(){
        int figureWidth = game.GetCurWidth();
        int figureHeight = game.GetCurHeight();
        Player player = game.GetCurPlayer();
        position += new Vector3(CELL_SIZE / 2, figureHeight - CELL_SIZE / 2, 0);
        int x = 0, y = 0;
        tilemap.GetCoords(position, out x, out y);

        if((x == lastShadowX && y == lastShadowY) || game.GetCurPlayer().IsFirstTurn() )
            return;
        if(isShadowDrawn)
            for(int i = 0; i < figureWidth; i++){
                for(int j = 0; j < figureHeight; j++){
                    Tilemap.TilemapObject.TilemapSprite tilemapSprite = game.GetSpriteInCaseOfBonus(lastShadowX + i, lastShadowY - j);
                    tilemap.SetTilemapSprite(lastShadowX + i, lastShadowY - j, tilemapSprite);
                }
            }

        if(game.CheckFigure(player, x, y, figureWidth, figureHeight)){
            DrawRectangle(figureWidth, figureHeight, x, y, Tilemap.TilemapObject.TilemapSprite.Shadow);
            lastShadowX = curTurnX = x;
            lastShadowY = curTurnY = y;
            isShadowDrawn = true;
        } else {
            if(((lastShadowX - x == 1 || lastShadowX - x == -1) && y == lastShadowY) ||
               ((lastShadowY - y == 1 || lastShadowY - y == -1) && x == lastShadowX)){
                DrawRectangle(figureWidth, figureHeight, lastShadowX, lastShadowY, Tilemap.TilemapObject.TilemapSprite.Shadow);
                curTurnX = lastShadowX;
                curTurnY = lastShadowY;
                isShadowDrawn = true;
            } else {
                lastShadowX = curTurnX = -1;
                lastShadowY = curTurnY = -1;
                isShadowDrawn = false;
            }
        }
    }   

    public struct PCTurns{
        public PCTurns(int a, int b, int c, int d){
            x = a;
            y = b;
            width = c;
            height = d;
        }
        public int x;
        public int y;
        public int width;
        public int height;
    }

    public void MakeTurnPC(){
        Thread.Sleep(game.GetNum(500, 1500));
        int figureWidth = game.GetCurWidth();
        int figureHeight = game.GetCurHeight();

        Stack<PCTurns> turns = new Stack<PCTurns>();

        PCTurns bonusTurn1 = new PCTurns(-1, -1, -1, -1);
        PCTurns bonusTurn2 = new PCTurns(-1, -1, -1, -1);

        if(!game.GetCurPlayer().IsFirstTurn()){
            if(!game.CanBePlaced(figureWidth, figureHeight, ref turns, ref bonusTurn1) && !game.CanBePlaced(figureHeight, figureWidth, ref turns, ref bonusTurn2)){
                game.ChangeTurn(ref tilemapSprite);
                CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
                game.skipNum++;
                if(game.skipNum == 2){
                    game.MakeNewRound(tilemap);
                }
                Draggable.throwBack = true;
                return;   
            }   
        }

        if(bonusTurn1.x != -1)
            turns.Push(bonusTurn1);
        if(bonusTurn2.x != -1)
            turns.Push(bonusTurn2);

        game.skipNum = 0;

        Player player = game.GetCurPlayer();
        if(player.IsFirstTurn()){
            MakeFirstTurnPC(figureWidth, figureHeight, ref player, ref tilemapSprite);
            player.FirstTurnDone();
        }
        PCTurns turn;
        if(turns.Count > 0)
            turn = turns.Pop();
        else
            return;
        DrawRectangle(turn.width, turn.height, turn.x, turn.y, tilemapSprite);
        isShadowDrawn = false;
        player.AddPoints(turn.width * turn.height);
        //add this figure to the matrix
        game.AddFigure(player, turn.x, turn.y, turn.width, turn.height);
        //change players
        game.ChangeTurn(ref tilemapSprite);
        Draggable.throwBack = true;
        //and create next
        CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
    } 
    
    //is called after player left the figure on the board
    public void ProcessTurn(int x, int y){
        int figureWidth = game.GetCurWidth();
        int figureHeight = game.GetCurHeight(); 

        if(!game.GetCurPlayer().IsFirstTurn()){
            if(!game.CanBePlaced(figureWidth, figureHeight) && !game.CanBePlaced(figureHeight, figureWidth)){
                game.ChangeTurn(ref tilemapSprite);
                CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
                game.skipNum++;
                if(game.skipNum == 2){
                    game.MakeNewRound(tilemap);
                }
                if(SoundManager.isOn){
                    wrongSound.Play();
                }
                Draggable.throwBack = true;
                return;   
            }   
        }

        game.skipNum = 0;

        Player player = game.GetCurPlayer();
        bool isFirst = false; 
        if(player.IsFirstTurn()){
            MakeFirstTurn(figureWidth, figureHeight, ref player, ref tilemapSprite);
            player.FirstTurnDone();
            isFirst = true;
        }
        //if figure is placed incorrectly? throw it back
        if(!game.CheckFigure(player, x, y, figureWidth, figureHeight)){
            Draggable.throwBack = true;
            if(SoundManager.isOn && !isFirst){
                wrongSound.Play();
            }
            return;
        }
        //else draw it on the field
        DrawRectangle(figureWidth, figureHeight, x, y, tilemapSprite);
        lastShadowX = -1;
        lastShadowY = -1;
        isShadowDrawn = false;
        player.AddPoints(figureWidth * figureHeight);
        //add this figure to the matrix
        game.AddFigure(player, x, y, figureWidth, figureHeight);

        if(game.isBomb){
            blowBomb(x, y, player);
            game.ResetBomb();
        }   

        //change players
        game.ChangeTurn(ref tilemapSprite);
        Draggable.throwBack = true;
        //and create next
        CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
    }

    private void blowBomb(int x, int y, Player player){
        Tilemap.TilemapObject.TilemapSprite tilemap = game.IsFirstPlayerTurn()? Tilemap.TilemapObject.TilemapSprite.Blue: Tilemap.TilemapObject.TilemapSprite.Red;
        
        if(x == GAME_WIDTH - 1){
            if(y == 0){
                DrawRectangle(2, 2, x - 1, y + 1, tilemap);
                game.AddFigure(player, x - 1, y + 1, 2, 2);
            }
            else if(y == GAME_HEIGHT - 1){
                DrawRectangle(2, 2, x - 1, y - 1, tilemap);
                game.AddFigure(player, x - 1, y - 1, 2, 2);
            }
            else{
                DrawRectangle(2, 3, x - 1, y+1, tilemap);
                game.AddFigure(player, x - 1, y+1, 2, 3);
            }
        }
        else if(x == 0){
            if(y == 0){
                DrawRectangle(2, 2, x, y + 1, tilemap);
                game.AddFigure(player, x, y + 1, 2, 2);
            }
            else if(y == GAME_HEIGHT - 1){
                DrawRectangle(2, 2, x, y - 1, tilemap);
                game.AddFigure(player, x, y - 1, 2, 2);
            }
            else{
                DrawRectangle(2, 3, x, y+1, tilemap);
                game.AddFigure(player, x, y+1, 2, 3);
            }
        }

        else if(y == 0){
            DrawRectangle(3, 2, x - 1, y + 1, tilemap);
            game.AddFigure(player, x - 1, y + 1, 3, 2);
        }

        else if(y == GAME_HEIGHT - 1){
            DrawRectangle(3, 2, x - 1, y - 1, tilemap);
            game.AddFigure(player, x - 1, y - 1, 3, 2);
        }

        else{
            DrawRectangle(3, 3, x - 1, y + 1, tilemap);
            game.AddFigure(player, x - 1, y + 1, 3, 3);
        }
    }

    private void MakeFirstTurn(int figureWidth, int figureHeight, ref Player player, ref Tilemap.TilemapObject.TilemapSprite tilemapSprite){
        int x = 0, y = 0;
        if (game.IsFirstPlayerTurn()){
            x = 0;
            y = GAME_HEIGHT - 1;
        } else {
            x = GAME_WIDTH - figureWidth;
            y = figureHeight - 1;
        }
        DrawRectangle(figureWidth, figureHeight, x, y, tilemapSprite);
        player.AddPoints(figureWidth * figureHeight);
        game.ChangeTurn(ref tilemapSprite);
        game.AddFigure(player, x, y, figureWidth, figureHeight);
        Draggable.throwBack = true;
        CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
    }

    private void MakeFirstTurnPC(int figureWidth, int figureHeight, ref Player player, ref Tilemap.TilemapObject.TilemapSprite tilemapSprite){
        int x = 0, y = GAME_HEIGHT - 1;
        DrawRectangle(figureWidth, figureHeight, x, y, tilemapSprite);
        player.AddPoints(figureWidth * figureHeight);
        game.ChangeTurn(ref tilemapSprite);
        game.AddFigure(player, x, y, figureWidth, figureHeight);
        Draggable.throwBack = true;
        CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
    }

    private void DrawRectangle(int width, int height, int x, int y, Tilemap.TilemapObject.TilemapSprite tilemapSprite){
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                tilemap.SetTilemapSprite(x + i, y - j, tilemapSprite);
            }
        }
    }
}
