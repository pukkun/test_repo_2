using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float gridSize = 1f;
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            GetInput();
        }
        else
        {
            MoveToTarget();
        }
    }

    void GetInput()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W)) inputDirection = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.S)) inputDirection = Vector3.back;
        if (Input.GetKeyDown(KeyCode.A)) inputDirection = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) inputDirection = Vector3.right;

        if (inputDirection != Vector3.zero)
        {
            targetPosition = transform.position + (inputDirection * gridSize);
            isMoving = true;
        }
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }
}
