using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        Combine();
    }

    void Combine()
    {
        // Lấy tất cả các MeshFilter của các đối tượng con
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // Duyệt qua từng MeshFilter và lưu trữ mesh và ma trận biến đổi của nó
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // Ẩn đối tượng con sau khi đã lấy mesh
            i++;
        }
        gameObject.SetActive(true);
        // Tạo một mesh mới
        Mesh combinedMesh = new Mesh();

        // Kết hợp các mesh sử dụng CombineInstance
        combinedMesh.CombineMeshes(combine);

        // Gán mesh mới cho MeshFilter của đối tượng hiện tại
        GetComponent<MeshFilter>().sharedMesh = combinedMesh;

        // Kích hoạt MeshRenderer của đối tượng hiện tại (nếu chưa được kích hoạt)
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}