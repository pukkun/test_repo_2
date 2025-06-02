using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Enums;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;
using System.Collections;
using Unity.EditorCoroutines.Editor;

public class SnakeGridTool : EditorWindow
{
    int levelToReset = 1;
    public GridManager.CellDataArray[] Grids;
    public static List<SnakeData> ShareSnake = new List<SnakeData>(); // List of snakes
    
    private float sizeOfCell = 1.36f;
    private int gridWidth = 5; // Grid width
    private int gridHeight = 5; // Grid height
    private int editorGridWidth = 5; // Grid width
    private int editorGridHeight = 5; // Grid height
    private int currentSnakeIncreaseId;
    private int currentPortalIncreaseId;
    private bool[,] occupiedGrid; // Tracks occupied cells (true = occupied)
    private List<SnakeData> snakes = new List<SnakeData>(); // List of snakes
    private List<PortalData> portals = new List<PortalData>(); // List of portal
    private bool isInitialized = false;
    private int selectedSnakeIndex = -1; // Currently edited snake
    private int selectedPortalIndex = -1; // Currently edited portal

    EditType currentEditor = EditType.Grid;
    private Vector2 mainScrollPosition;
    private Vector2 scrollPosition;
    private List<string> prefabNames = new List<string>();
    private int currentPage = 0;
    private const int ITEMS_PER_PAGE = 20;
    private const int MAX_COLUMN_TARGET = 10;
    private int currentLevelSelected;
    GameDataSO gameDataSO = null;
    int limitTime = -1;
    Texture2D textRemoveCell;

