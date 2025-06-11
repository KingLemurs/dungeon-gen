using UnityEngine;

public class AIWaypoint : MonoBehaviour
{
    public enum Type
    {
        SAFE, FORWARD, WALL
    }
    public Vector3 position;
    public Type type;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
        AIWaypointManager.Instance.AddWaypoint(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
