using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

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
        //Debug.Log(gameMode);

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
                processTurn(position);
                processingTurn = false;
            } else {
                DrawShadow(position);
            }
        } else {
            if (game.IsFirstPlayerTurn())
                MakeTurnPC();
            else{
                if(processingTurn){
                    processTurn(position);
                    processingTurn = false;
                } else {
                    DrawShadow(position);
                }
            }
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

        tilemapSprite = game.IsFirstPlayerTurn()? Tilemap.TilemapObject.TilemapSprite.Blue: Tilemap.TilemapObject.TilemapSprite.Red;
        
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

    public void DrawShadow(Vector3 position){
        if(game.GetCurPlayer().IsFirstTurn())
            return;
        int figureWidth = game.GetCurWidth();
        int figureHeight = game.GetCurHeight();
        position += new Vector3(0, figureHeight, 0);
        int x = 0, y = 0;
        tilemap.GetCoords(position, out x, out y);
        if(x == lastShadowX && y == lastShadowY)
            return;
        Player player = game.GetCurPlayer();
        if (lastShadowX != -1)
            for(int i = 0; i < figureWidth; i++){
                for(int j = 0; j < figureHeight; j++){
                    tilemap.SetTilemapSprite(lastShadowX + i, lastShadowY - j, Tilemap.TilemapObject.TilemapSprite.None);
                }
            }
        if(game.CheckFigure(player, x, y, figureWidth, figureHeight)){
            for(int i = 0; i < figureWidth; i++){
                for(int j = 0; j < figureHeight; j++){
                    tilemap.SetTilemapSprite(x + i, y - j, Tilemap.TilemapObject.TilemapSprite.Shadow);
                }
            }
            lastShadowX = x;
            lastShadowY = y;
        } else {
            lastShadowX = -1;
            lastShadowY = -1;
        }
    }   

    public void MakeTurnPC(){
        
    } 
    
    //is called after player left the figure on the board
    public void processTurn(Vector3 position){
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
        position += new Vector3(0, figureHeight, 0);
        int x = 0, y = 0;
        tilemap.GetCoords(position, out x, out y);
        //if figure is placed incorrectly? throw it back
        if(!game.CheckFigure(player, x, y, figureWidth, figureHeight)){
            Draggable.throwBack = true;
            if(SoundManager.isOn && !isFirst){
                wrongSound.Play();
            }
            return;
        }
        //else draw it on the field
        for(int i = 0; i < figureWidth; i++){
            for(int j = 0; j < figureHeight; j++){
                tilemap.SetTilemapSprite(x + i, y - j, tilemapSprite);
            }
        }
        lastShadowX = -1;
        lastShadowY = -1;
        player.AddPoints(figureWidth * figureHeight);
        //add this figure to the matrix
        game.AddFigure(player, x, y, figureWidth, figureHeight);
        //change players
        game.ChangeTurn(ref tilemapSprite);
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