    [MenuItem("Tools/Snake Grid Tool %#T")]
    public static void ShowWindow()
    {
        GetWindow<SnakeGridTool>("Snake Grid Tool");
    }
    private void OnEnable()
    {
        if(gameDataSO == null) gameDataSO =  UnityEditor.AssetDatabase.LoadAssetAtPath<GameDataSO>("Assets/Scripts/ScriptableObjects/GameDataSO.asset");
        textRemoveCell = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/unmovable_grid.png");
    }
    private void checkScene()
    {
        if (SceneManager.GetActiveScene().name != SceneConstant.SCENE_GAME)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_GAME + ".unity");
        }
    }
    private void OnGUI()
    {
        EditorGUIUtility.wideMode = true;
        mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition, false, true);
        EditorGUILayout.BeginHorizontal();
        drawListLevelSelection();
        EditorGUILayout.BeginVertical();
        // Grid size input
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        editorGridWidth = EditorGUILayout.IntField("Width", editorGridWidth);
        editorGridHeight = EditorGUILayout.IntField("Height", editorGridHeight);

        // Ensure valid size
        if (editorGridWidth < 1) editorGridWidth = 1;
        if (editorGridHeight < 1) editorGridHeight = 1;
        // Initialize grid button
        if (GUILayout.Button("Initialize Grid"))
        {
            checkScene();
            InitializeGrid();
        }
        if (GUILayout.Button("Fetch From Scene"))
        {
            checkScene();
            getDataFromScene();
        }

        if (isInitialized)
        {
            GUILayout.Space(10);
            GUILayout.Label("Goal Columns Management", EditorStyles.boldLabel);
            if (GUILayout.Button("Open Targets Windows", GUILayout.Width(200)))
            {
                checkScene();
                GetWindow<MatrixColorTool>("Matrix Color Tool");
            }
            GUILayout.Space(10);
            currentEditor = (EditType)EditorGUILayout.EnumPopup(currentEditor);
            if (currentEditor == EditType.Snake)
            {
                
                GUIStyle buttonStyleSelected = new GUIStyle(GUI.skin.button);
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyleSelected.normal.textColor = Color.green;
                buttonStyleSelected.hover.textColor = Color.green;
                GUIStyle styleManagement = new GUIStyle(EditorStyles.label);
                GUIStyle styleManagementGreen = new GUIStyle(EditorStyles.label);
                styleManagementGreen.normal.textColor = Color.green;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Snake Management");
                EditorGUILayout.EndHorizontal();
                // Snake list management
                for (int i = 0; i < snakes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Id: ", GUILayout.MaxWidth(30));
                    EditorGUILayout.LabelField(snakes[i].Id.ToString(), GUILayout.MaxWidth(30));
                    EditorGUILayout.LabelField("Pair: ", GUILayout.MaxWidth(30));
                    snakes[i].IsParing = EditorGUILayout.Toggle(snakes[i].IsParing, GUILayout.MaxWidth(30));
                    if (snakes[i].IsParing)
                    {
                        //string[] array = getListIdCanPairing(snakes[i].Id).ToArray();
                        //snakes[i].aaa = array[EditorGUILayout.Popup(aaa, array.ToArray(), GUILayout.MaxWidth(200))].ToInt();
                        snakes[i].PairingId = EditorGUILayout.IntField(snakes[i].PairingId, GUILayout.MaxWidth(200));
                        //snakes[i].PairingId = array[snakes[i].CurrentIndexPair].ToInt();
                    }
                    else
                    {
                        snakes[i].PairingId = 0;
                    }
                    string textCell = $"   Cell: {snakes[i].cells.Count.ToString()}";
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.normal.textColor = snakes[i].cells.Count > 0 ? Color.green : Color.red;
                    EditorGUILayout.LabelField(textCell, style, GUILayout.MaxWidth(80));
                    EditorGUILayout.LabelField("      Obstacle: ", GUILayout.MaxWidth(80));
                    snakes[i].Obtacle = (SnakeObtacle)EditorGUILayout.EnumPopup(snakes[i].Obtacle);
                    EditorGUILayout.LabelField("      Turn: ", GUILayout.MaxWidth(80));
                    snakes[i].Turn = EditorGUILayout.IntField(snakes[i].Turn, GUILayout.MaxWidth(200));
                    EditorGUILayout.LabelField("      Color: ", GUILayout.MaxWidth(80));
                    snakes[i].SelectedColor = (BlockColor)EditorGUILayout.EnumPopup(snakes[i].SelectedColor);
                    //snakes[i].color = EditorGUILayout.ColorField(snakes[i].color);
                    Rect colorRect = GUILayoutUtility.GetRect(20, 20);
                    EditorGUI.DrawRect(colorRect, GameUtils.GetColorFromOption(snakes[i].SelectedColor));
                    if (GUILayout.Button("Select", i == selectedSnakeIndex ? buttonStyleSelected : buttonStyle, GUILayout.Width(60)))
                    {
                        selectedSnakeIndex = i;
                        Debug.Log($"Selected snake: {snakes[i].name}, Color: {snakes[i].GetColor()}");
                        Repaint();
                    }
                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        RemoveSnake(i);
                        if (selectedSnakeIndex >= snakes.Count) selectedSnakeIndex = snakes.Count - 1;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5);
                }


                GUILayout.Space(10);

                // Add new snake button
                if (GUILayout.Button("Add New Snake"))
                {
                    BlockColor random = (BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1));
                    while (true)
                    {
                        if (random.ToString().Contains("Remove"))
                        {
                            random = (BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1));
                        }
                        else
                        {
                            break;
                        }
                    }
                    AddNewSnake(random);
                }
                
                
            }
            if (currentEditor == EditType.Portal)
            {
                drawPortals();
            }
            if (true || selectedSnakeIndex >= 0 && selectedSnakeIndex < snakes.Count)
            {
                GUILayout.Space(10);
                if(selectedSnakeIndex >= 0 && selectedSnakeIndex < snakes.Count) GUILayout.Label($"Editing: {snakes[selectedSnakeIndex].name} (Color: {snakes[selectedSnakeIndex].GetColor()})", EditorStyles.boldLabel);

                // Draw grid with custom square cells
                for (int y = gridHeight - 1; y >= 0; y--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int x = 0; x < gridWidth; x++)
                    {
                        DrawGridCell(x, y);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.Space(10);
                
                //if (GUILayout.Button("Create Grid"))
                //{

                //}
                limitTime = EditorGUILayout.IntField("Limit Time", limitTime);
                //if (GUILayout.Button("Save All Snakes"))
                //{
                //    SaveSnakes();
                //}
                if (GUILayout.Button("Init Level"))
                {
                    checkScene();
                    LevelManager levelManager = FindObjectOfType<LevelManager>();
                    if (!levelManager)
                    {
                        GameObject goLevel = new GameObject();
                        goLevel.name = "Level";
                        levelManager = goLevel.AddComponent<LevelManager>();
                    }
                    levelManager.transform.position = Vector3.zero;
                    levelManager.transform.localScale = Vector3.one;
                    createGrid();
                    InitializeSnakeController();
                    initHoleTarget();
                    levelManager.LimitTime = limitTime;
                    levelManager.transform.position = GameStatic.POS_LEVEL;
                    resize(levelManager);
                }
            }
            else
            {
                GUILayout.Label("Select a snake to edit.", EditorStyles.boldLabel);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }
    private List<string> getListIdCanPairing(int id)
    {
        List<string> result = new List<string>();
        List<SnakeData> newList = snakes.OrderBy(o => o.Id).ToList();
        foreach (var item in newList)
        {
            if (item.Id != id) result.Add(item.Id.ToString());
        }
        return result;
    }
    void drawPortals()
    {
        GUIStyle styleManagement = new GUIStyle(EditorStyles.label);
        GUIStyle styleManagementGreen = new GUIStyle(EditorStyles.label);
        styleManagementGreen.normal.textColor = Color.green;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Portal Management");
        EditorGUILayout.EndHorizontal();
        GUIStyle buttonStyleSelected = new GUIStyle(GUI.skin.button);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyleSelected.normal.textColor = Color.green;
        buttonStyleSelected.hover.textColor = Color.green;
        for (int i = 0; i < portals.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle(EditorStyles.label);
            EditorGUILayout.LabelField("Id: ", GUILayout.MaxWidth(30));
            EditorGUILayout.LabelField(portals[i].Id.ToString(), GUILayout.MaxWidth(30));
            EditorGUILayout.LabelField("      Color: ", GUILayout.MaxWidth(80));
            portals[i].SelectedColor = (BlockColor)EditorGUILayout.EnumPopup(portals[i].SelectedColor);
            EditorGUILayout.LabelField("      Turn: ", GUILayout.MaxWidth(80));
            portals[i].Turn = EditorGUILayout.IntField(portals[i].Turn, GUILayout.MaxWidth(200));
            EditorGUILayout.LabelField("CoveredTurn: ", GUILayout.MaxWidth(80));
            portals[i].CoveredTurn = EditorGUILayout.IntField(portals[i].CoveredTurn, GUILayout.MaxWidth(200));
            //snakes[i].color = EditorGUILayout.ColorField(snakes[i].color);
            Rect colorRect = GUILayoutUtility.GetRect(20, 20);
            EditorGUI.DrawRect(colorRect, GameUtils.GetColorFromOption(portals[i].SelectedColor));
            //GUI.DrawTexture(colorRect, drawCircle(GameUtils.GetColorFromOption(portals[i].SelectedColor)));
            if (GUILayout.Button("Select", i == selectedPortalIndex ? buttonStyleSelected : buttonStyle, GUILayout.Width(60)))
            {
                selectedPortalIndex = i;
                Debug.Log($"Selected portal Color: {portals[i].GetColor()}");
                Repaint();
            }
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                removePortal(i);
                if (selectedPortalIndex >= portals.Count) selectedPortalIndex = portals.Count - 1;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }
        if (GUILayout.Button("Add New Portal"))
        {
            BlockColor random = (BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1));
            while (true)
            {
                if (random.ToString().Contains("Remove"))
                {
                    random = (BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1));
                }
                else
                {
                    break;
                }
            }
            addNewPortal(random);
        }
    }
    private void getDataFromScene()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (!levelManager)
        {
            EditorUtility.DisplayDialog("Error", "Data sai. Hãy tạo map khác", "OK");
            return;
        }if (!gridManager)
        {
            EditorUtility.DisplayDialog("Error", "Data sai. Hãy tạo map khác", "OK");
            return;
        }
        limitTime = levelManager.LimitTime;
        levelManager.transform.position = Vector3.zero;
        gridHeight = gridManager.Grids.Length;
        gridWidth = gridManager.Grids[0].Widths.Length;
        occupiedGrid = new bool[gridWidth, gridHeight];
        Grids = gridManager.Grids;
        snakes.Clear();
        portals.Clear();
        ShareSnake.Clear();
        Snake[] snakeItems = FindObjectsOfType<Snake>();
        Array.Sort(snakeItems, (s1, s2) => s1.SnakeData.Id.CompareTo(s2.SnakeData.Id));
        for (int i = 0; i < snakeItems.Length; i++)
        {
            SnakeData newSnake = AddNewSnake(snakeItems[i].SnakeData.SelectedColor, snakeItems[i].SnakeData.Id);
            foreach (var item in snakeItems[i].SnakeData.cells)
            {
                occupiedGrid[item.x, item.y] = true;
                
            }
            newSnake.cells.AddRange(snakeItems[i].SnakeData.cells);
            newSnake.Turn = snakeItems[i].SnakeData.Turn;
            newSnake.Obtacle = snakeItems[i].SnakeData.Obtacle;
            newSnake.IsParing = snakeItems[i].SnakeData.IsParing;
            newSnake.PairingId = snakeItems[i].SnakeData.PairingId;
            newSnake.CurrentIndexPair = snakeItems[i].SnakeData.CurrentIndexPair;
            if (currentSnakeIncreaseId < snakeItems[i].SnakeData.Id) currentSnakeIncreaseId = snakeItems[i].SnakeData.Id;
        }
        currentSnakeIncreaseId++;

        PortalTarget[] portalTargets = FindObjectsOfType<PortalTarget>();
        Array.Sort(portalTargets, (s1, s2) => s1.PortalData.Id.CompareTo(s2.PortalData.Id));
        for (int i = 0; i < portalTargets.Length; i++)
        {
            PortalData newPortal = addNewPortal(portalTargets[i].PortalData.SelectedColor, portalTargets[i].PortalData.Id);
            foreach (var item in portalTargets[i].PortalData.Coord)
            {
                occupiedGrid[item.x, item.y] = true;
            }
            newPortal.Coord.AddRange(portalTargets[i].PortalData.Coord);
            newPortal.Turn = portalTargets[i].PortalData.Turn;
            newPortal.CoveredTurn = portalTargets[i].PortalData.CoveredTurn;
            if (currentPortalIncreaseId < portalTargets[i].PortalData.Id) currentPortalIncreaseId = portalTargets[i].PortalData.Id;
        }
        currentPortalIncreaseId++;

        selectedSnakeIndex = 0;
        isInitialized = true;
        Debug.Log("Grid initialized: " + gridWidth + "x" + gridHeight);
        Repaint();

        levelManager.transform.localScale = Vector3.one;
        createGrid();
        InitializeSnakeController();
        initHoleTarget();
        levelManager.transform.position = GameStatic.POS_LEVEL;
        resize(levelManager);
    }
    void initHoleTarget()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        // Find SnakeController in the scene
        PortalManager controller = FindObjectOfType<PortalManager>();
        if (controller == null)
        {
            GameObject go = new GameObject("PortalManager");
            go.transform.SetParent(levelManager.transform);
            controller = go.AddComponent<PortalManager>();
            controller.transform.position = Vector3.zero;
        }
        controller.ClearPortal();
        controller.CreatePortal(levelManager.GridManager, portals);
        levelManager.PortalManager = controller;
    }
    void resize(LevelManager levelManager)
    {
        GridManager gridManager = levelManager.GridManager;
        int minSize = 6;
        int minSizeW = 6;
        int minSizeH = 7;
        int maxSize = 8;
        int maxSizeW = 8;
        int maxSizeH = 9;

        float SizeW = gridManager.Grids[0].Widths.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE);
        float SizeH = gridManager.Grids.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE);

        float scaleW;
        if (SizeW > maxSize)
            scaleW = maxSize / SizeW;
        else if (SizeW < minSizeW)
            scaleW = minSizeW / SizeW;
        else
            scaleW = maxSizeW / SizeW;

        float scaleH;
        if (SizeH > maxSizeH)
            scaleH = maxSizeH / SizeH;
        else if (SizeH < minSizeH)
            scaleH = minSizeH / SizeH;
        else
            scaleH = maxSizeH / SizeH;
        float finalScale = Mathf.Min(scaleW, scaleH);



        float bigger = SizeH > SizeW ? SizeH : SizeW;
        float smaller = SizeH > SizeW ? SizeW : SizeH;
        float newFactor = 1f;

        float maxFactor = maxSize / bigger;
        float minFactor = minSize / smaller;
        if (SizeH > maxSize || SizeW > maxSize)
        {
            newFactor = maxFactor;
        }
        else if (SizeH < minSize || SizeW < minSize)
        {
            newFactor = Math.Min(minFactor, maxFactor);
        }
        levelManager.transform.localScale = Vector3.one * finalScale;
        Vector3 scale = levelManager.transform.localScale;
        SizeW = gridManager.Grids[0].Widths.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE) * scale.x;
        SizeH = gridManager.Grids.Length * (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE) * scale.y;
    }
    private void InitializeGrid()
    {
        gridWidth = editorGridWidth;
        gridHeight = editorGridHeight;
        occupiedGrid = new bool[gridWidth, gridHeight];
        ShareSnake.Clear();
        snakes.Clear();
        portals.Clear();
        selectedSnakeIndex = -1;
        selectedPortalIndex = -1;
        currentSnakeIncreaseId = 0;

        Grids = new GridManager.CellDataArray[gridHeight];
        for (int i = 0; i < gridHeight; i++)
        {
            Grids[i] = new GridManager.CellDataArray();
        }
        foreach (var item in Grids)
        {
            item.Widths = new CellData[gridWidth];
            for (int i = 0; i < item.Widths.Length; i++)
            {
                item.Widths[i] = new CellData();
            }
        }
        isInitialized = true;
        Debug.Log("Grid initialized: " + gridWidth + "x" + gridHeight);
        Repaint();
    }
    private PortalData addNewPortal(BlockColor blockColor, int id = -1)
    {
        int newId = id < 0 ? GetNextPortalId() : id;
        PortalData newPortal = new PortalData
        {
            Id = newId,
            SelectedColor = blockColor
            //(BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1))
        };
        portals.Add(newPortal);
        selectedPortalIndex = portals.Count - 1;
        Debug.Log($"Added new portal Color: {newPortal.GetColor()}");
        Repaint();
        return newPortal;
    }
    private SnakeData AddNewSnake(BlockColor blockColor, int id = -1)
    {
        int newId = id < 0 ? GetNextSnakeId() : id;
        SnakeData newSnake = new SnakeData
        {
            Id = newId,
            name = $"Snake {snakes.Count + 1}",
            SelectedColor = blockColor
            //(BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1))
        };
        snakes.Add(newSnake);
        ShareSnake.Add(newSnake);
        selectedSnakeIndex = snakes.Count - 1;
        Debug.Log($"Added new snake: {newSnake.name}, Color: {newSnake.GetColor()}");
        Repaint();
        return newSnake;
    }

    private void RemoveSnake(int index)
    {
        foreach (Vector2Int cell in snakes[index].cells)
        {
            occupiedGrid[cell.x, cell.y] = false;
            SetValueGrid(cell.x, cell.y, GridValue.None);
        }
        Debug.Log($"Removed snake: {snakes[index].name}");
        snakes.RemoveAt(index);
        ShareSnake.RemoveAt(index);
        Repaint();
    }

    private void removePortal(int index)
    {
        foreach (Vector2Int cell in portals[index].Coord)
        {
            occupiedGrid[cell.x, cell.y] = false;
            SetValueGrid(cell.x, cell.y, GridValue.None);
        }
        
        Debug.Log($"Removed portal: {portals[index].SelectedColor}");
        portals.RemoveAt(index);
        Repaint();
    }
    bool trueIndexSnake()
    {
        return (selectedSnakeIndex >= 0 && selectedSnakeIndex < snakes.Count);
    }
    bool trueIndexPortal()
    {
        return (selectedPortalIndex >= 0 && selectedPortalIndex < portals.Count);
    }
    private void HandleCellClick(int x, int y)
    {
        Vector2Int cell = new Vector2Int(x, y);
        if (currentEditor == EditType.Grid)
        {
            if (gridAvailable(x, y)) SetValueGrid(x, y, Enums.GridValue.Removed);
            else SetValueGrid(x, y, Enums.GridValue.None);
            return;
        }
        if (currentEditor == EditType.Portal)
        {
            // tao portal
            if (!trueIndexPortal()) return;

            PortalData currentPortal = portals[selectedPortalIndex];
            Debug.Log($"Clicked cell ({x}, {y}), Portal Color: {currentPortal.GetColor()}, Cells: {currentPortal.Coord.Count}");
            if (currentPortal.Coord.Contains(cell))
            {

            }
            else
            {
                // Add cell to portal
                if (occupiedGrid[x, y])
                {
                    Debug.LogWarning($"Cell ({x}, {y}) is already occupied by another snake!");
                    return;
                }

                // Allow first cell or adjacent cells
                if (currentPortal.Coord.Count == 0)
                {
                    if (gridAvailable(x, y))
                    {
                        SetValueGrid(x, y, GridValue.Portal, currentPortal.SelectedColor, currentPortal.Turn);
                        currentPortal.Coord.Add(cell);
                        occupiedGrid[x, y] = true;
                        
                        Repaint();
                    }
                    else
                    {
                        Debug.LogError("Không thể đặt vào ô đã remove");
                    }
                }
            }

            return;
        }


        //tao snake
        if (!trueIndexSnake()) return;
        
        
        SnakeData currentSnake = snakes[selectedSnakeIndex];
        Debug.Log($"Clicked cell ({x}, {y}), Snake: {currentSnake.name}, Color: {currentSnake.GetColor()}, Cells: {currentSnake.cells.Count}");
        if (currentSnake.cells.Contains(cell))
        {
            //// Remove cell from snake
            //currentSnake.cells.Remove(cell);
            //occupiedGrid[x, y] = false;
            //Debug.Log($"Removed cell ({x}, {y}) from {currentSnake.name}, Set to gray");
            //Repaint();
            //// Check connectivity
            //if (currentSnake.cells.Count > 0 && !IsSnakeConnected(currentSnake.cells))
            //{
            //    Debug.LogWarning("Snake is disconnected! Reverting change.");
            //    currentSnake.cells.Add(cell);
            //    occupiedGrid[x, y] = true;
            //    Repaint();
            //}
        }
        else
        {
            // Add cell to snake
            if (occupiedGrid[x, y])
            {
                Debug.LogWarning($"Cell ({x}, {y}) is already occupied by another snake!");
                return;
            }

            // Allow first cell or adjacent cells
            if (currentSnake.cells.Count == 0 || IsAdjacentToSnake(cell, currentSnake.cells))
            {
                if (gridAvailable(x, y))
                {
                    SetValueGrid(x, y, GridValue.Available, currentSnake.SelectedColor);
                    currentSnake.cells.Add(cell);
                    occupiedGrid[x, y] = true;
                    Debug.Log($"Added cell ({x}, {y}) to {currentSnake.name}, Should be color: {currentSnake.GetColor()}");
                    Repaint();
                }
                else
                {
                    Debug.LogError("Không thể đặt vào ô đã remove");
                }
            }
            else
            {
                Debug.LogWarning($"Cell ({x}, {y}) must be adjacent to the snake!");
                return;
            }
        }
    }

    private bool IsAdjacentToSnake(Vector2Int cell, List<Vector2Int> snakeCells)
    {
        if (snakeCells.Count > 0)
        {
            Vector2Int lastCell = snakeCells[snakeCells.Count - 1];
            if (lastCell.x != cell.x && lastCell.y != cell.y) return false;
            if (Mathf.Abs(cell.x - lastCell.x) + Mathf.Abs(cell.y - lastCell.y) > 1) return false;
        }
        
            //foreach (Vector2Int snakeCell in snakeCells)
            //{
            //    if (Mathf.Abs(cell.x - snakeCell.x) + Mathf.Abs(cell.y - snakeCell.y) > 1)
            //    {
            //        return false;;
            //    }
            //}
            return true;
    }

    private bool IsSnakeConnected(List<Vector2Int> cells)
    {
        if (cells.Count <= 1) return true;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(cells[0]);
        visited.Add(cells[0]);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (cells.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        bool isConnected = visited.Count == cells.Count;
        Debug.Log($"Snake connectivity check: {isConnected} (Visited: {visited.Count}, Total: {cells.Count})");
        return isConnected;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (cell.x > 0) neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        if (cell.x < gridWidth - 1) neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y > 0) neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.y < gridHeight - 1) neighbors.Add(new Vector2Int(cell.x, cell.y + 1));
        return neighbors;
    }
    int extractNumber(string input)
    {
        Match match = System.Text.RegularExpressions.Regex.Match(input, @"\d+");
        return match.Success ? int.Parse(match.Value) : 0;
    }
    private int getSnakeIdFromCell(int x, int y)
    {
        Vector2Int cell = new Vector2Int(x, y);
        // Check all snakes to find which one owns this cell
        for (int i = 0; i < snakes.Count; i++)
        {
            if (snakes[i].cells.Contains(cell))
            {
                return snakes[i].Id;//extractNumber(snakes[i].name);
            }
        }
        return 0;
    }
    private int getPortalIdFromCell(int x, int y)
    {
        Vector2Int cell = new Vector2Int(x, y);
        // Check all snakes to find which one owns this cell
        for (int i = 0; i < portals.Count; i++)
        {
            if (portals[i].Coord.Contains(cell))
            {
                return portals[i].Id;//extractNumber(snakes[i].name);
            }
        }
        return 0;
    }
    private bool isHeadSnakeFromCell(int x, int y)
    {
        Vector2Int cell = new Vector2Int(x, y);
        // Check all snakes to find which one owns this cell
        for (int i = 0; i < snakes.Count; i++)
        {
            if (snakes[i].cells.Count > 0)
            {
                if (snakes[i].cells[0].x == x && snakes[i].cells[0].y == y) return true;
            }
        }
        return false;
    }
    private bool isTailSnakeFromCell(int x, int y)
    {
        Vector2Int cell = new Vector2Int(x, y);
        // Check all snakes to find which one owns this cell
        for (int i = 0; i < snakes.Count; i++)
        {
            if (snakes[i].cells.Count > 0)
            {
                if (snakes[i].cells[snakes[i].cells.Count - 1].x == x && snakes[i].cells[snakes[i].cells.Count - 1].y == y) return true;
            }
        }
        return false;
    }
    private Color GetCellColor(int x, int y)
    {
        Vector2Int cell = new Vector2Int(x, y);
        for (int i = 0; i < snakes.Count; i++)
        {
            if (snakes[i].cells.Contains(cell))
            {
                return snakes[i].GetColor();
            }
        }
        for (int i = 0; i < portals.Count; i++)
        {
            if (portals[i].Coord.Contains(cell))
            {
                return portals[i].GetColor();
            }
        }
        return Color.gray;
    }

    private Texture2D MakeTextureWithColor(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
    private Texture2D MakeTexture2Color(Color color, Color color2, int size)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        int borderWidth = 3;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (x <= borderWidth || x >= size - borderWidth || y <= borderWidth || y >= size - borderWidth)
                {
                    pixels[y * size + x] = color2;
                }
                else
                {
                    pixels[y * size + x] = color;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void SaveSnakes()
    {
        if (snakes.Count == 0)
        {
            Debug.LogWarning("No snakes to save!");
            return;
        }

        string snakeData = "Snakes:\n";
        for (int i = 0; i < snakes.Count; i++)
        {
            snakeData += $"{snakes[i].name} (Color: {snakes[i].GetColor()}):\n";
            foreach (Vector2Int cell in snakes[i].cells)
            {
                snakeData += $"({cell.x}, {cell.y})\n";
            }
            snakeData += "\n";
        }
        Debug.Log(snakeData);
    }
    private void createGrid()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/CellGround.prefab");
        GameObject prefabBorderBot = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/Bot.prefab");
        GameObject prefabBorderTop = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/Top.prefab");
        GameObject prefabBorderLeft = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/Left.prefab");
        GameObject prefabBorderRight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/Right.prefab");
        GameObject prefabBorderTopLeft = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/TopLeft.prefab");
        GameObject prefabBorderBotLeft = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/BotLeft.prefab");
        GameObject prefabBorderTopRight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/TopRight.prefab");
        GameObject prefabBorderBotRight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GamePlay/Borders/BotRight.prefab");
        Sprite spriteBG = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/PrepareAssets/GamePlay/bg.png");
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        
        if (!levelManager)
        {
            GameObject goLevel = new GameObject();
            goLevel.name = "Level";
            levelManager = goLevel.AddComponent<LevelManager>();
        }
        
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (!gridManager)
        {
            GameObject grid = new GameObject();
            grid.transform.SetParent(levelManager.transform);
            grid.name = "Grid";
            gridManager = grid.AddComponent<GridManager>();
            gridManager.transform.position = Vector3.zero;

        }
        else
        {
            GameUtils.ClearTransformImmediate(gridManager.transform);
        }
        //SpriteRenderer spr = gridManager.GetComponent<SpriteRenderer>();
        //if (spr == null)
        //{
        //    spr = gridManager.gameObject.AddComponent<SpriteRenderer>();
            
        //}
        //spr.sprite = spriteBG;
        //spr.drawMode = SpriteDrawMode.Sliced;
        float _w = (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE) * gridWidth;
        float _h = (GridHelper.TILE * GridHelper.SCALE_TILE + GridHelper.SPACE) * gridHeight;
        //spr.sortingOrder = 4;
        //spr.sortingLayerName = "Game";
        //spr.size = new Vector2(_w + 0.2f, _h + 0.2f);
        gridManager.Grids = Grids;
        gridManager.gameObject.isStatic = true;
        //gridManager.InitGrid(gridWidth, gridHeight);
        List<Vector2Int> listCell = new List<Vector2Int>();
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (!gridAvailable(x, y)) continue;
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, gridManager.transform);
                instance.isStatic = true;
                float floatX = GridHelper.GetX(x, gridWidth);
                float floatY = GridHelper.GetY(y, gridHeight);
                listCell.Add(new Vector2Int(x, y));
                drawBorderCellVer2(gridManager, new Vector2Int(x, y));
                if ((x + y) % 2 == 0)
                {
                    //instance.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(true);
                }
                else
                {
                    //instance.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
                }
                instance.transform.position = new Vector3(floatX, floatY);
                instance.name = $"{x}_{y}";
                SetValueGrid(x, y, instance.GetComponent<Cell>());
                //instance.gameObject.SetActive(false);
            }
        }
        foreach (var item in listCell)
        {
            //drawBorderCell(gridManager, item);
            
        }
        foreach (var item in snakes)
        {
            foreach (var cell in item.cells)
            {
                SetValueGrid(cell.x, cell.y, GridValue.Available, item.SelectedColor, item.Turn);
            }
        }
        foreach (var item in portals)
        {
            Vector2Int coord = item.Coord[0];
            SetValueGrid(coord.x, coord.y, GridValue.Portal, item.SelectedColor, item.Turn);
        }
        levelManager.GridManager = gridManager;
    }
    
    List<(BorderType, Quaternion)> getBorderTypeWithRotation(Vector2Int coords)
    {
        List<(BorderType, Quaternion)> listResult = new List<(BorderType, Quaternion)>();

        bool haveUp = gridAvailable(coords.x, coords.y + 1);
        bool haveRight = gridAvailable(coords.x + 1, coords.y);
        bool haveDown = gridAvailable(coords.x, coords.y - 1);
        bool haveLeft = gridAvailable(coords.x - 1, coords.y);

        bool haveUpRight = gridAvailable(coords.x + 1, coords.y + 1);
        bool haveDownRight = gridAvailable(coords.x + 1, coords.y - 1);
        bool haveDownLeft = gridAvailable(coords.x - 1, coords.y - 1);
        bool haveUpLeft = gridAvailable(coords.x - 1, coords.y + 1);

        BorderType borderTypeLine = BorderType.End;

        if (!haveUp)
        {
            borderTypeLine = BorderType.End;
            if (haveUpRight) borderTypeLine = BorderType.StraightMR;
            if (haveUpLeft) borderTypeLine = BorderType.StraightML;
            if (haveUpRight && haveUpLeft) borderTypeLine = BorderType.StraightMLR;

            listResult.Add((borderTypeLine, Quaternion.Euler(-90, 0, 0)));
            if (haveUpLeft) listResult.Add((BorderType.CornerPointIn, Quaternion.Euler(-90, 0, 0)));
            if (!haveLeft)
            {
                if (!haveUpLeft) listResult.Add((BorderType.CornerPoint, Quaternion.Euler(0, -90, 90)));//todo
                else listResult.Add((BorderType.DiagonalJoined, Quaternion.Euler(0, -90, 0)));
                Debug.Log(coords + " have corner " + !haveUpLeft);
            }

        }

        if (!haveRight)
        {
            borderTypeLine = BorderType.End;
            if (haveDownRight) borderTypeLine = BorderType.StraightMR;
            if (haveUpRight) borderTypeLine = BorderType.StraightML;
            if (haveUpRight && haveDownRight) borderTypeLine = BorderType.StraightMLR;

            listResult.Add((borderTypeLine, Quaternion.Euler(0, 90, -90)));
            if (haveUpRight) listResult.Add((BorderType.CornerPointIn, Quaternion.Euler(0, 90, -90)));
            if (!haveUp)
            {
                if (!haveUpRight) listResult.Add((BorderType.CornerPoint, Quaternion.Euler(-90, 0, 0)));//todo
                else listResult.Add((BorderType.DiagonalJoined, Quaternion.Euler(0, 0, 0)));
                Debug.Log(coords + " have corner " + !haveUpRight);
            }

        }

        if (!haveDown)
        {
            borderTypeLine = BorderType.End;
            if (haveDownLeft) borderTypeLine = BorderType.StraightMR;
            if (haveDownRight) borderTypeLine = BorderType.StraightML;
            if (haveDownLeft && haveDownRight) borderTypeLine = BorderType.StraightMLR;

            listResult.Add((borderTypeLine, Quaternion.Euler(90, -180, 0)));//todo
            if (haveDownRight) listResult.Add((BorderType.CornerPointIn, Quaternion.Euler(90, 180, 0)));
            if (!haveRight)
            {
                if (!haveDownRight) listResult.Add((BorderType.CornerPoint, Quaternion.Euler(0, 90, -90)));//todo
                else listResult.Add((BorderType.DiagonalJoined, Quaternion.Euler(0, 90, -90)));
                Debug.Log(coords + " have corner " + !haveDownRight);
            }

        }

        if (!haveLeft)
        {
            borderTypeLine = BorderType.End;
            if (haveUpLeft) borderTypeLine = BorderType.StraightMR;
            if (haveDownLeft) borderTypeLine = BorderType.StraightML;
            if (haveDownLeft && haveUpLeft) borderTypeLine = BorderType.StraightMLR;

            listResult.Add((borderTypeLine, Quaternion.Euler(0, -90, 90)));
            if (haveDownLeft) listResult.Add((BorderType.CornerPointIn, Quaternion.Euler(0, -90, 90)));
            if (!haveDown)
            {
                if (!haveDownLeft) listResult.Add((BorderType.CornerPoint, Quaternion.Euler(90, 180, 0)));
                else listResult.Add((BorderType.DiagonalJoined, Quaternion.Euler(90, 180, 0)));
                Debug.Log(coords + " have corner " + !haveDownLeft);
            }
        }

        return listResult;
    }
    bool listTest(Vector2Int coor)
    {
        return true;
        List<Vector2Int> test = new List<Vector2Int>();
        test.Add(new Vector2Int(3, 4));
        //test.Add(new Vector2Int(1, 2));
        //test.Add(new Vector2Int(3, 2));
        //test.Add(new Vector2Int(2, 3));

        foreach (var item in test)
        {
            if (item.x == coor.x && item.y == coor.y) return true;
        }
        return false;
    }
    void drawBorderCellVer2(GridManager gridManager, Vector2Int coord)
    {
        if (listTest(coord))
        {
        }
        else
        {
            return;
        }
        Transform parentBoders = gridManager.transform.Find("Borders");
        if (parentBoders == null)
        {
            parentBoders = new GameObject("Borders").transform;
            parentBoders.localPosition = Vector3.zero;
            parentBoders.SetParent(gridManager.transform);
        }
        parentBoders.gameObject.isStatic = true;
        //Transform plane = parentBoders.Find("Plane");
        //if (plane == null)
        //{
        //    plane = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
        //    plane.transform.localPosition = Vector3.zero;
        //    plane.transform.localEulerAngles = new Vector3(-90, 0, 0);
        //    plane.transform.SetParent(parentBoders.transform);
        //}
        List<(BorderType, Quaternion)> borderInfos = getBorderTypeWithRotation(coord);
        Debug.Log("drawBorderCellVer2 " + coord + " -- "+ borderInfos.Count);
        foreach (var borderInfo in borderInfos)
        {
            var borderPrefab = gameDataSO.GetPrefabBorder(borderInfo.Item1);

            if (borderPrefab != null)
            {
                var border = (GameObject)PrefabUtility.InstantiatePrefab(borderPrefab, parentBoders.transform);
                border.isStatic = true;
                Vector3 worldPosition = new Vector3(GridHelper.GetX(coord.x, gridWidth), GridHelper.GetY(coord.y, gridHeight), 0);
                border.transform.position = worldPosition;
                border.transform.localRotation = borderInfo.Item2;
                border.name = $"{border.name} _{coord.x}_{coord.y}";

            }
        }
    }
    void drawBorderCell(GridManager gridManager, Vector2Int coor)
    {
        if (coor.x == 0 && coor.y == 2)
        {
            Debug.LogError("");
        }
        Transform parentBoders = gridManager.transform.Find("Borders");
        if (parentBoders == null)
        {
            parentBoders = new GameObject("Borders").transform;
            parentBoders.localPosition = Vector3.zero;
            parentBoders.SetParent(gridManager.transform);
        }
        int top = (int)GetValueGrid(coor.x, coor.y + 1);
        int left = (int)GetValueGrid(coor.x - 1, coor.y);
        

        int bot = (int)GetValueGrid(coor.x, coor.y - 1);
        int right = (int)GetValueGrid(coor.x + 1, coor.y);

        int topLeft = 0;
        int botLeft = 0;
        int topRight = 0;
        int botRight = 0;
        int extendLeft = 0;
        int extendRight = 0;
        int extendRightOut = 0;
        int extendTop = 0;
        int extendBot = 0;
        if (top < 0 || top >= (int)Enums.GridValue.Removed)
        {
            topLeft++;
            topRight++;
            extendTop++;
            createBorder("Top", coor, parentBoders);
        }
        if (bot < 0 || bot >= (int)Enums.GridValue.Removed)
        {
            botLeft++;
            botRight++;
            extendBot++;
            createBorder("Bot", coor, parentBoders);
        }
        else
        {
            extendLeft++;
            extendRight++;

            if (!needBorder(coor.x + 1, coor.y - 1))
            {
                if(needBorder(coor.x + 1, coor.y)) createBorder("BotRightOut", coor, parentBoders);
            }
            else
            {
                createBorder("RightExtend", coor, parentBoders);
            }
        }
        if (left < 0 || left >= (int)Enums.GridValue.Removed)
        {
            topLeft++;
            botLeft++;
            extendLeft++;
            createBorder("Left", coor, parentBoders);
        }
        

        
        if (right < 0 || right >= (int)Enums.GridValue.Removed)
        {
            topRight++;
            botRight++;
            createBorder("Right", coor, parentBoders);
        }
        else
        {
            extendTop++;
            extendBot++;
        }

        if (topLeft == 2)
        {
            createBorder("TopLeft", coor, parentBoders);
        }
        if (topRight == 2)
        {
            createBorder("TopRight", coor, parentBoders);
        }
        if (topLeft == 2)
        {
            createBorder("TopLeft", coor, parentBoders);
        }
        if (botRight == 2)
        {
            createBorder("BotRight", coor, parentBoders);
        }
        if (botLeft == 2)
        {
            createBorder("BotLeft", coor, parentBoders);
        }
        if (extendLeft == 2)
        {
            createBorder("LeftExtend", coor, parentBoders);
        }
        if (extendRight == 2)
        {
            //createBorder("RightExtend", coor, parentBoders);
        }
        if (extendTop == 2)
        {
            createBorder("TopExtend", coor, parentBoders);
        }
        if (extendBot == 2)
        {
            createBorder("BotExtend", coor, parentBoders);
        }
    }
    bool needBorder(int x, int y)
    {
        Enums.GridValue value = GetValueGrid(x, y);
        if ((int)value < 0 || (int)value >= (int)Enums.GridValue.Removed) return true;
        return false;
    }
    private void createBorder(string typePrefab, Vector2Int coor, Transform parentBoders)
    {
        GameObject prefabBorder = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/GamePlay/Borders/{typePrefab}.prefab");
        float posX = GridHelper.GetX(coor.x, gridWidth);
        float posY = GridHelper.GetY(coor.y, gridHeight);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabBorder, parentBoders);
        instance.transform.position = new Vector3(posX, posY);
        instance.name = $"{typePrefab}_{coor.x} - {coor.y}";
    }

    private void DrawGridCell(int x, int y)
    {
        int snakeId = getSnakeIdFromCell(x, y);
        int portalId = getPortalIdFromCell(x, y);
        Color color = GetCellColor(x, y); // Get color based on current state
        int _buttonSize = 30;
        Rect cellRect = GUILayoutUtility.GetRect(_buttonSize, _buttonSize,
            GUILayout.Width(_buttonSize), GUILayout.Height(_buttonSize));

        float margin = 2f;
        Rect innerRect = new Rect(
            cellRect.x + margin,
            cellRect.y + margin,
            cellRect.width - margin * 2,
            cellRect.height - margin * 2
        );
        Enums.GridValue value = GetValueGrid(x, y);
        if (value == Enums.GridValue.Removed)
        {
            GUI.DrawTexture(innerRect, textRemoveCell);
        }
        else if (value == Enums.GridValue.Portal)
        {
            GUI.DrawTexture(innerRect, drawCircle(color));
        }
        else
        {
            GUI.DrawTexture(innerRect, MakeTextureWithColor(color));
        }
        
        // background for container color
        //GUI.DrawTexture(innerRect, MakeColorTexture(color));

        if (Event.current.type == EventType.MouseDown && innerRect.Contains(Event.current.mousePosition))
        {
            //OnCellClicked(coords, cell);
            HandleCellClick(x, y);
            Event.current.Use();
        }
        GUIStyle lbl = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = Color.white },
        };
        if (snakeId > 0)
        {
            bool isHead = isHeadSnakeFromCell(x, y);
            bool isTail = isTailSnakeFromCell(x, y);
            string label = $"{snakeId}";
            if (isHead) label = $"{snakeId}(S)";
            if (isTail) label = $"{snakeId}(E)";
            GUI.Label(innerRect, label, lbl);
        }
        if (portalId > 0)
        {
            string label = $"{portalId}";
            GUI.Label(innerRect, label, lbl);
        }
    }
    private void InitializeSnakeController()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        // Find SnakeController in the scene
        SnakeManager controller = FindObjectOfType<SnakeManager>();
        if (controller == null)
        {
            GameObject go = new GameObject();
            go.name = "SnakeManager";
            go.transform.SetParent(levelManager.transform);
            controller = go.AddComponent<SnakeManager>();
            controller.transform.position = Vector3.zero;
        }
        levelManager.SnakeManager = controller;
        // Clear existing snakes in controller
        controller.ClearSnakes();

        // Pass snake data to controller
        controller.initShakes(levelManager.GridManager, snakes);
        Debug.Log($"Initialized SnakeController with {snakes.Count} snakes");
    }
    void drawListLevelSelection()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Prefab List", EditorStyles.boldLabel);

        if (prefabNames.Count == 0)
        {
            GUILayout.Label("No prefabs found in Assets/Prefabs/Levels/");
        }

        // Tính toán số trang
        int totalPages = Mathf.CeilToInt((float)prefabNames.Count / ITEMS_PER_PAGE);

        // Hiển thị danh sách prefab
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        int startIndex = currentPage * ITEMS_PER_PAGE;
        int endIndex = Mathf.Min(startIndex + ITEMS_PER_PAGE, prefabNames.Count);
        GUIStyle buttonStyleSelected = new GUIStyle(GUI.skin.button);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyleSelected.normal.textColor = Color.green;
        buttonStyleSelected.hover.textColor = Color.green;
        for (int i = startIndex; i < endIndex; i++)
        {
            EditorGUILayout.BeginHorizontal();
            int level = extractNumber(prefabNames[i]);

            if (GUILayout.Button($"Level {level}", i == (currentLevelSelected - 1) ? buttonStyleSelected : buttonStyle, GUILayout.Height(20)))
            {
                //OnLevelClicked(prefabNames[i]);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // Phần điều khiển phân trang
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = currentPage > 0;
        if (GUILayout.Button("Previous"))
        {
            currentPage--;
            scrollPosition = Vector2.zero;
        }

        GUILayout.FlexibleSpace();
        GUILayout.Label($"Page {currentPage + 1} of {totalPages}");
        GUILayout.FlexibleSpace();

        GUI.enabled = currentPage < totalPages - 1;
        if (GUILayout.Button("Next"))
        {
            currentPage++;
            scrollPosition = Vector2.zero;
        }

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        // Nút Refresh ở dưới cùng
        if (GUILayout.Button("Refresh"))
        {
            //refreshResource();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Reset All level", GUILayout.Height(50)))
        {
            //OnClickNewLevel();
            levelToReset = 1;
            resetAllLevel();
        }
        GUILayout.Space(20);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    private void resetAllLevel()
    {
        string path = $"Assets/Resources/GameLevels/Level_{levelToReset}.prefab";
        GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null)
        {
            Debug.Log($"Stop at: {path}");
            return;
        }
        GameObject clone = PrefabUtility.InstantiatePrefab(go) as GameObject;
        checkScene();
        getDataFromScene();
        
        EditorCoroutineUtility.StartCoroutineOwnerless(savePrefab(clone, path));
    }
    IEnumerator savePrefab(GameObject clone, string path)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new EditorWaitForSeconds(0.3f);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        bool status = false;
        PrefabUtility.RecordPrefabInstancePropertyModifications(clone);
        PrefabUtility.SaveAsPrefabAsset(clone, path, out status);
        Debug.Log($"Prefab changes saved to: {path} with status: {status}");

        DestroyImmediate(clone);

        GameObject prefabAsset = PrefabUtility.LoadPrefabContents(path);
        prefabAsset.transform.position = GameStatic.POS_LEVEL;
        EditorUtility.SetDirty(prefabAsset);
        PrefabUtility.SaveAsPrefabAsset(prefabAsset, path);
        PrefabUtility.UnloadPrefabContents(prefabAsset);
        //AssetDatabase.Refresh();
        levelToReset++;
        resetAllLevel();
    }
    bool gridAvailable(int x, int y)
    {
        GridValue value = GetValueGrid(x, y);
        if (value >= GridValue.Removed || value == GridValue.Wrong) return false;
        return true;
    }
    public GridValue GetValueGrid(int x, int y)
    {
        try
        {
            return Grids[y].Widths[x].GridValue;
        }
        catch (System.Exception)
        {
            return GridValue.Wrong;
        }
    }
    public void SetValueGrid(int x, int y, GridValue value, BlockColor blockColor = BlockColor.None, int turn = 0)
    {
        try
        {
            if (Grids[y].Widths[x] == null) Grids[y].Widths[x] = new CellData();
            Grids[y].Widths[x].GridValue = value;
            Grids[y].Widths[x].BlockColor = blockColor;
            Grids[y].Widths[x].Turn = turn;
        }
        catch (System.Exception)
        {
        }
    }
    public void SetValueGrid(int x, int y, Cell cell)
    {
        try
        {
            if (Grids[y].Widths[x] == null) Grids[y].Widths[x] = new CellData();
            Grids[y].Widths[x].Cell = cell;
        }
        catch (System.Exception)
        {
        }
    }
    Texture2D drawCircle(Color color)
    {
        int size = 64;
        Texture2D circleTexture = new Texture2D(size, size);
        Color[] pixels = circleTexture.GetPixels();
        float radiusSquared = (size / 2f) * (size / 2f);
        Vector2 center = new Vector2(size / 2f, size / 2f);

        for (int i = 0; i < pixels.Length; i++)
        {
            Vector2 pixelPos = new Vector2(i % size, i / size);
            if ((pixelPos - center).sqrMagnitude <= radiusSquared)
            {
                pixels[i] = color;
            }
            else
            {
                pixels[i] = Color.clear;
            }
        }
        circleTexture.SetPixels(pixels);
        circleTexture.Apply();
        return circleTexture;
    }
    enum EditType
    {
        Grid,
        Snake,
        Portal
    }
    public int GetNextSnakeId()
    {
        currentSnakeIncreaseId++;
        return currentSnakeIncreaseId;
    }
    public int GetNextPortalId()
    {
        currentPortalIncreaseId++;
        return currentPortalIncreaseId;
    }
}