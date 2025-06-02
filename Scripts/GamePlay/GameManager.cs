using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Enums;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    int defaultSlot = 4;
    int maxSlot = 6;
    float spacingW = 0.8f;
    float spacingH = 0.73f;
    float spacingL = 0.15f;
    LevelManager levelManager;
    [SerializeField] Camera cameraButterFly = null;
    public LevelManager LevelManager => levelManager;
    List<Slot> listSlot = new List<Slot>();
    //[Header("Prefab Fx")]
    //[SerializeField] public GameObject PrefabFxAppearButterFly = null;
    //[SerializeField] public GameObject PrefabFxPairDestroy = null;
    //[SerializeField] public GameObject PrefabFxPodBreak = null;
    
    
    //[Header("Prefab Game")]
    //[SerializeField] public ButterFly prefabButterFly = null;


    [Header("Target")]
    [SerializeField] private SpriteRenderer sprTargetBG = null;
    [SerializeField] private TargetBlock prefabTarget = null;
    [SerializeField] private Transform transformContainerTarget = null;

    [Space(5)]
    [Header("Slot")]
    [SerializeField] private Slot prefabSlot = null;
    [SerializeField] private Transform transformContainerSlots = null;

    [Header("Data")]
    [SerializeField] private GameDataSO gameDataSO = null;
    public GameDataSO GameDataSO => gameDataSO;

    [Header("Game UI")]
    [SerializeField] private GameViewUI gameViewUI = null;

    [SerializeField] private SpriteRenderer spr2LightSlotBG = null;

    [Header("Booster")]
    [SerializeField] private BoosterManager boosterManager = null;
    float convertPosY = 0;

    private LevelManager prefabLevelManager;



    //variable
    Slot adsSlot;
    List<Snake> listWaitingSnake = new List<Snake>();
    List<System.Tuple<Snake, Slot>> listButterFlyRun = new List<System.Tuple<Snake, Slot>>();
    private int needButterFly;
    float checkTimeButterFly;
    public bool AllowAction;
    bool isQuitGame;
    bool gameRun;
    int score = 0;//tam thoi score = tong so ran an dc
    public int Score => score;
    public bool IsEndGame;
    const int heightVisible = 5;
    int gridMaxW;
    int gridMaxH;
    TargetBlock[,] gridTargetBlocks;
    private int currentLevel;
    public int CurrentLevel => currentLevel;
    private float delayCheckEndGame;
    string usingBooster = null;
    List<PopupStartupGame> listPopupStartupGame = new List<PopupStartupGame>();
    public TypeLoseGame TypeLoseGame;
    enum PopupStartupGame
    {
        None,
        Timer,
        WarningLevel,
        UnlockBooster,
        NewFeature
    }

    Camera mainCamera;
    public Plane targetPlane = new Plane(Vector3.up, Vector3.zero);
    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();
        boosterManager.UseBoost = onUseBooster;
        boosterManager.CancelBoost = onCancelBooster;
        
    }
    void Start()
    {
        mainCamera = Camera.main;
        gameRun = false;
        AllowAction = false;
        LoadLevel(UserInfo.Level);

        if (!GameUtils.IsProduction())
        {
            DevMode devMode = Instantiate(gameDataSO.PrefabDevMode, transform);
            devMode.SetLevel(UserInfo.Level);
            devMode.OnGo += () =>
            {
                UserInfo.Level = devMode.GetLevel();
                if (UserInfo.Level == currentLevel) return;
                NextLevel();
            };
            devMode.OnWin += () => {
                
                DevWin(); 
            };
        }
    }
    public void LoadLevel(int level)
    {
        int levelMap = level;
        if (level > GameStatic.MAX_LEVEL)
        {
            int cache = GameUtils.GetIntPref(PrefConstant.RANDOM_LEVEL, 0);
            levelMap = cache;
            if (cache == 0)
            {
                levelMap = GameUtils.RandomRange(10, GameStatic.MAX_LEVEL - 5);
                var configBooster = GameStatic.ConfigLevel.GetDictionary("level_unlock").GetDictionary("booster");
                var configNewFeature = GameStatic.ConfigLevel.GetDictionary("level_unlock").GetDictionary("new_gameplay");
                var valuesBooster = configBooster.Values;
                var valuesFeature = configNewFeature.Values;
                while (true)
                {
                    
                    bool nextRandom = false;
                    foreach (var item in valuesBooster)
                    {
                        if(item.ToInt() == levelMap) nextRandom = true;
                    }
                    foreach (var item in valuesFeature)
                    {
                        if (item.ToInt() == levelMap) nextRandom = true;
                    }
                    if (nextRandom)
                    {
                        levelMap = GameUtils.RandomRange(10, GameStatic.MAX_LEVEL - 5);
                    }
                    else
                    {
                        GameUtils.SaveDataPref(PrefConstant.RANDOM_LEVEL, levelMap);
                        break;
                    }
                }
            }
        }
        Debug.LogError("level " + level);
        Debug.LogError("levelMap " + levelMap);
#if !UNITY_EDITOR
        if (levelManager != null)
        {
            Destroy(levelManager.gameObject);
        }
        prefabLevelManager = Resources.Load<LevelManager>($"GameLevels/Level_{levelMap}");

        levelManager = Instantiate(prefabLevelManager, transform);
#else

        if (levelManager == null) levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            prefabLevelManager = Resources.Load<LevelManager>($"GameLevels/Level_{levelMap}");

            levelManager = Instantiate(prefabLevelManager, transform);
        }
        
