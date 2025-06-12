using UnityEngine;
using System.Collections.Generic;
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
        occupied.Add(new Vector2Int(0, 0));
        iterations = 0;
        bool success = GenerateWithBacktracking(occupied, doors, 1);

        if (success && doors.Count > 0 && target != null)
        {
            Door openDoor = doors[Random.Range(0, doors.Count)];
            Vector2Int pos = openDoor.GetMatching().GetGridCoordinates();

            // Place the target room
            GameObject placed = target.Place(pos);
            generated_objects.Add(placed);

            // Connect with hallway
            Hallway h = openDoor.IsHorizontal() ? horizontal_hallway : vertical_hallway;
            generated_objects.Add(h.Place(openDoor));
        }
    }


    bool GenerateWithBacktracking(List<Vector2Int> occupied, List<Door> doors, int depth)
    {
        if (iterations > THRESHOLD) throw new System.Exception("Iteration limit exceeded");
        if (doors.Count == 0 && depth > 4)
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

            return true;
        }

        
        // pick door
        Door door = doors[Random.Range(0, doors.Count)];
        print(doors.Count);
        print(door.GetDirection());
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
            frontier.Remove(chosen);
            doors.Remove(door);
            
            // see if room type can fit gap
            GameObject placed = chosen.Place(door.GetMatching().GetGridCoordinates());
            Room pr = placed.GetComponent<Room>();
            bool reroll = false;

            foreach (Door d in pr.GetDoors())
            {
                // if neighboring rooms are occupied
                // otherwise we don't need to check for doors
                if (occupied.Contains(d.GetMatching().GetGridCoordinates()))
                {
                    bool found = false;
                    // if neighboring room has a door in that matching dir
                    foreach (Door remaining in doors)
                    {
                        if (d.GetMatching().IsMatching(remaining))
                        {
                            found = true;
                        }
                    }
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
                Destroy(placed);
                doors.Add(door);
                continue;
            }
            
            // add doors from new room
            foreach (Door d in chosen.GetDoors())
            {
                if (d.GetMatchingDirection() != door.GetDirection())
                {
                    doors.Add(d);
                }
            }
            occupied.Add(door.GetMatching().GetGridCoordinates());
            
            
            bool res = GenerateWithBacktracking(occupied, doors, depth + 1);
        
            // if false backtrack and try again
            if (!res)
            {
                foreach (Door d in chosen.GetDoors())
                {
                    doors.Remove(d);
                }
                Destroy(placed);
                doors.Add(door);
                occupied.Remove(door.GetMatching().GetGridCoordinates());
            }
            else
            {
                break;
            }
        }
        
        return true;
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
