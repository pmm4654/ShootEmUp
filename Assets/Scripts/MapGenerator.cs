using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0,1)]
    public float outlinePercent;
    [Range(0, 1)]
    public float obstaclePercent;

    public int seed = 1;

    Coord mapCenter;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        mapCenter = new Coord((int)(mapSize.x / 2), (int)(mapSize.y / 2));
        // get and shuffle tile coordinates
        allTileCoords = new List<Coord>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));


        // make a map holder and destroy it when you want to generate the map
        string holderName = "Generated Map";
        Transform mapHolder = transform.Find(holderName);
        if (mapHolder)
        {
            DestroyImmediate(mapHolder.gameObject);
        }
        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if(randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;

            } else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount )
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;
        int accessiblTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            for(int x = -1; x <= 1; x ++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if(x == 0 || y == 0)
                    {
                        // if inside of the obstacle map
                        if(neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
                        {
                            // is an adjacent tile that we haven't checked yet - since we're excluding obstacles, we won't be blocking the path
                            if(!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true; // now we have checked it
                                queue.Enqueue(new Coord(neighborX, neighborY)); // add to the queue to check its neighbors
                                accessiblTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessiblTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        // 0.5 is to offset so that the edge of the tile will be at that position - not the center
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

    }

}
