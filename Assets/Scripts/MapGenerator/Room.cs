using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{

    public const int GRID_SIZE = 12;
    public Tilemap tiles;
    public int weight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private (int,int) GetSize()
    {
        return (tiles.cellBounds.max.x - tiles.cellBounds.min.x, tiles.cellBounds.max.y - tiles.cellBounds.min.y);
    }

    public Vector2Int GetGridSize()
    {
        (int w, int h) = GetSize();
        return new Vector2Int((w+1) / Room.GRID_SIZE, (h+1) / Room.GRID_SIZE);
    }

    public List<Vector2Int> GetGridCoordinates(Vector2Int offset)
    {
        List<Vector2Int> coordinates = new List<Vector2Int>();
        (int width, int height) = GetSize();
        for (int x = 0; x < (width+1) / Room.GRID_SIZE; ++x)
            for (int y = 0; y < (height+1) / Room.GRID_SIZE; ++y)
                coordinates.Add(new Vector2Int(x, y) + offset);
        return coordinates;

    }

    protected bool IsDoor(int dx, int dy)
    {
        int x = tiles.cellBounds.min.x + dx;
        int y = tiles.cellBounds.min.y + dy;
        var tile = tiles.GetTile<Tile>(new Vector3Int(x, y, 0));
        return (tile.colliderType == Tile.ColliderType.None);
    }

    public List<Door> GetDoors()
    {
        return GetDoors(new Vector2Int(0, 0));
    }

    public List<Door> GetDoors(Vector2Int offset)
    {
        List<Door> doors = new List<Door>();
        (int width, int height) = GetSize();
        for (int x = 0; x < width; ++x)
        {
            if (IsDoor(x, 0)) doors.Add(new Door(new Vector2Int(x, 0) + offset*GRID_SIZE, Door.Direction.SOUTH));
            if (IsDoor(x, height-1)) doors.Add(new Door(new Vector2Int(x, height-1) + offset*GRID_SIZE, Door.Direction.NORTH));
        }

        for (int y = 0; y < height; ++y)
        {
            if (IsDoor(0, y)) doors.Add(new Door(new Vector2Int(0, y) + offset*GRID_SIZE, Door.Direction.WEST));
            if (IsDoor(width - 1, y)) doors.Add(new Door(new Vector2Int(width-1, y) + offset*GRID_SIZE, Door.Direction.EAST));
        }

        return doors;
    }

    public bool HasDoorOnSide(Door.Direction direction)
    {
        List<Door> doors = GetDoors();
        foreach (var d in doors)
        {
            if (d.GetDirection() == direction) return true;
        }
        return false;
    }

    public GameObject Place(Vector2Int where)
    {
        var new_room = Instantiate(this, new Vector3(where.x * GRID_SIZE, where.y * GRID_SIZE), Quaternion.identity);
        return new_room.gameObject;
    }
}
