using UnityEngine;
using System.Collections.Generic;
using System;

public class SnakeManager : MonoBehaviour
{
    public List<Snake> ListSnakes = new List<Snake>();
    public System.Action<Snake, bool> OnHandleClick;
    public System.Action<Snake> OnHandleSnakeEscape;

    public Vector3 Test1;
    public Vector3 Test2;
    public void InitSnakes(LevelManager levelManager)
    {
        foreach (var snake in ListSnakes)
        {
            snake.OnClick += OnHandleClick;
            snake.OnSnakeEscape += onHandleSnakeEscape;
            snake.OnSnakeEscape += levelManager.PortalManager.HandleSnakeEscape;
        }
    }

    private void onHandleSnakeEscape(Snake snakeEscape)
    {
        foreach (var snake in ListSnakes)
        {
            if (snake != snakeEscape)
            {
                snake.CheckTurn();
            }
        }
    }

    public bool IsWinGame()
    {
        foreach (var item in ListSnakes)
        {
            if (!item.IsDone) return false;
        }
        return true;
    }


    public static List<Tuple<Vector3, bool, Vector2Int>> DoubleListWithMidpoints(List<Tuple<Vector3, Vector2Int>> originalList)
    {
        List<Tuple<Vector3, bool, Vector2Int>> resultList = new List<System.Tuple<Vector3, bool, Vector2Int>>();

        if (originalList == null || originalList.Count < 2)
        {
            foreach (var item in originalList)
            {
                resultList.Add(Tuple.Create(item.Item1, true, item.Item2));
            }
            return resultList;
        }

        for (int i = 0; i < originalList.Count; i++)
        {
            resultList.Add(Tuple.Create(originalList[i].Item1, true, originalList[i].Item2));

            if (i < originalList.Count - 1)
            {
                Vector3 midpoint = (originalList[i].Item1 + originalList[i + 1].Item1) * 0.5f;
                resultList.Add(Tuple.Create(midpoint, false, Vector2Int.zero));
            }
        }
        Vector3 tail = (resultList[resultList.Count - 2].Item1 - resultList[resultList.Count - 1].Item1).normalized;
        float dis = 0.25f;
        Vector3 final = resultList[resultList.Count - 1].Item1 - tail * dis;
        resultList.Add(Tuple.Create(final, false, Vector2Int.zero));
        return resultList;
    }
    private List<Tuple<Vector3, bool, Vector2Int>> interpolateSnakeBody(GridManager gridManager, List<Vector2Int> cells)
    {
        int maxHeight = gridManager.Grids.Length;
        int maxWidth = gridManager.Grids[0].Widths.Length;
        List<Tuple<Vector3, Vector2Int>> listPos = new List<Tuple<Vector3, Vector2Int>>();
        foreach (var cell in cells)
        {
            Vector3 pos = new Vector3(GridHelper.GetX(cell.x, maxWidth), GridHelper.GetY(cell.y, maxHeight));
            listPos.Add(Tuple.Create(pos, cell));
        }
        List<Tuple<Vector3, bool, Vector2Int>> newList = DoubleListWithMidpoints(listPos);
        return newList;
    }
#if UNITY_EDITOR

