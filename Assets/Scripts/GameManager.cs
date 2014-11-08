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

    private class Turn
    {
        private GameObject _removedObject0;
        private GameObject _removedObject1;

        Turn(GameObject removedObject0, GameObject removedObject1)
        {
            _removedObject0 = removedObject0;
            _removedObject1 = removedObject1;
        }

        public void Do()
        {
            // TODO: Call select
            _removedObject0.SetActive(false);
            _removedObject0.SetActive(false);
        }
        public void Undo()
        {
            // TODO: Call unselect
            _removedObject0.SetActive(true);
            _removedObject0.SetActive(true);
        }
    };

    private GameObject[, ,] _level;
    private GameObject _selectedObject = null;
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
