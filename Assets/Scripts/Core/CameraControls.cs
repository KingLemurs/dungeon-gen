using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    public Vector2 move;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(move.x * Time.deltaTime * speed, move.y * Time.deltaTime * speed, 0, Space.World);
    }

    private void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

}
