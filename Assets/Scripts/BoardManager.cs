using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public GameObject[] wallTiles;
    public GameObject[] floorTiles;
    public GameObject exitTile;
    public GameObject playerTile;
    public GameObject[] enemyTiles;
    public GameObject batteryTile;
    //public GameObject door;

    private Transform board;

    private const int BoardWidth = 23;
    private const int BoardHeight = 16;

    void Awake()
    {
        board = new GameObject("Board").transform;
        LevelSetup(1); // TODO : disable - should be activated via GameManager
    }

    public void LevelSetup(int levelNr)
    {
        Debug.Log("LOAD LEVEL DATA");
        string levelFileName = "level" + levelNr;
        string filePath = levelFileName; /* "Levels/" +  */
        Debug.Log(filePath);
        var levelTextAsset = Resources.Load<TextAsset>(filePath);

        Debug.Log("PARSE LEVEL DATA");
        var levelDefinition = ParseLevel(levelTextAsset.text);

        Debug.Log("BUILD LEVEL");
        BuildLevel(levelDefinition);
    }

    private void BuildLevel(string[,] lvldef)
    {
        for (int r = 0; r < BoardHeight; ++r)
        {
            for (int c = 0; c < BoardWidth; ++c)
            {
                var tileType = lvldef[r, c];
                LoadGameObject(tileType, r, c);
            }
        }
    }

    private void LoadGameObject(string tileType, int row, int col)
    {
        /*
            0 = wall
            1 = floor
            P = player
            E = enemy
            X = eXit
            //D = door
            B = battery
         */
        if (tileType == "0")
        {
            var toInstantiate = wallTiles[Random.Range(0, wallTiles.Length - 1)];
            InstantiateBoardTile(toInstantiate, CoordsToPosition(row, col));
        }
        else if (tileType == "1")
        {
            InstantiateFloorTile(row, col);
        }
        else if (tileType == "P")
        {
            InstantiateFloorTile(row, col);
            InstantiateBoardTile(playerTile, CoordsToPosition(row, col));
        }
        else if (tileType == "E")
        {
            InstantiateFloorTile(row, col);
            InstantiateBoardTile(enemyTiles[Random.Range(0, enemyTiles.Length - 1)], CoordsToPosition(row, col));
        }
        else if (tileType == "X")
        {
            InstantiateFloorTile(row, col);
            InstantiateBoardTile(exitTile, CoordsToPosition(row, col));
        }
        else if (tileType == "B")
        {
            InstantiateFloorTile(row, col);
            InstantiateBoardTile(batteryTile, CoordsToPosition(row, col));
        }
        else
        {
            Debug.LogError("Unknown tile type(" + row + "," + col + "): " + tileType);
        }
    }

    private void InstantiateFloorTile(int row, int col)
    {
        var toInstantiate = floorTiles[Random.Range(0, floorTiles.Length - 1)];
        InstantiateBoardTile(toInstantiate, CoordsToPosition(row, col));
    }

    private Vector3 CoordsToPosition(int row, int col)
    {
        return new Vector3(col, -row, 0f);
    }

    private void InstantiateBoardTile(GameObject prefab, Vector3 pos)
    {
        var obj = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
        obj.transform.parent = board;
    }

    private string[,] ParseLevel(string levelText)
    {
        var rows = GetRows(levelText);

        string[,] level = new string[BoardHeight, BoardWidth];
        for (int r = 0; r < rows.Length; ++r)
        {
            var cols = Cut(rows[r].Trim()); // making sure we're not getting any excess
            for (int c = 0; c < cols.Length; ++c)
            {
                //Debug.Log(string.Format("r: {0}, c: {1}", r, c));
                level[r, c] = cols[c];
            }
        }

        return level;
    }

    private string[] GetRows(string input)
    {
        return input.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
    }

    private string[] Cut(string input, char separator = ' ')
    {
        return input.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries);
    }
}
