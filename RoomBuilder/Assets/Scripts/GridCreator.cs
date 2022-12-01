using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    public Vector3 originPosition;

    private int[,] gridArray;

    public MuseumObject[,] AllMuseumObjects;
    private TextMesh[,] debugTextArray;

    public GRID_ORIENTATION Orientation;

    public bool isActiveGrid;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, GRID_ORIENTATION orientation)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.Orientation = orientation;

        this.isActiveGrid = true;
        gridArray = new int[width, height];
        AllMuseumObjects = new MuseumObject[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {

                //debugTextArray[x, y] = MyCreateWorldText(
                //    null,
                //    gridArray[x, y].ToString(),
                //    DebugTextArrayTextPosition(x, y),
                //    8,
                //    Color.white,
                //    TextAnchor.MiddleCenter,
                //    TextAlignment.Left,
                //    5000,
                //    DebugTextArrayOrientation()
                //);
                Debug.DrawLine(GetWorldPositionFromGridCoords(x, y), GetWorldPositionFromGridCoords(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPositionFromGridCoords(x, y), GetWorldPositionFromGridCoords(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPositionFromGridCoords(0, height), GetWorldPositionFromGridCoords(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPositionFromGridCoords(width, 0), GetWorldPositionFromGridCoords(width, height), Color.white, 100f);
    }

    #region Debug Text Fns

    public Quaternion DebugTextArrayOrientation() {

        switch (Orientation)
        {
            case GRID_ORIENTATION.XY:
                return Quaternion.identity;
            case GRID_ORIENTATION.YZ:
                return Quaternion.Euler(0f, -90f, 0f);
            case GRID_ORIENTATION.XZ:
                return Quaternion.Euler(-90f, 0f, 0f);
            default:
                return Quaternion.identity;

        }
    }
    public Vector3 DebugTextArrayTextPosition(int x, int y) {
        switch (Orientation)
        {
            case GRID_ORIENTATION.XY:
                return GetWorldPositionFromGridCoords(x, y) + new Vector3(cellSize, cellSize, 0f) * 0.5f;
            case GRID_ORIENTATION.YZ:
                return GetWorldPositionFromGridCoords(x, y) + new Vector3(0f, cellSize, cellSize) * 0.5f;
            case GRID_ORIENTATION.XZ:
                return GetWorldPositionFromGridCoords(x, y) + new Vector3(cellSize, 0f, cellSize) * 0.5f;
            //return new Vector3(0f, y, x) * cellSize + originPosition;
            default:
                return new Vector3(x, y, 0f) * cellSize + originPosition;
        }


    }
    #endregion

    public Vector3 GetWorldPositionFromGridCoords(int x, int y) {
        return new Vector3(x, y, 0) * cellSize + originPosition;
    }

    public void GetGridXYFromWorldPosition(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public Vector3 GetGridFromWorldPositionFromXY(int x, int y)
    {
        return cellSize * new Vector3(x, y) + originPosition;
    }

    public void SetCellMuseumObject(int x, int y, MuseumObject ToPlace)
    {
        if (ToPlace == null) return;

        if (x < 0 || x + ToPlace.width > width || y < 0 || y + ToPlace.height > height)
            return;

        ToPlace.PrefabObj.SetActive(true);
        ToPlace.PrefabObj.transform.position = GetGridFromWorldPositionFromXY(x, y);

        ToPlace.origin = new Vector2(x, y);

        for (int i = 0; i < ToPlace.width; i++) {

            for (int j = 0; j < ToPlace.height; j++)
            {
                AllMuseumObjects[x + i, y + j] = ToPlace;
            }
        }
            
    }

    public void SetObjectFromWorldPosition(Vector3 worldPosition, MuseumObject value) {
        int x, y;
        GetGridXYFromWorldPosition(worldPosition, out x, out y);
        SetCellMuseumObject(x, y, value);
    }

    public void PrintGrid() {

        string final = "";
        for (int j = height - 1; j >= 0; j--)
        {
            for (int i = 0; i < width; i++)
                final += (AllMuseumObjects[i, j] == null ? " O | " : " X |");
            final += "\n";
        }
        Debug.Log(final);
    }

    public bool IsNewValidPosition(MuseumObject obj, int x, int y)
    {
        if (obj == null) return false;

        //Relic: transforming prefab scale to int height
        //Vector3 SpriteScale = obj.PrefabObj.GetComponentsInChildren<Transform>()[1].localScale;
        //int objWidth = obj.width;
        //int objHeight = obj.height;

        if (x < 0 || x + obj.width > width || y < 0 || y + obj.height > height) return false;
        return true;
    }

    public bool IsColliding(MuseumObject obj, int x, int y)
    {
        //Relic: transforming prefab scale to int height
        //Vector3 SpriteScale = obj.PrefabObj.GetComponentsInChildren<Transform>()[1].localScale;
        //int objWidth = Mathf.CeilToInt(SpriteScale.x / cellSize);
        //int objHeight = Mathf.CeilToInt(SpriteScale.y / cellSize);


        for (int i = 0; i < obj.width; i++)
        {
            for (int j = 0; j < obj.height; j++)
            {
                if (AllMuseumObjects[x+i,y+j] != null && AllMuseumObjects[x + i, y + j] != obj) {
                    Debug.Log("Collision found with "+ AllMuseumObjects[x + i, y + j].Title);
                    return true;
                }
                //AllMuseumObjects[x + i, y + j] = ToPlace;
            }
        }

        return false;
    }

    public void FindEmptySpace(MuseumObject obj, out int x, out int y) {

        x = -1;
        y = -1;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++) {
                if (!IsColliding(obj, col, row) && IsNewValidPosition(obj, col, row)) {
                    x = col;
                    y = row;
                    return;
                }
            }
        }

        if (x == -1 && y == -1) {

            Debug.Log("Couldn't find empty space");
        }

    }

    public void CheckRow()
    {


    }

#nullable enable
    public MuseumObject? GetMuseumObjectFromGridCoords(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return AllMuseumObjects[x, y];
        }
        else
        {
            return null;
        }
    }

#nullable enable
    public MuseumObject? GetMuseumObjectFromWorldPosition(Vector3 worldPosition) {
        int x, y;
        GetGridXYFromWorldPosition(worldPosition, out x, out y);
        return GetMuseumObjectFromGridCoords(x, y);
    }

    public void RemoveMuseumObjectFromGridCoords(int x, int y) {

        if (x < 0 && y < 0 && x > width && y > height) return;

        var obj = AllMuseumObjects[x, y];

        if (obj == null) return;

        Vector2 origin = obj.origin;

        for (int i = 0; i < obj.width; i++) {

            for (int j = 0; j < obj.height; j++)
            {
                    AllMuseumObjects[x+i, y+j] = null;
                
            }
        }
            

        obj.PrefabObj.SetActive(false);
        obj.origin = new Vector2(-1f, -1f);
    }

    // Create Text in the World
    public static TextMesh MyCreateWorldText(
        Transform parent,
        string text,
        Vector3 localPosition,
        int fontSize,
        Color color,
        TextAnchor textAnchor,
        TextAlignment textAlignment,
        int sortingOrder,
        Quaternion orientation
)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localRotation = orientation;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

}