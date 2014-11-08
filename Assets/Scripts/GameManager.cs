using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
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

    public uint Score { get; private set; }
    public float RoundTime { get; private set; }


    // Use this for initialization
    void Start()
    {
        StartRound(0);
    }

    // Update is called once per frame
    void Update()
    {
        RoundTime += Time.deltaTime;
    }

    void StartRound(uint roundIndex)
    {
        // Reset round properties
        _selectedObject = null;
        _turns.Clear();
        Score = 0;
        RoundTime = 0.0f;

        // TODO: Generate new level
    }
}
