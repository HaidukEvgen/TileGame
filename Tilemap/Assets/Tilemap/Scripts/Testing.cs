using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

public class Testing : MonoBehaviour {

    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private TilemapVisual curFigureVisual;
    private Tilemap curFigure;
    private Tilemap tilemap;
    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;
    private Game game;

    public static bool processingTurn = false;
    public static Vector3 position;

    public const int FIGURE_POS_X = 9;
    public const int FIGURE_POS_Y = 0;

    public const int GAME_POS_X = -16;
    public const int GAME_POS_Y = -7;

    public const int GAME_HEIGHT = 14;
    public const int GAME_WIDTH = 20;

    public const int FIGURE_SIZE = 6;

    public const float CELL_SIZE = 1f;

    private void Start() {
        //create tilemap of the game board and fill with gray sprites
        tilemap = new Tilemap(GAME_WIDTH, GAME_HEIGHT, CELL_SIZE, new Vector3(GAME_POS_X, GAME_POS_Y));
        tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Path;
        tilemap.FillMap(tilemapSprite);
        tilemap.SetTilemapVisual(tilemapVisual);

        //create gameboard matrix 
        game = new Game(GAME_WIDTH, GAME_HEIGHT, tilemap);
        UIController.gm = game;

        //create tilemap for current figure
        curFigure = new Tilemap(FIGURE_SIZE, FIGURE_SIZE, 1f, new Vector3(0, 0));

        int x = game.GetNum(1, 7);
        int y = game.GetNum(1, 7);

        CreateNextFigure(x, y);
    }

    private void Update() {
        //if tile was left correctly then put it on the board
        if(processingTurn){
            processTurn(position);
            processingTurn = false;
        }

        /*if (Input.GetMouseButtonDown(0)) {
            //processTurn();
        }  
        
        if (Input.GetKeyDown(KeyCode.T)) {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.None;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Ground;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }
        if (Input.GetKeyDown(KeyCode.U)) {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Path;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Dirt;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
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

    //create next figure: get its size, draw it and change its collider
    private void CreateNextFigure(int x, int y){
        Draggable.throwBack = true;
        game.SetCurWidth(x);
        game.SetCurHeight(y);
        Draggable.width = x;
        Draggable.height = y;

        tilemapSprite = game.IsFirstPlayerTurn()? Tilemap.TilemapObject.TilemapSprite.Ground: Tilemap.TilemapObject.TilemapSprite.Dirt;
        
        curFigure.DrawFigure(x, y, tilemapSprite);
        curFigure.SetTilemapVisual(curFigureVisual);

        Draggable.ChangeCollider(x, y);
    }

    //change rotate figure: change its size, draw it and change its collider
    public void ChangeRotateFigure(){
        int x = game.GetCurHeight();
        int y = game.GetCurWidth();
        CreateNextFigure(x, y);
    }

    //is called after player left the figure on the board
    public void processTurn(Vector3 position){
        int figureWidth = game.GetCurWidth();
        int figureHeight = game.GetCurHeight();

        if(!game.GetCurPlayer().IsFirstTurn()){
            if(!game.CanBePlaced(figureWidth, figureHeight)){
                game.ChangeTurn(ref tilemapSprite);
                CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
                game.skipNum++;
                if(game.skipNum == 2){
                    game.MakeNewRound();
                }
                return;   
            }   
        }

        game.skipNum = 0;

        Player player = game.GetCurPlayer();
        if(player.IsFirstTurn()){
            MakeFirstTurn(figureWidth, figureHeight, ref player, ref tilemapSprite);
            player.FirstTurnDone();
        }
        position += new Vector3(0, figureHeight, 0);
        int x = 0, y = 0;
        tilemap.GetCoords(position, out x, out y);
        //if figure is placed incorrectly? throw it back
        if(!game.CheckFigure(player, x, y, figureWidth, figureHeight)){
            Draggable.throwBack = true;
            return;
        }
        //else draw it on the field
        for(int i = 0; i < figureWidth; i++){
            for(int j = 0; j < figureHeight; j++){
                tilemap.SetTilemapSprite(x + i, y - j, tilemapSprite);
            }
        }
        player.AddPoints(figureWidth * figureHeight);
        //add this figure to the matrix
        //change players
        game.ChangeTurn(ref tilemapSprite);
        //mark figure in matrix
        game.AddFigure(player, x, y, figureWidth, figureHeight);
        Draggable.throwBack = true;
        //and create next
        CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
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
        for(int i = 0; i < figureWidth; i++){
            for(int j = 0; j < figureHeight; j++){
                tilemap.SetTilemapSprite(x + i, y - j, tilemapSprite);
            }
        }
        player.AddPoints(figureWidth * figureHeight);
        game.ChangeTurn(ref tilemapSprite);
        game.AddFigure(player, x, y, figureWidth, figureHeight);
        Draggable.throwBack = true;
        CreateNextFigure(game.GetNum(1, 7), game.GetNum(1, 7));
    }
}
