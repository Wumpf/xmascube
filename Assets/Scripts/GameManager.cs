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

    private WindowResizeWatcher _resizeWatcher = new WindowResizeWatcher();

    // Use this for initialization
    void Start()
    {
        _mainCameraRotationDest = Camera.main.transform.rotation;
        _roundTimer = GetComponent<RoundTimer>();

        _cubeTextures = Resources.LoadAll<Texture>("Textures");
        _cubeTextures.OrderBy(tex => tex.name);

        StartRound(0);

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

    private void StartRound(int roundIndex)
    {
        // Reset round properties
        _selectedObject = null;
        _turns.Clear();
        Score = 0;
        _roundTimer.Reset();

        // Generate new level
        int levelSize = 3 + roundIndex * 2;
        _level = new CubeBehaviour[levelSize, levelSize, levelSize];
        _puzzleBuilder = new PuzzleBuilder();
        List<int> tileIdentifier = new List<int>();

        tileIdentifier.AddRange(new int[] { 1, 2, 3, 4, 5 });
        int[, ,] levelDesc = _puzzleBuilder.GenerateLevel(levelSize, tileIdentifier);

        for (int x = 0; x < levelSize; ++x)
        {
            for (int y = 0; y < levelSize; ++y)
            {
                for (int z = 0; z < levelSize; ++z)
                {
                    GameObject gameObject = (GameObject)GameObject.Instantiate(CubePrefab, Vector3.zero, Quaternion.identity);
                    gameObject.transform.parent = CubeParentObject.transform;
                    gameObject.transform.position = Vector3.Scale(new Vector3(x - levelSize / 2, y - levelSize / 2, z - levelSize / 2),
                                                                  gameObject.transform.renderer.bounds.size);

                    _level[x, y, z] = gameObject.GetComponent<CubeBehaviour>();
                    _level[x, y, z].OnClicked += OnCubeClicked;
                    _level[x, y, z].GridPosition = new Vector3(x, y, z);
                    _level[x, y, z].TypeIndex = levelDesc[x, y, z];

                    if (_cubeTextures.Length <= levelDesc[x, y, z])
                    {
                        Debug.LogError("Not enough cube textures!");
                        continue;
                    }
                    ((MeshRenderer)_level[x, y, z].renderer).material.mainTexture = _cubeTextures[levelDesc[x, y, z]];
                }
            }
        }
        Camera.main.transform.position = new Vector3(0,0, -levelSize * 1.9f);

        CubeParentObject.GetComponent<CubeRotator>().Reset();
    }

    private void OnCubeClicked(CubeBehaviour cubeBehaviour)
    {
        // Shouldn't happen, but just to be sure...
        if (!cubeBehaviour.Active)
            return;

        // Don't select the middle object.
        if(cubeBehaviour.TypeIndex == 0)
        {
            // TODO: Not possible sound event.
        }
        else
        {
            // Check if removable
            uint numNeighbors = 0;
            List<Vector3> neighbors = _puzzleBuilder.GetNeighbors(cubeBehaviour.GridPosition);
            foreach(var pos in neighbors)
            {
                if (_level[(int)pos.x, (int)pos.y, (int)pos.z].Active)
                    ++numNeighbors;
            }
            if (numNeighbors >= 5)
            {
                // TODO: Not possible sound event.
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
                        // TODO: Not possible sound event.
                    }
                    else
                    {
                        Turn newTurn = new Turn(_selectedObject, cubeBehaviour);
                        newTurn.Do();
                        _turns.Push(newTurn);
                        _selectedObject = null;

                        // TODO: Check win condition.
                    }
                }
            }
        }
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
