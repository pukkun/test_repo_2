using UnityEngine;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;
    public List<Transform> bodyParts = new List<Transform>();

    private Vector3 direction = Vector3.forward;
    public Queue<Vector3> previousPositions = new Queue<Vector3>(); // Sử dụng Queue để tối ưu hóa
    void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) ||
        Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            MoveSnake();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.back;
        if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;
    }

    void MoveSnake()
    {
        Transform head = bodyParts[0];
        previousPositions.Enqueue(head.position); // Lưu vị trí đầu rắn trước khi di chuyển

        head.position += direction * gridSize; // Di chuyển đầu rắn

        // Kiểm tra số lượng phần tử trong hàng đợi trước khi cập nhật thân rắn
        while (previousPositions.Count < bodyParts.Count)
        {
            previousPositions.Enqueue(head.position); // Điền thêm vị trí vào danh sách
        }

        // Cập nhật vị trí thân rắn
        for (int i = 0; i < bodyParts.Count; i++)
        {
            bodyParts[i].position = previousPositions.Dequeue();
        }
    }
}
