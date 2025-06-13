using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

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
        GenerateWithBacktracking(occupied, occupiedRooms, doors, 1);
    }


    bool GenerateWithBacktracking(List<Vector2Int> occupied, Dictionary<Vector2Int, Room> occupiedRooms, List<Door> doors, int depth)
    {
        if (iterations > THRESHOLD) throw new System.Exception("Iteration limit exceeded");
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

            return depth > 4;
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
        List<Room> frontier = new List<Room>();
        
        foreach (Room room in rooms)
        {
            foreach (Door d in room.GetDoors())
            {
                if (d.GetDirection() == dir)
                {
                    frontier.Add(room);
                    break;
                }
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
            var location = door.GetMatching().GetGridCoordinates();
            frontier.Remove(chosen);
            
            // see if room type can fit gap
            bool reroll = false;

            print("location: " + door.GetGridCoordinates());
            print("just placed: " + location);
            
            // OFFSET IS NEW LOCATION
            foreach (Door d in chosen.GetDoors(location))
            {
                // if neighboring rooms are occupied
                // otherwise we don't need to check for doors
                var coords = d.GetMatching().GetGridCoordinates();
                print("to match: " + coords + " " + d.GetDirection());
                if (occupied.Contains(coords) && occupiedRooms.ContainsKey(coords))
                {
                    bool found = occupiedRooms[coords].HasDoorOnSide(d.GetMatchingDirection());
                    
                    // if we cannot find a matching door, that means there is a room there with a wall
                    if (!found)
                    {
                        reroll = true;
                        break;
                    }
                    
                }
            }

            if (reroll)
            {
                print("reroll");
                continue;
            }
            print("found all matching doors");
            
            // add doors from new room
            List<Door> added = new List<Door>();
            foreach (Door d in chosen.GetDoors(location))
            {
                if (d.GetMatchingDirection() != door.GetDirection())
                {
                    doors.Add(d);
                    added.Add(d);
                }
            }
            occupied.Add(location);
            occupiedRooms.Add(location, chosen);
            doors.Remove(door);
            
            
            bool res = GenerateWithBacktracking(occupied, occupiedRooms, doors, depth + 1);
        
            // if false backtrack and try again
            if (!res)
            {
                foreach (Door d in added)
                {
                    doors.Remove(d);
                }
                doors.Add(door);
                occupied.Remove(location);
                occupiedRooms.Remove(location);
            }
            else
            {
                foreach (Door d in chosen.GetDoors(location))
                {
                    var hallway = d.IsHorizontal()
                        ? horizontal_hallway.Place(d)
                        : vertical_hallway.Place(d);
                    
                    generated_objects.Add(hallway);
                }
                GameObject placed = chosen.Place(location);
                generated_objects.Add(placed);
                return true;
            }
        }


        return false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generated_objects = new List<GameObject>();
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
            Generate();
    }
}
