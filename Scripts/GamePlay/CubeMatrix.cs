using UnityEngine;

public class CubeMatrix : MonoBehaviour
{
    // Kích thước ma trận
    private int width = 10;  // Rộng
    private int depth = 3;   // Dày
    private int height = 7;  // Cao

    // Khoảng cách giữa các cube
    private float spacing = 1.5f;

    void Start()
    {
        GenerateCubeMatrix();
    }

    void GenerateCubeMatrix()
    {
        // Tính toán kích thước tổng thể của ma trận
        float totalWidth = (width - 1) * spacing;
        float totalHeight = (height - 1) * spacing;
        float totalDepth = (depth - 1) * spacing;

        // Tính toán offset để căn giữa ma trận
        Vector3 offset = new Vector3(
            -totalWidth / 2,  // Căn giữa theo trục X
            -totalHeight / 2, // Căn giữa theo trục Y
            -totalDepth / 2   // Căn giữa theo trục Z
        );

        // Vòng lặp để tạo cube theo chiều rộng, sâu và cao
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // Tạo cube mới
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    // Đặt vị trí của cube, có tính đến offset để căn giữa
                    cube.transform.position = new Vector3(
                        x * spacing,  // Vị trí theo trục X
                        y * spacing,  // Vị trí theo trục Y
                        z * spacing   // Vị trí theo trục Z
                    ) + offset;

                    // Đặt cube làm con của gameObject chứa script này
                    cube.transform.parent = transform;

                    // (Tùy chọn) Gán màu ngẫu nhiên cho cube
                    Renderer cubeRenderer = cube.GetComponent<Renderer>();
                    cubeRenderer.material.color = Random.ColorHSV();
                }
            }
        }
    }
}