    private Snake findSnakeOfPair(int pairingId)
    {
        foreach (var item in ListSnakes)
        {
            if (item.SnakeData.Id == pairingId) return item;
        }
        return null;
    }
    public void initShakes(GridManager gridManager, List<SnakeData> snakesInGrid)
    {
        int maxHeight = gridManager.Grids.Length;
        int maxWidth = gridManager.Grids[0].Widths.Length;
        GameDataSO gameDataSO = UnityEditor.AssetDatabase.LoadAssetAtPath<GameDataSO>("Assets/Scripts/ScriptableObjects/GameDataSO.asset");
        GameObject prefabHead = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/Head.prefab");
        GameObject prefabChain = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/Obstacle/Chains.prefab");
        GameObject prefabBomb = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/Obstacle/Bomb.prefab");
        GameObject prefabBody = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/Body.prefab");
        GameObject prefabBody_2 = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/Body_2.prefab");
        GameObject prefabKen = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/Body.prefab");
        GameObject prefabKenHead = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/BodyPodHead.prefab");
        GameObject prefabFoots = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Snakes/Foots.prefab");
        Singum prefabSingum = UnityEditor.AssetDatabase.LoadAssetAtPath<Singum>("Assets/Prefabs/GamePlay/Snakes/Singum.prefab");
        GameObject snakeManager = GameObject.Find("SnakeManager");
        Transform parentSnakes = snakeManager.transform;
        foreach (var sdt in snakesInGrid)
        {
            if (sdt.cells.Count == 0) continue;// ran ko add vao map
            List<Tuple<Vector3, bool, Vector2Int>> listPos = interpolateSnakeBody(gridManager, sdt.cells);
            GameObject goSnake = new GameObject();
            goSnake.name = sdt.name;
            goSnake.transform.SetParent(parentSnakes);
            goSnake.transform.position = Vector3.zero;
            Snake snake = goSnake.AddComponent<Snake>();
            Rigidbody rigidbody = goSnake.AddComponent<Rigidbody>();
            rigidbody.mass = 1000;
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            int segmentDistance = 1;
            
            Enums.SnakeObtacle snakeObtacle = sdt.Obtacle;
            GameObject parentKen = null;
            if (snakeObtacle == Enums.SnakeObtacle.Ken)
            {
                parentKen = new GameObject();
                parentKen.name = "Pods";
                parentKen.transform.SetParent(goSnake.transform);
            }
            for (int i = 0; i < listPos.Count; i++)
            {
                
                var data = listPos[i];
                var cell = data.Item3;
                bool isHead = i == 0;
                
                bool isTail = i == listPos.Count - 1;
                bool isPreTail = i == listPos.Count - 2;
                Snake.DataSegment dataSegment = new Snake.DataSegment();
                GameObject segment = null;
                GameObject kenObject = null;
                Vector3 pos = data.Item1;
                if (isHead)
                {
                    sdt.KeyDirection = GameUtils.GetDirection(listPos[1].Item1, listPos[0].Item1);
                    dataSegment.Direction = GameUtils.GetDirection(listPos[1].Item1, listPos[0].Item1);
                    segment = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabHead, goSnake.transform); ;// GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    if (snakeObtacle == Enums.SnakeObtacle.Xich)
                    {
                        GameObject chainGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabChain, segment.transform); ;// GameObject.CreatePrimitive
                        chainGO.GetComponentInChildren<TMPro.TextMeshPro>().text = sdt.Turn.ToString();
                    }else if (snakeObtacle == Enums.SnakeObtacle.Bomb)
                    {
                        GameObject bombGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabBomb, segment.transform); ;// GameObject.CreatePrimitive
                        bombGO.GetComponentInChildren<TMPro.TextMeshPro>().text = sdt.Turn.ToString();
                    }
                    if (snakeObtacle == Enums.SnakeObtacle.Ken)
                    {
                        kenObject = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabKenHead, parentKen.transform);
                    }

                }
                else if (i > 0)
                {
                    
                    dataSegment.Direction = GameUtils.GetDirection(listPos[i].Item1, listPos[i - 1].Item1);
                    segment = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabBody, goSnake.transform); ;// GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    if (snakeObtacle == Enums.SnakeObtacle.Ken)
                    {
                        kenObject = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabKen, parentKen.transform);
                    }
                }

                
                //cell.x * segmentDistance, cell.y * segmentDistance
                
                //pos.z = -0.1f * i;
                
                //segment.transform.parent = goSnake.transform;
                
                bool isBodyType2 = false;
                if (isHead)
                {
                    segment.transform.localScale = Vector3.one * 0.9f;
                    segment.name = "Head";
                    segment.transform.eulerAngles = snake.GetRotateHead(dataSegment.Direction);
                }
                else if(isPreTail)
                {
                    //isBodyType2 = true;
                    segment.transform.localScale = Vector3.one * 0.65f;
                    segment.name = i.ToString();
                    pos.z = -0.1f;
                    GameObject footsGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabFoots, segment.transform);
                    if (dataSegment.Direction == Enums.Direction.Left || dataSegment.Direction == Enums.Direction.Right) footsGO.transform.eulerAngles = new Vector3(0, 0, 90);
                    dataSegment.TransformFoots = footsGO.transform;
                }
                else if(isTail)
                {
                    segment.transform.localScale = Vector3.one * 0.3f;
                    segment.name = "Tail";
                    pos.z = -0.3f;
                    //isBodyType2 = true;
                }
                else
                {
                    //isBodyType2 = true;
                    segment.name = i.ToString();
                    segment.transform.localScale = Vector3.one * 0.85f;
                    GameObject footsGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefabFoots, segment.transform);
                    if (dataSegment.Direction == Enums.Direction.Left || dataSegment.Direction == Enums.Direction.Right) footsGO.transform.eulerAngles = new Vector3(0, 0, 90);
                    dataSegment.TransformFoots = footsGO.transform;
                }
                segment.transform.position = pos;
                if (kenObject != null)
                {
                    Renderer r = kenObject.GetComponentInChildren<Renderer>();
                    r.material = gameDataSO.WhiteMaterial;
                    kenObject.transform.SetParent(parentKen.transform);
                    kenObject.transform.position = pos;
                    kenObject.transform.localScale = segment.transform.localScale * 1.15f;
                    segment.gameObject.SetActive(false);
                }
                Quaternion quaternion = Snake.GetRotateBody(dataSegment.Direction);
                if (isBodyType2 && segment.transform != null) segment.transform.rotation = quaternion;
                Renderer renderer = segment.GetComponentInChildren<Renderer>();
                if (isBodyType2)// lay material thu 2
                {
                    var r = segment.transform.Find("than_2cham").GetComponentInChildren<Renderer>();
                    Material[] materials = r.sharedMaterials;

                    materials[2] = gameDataSO.GetMaterial(sdt.SelectedColor);
                    renderer.materials = materials;
                }
                else
                {
                    Transform mieng = segment.transform.Find("dau sau/Mount/mieng");
                    if (mieng != null)
                    {
                        mieng.GetComponent<Renderer>().material = gameDataSO.GetMaterial(sdt.SelectedColor);
                    }
                    renderer.material = gameDataSO.GetMaterial(sdt.SelectedColor);
                }
                //Material material = new Material(Shader.Find("Standard")); // Create new material
                //material.color = sdt.GetColor();
                
                snake.SnakeData = sdt;
                dataSegment.Transform = segment.transform;
                snake.Segments.Add(dataSegment);
                if (data.Item2)
                {
                    snake.ListCoors.Add(data.Item3);
                    gridManager.Set(cell.x, cell.y, Enums.GridValue.Available);
                }
                //Debug.Log($"Added segment at ({cell.x}, {cell.y}) for {sdt.name}, Color: {sdt.color}");
            }

            ListSnakes.Add(snake);
        }

        foreach (var item in ListSnakes)
        {
            if (item.SnakeData.IsParing)
            {
                Snake pair = findSnakeOfPair(item.SnakeData.PairingId);
                Singum singum = (Singum)UnityEditor.PrefabUtility.InstantiatePrefab(prefabSingum, transform);
                item.Singum = singum;
                pair.Singum = singum;
                Vector3 pos1 = item.Segments[1].Transform.position;
                Vector3 pos2 = pair.Segments[1].Transform.position;

                Vector3 pos3 = item.Segments[0].Transform.position;
                Vector3 pos4 = pair.Segments[0].Transform.position;
                singum.ListSnakePairing.Add(item);
                singum.ListSnakePairing.Add(pair);
                Vector3 a = (pos1 + pos2) / 2;
                Vector3 b = (pos3 + pos4) / 2;
                //(pos1 + pos2) / 2)
                //(pos3 + pos4) / 2
                Vector3 targetDirection = (pos3 - pos1).normalized;
                float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

                Vector3 final = (a + b) / 2;
                final.z = 0;
                singum.transform.position = final;

                singum.SpriteRenderer.transform.localScale = Vector3.one * 1.2f;
                singum.SpriteRenderer.transform.rotation = targetRotation;
                singum.SpriteRenderer.transform.localPosition = targetDirection * -0.3f;


            }
        }
        
    }

