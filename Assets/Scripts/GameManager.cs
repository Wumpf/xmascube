using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Helper;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public uint Score { get; private set; }

    public Camera GUICamera;
    public FancyButtonScript UndoButton;
    public GameObject CubePrefab;
    public GameObject CubeParentObject;
    public GameObject MiddleObject;
    public BgmScript BgmMusic;

    public enum State
    {
        MAINMENU,
        CREDITS,
        INGAME,
        WINNING
    };
    public State CurrentState = State.MAINMENU;

    private class Turn
    {
        private CubeBehaviour _removedObject0;
        private CubeBehaviour _removedObject1;

        public Turn(CubeBehaviour removedObject0, CubeBehaviour removedObject1)
        {
            _removedObject0 = removedObject0;
            _removedObject1 = removedObject1;
        }

        public void Do()
        {
            _removedObject0.Disappear();
            _removedObject1.Disappear();
        }
        public void Undo()
        {
            _removedObject0.gameObject.SetActive(true);
            _removedObject1.gameObject.SetActive(true);
            _removedObject0.Reappear();
            _removedObject1.Reappear();
        }
    };

    private CubeBehaviour[, ,] _level;
    private PuzzleBuilder _puzzleBuilder;

    private CubeBehaviour _selectedObject = null;
    private Stack<Turn> _turns = new Stack<Turn>();
    private RoundTimer _roundTimer;
    private Texture[] _cubeTextures;
    private Quaternion _mainCameraRotationDest;
    private int _currentRoundIndex = 0;

    private WindowResizeWatcher _resizeWatcher = new WindowResizeWatcher();
    private Vector2 creditsScrollPosition = Vector2.zero;

    // Use this for initialization
    void Start()
    {
        _mainCameraRotationDest = Camera.main.transform.rotation;
        _roundTimer = GetComponent<RoundTimer>();

        _cubeTextures = Resources.LoadAll<Texture>("Textures");
        _cubeTextures.OrderBy(tex => tex.name);

        GUICamera.enabled = false;
        //StartRound(_currentRoundIndex);

        // Undo button.
        UndoButton.ButtonClickedEvent += OnUndoButtonClicked;
        _resizeWatcher.ResizeEvent += (int width, int height) =>
        {
            float camHalfHeight = GUICamera.orthographicSize;
            float camHalfWidth = GUICamera.aspect * camHalfHeight;
            Vector3 rightBottomPosition = new Vector3(camHalfWidth, -camHalfHeight, 0.0f) + GUICamera.transform.position;
            rightBottomPosition += new Vector3(-UndoButton.ActiveSprite.bounds.size.x * 0.5f, UndoButton.ActiveSprite.bounds.size.y * 0.5f, 0.5f);
            UndoButton.transform.position = rightBottomPosition;
        };
        StartCoroutine(_resizeWatcher.CheckForResize());
    }

    void OnDestroy()
    {
        _resizeWatcher.Dispose();
    }

    void OnGUI()
    {
        if(CurrentState == State.MAINMENU)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 100, 150, 50), "Start Game"))
            {
                StartRound(0);
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2, 150, 50), "Credits"))
            {
                CurrentState = State.CREDITS;
            }
        }
        else if(CurrentState == State.CREDITS)
        {
            var content = new GUIContent("Game by:\nEnrico\nXenja\nAndreas\n\nAdditional materials:\nMusic:\n\nFour Winter by pheonton\nfound on\nhttp://opengameart.org/content/four-winter\n\nUI Sounds by  StumpyStrust\nfound on\nhttp://opengameart.org/content/ui-sounds\n\nPlatform small Sound Effect pack by RaoulWB\nfound on\nhttp://opengameart.org/content/platform-small-sound-effect-pack\n\nLively Meadow (Victory Fanfare and Song) by matthew.pablo\nfound on\nhttp://opengameart.org/content/lively-meadow-victory-fanfare-and-song\n\n\nIcons:\nhttp://www.iconarchive.com/show/oxygen-icons-by-oxygen-icons.org.1.html");

            creditsScrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2 - 150, Screen.height/2 - 100, 300, 200), creditsScrollPosition, new Rect(Screen.width/2 - 140, Screen.height/2 - 250, 400, 500));
            GUI.Label(new Rect(Screen.width/2 - 140, Screen.height/2 - 250, 400, 500), content, GUI.skin.textField );
            GUI.EndScrollView();

            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 100, 150, 50), "Back"))
            {
                CurrentState = State.MAINMENU;
            }
        }
    }

    private void StartRound(int roundIndex)
    {
        // Reset round properties
        if (_level != null)
        {
            foreach (var obj in _level)
            {
                if(obj != null)
                    GameObject.Destroy(obj.gameObject);
            }
        }
        _selectedObject = null;
        _turns.Clear();
        Score = 0;
        _roundTimer.Reset();
        CurrentState = State.INGAME;
        GUICamera.enabled = true;
        CubeParentObject.gameObject.transform.rotation = Quaternion.identity;
        CubeParentObject.gameObject.transform.localScale = Vector3.one;
        

        // Generate new level
        int levelSize = 1 + (roundIndex+1) * 2;
        if (levelSize > 7)
            levelSize = 7;
        _level = new CubeBehaviour[levelSize, levelSize, levelSize];
        _puzzleBuilder = new PuzzleBuilder();
        List<int> tileIdentifier = new List<int>();

        tileIdentifier.AddRange(Enumerable.Range(1, _cubeTextures.Length));
        //int[, ,] levelDesc = _puzzleBuilder.GenerateLevel(roundIndex+1,tileIdentifier);
        int[, ,] levelDesc = _puzzleBuilder.GenerateCubeLevel(levelSize, tileIdentifier);

        for (int x = 0; x < levelSize; ++x)
        {
            for (int y = 0; y < levelSize; ++y)
            {
                for (int z = 0; z < levelSize; ++z)
                {
                    if (levelDesc[x, y, z] == 0 || levelDesc[x,y,z] == -2)
                        continue;

                    GameObject gameObject = (GameObject)GameObject.Instantiate(CubePrefab, Vector3.zero, Quaternion.identity);
                    gameObject.transform.parent = CubeParentObject.transform;
                    gameObject.transform.position = Vector3.Scale(new Vector3(x - levelSize / 2, y - levelSize / 2, z - levelSize / 2),
                                                                  gameObject.transform.renderer.bounds.size);
                    gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    _level[x, y, z] = gameObject.GetComponent<CubeBehaviour>();
                    _level[x, y, z].OnClicked += OnCubeClicked;
                    _level[x, y, z].GridPosition = new Vector3(x, y, z);
                    _level[x, y, z].TypeIndex = levelDesc[x, y, z];

                    if (_cubeTextures.Length < levelDesc[x, y, z])
                    {
                        Debug.LogError("Not enough cube textures!");
                        continue;
                    }
                    ((MeshRenderer)_level[x, y, z].renderer).material.mainTexture = _cubeTextures[levelDesc[x, y, z] - 1];
                }
            }
        }

        float scale = 1.1f / Mathf.Sqrt(levelSize / 2);
        CubeParentObject.transform.localScale = new Vector3(scale, scale,scale);
        MiddleObject.transform.localScale = new Vector3(0.15f * scale, 0.15f * scale, 0.15f * scale);
        CubeParentObject.GetComponent<CubeRotator>().Reset();

        _currentRoundIndex = roundIndex;
        BgmMusic.PlayBgmInGame(true);
    }

    private void OnCubeClicked(CubeBehaviour cubeBehaviour)
    {
        // Don't select the middle object.
        if(cubeBehaviour.TypeIndex == 0)
        {
            cubeBehaviour.Fail();
        }
        else
        {
            // Check if removable
            uint numNeighbors = 0;
            List<Vector3> neighbors = _puzzleBuilder.GetNeighbors(cubeBehaviour.GridPosition);
            foreach(var pos in neighbors)
            {
                CubeBehaviour cube = _level[(int)pos.x, (int)pos.y, (int)pos.z];
                if (cube == null || cube.Active)
                //if (cube != null && cube.Active || pos == _puzzleBuilder.Center)
                    ++numNeighbors;
            }
            if (numNeighbors >= 4)
            {
                cubeBehaviour.Fail();
            }
            else
            {
                if (_selectedObject == null)
                {
                    _selectedObject = cubeBehaviour;
                    _selectedObject.Select();
                }
                else if (cubeBehaviour == _selectedObject)
                {
                    _selectedObject.Unselect();
                    _selectedObject = null;
                }
                else
                {
                    if (cubeBehaviour.TypeIndex != _selectedObject.TypeIndex)
                    {
                        cubeBehaviour.Fail();
                    }
                    else
                    {
                        Turn newTurn = new Turn(_selectedObject, cubeBehaviour);
                        newTurn.Do();
                        _turns.Push(newTurn);
                        _selectedObject = null;

                        CheckWin();
                    }
                }
            }
        }
    }

    private void CheckWin()
    {
        bool winning = true;
        foreach(var cube in _level)
        {
            if (cube != null && cube.Active)
            {
                winning = false;
                break;
            }
        }

        if (winning)
        {
            CurrentState = State.WINNING;
            BgmMusic.PlayBgmLevelClear(true);
            StartCoroutine(WinningAnimations());
        }
    }

    private IEnumerator WinningAnimations()
    {
        float winningAnimationStartTime = Time.timeSinceLevelLoad;

        var oldScale = MiddleObject.transform.localScale;
        var oldRotation = MiddleObject.transform.rotation;

        const float animationLength = 6.0f;

        GUICamera.enabled = false;
        
        float animationTime = 0.0f;
        do
        {
            animationTime = Time.timeSinceLevelLoad - winningAnimationStartTime;
            MiddleObject.transform.Rotate(0, Time.deltaTime * 60.0f, 0.0f);
            float scale = (Mathf.Clamp01(animationTime * 0.5f) * 3.0f + 1.0f) * oldScale.magnitude;
            MiddleObject.transform.localScale = new Vector3(scale, scale, scale);

            yield return new WaitForEndOfFrame();
        } while (animationTime < animationLength);

        GUICamera.enabled = true;

        MiddleObject.transform.localScale = oldScale;
        MiddleObject.transform.rotation = oldRotation;
        StartRound(_currentRoundIndex + 1);
    }

    private void OnUndoButtonClicked()
    {
        // TODO: Feedback etc.
        if (_turns.Count > 0)
        {
            _turns.Pop().Undo();
            _roundTimer.AddUndoPenalty();
        }
    }
}
