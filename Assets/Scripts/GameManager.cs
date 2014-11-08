using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Helper;

public class GameManager : MonoBehaviour
{
    public uint Score { get; private set; }
    public float RoundTime { get; private set; }

    public Camera GUICamera;
    public FancyButtonScript UndoButton;
    public GameObject CubePrefab;

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
            _removedObject0.Disappear();
        }
        public void Undo()
        {
            _removedObject0.Reappear();
            _removedObject0.Reappear();
        }
    };

    private CubeBehaviour[, ,] _level;
    private CubeBehaviour _selectedObject = null;
    private Stack<Turn> _turns = new Stack<Turn>();

    private WindowResizeWatcher _resizeWatcher = new WindowResizeWatcher();

    // Use this for initialization
    void Start()
    {
        StartRound(0);
        UndoButton.ButtonClickedEvent += OnUndoButtonClicked;

        _resizeWatcher.ResizeEvent += (int width, int height) => {
            float camHalfHeight = GUICamera.orthographicSize;
            float camHalfWidth = GUICamera.aspect * camHalfHeight;
            Vector3 rightBottomPosition = new Vector3(camHalfWidth, -camHalfHeight, 0.0f) + GUICamera.transform.position;
            rightBottomPosition += new Vector3(-UndoButton.ActiveSprite.bounds.size.x * 0.5f, UndoButton.ActiveSprite.bounds.size.y * 0.5f, 0.5f);
            UndoButton.transform.position = rightBottomPosition;
        };
        StartCoroutine(_resizeWatcher.CheckForResize());
    }

    // Update is called once per frame
    void Update()
    {
        RoundTime += Time.deltaTime;
    }

    private void StartRound(uint roundIndex)
    {
        // Reset round properties
        _selectedObject = null;
        _turns.Clear();
        Score = 0;
        RoundTime = 0.0f;

        // TODO: Generate new level

        // DUMMY LEVEL
        GameObject cube0 = (GameObject)GameObject.Instantiate(CubePrefab, Vector3.zero, Quaternion.identity);
        cube0.GetComponent<CubeBehaviour>().OnClicked += OnCubeClicked;
        GameObject cube1 = (GameObject)GameObject.Instantiate(CubePrefab, new Vector3(0, 5, 0), Quaternion.identity);
        cube1.GetComponent<CubeBehaviour>().OnClicked += OnCubeClicked;
    }

    void OnCubeClicked(CubeBehaviour cubeBehaviour)
    {
        Debug.Log("Clicked!");

        // TODO: Check if this even possible
        if (_selectedObject == null)
        {
            _selectedObject = cubeBehaviour;
            _selectedObject.Select();
        }
        else if (cubeBehaviour != _selectedObject)
        {
            _selectedObject.Unselect();
            _selectedObject = null;
        }
        else
        {
            Turn newTurn = new Turn(_selectedObject, cubeBehaviour);
            newTurn.Do();
            _turns.Push(newTurn);
        }
    }

    private void OnUndoButtonClicked()
    {
        // TODO: Feedback etc.
        if(_turns.Count > 0)
        {
            _turns.Pop().Undo();
        }
    }
}
