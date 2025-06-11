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
        
        generated_objects.Add(start.Place(new Vector2Int(0,0)));
        List<Door> doors = start.GetDoors();
        List<Vector2Int> occupied = new List<Vector2Int>();
        occupied.Add(new Vector2Int(0, 0));
        iterations = 0;
        GenerateWithBacktracking(occupied, doors, 1);
    }


    bool GenerateWithBacktracking(List<Vector2Int> occupied, List<Door> doors, int depth)
    {
        if (iterations > THRESHOLD) throw new System.Exception("Iteration limit exceeded");
        if (doors.Count == 0)
        {
            return depth > 4 ? true : false;
        }
        
        // pick door
        Door door = doors[Random.Range(0, doors.Count)];
        print(doors.Count);
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
        
        // if no avail rooms
        if (frontier.Count == 0)
        {
            return false;
        }
        
        // choose random matching room from frontier
        foreach (Room chosen in frontier)
        {
            doors.Remove(door);
            // add doors from new room
            doors.AddRange(chosen.GetDoors());
            bool res = GenerateWithBacktracking(occupied, doors, depth + 1);

            // if true stop
            if (res)
            {
                chosen.Place(door.GetMatching().GetGridCoordinates());
                occupied.Add(door.GetMatching().GetGridCoordinates());
                break;
            }
            
            // if false backtrack and try again
            foreach (Door d in chosen.GetDoors())
            {
                doors.Remove(d);
            }
            doors.Add(door);
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
