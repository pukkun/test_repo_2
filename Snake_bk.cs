using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake_bk : MonoBehaviour
{
    public List<Transform> segments = new List<Transform>();
    public float moveSpeed = 1f;
    public float segmentDistance = 1f; 
    private Vector3 direction = Vector3.right; 
    private bool moveByHead = true;
    private List<Vector3> positions = new List<Vector3>();
    public List<Vector2Int> ListCoors = new List<Vector2Int>();

    public List<Vector3> TestTarget = new List<Vector3>();
    void Start()
    {
        foreach (Transform segment in segments)
        {
            positions.Add(segment.position);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (TestTarget.Count > 0)
            {
                move(TestTarget[0]);
            }
        }
        // Điều khiển hướng bằng phím
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector3.up;
            MoveSnake();
            
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Vector3.down;
            MoveSnake();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector3.left;
            MoveSnake();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector3.right;
            MoveSnake();
        }
        // Chuyển đổi chế độ di chuyển
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moveByHead = !moveByHead;
        }

    }
    private void move(Vector3 newHeadPos)
    {
        float speed = 5f; // Adjust this to control how fast the snake moves
        float smoothStep = speed * Time.deltaTime; // Smooth movement based on time
        //Vector3 newHeadPos = segments[0].position + direction * segmentDistance;

        // Kiểm tra va chạm với thân (bỏ qua đầu)
        for (int i = 1; i < segments.Count; i++)
        {
            if (Vector3.Distance(newHeadPos, segments[i].position) < 0.1f)
            {
                Debug.Log("Chạm thân! Di chuyển bị chặn.");
                return; // Chặn di chuyển
            }
        }

        // Di chuyển từ đầu
        positions.Insert(0, newHeadPos);
        positions.RemoveAt(positions.Count - 1);

        //// Cập nhật vị trí các khối
        for (int i = 0; i < segments.Count; i++)
        {
            Vector3 targetPos = positions[i];
            segments[i].position = positions[i];
        }
    }
    void MoveSnake()
    {
        if (moveByHead)
        {
            // Tính vị trí mới cho đầu
            Vector3 newHeadPos = segments[0].position + direction * segmentDistance;

            // Kiểm tra va chạm với thân (bỏ qua đầu)
            for (int i = 1; i < segments.Count; i++)
            {
                if (Vector3.Distance(newHeadPos, segments[i].position) < 0.1f)
                {
                    Debug.Log("Chạm thân! Di chuyển bị chặn.");
                    return; // Chặn di chuyển
                }
            }

            // Di chuyển từ đầu
            positions.Insert(0, newHeadPos);
            positions.RemoveAt(positions.Count - 1);

            // Cập nhật vị trí các khối
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].position = positions[i];
            }
        }
        else
        {
            // Tính vị trí mới cho đuôi, nhưng giữ hướng điều khiển nhất quán
            Vector3 newTailPos = segments[segments.Count - 1].position + direction * segmentDistance;

            // Kiểm tra va chạm với thân (bỏ qua đuôi)
            for (int i = 0; i < segments.Count - 1; i++)
            {
                if (Vector3.Distance(newTailPos, segments[i].position) < 0.1f)
                {
                    Debug.Log("Chạm thân! Di chuyển bị chặn.");
                    return; // Chặn di chuyển
                }
            }

            // Di chuyển từ đuôi
            positions.RemoveAt(0);
            positions.Add(newTailPos);

            // Cập nhật vị trí các khối
            for (int i = 0; i < segments.Count; i++)
            {
                segments[i].position = positions[i];
            }
        }
    }
}
