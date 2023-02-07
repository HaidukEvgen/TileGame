using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap {

    //public event EventHandler OnLoaded;

    private Grid<TilemapObject> grid;
    public const int FIGURE_SIZE = 6;

    public Tilemap(int width, int height, float cellSize, Vector3 originPosition) {
        grid = new Grid<TilemapObject>(width, height, cellSize, originPosition, (Grid<TilemapObject> g, int x, int y) => new TilemapObject(g, x, y));
    }

    public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite) {
        TilemapObject tilemapObject = grid.GetGridObject(worldPosition);
        if (tilemapObject != null) {
            tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }

    public void SetTilemapSprite(int x, int y, TilemapObject.TilemapSprite tilemapSprite) {
        TilemapObject tilemapObject = grid.GetGridObject(x, y);
        if (tilemapObject != null) {
            tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }

    public void GetCoords(Vector3 worldPosition, out int x, out int y){
        grid.GetXY(worldPosition, out x, out y);
    }

    public void SetTilemapVisual(TilemapVisual tilemapVisual) {
        tilemapVisual.SetGrid(this, grid);
    }

    public void FillMap(TilemapObject.TilemapSprite tilemapSprite){
        for(int i = 0; i < grid.GetWidth(); i++){
            for(int j = 0; j < grid.GetHeight(); j++){
                TilemapObject tilemapObject = grid.GetGridObject(i, j);
                if (tilemapObject != null)
                    tilemapObject.SetTilemapSprite(tilemapSprite);
            }
        }   
    }

    public void SetRowBlank(int i, TilemapObject.TilemapSprite tilemapSprite){
        if(SoundManager.isOn){
            TilemapVisual.PlayDeleteSound();
        } 
        for(int j = 0; j < grid.GetWidth(); j++){
            TilemapObject tilemapObject = grid.GetGridObject(j, i);
            if (tilemapObject != null)
                tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }

    public void SetColmnBlank(int i, TilemapObject.TilemapSprite tilemapSprite){
        if(SoundManager.isOn){
            TilemapVisual.PlayDeleteSound();
        }
        for(int j = 0; j < grid.GetHeight(); j++){
            TilemapObject tilemapObject = grid.GetGridObject(i, j);
            if (tilemapObject != null)
                tilemapObject.SetTilemapSprite(tilemapSprite);
        }
    }

    public void DrawFigure(int figureWidth, int figureHeight, TilemapObject.TilemapSprite tilemapSprite){
        for(int i = 0; i < FIGURE_SIZE; i++)
            for(int j = 0; j < FIGURE_SIZE; j++){
                TilemapObject tilemapObject = grid.GetGridObject(i, j);
                tilemapObject.SetTilemapSprite(Tilemap.TilemapObject.TilemapSprite.Empty);
            }
        for(int i = 0; i < figureWidth; i++)
            for(int j = 0; j < figureHeight; j++){
                TilemapObject tilemapObject = grid.GetGridObject(i, j);
                tilemapObject.SetTilemapSprite(tilemapSprite);
            }
    }

    public Grid<TilemapObject> GetGrid(){
        return this.grid;
    }
    /*
    public class SaveObject {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }

    public void Save() {
        List<TilemapObject.SaveObject> tilemapObjectSaveObjectList = new List<TilemapObject.SaveObject>();
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                TilemapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObjectSaveObjectList.Add(tilemapObject.Save());
            }
        }

        SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

        SaveSystem.SaveObject(saveObject);
    }

    public void Load() {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        foreach (TilemapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray) {
            TilemapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }
    */


    /*
     * Represents a single Tilemap Object that exists in each Grid Cell Position
     * */
    public class TilemapObject {

        public enum TilemapSprite {
            Empty,
            Obstacle,
            Blue,
            None,
            Red,
            Shadow            
        }

        private Grid<TilemapObject> grid;
        private int x;
        private int y;
        private TilemapSprite tilemapSprite;

        public TilemapObject(Grid<TilemapObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTilemapSprite(TilemapSprite tilemapSprite) {
            this.tilemapSprite = tilemapSprite;
            grid.TriggerGridObjectChanged(x, y);
        }

        public TilemapSprite GetTilemapSprite() {
            return tilemapSprite;
        }

        public override string ToString() {
            return tilemapSprite.ToString();
        }


        /*
        [System.Serializable]
        public class SaveObject {
            public TilemapSprite tilemapSprite;
            public int x;
            public int y;
        }
    
        public SaveObject Save() {
            return new SaveObject { 
                tilemapSprite = tilemapSprite,
                x = x,
                y = y,
            };
        }

        public void Load(SaveObject saveObject) {
            tilemapSprite = saveObject.tilemapSprite;
        }*/
    }
}
