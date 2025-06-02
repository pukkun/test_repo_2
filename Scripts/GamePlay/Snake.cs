using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Snake : MonoBehaviour, IPointerClickHandler
{
    private float delayClick = 0;
    bool isRunning;
    bool isInSlot;
    public SnakeData SnakeData;
    public System.Action<Snake, bool> OnClick;
    public System.Action OnRunStart;
    public System.Action OnRunFinish;
    public System.Action<Snake> OnSnakeEscape;
    public List<DataSegment> Segments = new List<DataSegment>();
    public float moveSpeed = 1f;
    public float segmentDistance = 1f;
    private Vector3 direction = Vector3.right;
    private bool moveByHead = true;
    private List<Transform> listBodys = new List<Transform>();
    [SerializeField] private List<Vector3> positions = new List<Vector3>();
    public List<Vector2Int> ListCoors = new List<Vector2Int>();

    public List<Vector3> TestTarget = new List<Vector3>();
    bool isMoving;

    public ISnakeTurn ObstacleTurn;

    private int stepMove;

    private int countCollectView;
    private int countCollectLogic;
    public bool IsDone;
    private Transform transformAnimHead;
    TMPro.TextMeshPro textRemain;
    private Dictionary<DataSegment, Sequence> dicAnimFocus = new Dictionary<DataSegment, Sequence>();

    public Singum Singum;

    HeadController headController;
    public HeadController HeadController => headController;

    [System.Serializable]
    public class SnakeObstacle : ISnakeTurn
    {
        public Snake Snake;
        public Transform Transform;
        public int Turn;
        public TMPro.TextMeshPro Text;
        public System.Action OnOpen;
        public System.Action OnExplosion;
        public void ShowTurnWithColor()
        {
            if (Turn >= 4)
            {
                Text.color = ColorHelper.HexToColor("#00FF00");
            }
            else if (Turn >= 2)
            {
                Text.color = ColorHelper.HexToColor("#FF9933");
            }
            else if (Turn >= 1)
            {
                Text.color = ColorHelper.HexToColor("#FF0000");
            }
        }
        public void EarlyTurn()
        {
            if (Snake.SnakeData.Obtacle == Enums.SnakeObtacle.Bomb)
            {
                Snake.SnakeData.BomdDefused = true;
            }
        }
        public Transform GetTransform()
        {
            return Transform;
        }
        public void ActiveTurn()
        {
            if (Turn > 0)
            {
                Turn--;
                if (Snake.SnakeData.Obtacle == Enums.SnakeObtacle.Bomb)
                {
                    if (Turn == 1)
                    {
                        if(!SoundController.Instance.IsPlayingSoundEffect("bombWarning")) SoundController.Instance.PlaySoundEffectOneShot("bombWarning");
                    }
                    ShowTurnWithColor();
                }
                
                Text.transform.DOScale(1.2f, 0.1f).OnComplete(() => {
                    Text.text = Turn.ToString();
                    Text.transform.DOScale(1, 0.1f).OnComplete(() => {

                    });
                });
                
                //
                if (Turn <= 0)
                {
                    if (Snake.SnakeData.Obtacle == Enums.SnakeObtacle.Bomb)
                    {
                        if (!Snake.SnakeData.BomdDefused)
                        {
                            OnExplosion?.Invoke();
                        }
                        return;
                    }
                    // them effect
                    ShowEffectBreakObstacle(() =>
                    {
                        Snake.DoShakeScale();
                        Transform.gameObject.SetActive(false);
                        OnOpen?.Invoke();
                    });

                }
            }
        }
        public void ShowEffectBreakObstacle(System.Action complete)
        {
            if (Snake.SnakeData.Obtacle == Enums.SnakeObtacle.Ken)
            {
                Debug.LogError("Transform "+ Transform);
                GameObject prefab = GameManager.Instance.GameDataSO.PrefabFxPodBreak;
                for (int i = 0; i < Transform.childCount; i++)
                {
                    Transform parent = Transform.GetChild(i);
                    GameObject go = Instantiate(prefab, Snake.transform);
                    go.transform.position = parent.transform.position;
                    go.transform.localScale = parent.transform.localScale * 0.7f;
                }
                SoundController.Instance.PlaySoundEffectOneShot("WormCocoon");
                DOVirtual.DelayedCall(0.3f, () => { complete?.Invoke(); });
                return;
            }
            else if (Snake.SnakeData.Obtacle == Enums.SnakeObtacle.Xich)
            {
                Debug.LogError("Transform " + Transform);
                GameObject prefab = GameManager.Instance.GameDataSO.PrefabFxXichBreak;
                GameObject go = Instantiate(prefab, Snake.transform);
                Transform parent = Snake.ObstacleTurn.GetTransform();
                Transform target = parent.Find("TargetFx");
                Vector3 pos = target.transform.position;
                pos.z = -1;
                go.transform.position = pos;
                go.transform.localScale = parent.transform.localScale * 0.25f;
                DOVirtual.DelayedCall(0.0f, () => { complete?.Invoke(); });
                return;
            }
            complete?.Invoke();
        }
    }
    private void Awake()
    {
        initObstacleObject();
        headController = Segments[0].Transform.GetComponent<HeadController>();
        headController.Init(GameManager.Instance.GameDataSO, SnakeData.SelectedColor);
        // tat anh sang tren dau sau
        //MeshRenderer meshRenderer = transformAnimHead.GetComponentInChildren<MeshRenderer>();
        //meshRenderer.material.SetColor("_SpecColor", Color.black);
        transform.localPosition = new Vector3(0, 0, 1);
    }
    void Start()
    {
        foreach (var segment in Segments)
        {
            listBodys.Add(segment.Transform);
            Vector3 pos = segment.Transform.position;
            positions.Add(pos);
            segment.Scale = segment.Transform.localScale;
            //if (segment.Transform.name == "1")
            //{
            //    textRemain = Instantiate(MainGameController.Instance.MainSO.PrefabTextRemainInSnake, segment.Transform);

            //    textRemain.transform.localPosition = new(0, 0, -1);
            //    textRemain.text = GetRemainView().ToString();
            // }
        }
        OnRunStart += () => {
            //textRemain.text = string.Empty;
            isRunning = true;
        };
        OnRunFinish += () => { 
            isRunning = false;
            isInSlot = true;
        };
    }
    public void ResetDataPosition()
    {
        positions.Clear();
        foreach (var segment in Segments)
        {
            Vector3 pos = segment.Transform.position;
            positions.Add(pos);
        }
    }
    //void Update()
    //{
    //    if (textRemain != null) textRemain.transform.eulerAngles = Vector3.zero;
    //}
    private void initObstacleObject()
    {
        if (SnakeData.Obtacle == Enums.SnakeObtacle.None) return;
        SnakeObstacle snakeObstacle = new SnakeObstacle();
        snakeObstacle.Snake = this;
        snakeObstacle.Turn = SnakeData.Turn;
        if (SnakeData.Obtacle == Enums.SnakeObtacle.Xich)
        {
            Transform transformChain = Segments[0].Transform.Find("Chains");
            snakeObstacle.Text = transformChain.GetComponentInChildren<TMPro.TextMeshPro>();
            snakeObstacle.Transform = transformChain;
            snakeObstacle.Turn = SnakeData.Turn;
        } else if (SnakeData.Obtacle == Enums.SnakeObtacle.Ken)
        {
            Transform transformPod = transform.Find("Pods");
            snakeObstacle.Text = transformPod.Find("BodyPodHead").GetComponentInChildren<TMPro.TextMeshPro>(true);
            snakeObstacle.Transform = transformPod;
            snakeObstacle.Turn = SnakeData.Turn;

        }
        else if (SnakeData.Obtacle == Enums.SnakeObtacle.Bomb)
        {
            Transform transformBomb = Segments[0].Transform.Find("Bomb");
            snakeObstacle.Text = transformBomb.GetComponentInChildren<TMPro.TextMeshPro>();
            snakeObstacle.Transform = transformBomb;
            snakeObstacle.Turn = SnakeData.Turn;
            snakeObstacle.ShowTurnWithColor();
        }
        snakeObstacle.Text.text = snakeObstacle.Turn.ToString();
        snakeObstacle.OnOpen += onHandleObstacleComplete;
        snakeObstacle.OnExplosion += onHandleObstacleExplosion;
        ObstacleTurn = snakeObstacle;
    }
    
    private void onHandleObstacleComplete()
    {
        if (SnakeData.Obtacle == Enums.SnakeObtacle.Ken)
        {
            foreach (var item in Segments)
            {
                item.Transform.gameObject.SetActive(true);
            }
        }
        SnakeData.Obtacle = Enums.SnakeObtacle.None;
    }
    private void onHandleObstacleExplosion()
    {
        if (SnakeData.Obtacle == Enums.SnakeObtacle.Bomb)
        {
            GameManager.Instance.BombExplosion(this);
        }
        SnakeData.Obtacle = Enums.SnakeObtacle.None;
    }
    public void CheckTurn()
    {
        if (ObstacleTurn != null) ObstacleTurn.ActiveTurn();
    }
    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Space))
    //    //{
    //    //    if (TestTarget.Count > 0)
    //    //    {
    //    //        move(TestTarget[0]);
    //    //    }
    //    //}
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        MoveAHead();
    //    }
    //    // Điều khiển hướng bằng phím
    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        direction = Vector3.up;
    //        MoveSnake();

    //    }
    //    else if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        direction = Vector3.down;
    //        MoveSnake();
    //    }
    //    else if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        direction = Vector3.left;
    //        MoveSnake();
    //    }
    //    else if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        direction = Vector3.right;
    //        MoveSnake();
    //    }
    //    // Chuyển đổi chế độ di chuyển
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        moveByHead = !moveByHead;
    //    }

    //}
    private void move(PortalTarget portalTarget, Vector3 newHeadPos, bool isFinish, bool fourLasted, System.Action callback = null)
    {
        
        SoundController.Instance.PlaySoundEffectOneShot("sndWalk");
        positions.Insert(0, newHeadPos);
        positions.RemoveAt(positions.Count - 1);
        Vector3 tail = (positions[positions.Count - 3] - positions[positions.Count - 2]).normalized;
        float disTail = 0.3f;
        Vector3 finalTail = positions[positions.Count - 2] - tail * disTail;
        float factorScale = 0;
        float posNextHead = 0;
        if (isFinish)
        {
            factorScale = GameManager.Instance.ScaleFactor();
            posNextHead = newHeadPos.y - 0.5f * factorScale;
        }
        if (isFinish)
        {
            //Debug.LogError("chui lo " + Segments[0].Transform.name + " --- " + newHeadPos);
        }
        for (int i = 0; i < Segments.Count; i++)
        {
            int index = i;
            Vector3 targetPos = positions[i];
            if (index == Segments.Count - 1)
            {
                targetPos = finalTail;
            }
            DataSegment dataSegment = Segments[index];
            
            Enums.Direction newDirection = GameUtils.GetDirection(Segments[i].Transform.position, targetPos);
            if (newDirection == Enums.Direction.None) newDirection = dataSegment.Direction;
            dataSegment.Direction = newDirection;
            Vector3 rotate = GetRotateHead(newDirection);
            Quaternion quaternion = GetRotateBody(newDirection);
            float time = 0.04f;
            if (fourLasted)
            {
                time = 0.07f;
                dataSegment.SetScale(Vector3.zero);
            }
            if (isFinish)
            {
                time = 0;
            }
            if (dataSegment.TransformFoots != null) dataSegment.TransformFoots.eulerAngles = rotate;
            //Debug.LogError("isFinishisFinish " + isFinish + " -- " + targetPos + " --- "+time + " -- "+ dataSegment.Transform.name);
            dataSegment.Transform.DOMove(targetPos, time * 1.2f).OnComplete(() => {
                if (isFinish)
                {
                    if (index <= 3)//setup 4 doan nam vi tri lo
                    {
                        Vector3 posHole = dataSegment.Transform.position;
                        float z = posHole.z;
                        posHole = positions[0];
                        posHole.z = z;
                        dataSegment.Transform.DOMove(posHole, 0f);
                    }
                    if (index == 0)
                    {
                        //targetPos.z = 2;
                        //dataSegment.Transform.DOScale(0, time);
                        //dataSegment.Transform.DOMove(targetPos, time);
                    }
                }
                if (index == 0) dataSegment.Transform.eulerAngles = rotate;
                if (dataSegment.TransformFoots != null) dataSegment.TransformFoots.eulerAngles = rotate;
                //if (dataSegment.Transform != null) dataSegment.Transform.rotation = quaternion;
                if (index == 0)
                {
                    isMoving = false;
                    if (isFinish)// && Segments.Count > 4
                    {
                        dataSegment.SetScale(Vector3.zero);
                        dataSegment.Transform.eulerAngles = new Vector3(0, 0, 180);
                        targetPos.z = -3;
                        //dataSegment.Transform.DOMove(targetPos, 0.2f);
                        //dataSegment.TweenScale(dataSegment.Scale, 0.2f);
                        foreach (var item in Segments)
                        {
                            if (item.Transform.name != "Head" && item.Transform.name != "1")
                            {
                                item.Transform.gameObject.SetActive(false);
                            }
                        }
                        //moveCollapse_ver2(1, newHeadPos, factorScale, portalTarget);
                    }
                    callback?.Invoke();
                }
                else if (index == Segments.Count)
                {

                }
            });
            //segments[i].position = positions[i];
        }
    }
    private void moveCollapse_ver2(int startSegement, Vector3 newHeadPos, float factorScale, PortalTarget portalTarget)
    {
        float time = 0.04f;
        if (Segments.Count < 2)
        {
            portalTarget.DoNormal(time);
            return;
        }
        Debug.LogError("chui lo " + Segments[startSegement].Transform.name + " --- "+ newHeadPos);
        for (int i = startSegement; i < Segments.Count; i++)
        {
            int index = i;
            if (i - 1 < positions.Count)
            {
                Vector3 targetPos = positions[i - 1];
                targetPos.z = -0.5f * Segments.Count;
                DataSegment dataSegment = Segments[index];
                Segments[i].Transform.DOMove(targetPos, time).OnComplete(() =>
                {
                    
                    if (index == startSegement)
                    {
                        targetPos.z = 2;
                        dataSegment.Transform.DOScale(0, time).SetDelay(0.2f);
                        dataSegment.Transform.DOMove(targetPos, time).SetDelay(0.2f);
                        Segments.RemoveAt(0);
                        moveCollapse_ver2(startSegement, newHeadPos, factorScale, portalTarget);
                    }
                });
            }

        }
    }
    private void moveCollapse(int startSegement, int offset, Vector3 newHeadPos, float factorScale)
    {
        if (true || Segments.Count < 5)
        {
            //positions.RemoveAt(0);
        }
        //Debug.LogError("newHeadPos " + Segments.Count);
        //Vector3 tail = (positions[Segments.Count - 2] - positions[Segments.Count - 1]).normalized;
        //float disTail = 0.3f;
        //Vector3 finalTail = positions[Segments.Count - 1] - tail * disTail;

        for (int i = startSegement; i < Segments.Count; i++)
        {
            int index = i;
            if (i - 1 < positions.Count)
            {
                Vector3 targetPos = positions[i - 1];
                if (i == Segments.Count - 1)
                {
                    //targetPos = finalTail;
                }
                //Debug.LogError("Segments[i]" + Segments[i].Transform.name + "-- " + targetPos);
                Segments[i].Transform.DOMove(targetPos, 0.03f).OnComplete(() =>
                {
                    if (index == startSegement)
                    {
                        if (Segments.Count > 4)
                        {
                            Segments.RemoveAt(1);
                            if (Segments.Count == 4)
                            {
                                positions.RemoveAt(0);
                            }
                            moveCollapse(startSegement, offset, newHeadPos, factorScale);
                        }
                    }
                });
            }

        }
    }
    void MoveSnake()
    {
        if (moveByHead)
        {
            // Tính vị trí mới cho đầu
            Vector3 newHeadPos = Segments[0].Transform.position + direction * segmentDistance;

            // Kiểm tra va chạm với thân (bỏ qua đầu)
            for (int i = 1; i < Segments.Count; i++)
            {
                if (Vector3.Distance(newHeadPos, Segments[i].Transform.position) < 0.1f)
                {
                    Debug.Log("Chạm thân! Di chuyển bị chặn.");
                    return; // Chặn di chuyển
                }
            }

            // Di chuyển từ đầu
            positions.Insert(0, newHeadPos);
            positions.RemoveAt(positions.Count - 1);

            // Cập nhật vị trí các khối
            for (int i = 0; i < Segments.Count; i++)
            {
                Segments[i].Transform.position = positions[i];
            }
        }
        else
        {
            // Tính vị trí mới cho đuôi, nhưng giữ hướng điều khiển nhất quán
            Vector3 newTailPos = Segments[Segments.Count - 1].Transform.position + direction * segmentDistance;

            // Kiểm tra va chạm với thân (bỏ qua đuôi)
            for (int i = 0; i < Segments.Count - 1; i++)
            {
                if (Vector3.Distance(newTailPos, Segments[i].Transform.position) < 0.1f)
                {
                    Debug.Log("Chạm thân! Di chuyển bị chặn.");
                    return; // Chặn di chuyển
                }
            }

            // Di chuyển từ đuôi
            positions.RemoveAt(0);
            positions.Add(newTailPos);

            // Cập nhật vị trí các khối
            for (int i = 0; i < Segments.Count; i++)
            {
                Segments[i].Transform.position = positions[i];
            }
        }
    }
    public void RemoveSingum(bool userInteract, out bool haveRemove)
    {
        haveRemove = false;
        if (Singum != null)
        {
            haveRemove = true;
            if(userInteract) GameManager.Instance.ShowFxCross(Singum.SpriteRenderer.transform);
            Singum.SpriteRenderer.DOFade(0, 0.1f).SetDelay(0.1f).OnComplete(() =>
            {
                //Singum.gameObject.SetActive(false);
            });
            this.Wait(2, () =>
            {
                Singum.gameObject.SetActive(false);
            });
        }
    }
    public void MoveByPath(float timeDelay, PortalTarget portalTarget, List<Vector3> paths, bool isEscape, int observeStepEscape, System.Action callback, System.Action callbackEscape = null, System.Action callbackPreHole = null)
    {
        StartCoroutine(enumeratorMove(timeDelay, portalTarget, paths, isEscape, observeStepEscape, callback, callbackEscape, callbackPreHole));
    }
    private IEnumerator enumeratorMove(float timeDelay, PortalTarget portalTarget, List<Vector3> paths, bool isEscape, int observeStepEscape, System.Action callback, System.Action callbackEscape = null, System.Action callbackPreHole = null)
    {
        
        stepMove = 0;
        yield return new WaitForSeconds(timeDelay);
        while (paths.Count > 0)
        {
            Vector3 nextPos = paths[0];
            paths.RemoveAt(0);
            stepMove++;
            isMoving = true;
            if (stepMove == observeStepEscape && observeStepEscape > 0)
            {
                //SoundController.Instance.PlaySoundEffectOneShot("SFX_UI_Click_Select_1");
                OnSnakeEscape?.Invoke(this);
                callbackEscape?.Invoke();
            }
            if (paths.Count == 2)
            {
                callbackPreHole?.Invoke();
            }
            move(portalTarget, nextPos, paths.Count == 0 && isEscape, paths.Count <= SnakeData.segments.Count - 1);
            yield return new WaitUntil(() => isMoving == false);
        }
        
        yield return null;
        ShowPrepareComplete(() => {
            
            
            ChangeToOutLine();
        });
        Debug.LogError("finish");
        OnRunFinish?.Invoke();
        callback?.Invoke();

    }
    public float scaleFactor;
    public Transform pivotPoint;
    public void MoveAHead()
    {
        //Vector3 head = Segments[0].Transform.position;
        //Vector3 next = Segments[1].Transform.position;
        //Vector3 direction = (next - head);
        Debug.LogError("direction " + direction);


    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        
        if (isInSlot) return;
        Debug.LogError($"OnPointerClick {isRunning}");
        if (SnakeData.Obtacle != Enums.SnakeObtacle.None && SnakeData.Obtacle != Enums.SnakeObtacle.Bomb)
        {
            RunAnimShakeHead();
            return;
        }
        
        if (isRunning) return;
        if (delayClick > 0) return;
        OnClick?.Invoke(this, true);
    }
    public void RunAnimShakeHead()
    {
        SoundController.Instance.PlaySoundEffectOneShot($"sndNo{GameUtils.RandomRange(1, 6)}");
        float time = 0.1f;
        headController.TransformHead.DOLocalRotate(new Vector3(-90, 0, -5), time).OnComplete(() => {
            headController.TransformHead.DOLocalRotate(new Vector3(-90, 0, 5), time).OnComplete(() =>
            {
                headController.TransformHead.DOLocalRotate(new Vector3(-90, 0, -0), time);
            });
        });
    }
    public void Look(Transform loook)
    {
        if (loook == null) return;
        DOTween.Kill(Segments[0].Transform, true);
        Vector3 direction = loook.position - Segments[0].Transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //Segments[0].Transform.localRotation = Quaternion.Euler(0f, 0f, 90 + angle);
        Segments[0].Transform.DOLocalRotate(new Vector3(0f, 0f, 90 + angle), 0.1f);
    }
    public void SetStateRunning(bool isRun)
    {
        isRunning = isRun;
    }
    [System.Serializable]
    public class DataSegment
    {
        public Transform Transform;
        public Transform TransformFoots;
        public Enums.Direction Direction;
        public Vector3 Scale;

        public void SetScale(Vector3 scale)
        {
            //if (Transform.name == "Head")
            //{
            //    Debug.LogError("SetScale " + scale);
            //}
            Transform.localScale = scale;
        }
        public void TweenScale(Vector3 scale, float time)
        {
            //if (Transform.name == "Head")
            //{
            //    Debug.LogError("SetScale TweenScale " + scale);
            //}
            Transform.DOScale(scale, time);
        }
    }

    //void OnMouseDown()
    //{
    //    Debug.LogError("OnMouseDown");
    //}

    public Vector2Int GetHeadCoor()
    {
        return ListCoors[0];
    }
    public Enums.Direction GetHeadDirection()
    {
        return Segments[0].Direction;
    }

    public void UpdateCoor(Vector2Int newCoor)
    {
        for (int i = ListCoors.Count - 1; i >= 0; i--)
        {
            if (i > 0)
            {
                ListCoors[i] = ListCoors[i - 1];
            }
            else
            {
                ListCoors[0] = newCoor;
            }
        }
    }
    public Vector3 GetRotateHead(Enums.Direction direction)
    {
        if (direction == Enums.Direction.Left) return new Vector3(0, 0, -90);
        else if (direction == Enums.Direction.Right) return new Vector3(0, 0, 90);
        else if (direction == Enums.Direction.Down) return new Vector3(0, 0, 0);
        else if (direction == Enums.Direction.Up) return new Vector3(0, 0, 180);
        return Vector3.zero;
    }
    public static Quaternion GetRotateBody(Enums.Direction direction)
    {
        if (direction == Enums.Direction.Left) return new Quaternion(0, 0, 90,0);
        else if (direction == Enums.Direction.Right) return new Quaternion(0, 0, 90, 0);
        else if (direction == Enums.Direction.Down) return new Quaternion(0, 0, 0, 0);
        else if (direction == Enums.Direction.Up) return new Quaternion(0, 0, 0, 0);
        return Quaternion.identity;
    }

    public void DoShakeScale(float strong = 1.3f)
    {
        foreach (var item in Segments)
        {
            Vector3 scale = item.Scale;
            item.Transform.DOScale(scale * strong, 0.05f).OnComplete(() =>
            {
                item.Transform.DOScale(scale, 0.05f).OnComplete(() =>
                {
                });
            });
        }
    }
    public void DoDisAppear(Vector3 pivotPoint, System.Action action)
    {
        float time = .2f;
        float scaleFactor = 0;
        foreach (var obj in listBodys)
        {
            // Get the current offset from the pivot
            Vector3 offset = obj.position - pivotPoint;

            // Scale the offset by the scale factor
            Vector3 newOffset = offset * scaleFactor;

            // Update position: new position = pivot + scaled offset
            //obj.Transform.position = pivotPoint.position + newOffset;

            // Apply the scale to the GameObject
            //obj.Transform.localScale *= scaleFactor;
            obj.DOScale(scaleFactor, time);
            obj.DOMove(pivotPoint + newOffset, time);
        }
        this.Wait(time, () => { 
            IsDone = true; 
            action?.Invoke();
            gameObject.SetActive(false);
        });
    }

    public void DoScaleInSlot(Vector3 pivotPoint, System.Action action)
    {
        //return;
        float time = .1f;
        float scaleFactor = 0.8f / GameManager.Instance.LevelManager.transform.localScale.x;
        Vector3 offset = transform.position - pivotPoint;
        Vector3 newOffset = offset * scaleFactor;
        transform.DOScale(scaleFactor, time);
        transform.DOMove(pivotPoint + newOffset, time);
        return;
        //foreach (var obj in listBodys)
        //{
        //    // Get the current offset from the pivot
        //    Vector3 offset = obj.position - pivotPoint;

        //    // Scale the offset by the scale factor
        //    Vector3 newOffset = offset * scaleFactor;

        //    // Update position: new position = pivot + scaled offset
        //    //obj.Transform.position = pivotPoint.position + newOffset;

        //    // Apply the scale to the GameObject
        //    //obj.Transform.localScale *= scaleFactor;
        //    obj.DOScale(scaleFactor, time);
        //    obj.DOMove(pivotPoint + newOffset, time);
        //}
    }

    public int GetRemainView()
    {
        int result =  (SnakeData.cells.Count -1) * 5 - countCollectView;
        return result;
    }
    public int GetRemainLogic()
    {
        int result = (SnakeData.cells.Count - 1) * 5 - countCollectLogic;
        return result;
    }
    public void CollectView()
    {
        countCollectView++;
    }
    public void CollectLogic()
    {
        countCollectLogic++;
    }
    public Vector3 getFakePosEverySegment(DataSegment segment)
    {
        Enums.Direction direction = segment.Direction;
        Vector3 next = Vector3.zero;
        float delta = 0.05f;
        switch (direction)
        {
            case Enums.Direction.None:
                break;
            case Enums.Direction.Up:

                next = new Vector3(segment.Transform.position.x, segment.Transform.position.y + delta);
                break;
            case Enums.Direction.Down:
                next = new Vector3(segment.Transform.position.x, segment.Transform.position.y - delta);
                break;
            case Enums.Direction.Left:
                next = new Vector3(segment.Transform.position.x - delta, segment.Transform.position.y + 0);
                break;
            case Enums.Direction.Right:
                next = new Vector3(segment.Transform.position.x + delta, segment.Transform.position.y + 0);
                break;
            default:
                break;
        }
        return next;
    }
    public void FakeNextMove()
    {
        positions.Clear();
        foreach (var segment in Segments)
        {
            Vector3 pos = segment.Transform.position;
            positions.Add(pos);
        }
        moveFake(rewind);
    }
    public void LookAt(Vector3 pos)
    {
        EyeAnimations eyeAnimations = transform.GetComponentInChildren<EyeAnimations>();
        eyeAnimations.LookPosition(pos);
    }
    public void ShowStun()
    {
        delayClick = 3;
        this.Wait(delayClick, () => { delayClick = 0; });
        headController.ShowStun();
    }
    public void LookAt(Transform transformLook)
    {
        EyeAnimations eyeAnimations = transform.GetComponentInChildren<EyeAnimations>();
        if(eyeAnimations != null) eyeAnimations.LookPosition(transformLook);
    }
    private void rewind()
    {
        EyeAnimations eyeAnimations = transform.GetComponentInChildren<EyeAnimations>();
        if (eyeAnimations != null)
        {
            SoundController.Instance.PlaySoundEffectOneShot("SFX_VOX_Female_Jump");
            eyeAnimations.ForceRunStun();
            Transform _t = eyeAnimations.transform.Find("FxStun");
            _t.gameObject.SetActive(true);
            this.Wait(1.5f, () =>
            {
                _t.gameObject.SetActive(false);
            });
        }
        Vector3 newTailPos = Segments[Segments.Count - 1].Transform.position + direction * segmentDistance;

        for (int i = 0; i < Segments.Count; i++)
        {
            int index = i;
            Vector3 targetPos = positions[i];
            DataSegment dataSegment = Segments[index];
            dataSegment.Transform.DOMove(targetPos, 0.05f).OnComplete(() => {
                if (index == 0)
                {
                    //callback?.Invoke();
                }
            });
            //segments[i].position = positions[i];
        }
    }
    private void moveFake(System.Action callback = null)
    {
        for (int i = 0; i < Segments.Count; i++)
        {
            int index = i;
            
            DataSegment dataSegment = Segments[index];
            Vector3 targetPos = getFakePosEverySegment(dataSegment);
            dataSegment.Transform.DOMove(targetPos, 0.05f).OnComplete(() => {
                if (index == 0)
                {
                    HapticHelper.DeviceVibrate();
                    callback?.Invoke();
                }
            });
            //segments[i].position = positions[i];
        }
    }
    bool check;
    public void ReturnNormal()
    {
        foreach (var item in Segments)
        {
            Sequence sequence = dicAnimFocus.Get(item);
            if (sequence != null) sequence.Kill();
            item.SetScale(item.Scale);
        }
    }
    public void ShowFocus(float strong = 1.1f)
    {
        if (SnakeData.Obtacle == Enums.SnakeObtacle.Xich) return;
        if (SnakeData.Obtacle == Enums.SnakeObtacle.Ken) return;
        dicAnimFocus.Clear();
        foreach (var item in Segments)
        {
            
            Sequence sequence = DOTween.Sequence();
            Vector3 scale = item.Scale;
            float time = 0.15f;
            sequence.Append(item.Transform.DOScale(scale * strong, time));
            sequence.Append(item.Transform.DOScale(scale, time));
            sequence.Append(item.Transform.DOScale(scale / strong, time));
            sequence.Append(item.Transform.DOScale(scale, time));
            sequence.SetLoops(int.MaxValue);
            sequence.Play();
            dicAnimFocus.Add(item, sequence);
        }
    }

    public void RemoveFxGhosting()
    {
        Transform transform = listBodys[listBodys.Count - 2].Find("FxGhostingTail");
        if(transform != null) Destroy(transform.gameObject);

        transform = Segments[0].Transform.Find("FxGhostingHead");
        if (transform != null) Destroy(transform.gameObject);

        foreach (var item in Segments)
        {
            transform = item.Transform.Find("FxGhostingHead");
            if (transform != null) Destroy(transform.gameObject);
        }
    }
    public void ChangeAlphaMaterial()
    {
        Material material = GameManager.Instance.GameDataSO.AlphaMaterial;
        foreach (var item in Segments)
        {
            item.Transform.GetComponentInChildren<MeshRenderer>().material = material;
        }
    }
    public void RevertMaterial()
    {
        Material material = GameManager.Instance.GameDataSO.GetMaterial(SnakeData.SelectedColor);
        foreach (var item in Segments)
        {
            item.Transform.GetComponentInChildren<MeshRenderer>().material = material;
        }
    }
    public void AddTrailToTail()
    {
        GameObject prefab = Instantiate(GameManager.Instance.GameDataSO.PrefabTrailGhost, listBodys[listBodys.Count - 2]);
        prefab.name = "FxGhostingTail";
        prefab.transform.localPosition = Vector3.zero;
    }
    public void AddFxGhostingToHead()
    {
        //GameObject prefab = Instantiate(GameManager.Instance.GameDataSO.PrefabHeadGhosting, Segments[0].Transform);
        //prefab.name = "FxGhostingHead";

        foreach (var item in Segments)
        {
            GameObject prefab = Instantiate(GameManager.Instance.GameDataSO.PrefabHeadGhosting, item.Transform);
            prefab.name = "FxGhostingHead";
        }
    }
    public Snake GetPairSnake()
    {
        if (Singum != null)
        {
            foreach (var item in Singum.ListSnakePairing)
            {
                if (item != this) return item;
            }
        }
        return null;
    }
    public void ChangeToOutLine()
    {
        return;
        Material outLine = GameDataSO.Instance.GetDataMaterial(SnakeData.SelectedColor).Material_Outline;
        MeshRenderer meshRenderer = headController.TransformHead.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = outLine;
        MeshRenderer _meshRenderer = Segments[1].Transform.GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material = outLine;
    }
    public void ShowPrepareComplete(System.Action callback)
    {
        RevertMaterial();
        float factor = GameManager.Instance.ScaleFactor();
        foreach (var item in Segments)
        {
            item.Scale /= factor;
            item.SetScale(item.Scale);
        }
        Vector3 pos = Segments[0].Transform.position;
        pos.y -= 0.25f;
        Vector3 pos1 = pos;
        pos1.z = pos.z + 0.2f;
        Segments[1].Transform.DOMove(pos1, 0);
        Segments[1].TweenScale(Segments[1].Scale, 0);
        Segments[0].TweenScale(Segments[0].Scale, 0);
        pos.y += 0.5f;
        //pos.y -= 0.15f;
        Segments[0].Transform.DOMove(pos, 0.15f).OnComplete(()=> {
            this.Wait(0.05f, () =>
            {
                callback?.Invoke();
            });
        });
    }

    public Transform GetTargetMove()
    {
        return headController.TransformMountTarget;
    }

    public void Shooting(Slot slot, TargetBlock target, TargetBlock left, TargetBlock right, float delay, out float timeMove)
    {
        float v = 20f;
        float time = 0.3f;
        Vector3 startPos = GetTargetMove().position;
        float dis = Vector3.Distance(startPos, target.transform.position);
        time = (dis / v);
        timeMove = time;
        //Debug.LogError("dis " + dis + " time: " + time + " delay "+ delay);
        this.Wait(delay, () => {
            Look(target.transform);
            FxWormProjectile projectile = PoolManager.Instance.GetFxWormProjectile();
            projectile.SetColor(SnakeData.SelectedColor);
            projectile.transform.position = GetTargetMove().position;
            projectile.ShowProjectile();
            projectile.transform.DOMove(target.transform.position, time).OnComplete(() => {
                target.ReturnToPool();
                if (left != null) left.DoExplosion(target.Rigidbody, Vector3.forward);
                if (right != null) right.DoExplosion(target.Rigidbody, Vector3.back);
                projectile.ShowExplosion();
                HapticHelper.DeviceVibrate();
                SoundController.Instance.PlaySoundEffectOneShot("SFX_Letter_Press_Soft_Bubble_Tier_05");
                DoShakeScale(1.08f);
                CollectView();
                slot.ShowRemain();
                if (GetRemainView() == 0)
                {
                    GameManager.Instance.AddButterToQueue(this, slot);
                }
            });
        });
        
    }
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Snake))]
    public class SnakeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Move AHead"))
            {
                var component = (Snake)target;
                component.MoveAHead();
            }
        }
    }
    //public Transform test;
    //void Update()
    //{
    //    Look(test);
    //    return;
    //    if (gameObject.name == "Snake 1")
    //    {
    //        Debug.LogError(Segments[Segments.Count - 1].Transform.transform.localScale);
    //    }
    //}
#endif
}
