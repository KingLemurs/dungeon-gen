using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public List<Room> rooms;
    public Hallway vertical_hallway;
    public Hallway horizontal_hallway;
    public Room start;
    public Room target;

    // Constraint: How big should the dungeon be at most
    // this will limit the run time (~10 is a good value 
    // during development, later you'll want to set it to 
    // something a bit higher, like 25-30)
    public int MAX_SIZE;

    // set this to a high value when the generator works
    // for debugging it can be helpful to test with few rooms
    // and, say, a threshold of 100 iterations
    public int THRESHOLD;

    // keep the instantiated rooms and hallways here 
    private List<GameObject> generated_objects;
    
    int iterations;
    private int targetRooms;

    public void Generate()
    {
        // dispose of game objects from previous generation process
        foreach (var go in generated_objects)
        {
            Destroy(go);
        }
        generated_objects.Clear();

        generated_objects.Add(start.Place(new Vector2Int(0, 0)));
        List<Door> doors = start.GetDoors();
        List<Vector2Int> occupied = new List<Vector2Int>();
        Dictionary<Vector2Int, Room> occupiedRooms = new Dictionary<Vector2Int, Room>();
        occupied.Add(new Vector2Int(0, 0));
        occupiedRooms.Add(new Vector2Int(0,0), start);
        iterations = 0;
        targetRooms = 0;
        bool res = GenerateWithBacktracking(occupied, occupiedRooms, doors, 1);
        if (!res) throw new Exception("could not generate dungeon within constraints");
    }

    bool CheckRoomWithGap(HashSet<Door.Direction> gap, Room r, List<Vector2Int> occupied, Vector2Int loc)
    {
        HashSet<Door.Direction> layout = new HashSet<Door.Direction>();
        foreach (Door d in r.GetDoors(loc))
        {
            // if room has door that is not in gap, AND there is a room matching, there is a WALL
            if (!gap.Contains(d.GetMatchingDirection()) && occupied.Contains(d.GetMatching().GetGridCoordinates()))
            {
                return false;
            }

            layout.Add(d.GetMatchingDirection());
        }
        
        // if no obstructions return if all the required doors are present

        return gap.SetEquals(layout) || gap.IsSubsetOf(layout);
    }

    HashSet<Door.Direction> GetGapFromLocation(Vector2Int location,  List<Vector2Int> occupied, Dictionary<Vector2Int, Room> occupiedRooms)
    {
        HashSet<Door.Direction> output = new HashSet<Door.Direction>();
        Dictionary<Vector2Int, Door.Direction> toCheck = new Dictionary<Vector2Int, Door.Direction>()
        {
            { location + Vector2Int.up, Door.Direction.SOUTH },
            { location + Vector2Int.down, Door.Direction.NORTH },
            { location + Vector2Int.left, Door.Direction.EAST },
            { location + Vector2Int.right, Door.Direction.WEST }
        };
        
        foreach (Vector2Int loc in toCheck.Keys)
        {
            // we only care if the room has a door facing us
            if (occupied.Contains(loc) && occupiedRooms[loc].HasDoorOnSide(toCheck[loc]))
            {
                print("Adding dir: " + toCheck[loc].ToString() + " from " + loc + " to " + location);
                output.Add(toCheck[loc]);
            }
        }

        return output;
    }


    bool GenerateWithBacktracking(List<Vector2Int> occupied, Dictionary<Vector2Int, Room> occupiedRooms, List<Door> doors, int depth)
    {
        iterations++;
        if (iterations > THRESHOLD) throw new System.Exception("Iteration limit exceeded");
        if (depth > MAX_SIZE)
        {
            print("hit max size");
            return false;
        }
        
        if (doors.Count == 0)
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;

            foreach (Vector2Int cell in occupied)
            {
                if (cell.x < minX) minX = cell.x;
                if (cell.x > maxX) maxX = cell.x;
                if (cell.y < minY) minY = cell.y;
                if (cell.y > maxY) maxY = cell.y;
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            // if difference between width and height is too big, backtrack
            if (Mathf.Abs(width - height) > 2)
            {
                return false;
            }

            return depth > 4 && targetRooms == 1;
        }

        
        // pick door
        Door door = doors[Random.Range(0, doors.Count)];
        foreach (Door d in doors)
        {
            print(d.GetGridCoordinates());
            print(d.GetDirection());
        }

        foreach (var coo in occupied)
        {
            print("occupied: " + coo);
        }
        
        Door.Direction dir = door.GetMatchingDirection();
        Vector2Int nextLoc = door.GetMatching().GetGridCoordinates();
        List<Room> frontier = new List<Room>();

        if (occupied.Contains(nextLoc))
        {
            return false;
        }
        
        // get gap from next location
        var currGap = GetGapFromLocation(nextLoc, occupied, occupiedRooms);
        occupied.Add(nextLoc);
        
        // find rooms that fill that gap
        foreach (Room r in rooms)
        {
            if (CheckRoomWithGap(currGap, r, occupied, nextLoc))
            {
                frontier.Add(r);
            }
        }
        
        // if no avail room types
        if (frontier.Count == 0)
        {
            return false;
        }

        while (frontier.Count != 0)
        {
            // choose random matching room type from frontier
            List<Room> avail = new List<Room>();
            foreach (var toAdd in frontier)
            {
                for (int i = 0; i < toAdd.weight; i++)
                {
                    avail.Add(toAdd);
                }
            }
            
            Room chosen = avail[Random.Range(0, avail.Count)];
            if (chosen == target)
            {
                targetRooms++;
            }
            frontier.Remove(chosen);
            occupiedRooms.Add(nextLoc, chosen);
            doors.Remove(door);
            
            // add doors from new room
            List<Door> added = new List<Door>();
            foreach (Door d in chosen.GetDoors(nextLoc))
            {
                if (d.GetMatching().GetGridCoordinates() != door.GetGridCoordinates())
                {
                    added.Add(d);
                }
            }

            foreach (Door d in added)
            {
                doors.Add(d);
            }
            
            bool res = GenerateWithBacktracking(occupied, occupiedRooms, doors, depth + 1);
            
            if (res)
            {
                foreach (Door d in chosen.GetDoors(nextLoc))
                {
                    var hallway = d.IsHorizontal()
                        ? horizontal_hallway.Place(d)
                        : vertical_hallway.Place(d);
                    
                    generated_objects.Add(hallway);
                }
                GameObject placed = chosen.Place(nextLoc);
                generated_objects.Add(placed);
                return true;
            }
            
            print("backtrack");
            foreach (Door d in added)
            {
                doors.Remove(d);
            }
            doors.Add(door);
            occupiedRooms.Remove(nextLoc);
            if (chosen == target)
            {
                targetRooms--;
            }
        }

        print("ran out of rooms at: " + nextLoc);
        occupied.Remove(nextLoc);
        return false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generated_objects = new List<GameObject>();
        rooms.Add(target);
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
            Generate();
    }
}
