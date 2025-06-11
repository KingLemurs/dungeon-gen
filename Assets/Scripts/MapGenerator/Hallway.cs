using UnityEngine;

public class Hallway : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Place(Door door)
    {
        Vector2Int where = door.GetGridCoordinates();
        var dir = door.GetDirection();
        if (dir == Door.Direction.EAST) where.x++;
        if (dir == Door.Direction.NORTH) where.y++;
        int dx = 0;
        int dy = 0;
        if (door.IsHorizontal()) dx = -1;
        if (door.IsVertical()) dy = -1;
        var new_room = Instantiate(this, new Vector3(where.x * Room.GRID_SIZE + dx, where.y * Room.GRID_SIZE + dy), Quaternion.identity);
        return new_room.gameObject;
    }
}
