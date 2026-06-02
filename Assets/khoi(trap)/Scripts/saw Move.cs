using UnityEngine;

public class SawTrapMove : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.right;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    public float rotateSpeed = 360f;

    private Vector2 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + moveDirection.normalized * movement;

        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}