#endif
    //public void InitializeFromGridTool(List<SnakeData> gridSnakes)
    //{
    //    float segmentDistance = 1;
    //    foreach (var gridSnake in gridSnakes)
    //    {
    //        SnakeData2 snake = new SnakeData2 { color = gridSnake.color };
    //        foreach (Vector2Int cell in gridSnake.cells)
    //        {
    //            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //            segment.transform.position = new Vector3(cell.x * segmentDistance, cell.y * segmentDistance, 0);
    //            segment.transform.parent = transform; // Parent to SnakeController
    //            Renderer renderer = segment.GetComponent<Renderer>();
    //            Material material = new Material(Shader.Find("Standard")); // Create new material
    //            material.color = gridSnake.color;
    //            renderer.material = material;
    //            snake.segments.Add(segment.transform);
    //            Debug.Log($"Added segment at ({cell.x}, {cell.y}) for {gridSnake.name}, Color: {gridSnake.color}");
    //        }
    //        snakes.Add(snake);
    //    }
    //    Debug.Log($"SnakeController initialized with {snakes.Count} snakes");
    //}
    public void FetchOldSnake()
    {
        if (true || ListSnakes.IsNullOrEmpty())
        {
            Snake[] snakes = FindObjectsOfType<Snake>(true);
            foreach (var item in snakes)
            {
                ListSnakes.Add(item);
            }
        }
    }
    public void ClearSnakes()
    {
        FetchOldSnake();
        foreach (var sin in FindObjectsOfType<Singum>())
        {
            DestroyImmediate(sin.gameObject);
        }
        foreach (var snake in ListSnakes)
        {
            if (snake != null)
            {
                foreach (var segment in snake.Segments)
                {
                    if (segment.Transform != null)
                    {
                        DestroyImmediate(segment.Transform.gameObject);
                    }
                }
                DestroyImmediate(snake.gameObject);
            }
        }
        ListSnakes.Clear();
        Debug.Log("Cleared all snakes in SnakeController");
    }
}