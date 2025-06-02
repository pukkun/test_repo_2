using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static Enums;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MatrixColorTool : EditorWindow
{
    public List<ColorArray> GridColors;
    private int width = 1;
    private int height = 1;
    private int layer = 1;
    private List<Color> cellColors = new List<Color>();
    private List<BlockColor> blockColors = new List<BlockColor>();
    private Dictionary<BlockColor, int> dicCheckCountColor = new Dictionary<BlockColor, int>();
    private Dictionary<BlockColor, int> dicCheckCountColorOfTarget = new Dictionary<BlockColor, int>();
    private Vector2 scrollPosition;
    BlockColor mainBlockColor;
    [MenuItem("Tools/Matrix Color Tool")]
    public static void ShowWindow()
    {
        GetWindow<MatrixColorTool>("Matrix Color Tool");
    }
    private void OnEnable()
    {
        InitializeColors();
        if (SnakeGridTool.ShareSnake.IsNullOrEmpty())
        {
            Snake[] snakeItems = FindObjectsOfType<Snake>();
            foreach (var item in snakeItems)
            {
                SnakeGridTool.ShareSnake.Add(item.SnakeData);
            }
            
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Matrix Color Tool", EditorStyles.boldLabel);


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Width (X):", GUILayout.Width(80));
        int newWidth = EditorGUILayout.IntField(width, GUILayout.Width(50));
        if (newWidth != width && newWidth > 0)
        {
            width = newWidth;
            checkAddColor();
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        // Hiển thị thông tin chiều dài
        GUILayout.Label("Height", GUILayout.Width(80));
        int newHeight = EditorGUILayout.IntField(height, GUILayout.Width(50));
        if (newHeight != height && newHeight > 0)
        {
            height = newHeight;
            checkAddColor();
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        // Hiển thị thông tin chiều dài
        GUILayout.Label("Layer", GUILayout.Width(80));
        layer = EditorGUILayout.IntField(layer, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Main Color", GUILayout.Width(80));
        mainBlockColor = (BlockColor)EditorGUILayout.EnumPopup(mainBlockColor);
        Rect mColorRect = GUILayoutUtility.GetRect(20, 20);
        EditorGUI.DrawRect(mColorRect, GameUtils.GetColorFromOption(mainBlockColor));
        EditorGUILayout.EndHorizontal();

        showNeedOfColors();
        // Hiển thị lưới màu
        GUILayout.Space(200);
        GUILayout.Label("Color Matrix:", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));
        for (int h = 0; h < height; h++)
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false), GUILayout.MaxWidth(width * 150)))
            {
                for (int w = 0; w < width; w++)
                {
                    int index = h * width + w;
                    //blockColors[index] = (BlockColor)EditorGUILayout.EnumPopup(blockColors[index], GUILayout.Width(50));
                    GridColors[h].Widths[w].BlockColor = (BlockColor)EditorGUILayout.EnumPopup(GridColors[h].Widths[w].BlockColor, GUILayout.Width(50));
                    GridColors[h].Widths[w].Color = GameUtils.GetColorFromOption(GridColors[h].Widths[w].BlockColor);
                    //cellColors[index] = GameUtils.GetColorFromOption(GridColors[h].Widths[w].BlockColor);
                    Rect colorRect = GUILayoutUtility.GetRect(20, 20);
                    EditorGUI.DrawRect(colorRect, GameUtils.GetColorFromOption(GridColors[h].Widths[w].BlockColor));
                    GUILayout.Space(20);
                    if (Event.current.type == EventType.MouseDown && colorRect.Contains(Event.current.mousePosition))
                    {
                        if (mainBlockColor != BlockColor.None)
                        {
                            GridColors[h].Widths[w].BlockColor = mainBlockColor;
                            GridColors[h].Widths[w].Color = GameUtils.GetColorFromOption(mainBlockColor);
                            //blockColors[index] = mainBlockColor;
                            //cellColors[index] = GameUtils.GetColorFromOption(mainBlockColor);
                        }
                        Event.current.Use();
                    }
                }
            }
            GUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Random All Color"))
        {
            randomAllColors();
        }
        if (GUILayout.Button("Apply To Level"))
        {
            checkScene();
            Debug.Log("applied!");
            TargetGoals targetGoals = FindObjectOfType<TargetGoals>();
            if (!targetGoals)
            {
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                GameObject goGoals = new GameObject();
                if(!levelManager)
                {
                    GameObject goLevel = new GameObject();
                    levelManager = goLevel.AddComponent<LevelManager>();
                    goLevel.name = "Level";
                    goLevel.transform.position = GameStatic.POS_LEVEL;
                }
                goGoals.name = "Target Goals";
                goGoals.transform.SetParent(levelManager.transform);
                targetGoals = goGoals.AddComponent<TargetGoals>();
                levelManager.TargetGoals = targetGoals;
                saveColor(targetGoals);
                Debug.LogError("complete " + cellColors);

            }
            else
            {
                saveColor(targetGoals);
            }
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    private void showNeedOfColors()
    {
        GUIStyle styleRed = new GUIStyle(EditorStyles.label);
        styleRed.normal.textColor = Color.red;
        styleRed.hover.textColor = Color.red;

        GUIStyle styleGreen = new GUIStyle(EditorStyles.label);
        styleGreen.normal.textColor = Color.green;
        styleGreen.hover.textColor = Color.green;
        dicCheckCountColor.Clear();
        dicCheckCountColorOfTarget.Clear();
        foreach (var snake in SnakeGridTool.ShareSnake)
        {
            if (snake == null) continue;
            BlockColor blockColor = snake.SelectedColor;
            if (dicCheckCountColor.ContainsKey(blockColor))
            {
                dicCheckCountColor.Put(blockColor, dicCheckCountColor.Get(blockColor) + (snake.cells.Count - 1) * 5);
            }
            else
            {
                dicCheckCountColor.Add(blockColor, (snake.cells.Count - 1) * 5);
            }
        }
        foreach (var item in GridColors)
        {
            foreach (var _item in item.Widths)
            {
                if (dicCheckCountColorOfTarget.ContainsKey(_item.BlockColor))
                {
                    dicCheckCountColorOfTarget.Put(_item.BlockColor, dicCheckCountColorOfTarget.Get(_item.BlockColor) + 1);
                }
                else
                {
                    dicCheckCountColorOfTarget.Add(_item.BlockColor, 1);
                }
            }
        }
        //foreach (var blockColor in )
        //{
        //    if (dicCheckCountColorOfTarget.ContainsKey(blockColor))
        //    {
        //        dicCheckCountColorOfTarget.Put(blockColor, dicCheckCountColorOfTarget.Get(blockColor) + 1);
        //    }
        //    else
        //    {
        //        dicCheckCountColorOfTarget.Add(blockColor, 1);
        //    }
        //}
        foreach (var item in dicCheckCountColor)
        {
            EditorGUILayout.BeginHorizontal();
            Rect colorRect = GUILayoutUtility.GetRect(20, 20);
            EditorGUI.DrawRect(colorRect, GameUtils.GetColorFromOption(item.Key));
            if (Event.current.type == EventType.MouseDown && colorRect.Contains(Event.current.mousePosition))
            {
                mainBlockColor = item.Key;
                Event.current.Use();
            }
            int countTarget = dicCheckCountColorOfTarget.Get(item.Key);
            GUILayout.Label("Tổng ô sâu: " + item.Value.ToString());
            GUILayout.Label("Tổng mục tiêu đã có: " + countTarget.ToString(), countTarget == item.Value ? styleGreen : styleRed);
            EditorGUILayout.EndHorizontal();
        }
        
    }
    private void randomAllColors()
    {
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                int index = h * width + w;
                GridColors[h].Widths[w].BlockColor = (BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1));
                //blockColors[index] = (BlockColor)(GameUtils.RandomRange(1, System.Enum.GetValues(typeof(BlockColor)).Length - 1));
                cellColors[index] = GameUtils.GetColorFromOption(GridColors[h].Widths[w].BlockColor);
            }
        }
    }
    private void checkScene()
    {
        if (SceneManager.GetActiveScene().name != SceneConstant.SCENE_GAME)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + SceneConstant.SCENE_GAME + ".unity");
        }
    }
    private void saveColor(TargetGoals targetGoals)
    {
        targetGoals.ListTargetColor.Clear();
        targetGoals.ListTargetBlockColor.Clear();
        int check = cellColors.Count / width;
        for (int i = check - 1; i >= 0; i--)
        {

            for (int j = 0; j < width; j++)
            {
                int index = i * width + j;

                for (int l = 0; l < layer; l++)
                {
                    //targetGoals.ListTargetColor.Add(cellColors[index]);
                    //targetGoals.ListTargetColor.Add(cellColors[index]);
                    //targetGoals.ListTargetBlockColor.Add(blockColors[index]);
                    targetGoals.ListTargetColor.Add(GridColors[i].Widths[j].Color);
                    targetGoals.ListTargetBlockColor.Add(GridColors[i].Widths[j].BlockColor);
                }
            }

        }
        targetGoals.GridColors = GridColors;
        targetGoals.ListRootColor = cellColors;
        targetGoals.ListRootBlockColor = blockColors;
        targetGoals.Layer = layer;
        targetGoals.Height = height;
        targetGoals.Width = width;

        PrefabUtility.RecordPrefabInstancePropertyModifications(targetGoals);
    }

    private void InitializeColors()
    {
        TargetGoals targetGoals = FindObjectOfType<TargetGoals>();
        if (targetGoals != null)
        {
            blockColors.Clear();
            cellColors.Clear();
            cellColors = targetGoals.ListRootColor;
            blockColors = targetGoals.ListRootBlockColor;
            GridColors = targetGoals.GridColors;
            width = targetGoals.Width;
            height = targetGoals.Height;
            layer = targetGoals.Layer;
        }
        checkAddColor();
        blockColors.Clear();
        cellColors.Clear();
        for (int i = 0; i < width * height; i++)
        {
            cellColors.Add(Color.white);
            blockColors.Add(BlockColor.None);
        }
        
    }
    private void checkAddColor()
    {
        if (GridColors.IsNullOrEmpty())
        {
            GridColors = new List<ColorArray>();
            for (int i = 0; i < height; i++)
            {
                ColorArray colorArray = new ColorArray();
                colorArray.Widths = new List<DataColor>();
                for (int j = 0; j < width; j++)
                {
                    colorArray.Widths.Add(new DataColor());
                }
                GridColors.Add(colorArray);
            }
        }

        while (GridColors[0].Widths.Count > width)
        {
            for (int i = 0; i < height; i++)
            {
                GridColors[i].Widths.RemoveAt(GridColors[i].Widths.Count - 1);
            }
        }
        while (GridColors[0].Widths.Count < width)
        {
            for (int i = 0; i < height; i++)
            {
                GridColors[i].Widths.Add(new DataColor());
            }
        }
        while (GridColors.Count > height)
        {
            GridColors.RemoveAt(0);
        }
        while (GridColors.Count < height)
        {
            ColorArray colorArray = new ColorArray();
            colorArray.Widths = new List<DataColor>();
            for (int j = 0; j < width; j++)
            {
                colorArray.Widths.Add(new DataColor());
            }
            GridColors.Insert(0, colorArray);
        }
        int max = width * height;
        while(cellColors.Count < max)
        {
            cellColors.Insert(0, Color.white);
            blockColors.Insert(0, BlockColor.None);
        }
        while (cellColors.Count > max)
        {
            cellColors.RemoveAt(0);
            blockColors.RemoveAt(0);
        }
    }
}