#endif
        initLevel(level);
    }

    void initLevel(int level)
    {
        currentLevel = level;
#if UNITY_EDITOR
        if (levelManager == null) levelManager = FindObjectOfType<LevelManager>();
#endif
        levelManager.transform.localPosition = GameStatic.POS_LEVEL;
        levelManager.SnakeManager.OnHandleClick += onHandleClickSnake;
        levelManager.SnakeManager.InitSnakes(levelManager);
        needButterFly = levelManager.SnakeManager.ListSnakes.Count;
        gridMaxW = levelManager.GridManager.Grids[0].Widths.Length;
        gridMaxH = levelManager.GridManager.Grids.Length;

        convertPosY = transformContainerSlots.transform.position.y + levelManager.SnakeManager.transform.position.y;
        //levelManager.transform.localEulerAngles = new Vector3(5, 0, 0);
        Vector3 pos = levelManager.transform.position;
        //pos.z = -5;
        levelManager.transform.position = pos;
        sprTargetBG.gameObject.SetActive(false);
        initSlots();
        bool isLevelHard = GameUtils.IsLevelHard(level);
        gameViewUI.ResetView();
        gameViewUI.ShowLevelHard(false, null);
        gameViewUI.ShowLevel(level);
        gameViewUI.InitTimer(levelManager.LimitTime, false);
        boosterManager.InitBoost();
        boosterManager.gameObject.SetActive(true);
        checkPopupStartupGame();
        IsEndGame = false;
        isQuitGame = false;
        gameRun = true;
        AllowAction = true;
        score = 0;
        gameViewUI.HideHelpGuide();
        gameViewUI.HideHandGuide();
        bool showGuide = true;
#if UNITY_EDITOR
        if (!UserInfo.Inited) showGuide = false;
#endif
        if (currentLevel == 1 && showGuide)
        {
            this.Wait(1, () => {
                levelManager.SnakeManager.ListSnakes[0].ShowFocus();
                gameViewUI.ShowHandGuide(levelManager.SnakeManager.ListSnakes[0].Segments[6].Transform.position);
                gameViewUI.ShowHelpGuide("Tap the worm to start!");
            });
            
        }
        TrackingHelper.MissionStarted(currentLevel == 1,
            TrackingConstant.MType_MAIN,
            TrackingConstant.MName_MAIN_NAME + level,
            currentLevel.ToString(),
            GameUtils.GetMissionDataTracking(level));
        //this.Wait(1, checkSnakeCanComplete);
        LoadingController.Instance.HideGameSlash();
        if (!GameUtils.IsProduction())
        {
            DevMode devMode = FindAnyObjectByType<DevMode>();
            devMode?.SetLevel(level);
        }
    }
    private void checkSnakeCanComplete()
    {
        foreach (var item in levelManager.SnakeManager.ListSnakes)
        {
            List<PortalTarget> listPortal = findPortals(item.SnakeData.SelectedColor);
            if (listPortal != null)
            {
                listPortal = listPortal.OrderBy(o => o.PortalData.Turn).ToList();
                if (snakeCanComplete(item, listPortal))
                {
                    item.ShowFocus(1.02f);
                }
            }
        }
    }
    bool snakeCanComplete(Snake snake, List<PortalTarget> listPortal)
    {
        for (int i = 0; i < listPortal.Count; i++)
        {
            PortalTarget portalTarget = listPortal[i];
            if (portalTarget.IsClose) continue;
            (bool, List<Vector2Int>) dataResult = GridHelper.FindSuccessPath(levelManager.GridManager, snake, portalTarget, false);
            List<Vector2Int> paths = dataResult.Item2;
            if (paths != null && paths.Count > 0)
            {
                return true;
            }
        }
        return false;
    }
    private void checkPopupStartupGame()
    {
        listPopupStartupGame.Clear();
        int value = GameUtils.GetIntPref($"earn_booster_free_at_level_{currentLevel}", 0);
        
        if (!getLevelUnlockNewGamePlay().IsNullOrEmpty())
        {
            listPopupStartupGame.Add(PopupStartupGame.NewFeature);
        }
        bool isLevelHard = GameUtils.IsLevelHard(currentLevel);
        if (isLevelHard)
        {
            listPopupStartupGame.Add(PopupStartupGame.WarningLevel);
        }
        if (!getLevelUnlockBooster().IsNullOrEmpty() && value == 0)
        {
            listPopupStartupGame.Add(PopupStartupGame.UnlockBooster);
        }
        runPopupStartupGame();
    }
    private void runPopupStartupGame()
    {
        Debug.LogError($"runPopupStartupGame level: {currentLevel}");
        if (listPopupStartupGame.Count > 0)
        {
            PopupStartupGame popupStartupGame = listPopupStartupGame[0];
            listPopupStartupGame.RemoveAt(0);
            showPopupStartupGame(popupStartupGame);
        }
        else
        {
            // chinh thuc play game
            initTargets();
            gameViewUI.InitTimer(levelManager.LimitTime, true);
            //this.Wait(0.5f, () => { AllowAction = true; });
        }
    }
    private void showPopupStartupGame(PopupStartupGame popupStartupGame)
    {
        switch (popupStartupGame)
        {
            case PopupStartupGame.Timer:
                break;
            case PopupStartupGame.WarningLevel:
                gameViewUI.ShowLevelHard(true, runPopupStartupGame);
                break;
            case PopupStartupGame.NewFeature:
                gameViewUI.ShowNewFeature(getLevelUnlockNewGamePlay(), runPopupStartupGame);
                break;
            case PopupStartupGame.UnlockBooster:
                string keyBoost = getLevelUnlockBooster();
                BoostItem boostItem = boosterManager.GetBoostItemByKey(keyBoost);
                gameViewUI.ShowBoosterUnlock(boostItem, () => {

                    boostItem.InitData(UserInfo.GetItemByKey(keyBoost));
                    
                    boosterManager.ShowHandGuideBooster(keyBoost);
                    runPopupStartupGame();
                });
                break;
            default:
                break;
        }
    }
    private string getLevelUnlockBooster()
    {
        if (currentLevel == GameUtils.GetRequireLevelBooster("freeze")) return "freeze";
        if (currentLevel == GameUtils.GetRequireLevelBooster("ghost")) return "ghost";
        if (currentLevel == GameUtils.GetRequireLevelBooster("addhole")) return "addhole";
        return string.Empty;
    }
    private string getLevelUnlockNewGamePlay()
    {
        if (currentLevel == GameUtils.GetRequireLevelNewGamePlay("xich")) return "xich";
        if (currentLevel == GameUtils.GetRequireLevelNewGamePlay("pair")) return "pair";
        if (currentLevel == GameUtils.GetRequireLevelNewGamePlay("ken")) return "ken";
        if (currentLevel == GameUtils.GetRequireLevelNewGamePlay("bomb")) return "bomb";
        if (currentLevel == GameUtils.GetRequireLevelNewGamePlay("secret_hole")) return "secret_hole";
        return string.Empty;
    }

    private void initTargets()
    {
        GameUtils.ClearTransform(transformContainerTarget);
        int maxW = levelManager.TargetGoals.Width;
        List<Color> listColor = levelManager.TargetGoals.ListTargetColor;
        float totalWidth = (maxW - 1) * spacingW;
        
        int maxH = levelManager.TargetGoals.Height;
        int maxL = levelManager.TargetGoals.Layer;
        Vector3 offset = new Vector3(-totalWidth / 2, 0, 0);
        int index = 0;
        float wTarget = 0.5f;
        float _w = maxW * spacingW + wTarget;
        sprTargetBG.size = new Vector2(_w - 0.1f, sprTargetBG.size.y);
        sprTargetBG.gameObject.SetActive(true);
        gridTargetBlocks = new TargetBlock[maxW, maxH];
        float delay = 0;
        float delayH = 0;
        float timeDelay = 0.05f;
        for (int h = 0; h < maxH; h++)
        {
            //GameObject go = new GameObject();
            //go.name = "-----";
            //go.transform.SetParent(transformContainerTarget);
            delayH = (5) * timeDelay * h;
            for (int w = 0; w < maxW; w++)
            {
                delay = (maxW - w);
                //go = new GameObject();
                //go.name = "-----";
                //go.transform.SetParent(transformContainerTarget);
                for (int l = 0; l < maxL; l++)
                {
                    BlockColor blockColor = levelManager.TargetGoals.ListTargetBlockColor[index];
                    if (blockColor != BlockColor.None)
                    {
                        TargetBlock targetBlock = PoolManager.Instance.GetTargetBlockItem(); //Instantiate(prefabTarget, transformContainerTarget);
                        targetBlock.transform.SetParent(transformContainerTarget);
                        GameUtils.SetLayerRecursively(targetBlock.gameObject, "TargetBlock");
                        targetBlock.transform.localScale = Vector3.one;
                        Vector3 pos = new Vector3(w * spacingW, h * spacingH + (spacingL * l), -l - 1) + offset;
                        float finalY = pos.y;
                        bool showAnim = false;
                        if(h <= heightVisible)
                        {
                            showAnim = true;
                            pos.y += 1;
                            targetBlock.gameObject.SetActive(false);
                        }
                        if (showAnim)
                        {
                            float finalDelay = (delay * timeDelay) + delayH;
                            this.Wait(finalDelay, () =>
                            {
                                targetBlock.gameObject.SetActive(true);
                            }, true);
                            targetBlock.ShowBlur(finalDelay, 0.1f);
                            float extendY = -0.25f;
                            if (h > 0)
                            {
                                extendY = -0.1f;
                            }
                            targetBlock.transform.DOLocalMoveY(finalY + extendY, 0.1f).SetDelay(finalDelay).OnComplete(() =>
                            {
                                targetBlock.transform.DOLocalMoveY(finalY, 0.1f).OnComplete(() =>
                                {

                                });
                            });
                        }
                        targetBlock.transform.localPosition = pos;
                        //targetBlock.transform.localEulerAngles = Vector3.right * 10;
                        //targetBlock.name = $"{w} - {h} - {l}";
                        targetBlock.gameObject.SetActive(false);//h <= heightVisible
                        var dataColor = gameDataSO.GetDataMaterial(blockColor);
                        targetBlock.InitCoord(w, h, l);
                        targetBlock.InitData(this, blockColor, dataColor.SpriteCubeTarget, dataColor.SpriteCircleTarget);
                        index++;
                        gridTargetBlocks[w, h] = targetBlock;
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }
    }
    private int stepCheck;
    List<Slot> listSlotEffect = new List<Slot>();
    private void checkCollect()
    {
        //Debug.LogError("start check "+ stepCheck);
        bool haveSnake = false;
        int countSlotEmptyRemain = listSlot.Count;
        Slot lastSlotEmpty = null;
        spr2LightSlotBG.gameObject.SetActive(false);
        for (int i = 0; i < listSlot.Count; i++)
        {
            Slot slot = listSlot[i];
            slot.Hide2Light();
            if (slot.IsFull && slot.Snake != null)
            {
                //slot.Snake.HeadController.HideMount();
                haveSnake = true;
                countSlotEmptyRemain--;
            }
            else
            {
                lastSlotEmpty = slot;
            }
        }
        if (countSlotEmptyRemain == 1 && lastSlotEmpty != null)
        {
            spr2LightSlotBG.gameObject.SetActive(true);
            lastSlotEmpty.Show2Light();
            for (int i = 0; i < listSlot.Count; i++)
            {
                //if (listSlot[i] != lastSlotEmpty) listSlot[i].Hide2Light();
            }
        
        }
        if (!haveSnake) return;// yield break;
        bool haveCollect = false;
        int maxW = levelManager.TargetGoals.Width;
        int count = 0;
        bool haveRelease = false;
        listSlotEffect.Clear();
        //Debug.LogError("delayCheckEndGame " + delayCheckEndGame);
        for (int w = 0; w < maxW; w++)
        {
            TargetBlock targetBlock = gridTargetBlocks[w, 0];
            for (int i = 0; i < listSlot.Count; i++)
            {
                Slot slot = listSlot[i];
                if (slot.IsFull && slot.Snake != null)
                {
                    if (targetBlock != null && targetBlock.BlockColor == slot.Snake.SnakeData.SelectedColor && targetBlock.IsReady && slot.Snake.GetRemainLogic() > 0)
                    {
                        listSlotEffect.Add(slot);
                        slot.Snake.HeadController.ShowMount();
                        slot.Snake.CollectLogic();
                        haveCollect = true;
                        targetBlock.IsReady = false;
                        gridTargetBlocks[w, 0] = null;
                        float delay = count++ * 0.1f;
                        
                        TargetBlock left = null;
                        TargetBlock right = null;
                        if (w > 0) left = gridTargetBlocks[w - 1, 0];
                        if (w < maxW - 1) right = gridTargetBlocks[w + 1, 0];
                        Debug.LogError("MoveToSnake " + slot.Snake.GetRemainLogic(), slot);
                        float timeMove = 0;
                        slot.Snake.Shooting(slot, targetBlock, left, right, delay, out timeMove);
                        Debug.LogError("timeMove " + timeMove);
                        dropBlock(w, delay + timeMove);
                        //targetBlock.MoveToSnake(left, right, delay, slot);
                        if (slot.Snake.GetRemainLogic() == 0)
                        {
                            haveRelease = true;
                            delayCheckEndGame = 1.5f;
                        }
                        else
                        {
                            if (delayCheckEndGame < 1) delayCheckEndGame = 1;// wait drop to check nexk
                        }
                    }
                }
            }
        }
        //Debug.LogError("middle check");
        //Debug.LogError("haveCollect " + haveCollect);
        //Debug.LogError("haveRelease " + haveRelease);
        stepCheck++;
        //Debug.LogError("haveRelease " + haveRelease);
        //Debug.LogError("haveCollect " + haveCollect);
        if (!haveCollect && !haveRelease)
        {
            int countFull = 0;
            for (int i = 0; i < listSlot.Count; i++)
            {
                Slot slot = listSlot[i];
                if (slot.IsFull && slot.Snake != null)
                {
                    countFull++;
                }
            }
            if (listSlot.Count == countFull && delayCheckEndGame <= 0)
            {
                noSpace();
            }
        }
    }
    private void noSpace()
    {
        Debug.LogError("end game   delayCheckEndGame " + delayCheckEndGame);
        gameRun = false;
        AllowAction = false;
        this.Wait(1, () => {
            if (maxSlot > listSlot.Count)
            {
                TypeLoseGame = TypeLoseGame.NoSpace;
                gameViewUI.ShowRevive(currentLevel, TypeLoseGame);
            }
            else
            {
                gameViewUI.ShowEndGame(false, currentLevel, TypeLoseGame.NoSpace);
            }
        });
    }
    private void dropBlock(int w, float delay)
    {
        Debug.LogError("dropBlock " + w + " delay " + delay);
        int maxH = levelManager.TargetGoals.Height;
        for (int h = 0; h < maxH; h++)
        {
            if (gridTargetBlocks[w, h] == null)
            {
                
            }
            TargetBlock above = null;
            if(h < maxH - 1) above = gridTargetBlocks[w, h + 1];
            if (above != null) above.MoveDrop(delay, h, spacingH, spacingL);
            gridTargetBlocks[w, h] = above;
            if (gridTargetBlocks[w, h] != null) gridTargetBlocks[w, h].gameObject.SetActive(h <= heightVisible);
        }
    }
    private void initSlots()
    {
        GameUtils.ClearTransform(transformContainerSlots);
        listSlot.Clear();
        float spacing = 1.3f;
        float totalWidth = (defaultSlot - 1) * spacing;
        Vector3 offset = new Vector3(-totalWidth / 2, 0, 0);
        for (int i = 0; i < defaultSlot; i++)
        {
            Slot slot = Instantiate(prefabSlot, transformContainerSlots);
            slot.IndexSlot = i;
            slot.transform.localPosition = new Vector3(i * spacing, 0, 0) + offset;
            slot.OnRelease = onHaveSlotRelease;
            listSlot.Add(slot);
        }
        // ads slot
        bool isRight = listSlot.Count % 2 == 0;
        adsSlot = Instantiate(prefabSlot, transformContainerSlots);
        adsSlot.transform.localPosition = getPosForNextSlot(isRight);
        adsSlot.ShowSlotAds();
        adsSlot.OnClick = adsMoreSlot;
    }
    private void adsMoreSlot()
    {
        AdsHelper.ShowReward("unlock_slot", currentLevel, () =>
        {
            addNewSlot(1);
        }, ()=> {
            MainGameController.Instance.ShowBubbleAlertNoAds();
        }, null, null, GameUtils.GetMissionDataTracking(currentLevel));
    }
    void playSoundCantClickSnake()
    {
        SoundController.Instance.PlaySoundEffectOneShot($"sndNo{GameUtils.RandomRange(1, 6)}");
    }

    private bool preCheckPairSnake(Snake snake, Snake pair, out bool isEarlyLose)
    {
        isEarlyLose = false;
        if (pair == null) return true;

        List<Slot> slots = findEmptySlots();
        if (slots.Count < 2)
        {
            isEarlyLose = true;
            //return false;
        }
        List<PortalTarget> listPortal = findPortals(snake.SnakeData.SelectedColor);
        if (listPortal.IsNullOrEmpty()) return false;
        listPortal = listPortal.OrderBy(o => o.PortalData.Turn).ToList();

        List<PortalTarget> listPortalPair = findPortals(pair.SnakeData.SelectedColor);
        if (listPortalPair.IsNullOrEmpty()) return false;
        listPortalPair = listPortalPair.OrderBy(o => o.PortalData.Turn).ToList();
        bool isGhosting = usingBooster == "ghost";
        bool canMove = false;
        bool canMovePair = false;
        for (int i = 0; i < listPortal.Count; i++)
        {
            PortalTarget portalTarget = listPortal[i];
            if (portalTarget.IsTempClose) continue;
            (bool, List<Vector2Int>) dataResult = GridHelper.FindSuccessPath(levelManager.GridManager, snake, portalTarget, isGhosting);
            List<Vector2Int> paths = dataResult.Item2;
            if (paths != null && paths.Count > 0)
            {
                canMove = true;
                if (portalTarget.PortalData.Turn == 1)
                {
                    portalTarget.IsTempClose = true;
                }
                break;
            }
        }


        for (int i = 0; i < listPortalPair.Count; i++)
        {
            PortalTarget portalTarget = listPortalPair[i];
            if (portalTarget.IsTempClose) continue;
            (bool, List<Vector2Int>) dataResult = GridHelper.FindSuccessPath(levelManager.GridManager, pair, portalTarget, isGhosting);
            List<Vector2Int> paths = dataResult.Item2;
            if (paths != null && paths.Count > 0)
            {
                canMovePair = true;
                if (portalTarget.PortalData.Turn == 1)
                {
                    portalTarget.IsTempClose = true;
                }
                break;
            }
        }
        for (int i = 0; i < listPortal.Count; i++)
        {
            PortalTarget portalTarget = listPortal[i];
            portalTarget.IsTempClose = false;
        }
        for (int i = 0; i < listPortalPair.Count; i++)
        {
            PortalTarget portalTarget = listPortalPair[i];
            portalTarget.IsTempClose = false;
        }
        if (!canMove || !canMovePair) return false;
        return true;
    }
    private void onHandleClickSnake(Snake snake, bool userInteract)
    {
        if (boosterManager.IsHandGuiding()) return;
        if (!AllowAction) return;
        Snake pairSnake = null;
        bool isEarlyLose = false;// click 2 con force lose
        if (userInteract)// user chu dong click, neu ko thi pair auto click
        {
            pairSnake = snake.GetPairSnake();
            if (pairSnake != null)
            {
                List<Slot> slots = findEmptySlots();
                //if (slots.Count < 2)
                //{
                //    snake.RunAnimShakeHead();
                //    pairSnake.RunAnimShakeHead();
                //    playSoundCantClickSnake();
                //    return;
                //}
                if (!preCheckPairSnake(snake, pairSnake, out isEarlyLose))
                {
                    snake.RunAnimShakeHead();
                    pairSnake.RunAnimShakeHead();
                    playSoundCantClickSnake();
                    return;
                }
            }
        }
        Slot emptySlot = findEmptySlot();
        if (!emptySlot && userInteract)
        {
            playSoundCantClickSnake();
            return;
        }
        if (!userInteract && emptySlot == null)
        { 
            
        }
        bool isGhosting = usingBooster == "ghost";
        if (isGhosting)
        {
            bool isGuiding = gameViewUI.IsHandGuide();
            if (isGuiding)
            {

                if (currentLevel == 10)
                {
                    if (snake.SnakeData.SelectedColor != BlockColor.Orange) return;
                }
            }
            SoundController.Instance.PlaySoundEffectOneShot("BoosterJellyGhost");
            boosterManager.OnUseItemComplete(usingBooster);
            usingBooster = null;
            allSnakeNormal();
            gameViewUI.VisibleButtonSetting(true);
            boosterManager.HideReady();
            GameObject go = Instantiate(gameDataSO.PrefabClickStartGhosting, transform);
            Vector3 worldPosition = Vector3.zero;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                worldPosition = hit.point;
            }
            go.transform.position = worldPosition;
        }
        List<PortalTarget> listPortal = findPortals(snake.SnakeData.SelectedColor);
        
        Debug.LogError($"onHandleClickSnake: {snake.name} with direction: {snake.Segments[0].Direction}");
        if (listPortal.IsNullOrEmpty())
        {
            playSoundCantClickSnake();
            snake.RunAnimShakeHead();
            return;
        }
        
        listPortal = listPortal.OrderBy(o => o.PortalData.Turn).ToList();
        HapticHelper.DeviceVibrate(0);
        bool isEscape = false;
        List<Vector2Int> listCoorsBefore = new List<Vector2Int>(snake.ListCoors);
        for (int i = 0; i < listPortal.Count; i++)
        {
            PortalTarget portalTarget = listPortal[i];
            if (portalTarget.IsClose) continue;
            (bool, List<Vector2Int>) dataResult = GridHelper.FindSuccessPath(levelManager.GridManager, snake, portalTarget, isGhosting);
            List<Vector2Int> paths = dataResult.Item2;
            if (paths != null && paths.Count > 0)
            {
                if (portalTarget.PortalData.Turn == 1)
                {
                    portalTarget.IsClose = true;
                }
                score++;
                if(snake.ObstacleTurn != null) snake.ObstacleTurn.EarlyTurn();
                gameViewUI.HideHelpGuide();
                gameViewUI.HideHandGuide();
                allSnakeLookEscpase(snake);
                listPortal[i].DoExpand();
                List<Vector3> list = new List<Vector3>();
                list.Add(snake.Segments[0].Transform.position);
                foreach (var item in paths)
                {
                    Vector3 pos = new Vector3(GridHelper.GetX(item.x, gridMaxW), GridHelper.GetY(item.y, gridMaxH));
                    pos = TransformPoint(pos);
                    pos.z = 0;
                    list.Add(pos);
                }
                int observeStepEscape = 0;
                observeStepEscape = (list.Count - 2 ) * 2 + 2;// 1 head, 1 star, 1 end
                list = DoubleListWithMidpoints(list);
                
                Vector3 lastPos = list[list.Count - 1];
                for (int a = 0; a < snake.Segments.Count; a++)
                {
                    lastPos.z = 1 * (a + 1);
                    list.Add(lastPos);
                }
                if (emptySlot != null)
                {
                    Vector3 slotPos = emptySlot.transform.position;
                    slotPos.y -= 0.1f;//offset cho dep
                    slotPos.z = -5;
                    list.Add(slotPos);
                }
                if (snake.Segments.Count > 4)
                {
                    for (int l = 0; l < snake.Segments.Count - 4; l++)
                    {
                        //list.Add(slotPos);
                    }
                }
                listTst = list;
                isEscape = true;//luon luon escape
                if(emptySlot) emptySlot.IsFull = true;
                snake.OnRunStart?.Invoke();
                updateGridCoors(isEscape, listCoorsBefore, null);
                bool haveRemovePair = false;
                snake.RemoveSingum(userInteract, out haveRemovePair);
                float timeDelay = 0;
                if (haveRemovePair)
                {
                    timeDelay = 0.2f;
                }
                if (isGhosting)
                {
                    snake.AddTrailToTail();
                    snake.ChangeAlphaMaterial();
                    snake.AddFxGhostingToHead();
                }
                paths.RemoveAt(paths.Count - 1);// bo o target ko 2light
                StartCoroutine(enumeratorPath(false, paths));
                list.RemoveAt(0);// remove vi tri hien tai
                snake.MoveByPath(timeDelay, portalTarget, list, isEscape, observeStepEscape, () =>
                {
                    Debug.LogError("move finish");
                    
                    if (portalTarget.PortalData.Turn == 1)
                    {
                        portalTarget.RemovePortal();
                        levelManager.GridManager.Set(portalTarget.PortalData.Coord[0].x, portalTarget.PortalData.Coord[0].y, Enums.GridValue.Available);
                    }
                    else
                    {
                        portalTarget.DoNormal();
                    }
                    if (emptySlot)
                    {
                        emptySlot.SetSnake(snake);
                        if (isEarlyLose)
                        {
                            gameRun = false;
                            noSpace();
                        }
                    }
                    else
                    {
                        listWaitingSnake.Add(snake);
                    }
                    //allSnakeNormal();
                    //checkSnakeCanComplete();
                    snake.RemoveFxGhosting();
                }, ()=> {
                    portalTarget.ShowFxHole(levelManager.transform.localScale.x);
                    snakeEscapse(snake);
                }, ()=> {
                    
                    if(emptySlot) emptySlot.ShowSlot(snake.SnakeData.SelectedColor);
                });
                if (pairSnake != null)
                {
                    onHandleClickSnake(pairSnake, false);
                }
                return;
                break;
            }
        }

        playSoundCantClickSnake();
        for (int i = 0; i < listPortal.Count; i++)
        {
            PortalTarget portalTarget = listPortal[i];
            (bool, List<Vector2Int>) dataResult = GridHelper.FindPathInorgeSnake(levelManager.GridManager, snake.ListCoors, portalTarget.PortalData.Coord[0]);
            List<Vector2Int> paths = dataResult.Item2;
            if (paths != null && paths.Count > 0)
            {
                paths.RemoveAt(0);
                this.Wait(paths.Count * 0.01f, () =>
                {
                    portalTarget.ShowFxCantMoveToPortal(GameDataSO.Instance.GetDataMaterial(portalTarget.PortalData.SelectedColor).MainColor);
                });
                StartCoroutine(enumeratorPath(true, paths));
                snake.RunAnimShakeHead();
                snake.ShowStun();
                return;
                break;
            }
        }

        
        //List<Vector3> paths = findPath(snake, isGhosting, out isEscape);
        //if (isEscape) emptySlot.IsFull = true;
        //if (paths.Count == 0)
        //{
        //    snake.FakeNextMove();
        //    return;
        //}
        //int observeStepEscape = 0;
        //if (isEscape)
        //{
        //    observeStepEscape = paths.Count * 2;// duplicate
        //    paths = extendToTarget(paths, emptySlot.transform.position);
        //}
        //snake.OnRunStart?.Invoke();
        //List<Vector2Int> listCoorsAfter = snake.ListCoors;
        //updateGridCoors(isEscape, listCoorsBefore, listCoorsAfter);
        //List<Vector3> list = new List<Vector3>();
        //list.Add(snake.Segments[0].Transform.position);
        //list.AddRange(paths);
        //list = DoubleListWithMidpoints(list);
        //if (snake.Segments.Count >= 4 && isEscape) list = adjustFinalList(list);
        //listTst = list;
        //if (isGhosting)
        //{
        //    snake.AddTrailToTail();
        //}
        //snake.MoveByPath(list, isEscape, observeStepEscape, () => {
        //    Debug.LogError("move finish");
        //    if (isEscape)
        //    {
        //        snake.DoScaleInSlot(emptySlot.transform.position, null);
        //        emptySlot.SetSnake(snake);
        //    }
        //});
    }
    private void allSnakeLookEscpase(Snake snake)
    {
        foreach (var item in levelManager.SnakeManager.ListSnakes)
        {
            if (item != snake)
            {
                item.LookAt(snake.Segments[0].Transform);
            }
        }
    }
    public Transform test;

    private IEnumerator enumeratorPath(bool wrongPath, List<Vector2Int> paths)
    {
        while (paths.Count > 0)
        {
            Vector2Int nextPos = paths[0];
            paths.RemoveAt(0);
            GridValue value = levelManager.GridManager.GetGridValue(nextPos.x, nextPos.y);
            if (wrongPath)
            {
                levelManager.GridManager.GetCell(nextPos.x, nextPos.y).ShowHighlight();
                if (value == GridValue.Available) yield break;
                if (value == GridValue.Portal) yield break;
            }
            else
            {
                levelManager.GridManager.GetCell(nextPos.x, nextPos.y).ShowHighlightMove();
            }
            
            yield return new WaitForSeconds(0.01f);
        }
    }
    //private void onHandleClickSnake(Snake snake)
    //{
    //    if (!AllowAction) return;
    //    bool isGhosting = usingBooster == "ghost";
    //    if (isGhosting)
    //    {
    //        usingBooster = null;
    //        allSnakeNormal();
    //        boosterManager.HideReady();
    //    }
    //    Slot emptySlot = findEmptySlot();
    //    Debug.LogError($"onHandleClickSnake: {snake.name} with direction: {snake.Segments[0].Direction}");
    //    string slotName = emptySlot == null ? "null" : "emptySlot.name";
    //    Debug.LogError($"into slot: {slotName}");
    //    if (!emptySlot)
    //    {
    //        playSoundCantClickSnake();
    //        return;
    //    }
    //    HapticHelper.DeviceVibrate(0);
    //    bool isEscape = false;
    //    List<Vector2Int> listCoorsBefore = new List<Vector2Int>(snake.ListCoors);

    //    List<Vector3> paths = findPath(snake, isGhosting, out isEscape);
    //    if(isEscape) emptySlot.IsFull = true;
    //    if (paths.Count == 0)
    //    {
    //        snake.FakeNextMove();
    //        return;
    //    }
    //    int observeStepEscape = 0;
    //    if (isEscape)
    //    {
    //        observeStepEscape = paths.Count * 2;// duplicate
    //        paths = extendToTarget(paths, emptySlot.transform.position);
    //    }
    //    snake.OnRunStart?.Invoke();
    //    List<Vector2Int> listCoorsAfter = snake.ListCoors;
    //    updateGridCoors(isEscape, listCoorsBefore, listCoorsAfter);
    //    List<Vector3> list = new List<Vector3>();
    //    list.Add(snake.Segments[0].Transform.position);
    //    list.AddRange(paths);
    //    list = DoubleListWithMidpoints(list);
    //    if(snake.Segments.Count >= 4 && isEscape) list = adjustFinalList(list);
    //    listTst = list;
    //    if (isGhosting)
    //    {
    //        snake.AddTrailToTail();
    //    }
    //    snake.MoveByPath(list, isEscape, observeStepEscape, () => {
    //        Debug.LogError("move finish");
    //        if (isEscape)
    //        {
    //            snake.DoScaleInSlot(emptySlot.transform.position, null);
    //            emptySlot.SetSnake(snake);
    //        }
    //    });
    //}
    private List<Vector3> adjustFinalList(List<Vector3> list)
    {
        Vector3 final = list[list.Count - 1];
        Vector3 final1 = list[list.Count - 2];
        Vector3 final2 = list[list.Count - 3];
        Vector3 final3 = list[list.Count - 4];
        Vector3 final4 = list[list.Count - 5];
        final1.y = final.y - 0.5f;
        final2.y = final1.y - 0.35f;
        final3.y = final2.y - 0.25f;
        list[list.Count - 1] = final;
        list[list.Count - 2] = final1;
        list[list.Count - 3] = final2;
        list[list.Count - 4] = final3;
        if (final4.y > final3.y && final3.x == final4.x)
        {
            list.RemoveAt(list.Count - 5);
        }
        return list;
    }
    public List<Vector3> listTst;
    void OnDrawGizmosSelected()
    {
        if (listTst == null) return;
        // Đặt màu cho Gizmos (màu đỏ)
        Gizmos.color = Color.red;

        // Lặp qua từng điểm và vẽ một quả cầu nhỏ tại vị trí điểm
        foreach (Vector3 point in listTst)
        {
            Gizmos.DrawSphere(point, 0.1f); // 0.1f là bán kính của quả cầu
        }
    }
    //void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        addNewSlot(1);
    //    }
    //    if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            // 'hit.collider.gameObject' is the object that was clicked
    //            Debug.Log("Clicked on: " + hit.collider.gameObject.name);

    //            // You can now perform actions on the clicked object
    //            // For example:
    //            // hit.collider.GetComponent<MyClickableScript>()?.OnClicked();
    //        }
    //    }
    //}
    private void LateUpdate()
    {
        if (checkTimeButterFly > 0) checkTimeButterFly -= Time.deltaTime;
        if (!gameRun) return;
        if (delayCheckEndGame > 0) delayCheckEndGame -= Time.deltaTime;
        checkCollect();
        //if (listButterFlyRun.Count > 0)
        //{
        //    Debug.LogError("checkTimeButterFly " + checkTimeButterFly);
        //}
        if (listButterFlyRun.Count > 0 && checkTimeButterFly <= 0)
        {
            System.Tuple<Snake, Slot> dataButterFly = listButterFlyRun[0];
            listButterFlyRun.RemoveAt(0);
            Snake snake = dataButterFly.Item1;
            //snake.ShowPrepareComplete(()=> {

            //});
            snake.DoDisAppear(dataButterFly.Item2.transform.position, () =>
            {
                
            });
            ShowButterFly(snake.SnakeData.SelectedColor, dataButterFly.Item2);
            dataButterFly.Item2.ClearSnake(0.5f);
            dataButterFly.Item2.ShowEffectComplete();
            dataButterFly.Item2.ShowSlotGray();
            checkTimeButterFly = 0.5f;
            needButterFly--;

        }
        if (isWin() && checkTimeButterFly <= 0)
        {
            winGame();
        }
        //StartCoroutine(checkCollect());
    }

    private void winGame()
    {
        gameRun = false;
        AllowAction = false;
        IsEndGame = true;
        gameViewUI.PauseTime();
        GameUtils.SaveDataPref(PrefConstant.RANDOM_LEVEL, 0);
        UserInfo.IncreaseLevel();
        TrackingHelper.MissionCompleted(false,
                 TrackingConstant.MType_MAIN,
                 TrackingConstant.MName_MAIN_NAME + currentLevel,
                 currentLevel.ToString(),
                 score,
                 GameUtils.GetMissionDataTracking(currentLevel),
                 null
                 );
        this.Wait(0.7f, () =>
        {
            gameViewUI.ShowEndGame(true, currentLevel, TypeLoseGame.None);
        });
    }
    private bool isWin()
    {
        return !isQuitGame && needButterFly == 0;
    }
    private void updateGridCoors(bool isEscape, List<Vector2Int> listCoorsBefore, List<Vector2Int> listCoorsAfter)
    {
        foreach (var item in listCoorsBefore)
        {
            levelManager.GridManager.Set(item.x, item.y, 0);
        }
        if (listCoorsAfter == null) return;
        foreach (var item in listCoorsAfter)
        {
            levelManager.GridManager.Set(item.x, item.y, isEscape ? GridValue.None : GridValue.Available);
        }
    }
    private Slot findEmptySlot()
    {
        for (int i = 0; i < listSlot.Count; i++)
        {
            if (!listSlot[i].IsFull) return listSlot[i];
        }
        return null;
    }
    private List<Slot> findEmptySlots()
    {
        List<Slot> result = new List<Slot>();
        for (int i = 0; i < listSlot.Count; i++)
        {
            if (!listSlot[i].IsFull) result.Add(listSlot[i]);
        }
        return result;
    }
    private List<PortalTarget> findPortals(BlockColor blockColor)
    {
        List<PortalTarget> result = null;
        var listPortal = levelManager.PortalManager.ListPortal;
        for (int i = 0; i < listPortal.Count; i++)
        {
            if (listPortal[i].TruePortal(blockColor))
            {
                if (result == null) result = new List<PortalTarget>();
                result.Add(listPortal[i]);
            }
        }
        return result;
    }
    List<Vector3> findPath(Snake snake, bool isGhosting, out bool isEscape)
    {
        var gridManager = levelManager.GridManager;

        Vector2Int coor = snake.GetHeadCoor();
        Enums.Direction direction = snake.GetHeadDirection();
        List<Vector2Int> listPath = new List<Vector2Int>();
        List<Vector3> listResult = new List<Vector3>();
        Vector2Int vectorDirection = getDirection(direction);
        isEscape = false;
        while (true)
        {
            Vector2Int next = coor + vectorDirection;
            if (next.x < 0 || next.y < 0)
            {
                isEscape = true;
                break;
            }
            if (next.x >= gridMaxW || next.y >= gridMaxH)
            {
                isEscape = true;
                break;
            }
            if (gridManager.GetGridValue(next.x, next.y) > 0 && !isGhosting) break;
            snake.UpdateCoor(next);
            listPath.Add(next);
            Vector3 pos = new Vector3(GridHelper.GetX(next.x, gridMaxW), GridHelper.GetY(next.y, gridMaxH));
            pos = TransformPoint(pos);
            pos.z = 0;
            listResult.Add(pos);
            coor = next;
        }
        if (isEscape)
        {
            findPathExtend(listResult, snake, coor, vectorDirection);
            
        }
        return listResult;
    }
    List<Vector3> findPathExtend(List<Vector3> listResult, Snake snake, Vector2Int coor, Vector2Int vectorDirection)// ran escape ra ngoai roi
    {
        bool firstUp = vectorDirection == Vector2Int.up;
        Vector2Int next = coor + vectorDirection;
        Vector3 pos = new Vector3(GridHelper.GetX(next.x, gridMaxW), GridHelper.GetY(next.y, gridMaxH));
        pos = TransformPoint(pos);
        listResult.Add(pos);
        coor = next;
        if (!firstUp)
        {
            int index = 0;
            while (true)
            {
                index++;
                if (coor.y < 0)// xuong bien duoi
                {
                    if (coor.x < gridMaxW / 2)// qua trai
                    {
                        vectorDirection = Vector2Int.left;
                    }
                    else // qua phai
                    {
                        vectorDirection = Vector2Int.right;
                    }
                    if (coor.x < 0 || coor.x >= gridMaxW)
                    {
                        vectorDirection = Vector2Int.up;
                    }
                }
                else if (coor.x < 0 || coor.x >= gridMaxW)
                {
                    vectorDirection = Vector2Int.up;
                }
                next = coor + vectorDirection;
                snake.UpdateCoor(next);
                pos = new Vector3(GridHelper.GetX(next.x, gridMaxW), GridHelper.GetY(next.y, gridMaxH));
                pos = TransformPoint(pos);
                listResult.Add(pos);
                coor = next;
                if (coor.y >= gridMaxH) break;
                if (index >= 2000)
                {
                    throw new System.Exception("aa");
                    break;
                }
            }
        }
        
        return listResult;
    }

    List<Vector3> extendToTarget(List<Vector3> paths, Vector3 slotPos)
    {
        float space = 1f;
        Vector3 lastPos = paths[paths.Count - 1];
        int index = 0;
        float maxSlotY = slotPos.y + 0.5f;
        while (true)
        {
            index++;
            if (lastPos.x > slotPos.x)
            {
                lastPos.x -= space;
                if (lastPos.x < slotPos.x)
                {
                    lastPos.x = slotPos.x;
                    paths.Add(lastPos);
                    break;
                }
                paths.Add(lastPos);
            }else if (lastPos.x < slotPos.x)
            {
                lastPos.x += space;
                if (lastPos.x > slotPos.x)
                {
                    lastPos.x = slotPos.x;
                    paths.Add(lastPos);
                    break;
                }
                paths.Add(lastPos);
            }
            if (index > 1000) break;
        }
        index = 0;
        while (true)
        {
            index++;
            if (lastPos.y < maxSlotY)
            {
                lastPos.y += space;
                if (lastPos.y > maxSlotY)
                {
                    lastPos.y = maxSlotY;
                    paths.Add(lastPos);
                    break;
                }
                paths.Add(lastPos);
            }
            if (index > 1000) break;
        }
        return paths;
    }
    Vector2Int getDirection(Enums.Direction direction)
    {
        Vector2Int vectorDirection = Vector2Int.zero;
        switch (direction)
        {
            case Enums.Direction.None:
                break;
            case Enums.Direction.Up:
                vectorDirection = Vector2Int.up;
                break;
            case Enums.Direction.Down:
                vectorDirection = Vector2Int.down;
                break;
            case Enums.Direction.Left:
                vectorDirection = Vector2Int.left;
                break;
            case Enums.Direction.Right:
                vectorDirection = Vector2Int.right;
                break;
        }
        return vectorDirection;
    }
    public static List<Vector3> DoubleListWithMidpoints(List<Vector3> originalList)
    {
        List<Vector3> resultList = new List<Vector3>();

        if (originalList == null || originalList.Count < 2)
        {
            foreach (var item in originalList)
            {
                resultList.Add(item);
            }
            return resultList;
        }

        for (int i = 0; i < originalList.Count; i++)
        {
            resultList.Add(originalList[i]);

            if (i < originalList.Count - 1)
            {
                Vector3 midpoint = (originalList[i] + originalList[i + 1]) * 0.5f;
                resultList.Add(midpoint);
            }
        }
        resultList.RemoveAt(0);
        //Vector3 tail = (resultList[resultList.Count - 2] - resultList[resultList.Count - 1]).normalized;
        //float dis = 0.3f;
        //Vector3 final = resultList[resultList.Count - 1] - tail * dis;
        //resultList.Add(final);
        return resultList;
    }
    public void AddButterToQueue(Snake snake, Slot slot)
    {
        checkTimeButterFly = 0.3f;
        listButterFlyRun.Add(System.Tuple.Create(snake, slot));
    }
    public void ShowButterFly(BlockColor blockColor, Slot slot)
    {
        Vector3 pos = slot.transform.position;
        ButterFly butterFly = Instantiate(GameDataSO.PrefabButterFly, transform);
        GameUtils.SetLayerRecursively(butterFly.gameObject, "ButterFly");
        pos.z = -1;
        //pos.z += slot.IndexSlot;
        butterFly.transform.position = pos;
        butterFly.transform.localScale = Vector3.zero;
        butterFly.InitColor(gameDataSO, blockColor);
        butterFly.transform.DOScale(1, 0.3f).OnComplete(() => {
            butterFly.Fly();
            
            SoundController.Instance.PlaySoundEffectOneShot($"sndGo{GameUtils.RandomRange(1, 7)}");
        });
        
    }
    public Vector3 TransformPoint(Vector3 point)
    {
        return levelManager.transform.TransformPoint(point);
    }
    public float ScaleFactor()
    {
        return levelManager.transform.localScale.x;
    }


    private void resetData()
    {
        listWaitingSnake.Clear();
        gameViewUI.HideHandGuide();
        gameViewUI.HideHelpGuide();
        foreach (var item in gridTargetBlocks)
        {
            if (item != null) item.ReturnToPool();
        }
        Destroy(levelManager.gameObject);
        levelManager = null;
        gameRun = false;
        AllowAction = false;
        adsSlot = null;
        foreach (var item in listSlot)
        {
            item.OnRelease = null;
            item.ClearSnake();
        }
    }
    public void ReplayGame()
    {
        LoadingController.Instance.ShowGameSlash();
        resetData();
        this.Wait(0, () => { LoadLevel(UserInfo.Level); });
    }
    public void NextLevel()
    {
        resetData();
        LoadingController.Instance.ShowGameSlash();
        this.Wait(0, () => { LoadLevel(UserInfo.Level); });
    }
    public void DevWin() 
    {
        winGame();
    }
    void onCancelBooster()
    {
        gameViewUI.VisibleButtonSetting(true);
        usingBooster = null;
        allSnakeNormal();
    }
    void onUseBooster(string key)
    {
        if (!AllowAction) return;
        bool success = false;
        bool isGuiding = boosterManager.IsHandGuiding();
        if (isGuiding)
        {
            
            if (currentLevel == 10 && key == "ghost")
            {
                gameViewUI.ShowHandGuide((levelManager.SnakeManager.ListSnakes[2].Segments[1].Transform.position));
                levelManager.SnakeManager.ListSnakes[2].ShowFocus();
            }
        }
        boosterManager.ShowHandGuideBooster(null);
        switch (key)
        {
            case "freeze":
                SoundController.Instance.PlaySoundEffectOneShot("BoosterIceTimer");
                boosterManager.OnUseItemComplete(key);
                gameViewUI.FreezeTimer(10);
                success = true;
                break;
            case "addhole":
                //SoundController.Instance.PlaySoundEffectOneShot("BoosterIceTimer");
                boosterManager.OnUseItemComplete(key);
                addNewSlot(1);
                success = true;
                break;
            case "ghost":
                gameViewUI.VisibleButtonSetting(false);
                boosterManager.ShowReady(key, isGuiding);
                allSnakeFocus();
                usingBooster = key;
                success = true;
                break;
        }
        //boosterManager.CancelUseBoost = onCancelBooster;
    }
    private void allSnakeFocus()
    {
        foreach (var item in levelManager.SnakeManager.ListSnakes)
        {
            item.ShowFocus();
        }
    }
    private void allSnakeNormal()
    {
        foreach (var item in levelManager.SnakeManager.ListSnakes)
        {
            item.ReturnNormal();
        }
    }
    public void TimeOver()
    {
        AllowAction = false;
        TypeLoseGame = TypeLoseGame.TimeUp;
        gameViewUI.ShowRevive(currentLevel, TypeLoseGame);
        //gameViewUI.ShowEndGame(false, currentLevel);
    }
    public void ShowFxCross(Transform parent)
    {
        GameObject go = Instantiate(GameDataSO.PrefabFxPairDestroy, parent);
        go.transform.localPosition = Vector3.zero;
        //Destroy(go, 2);
    }
    public void AddMoreTime(int addTime)
    {
        gameViewUI.InitTimer(gameViewUI.GetCurrentTime() + addTime, true);
    }
    private Vector3 getPosForNextSlot(bool isRight)
    {
        float spacing = 1.3f;
        
        float totalWidth = (listSlot.Count - 1) * spacing;
        Vector3 offset = new Vector3(-totalWidth / 2, 0, 0);

        
        float index = isRight ? (listSlot.Count) : -0.5f;
        return new Vector3(index * spacing, 0, 0) + offset;
    }
    private void addNewSlot(int addSlot)
    {
        bool isRight = listSlot.Count % 2 == 0;
        Slot newSlot = Instantiate(prefabSlot, transformContainerSlots);
        newSlot.transform.localPosition = getPosForNextSlot(isRight);
        newSlot.OnRelease = onHaveSlotRelease;
        newSlot.ClearSnake();
        if (isRight)
        {
            listSlot.Add(newSlot);
        }
        else
        {
            listSlot.Insert(0, newSlot);
        }
        
        
        //Slot slotLeft = Instantiate(prefabSlot, transformContainerSlots);
        //slotLeft.transform.localPosition = new Vector3(-1 * spacing, 0, 0) + offset;
        //listSlot.Add(slotLeft);

        //Slot slotRight = Instantiate(prefabSlot, transformContainerSlots);
        //slotRight.transform.localPosition = new Vector3(maxSlot * spacing, 0, 0) + offset;
        //listSlot.Add(slotRight);

        for (int i = 0; i < listSlot.Count; i++)
        {
            listSlot[i].IndexSlot = i;
        }
        newSlot.transform.localScale = Vector3.zero;
        newSlot.transform.localScale = Vector3.zero;
        newSlot.transform.DOScale(1.2f, 0.05f).OnComplete(() => {
            newSlot.ShowFxAppear();
            newSlot.transform.DOScale(1, 0.15f).OnComplete(() => {
            });
        });
        //slotLeft.transform.DOScale(1.2f, 0.05f).OnComplete(() => {
        //    slotLeft.ShowFxAppear();
        //    slotLeft.transform.DOScale(1, 0.15f).OnComplete(() => {
                
        //    });
        //});

        SoundController.Instance.PlaySoundEffectOneShot("AddHole");

        if (adsSlot != null)
        {
            isRight = listSlot.Count % 2 == 0;
            adsSlot.transform.localPosition = getPosForNextSlot(isRight);
        }
    }
    void onHaveSlotRelease(Slot slot, float waitRelease)
    {
        Debug.LogError("onHaveSlotRelease");
        if (listWaitingSnake.Count > 0)
        {
            Snake snake = listWaitingSnake[0];
            listWaitingSnake.RemoveAt(0);
            List<Vector3> list = new List<Vector3>();
            Vector3 slotPos = slot.transform.position;
            slotPos.y -= 0.1f;//offset cho dep
            slotPos.z = -5;
            list.Add(slotPos);


            if (slot) slot.IsFull = true;
            float timeDelay = waitRelease;
            snake.MoveByPath(timeDelay, null, list, false, -1, () =>
            {
                Debug.LogError("move finish");
                if (slot) slot.SetSnake(snake);
            }, () =>
            {
                snakeEscapse(snake);
            }, () =>
            {

                if (slot) slot.ShowSlot(snake.SnakeData.SelectedColor);
            });
        }
    }
    private void snakeEscapse(Snake snake)
    {
        if (snake.SnakeData.Obtacle == SnakeObtacle.Bomb)
        {
            Transform transformBomb = snake.ObstacleTurn.GetTransform();
            transformBomb.SetParent(transform);
            Transform container = transformBomb.Find("Container");
            float time = 0.5f;
            container.DOScale(new Vector3(1.35f, 1.3f, 1), time);
            float _y = transformBomb.localPosition.y;
            transformBomb.DOLocalMoveY(_y + 1, time).OnComplete(() =>
            {
                container.DOScale(0, 0.2f).SetDelay(0.05f).OnComplete(() =>
                {
                    GameObject go = Instantiate(GameDataSO.PrefabFxBombRelease, transformBomb);
                    go.transform.position = container.position;
                    this.Wait(0.5f, () => {
                        Destroy(go);
                        Destroy(transformBomb.gameObject);
                        snake.ObstacleTurn = null;
                        snake.SnakeData.Obtacle = SnakeObtacle.None;
                    });
                });
            });
            container.DOLocalRotate(new Vector3(0, 0, 480), time, RotateMode.FastBeyond360);
        }
    }
    public void BombExplosion(Snake snake)
    {
        SoundController.Instance.PlaySoundEffectOneShot("Explore");
        Transform transformBomb = snake.ObstacleTurn.GetTransform();
        GameObject go = Instantiate(GameDataSO.PrefabFxBombExplosion, transform);
        Vector3 pos = transformBomb.position;
        pos.z = -1.5f;
        go.transform.position = pos;
        Destroy(transformBomb.gameObject);
        snake.ObstacleTurn = null;
        gameRun = false;
        AllowAction = false;
        this.Wait(1, () => {
            TypeLoseGame = TypeLoseGame.Bomb;
            gameViewUI.ShowRevive(currentLevel, TypeLoseGame);
        });
    }
    public void OnRevive()
    {
        gameRun = true;
        AllowAction = true;
        gameViewUI.ResumeTime();
        if (TypeLoseGame == TypeLoseGame.TimeUp) AddMoreTime(20);
        else if (TypeLoseGame == TypeLoseGame.NoSpace) addNewSlot(1);
        TrackingHelper.MissionStep(currentLevel == 1,
            TrackingConstant.MType_MAIN,
            TrackingConstant.MName_MAIN_NAME + currentLevel,
            currentLevel.ToString(),
            score,
            GameUtils.GetMissionDataTracking(currentLevel));
    }
    public void QuitGame()
    {
        listButterFlyRun.Clear();
        isQuitGame = true;
        AllowAction = false;
        gameRun = false;
        TrackingHelper.MissionAbandoned(false,
            TrackingConstant.MType_MAIN,
            TrackingConstant.MName_MAIN_NAME + currentLevel,
             currentLevel.ToString(),
              score,
              GameUtils.GetMissionDataTracking(currentLevel)
            );
    }
    public void SetDepthCameraButterFly(int d)
    {
        //cameraButterFly.depth = d;
    }
}
