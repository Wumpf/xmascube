using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundTimer : MonoBehaviour {

    public float RoundTime { get; private set; }
    public GUISkin Skin;

    public float UndoTimePenalty = 20.0f;

    private List<float> _penaltyMessages = new List<float>();
    private const float _penaltyMessageSpeed = 0.5f;
    private readonly string _penaltyMessageText;
    const float _penaltyMessageStartHeight = 40.0f;
    const float _penaltyMessageEndHeight = -20.0f;

    public RoundTimer()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RoundTime += Time.deltaTime;
    }

    void OnGUI()
    {
        GUI.skin = Skin;

        int minutes = (int)(RoundTime / 60.0f);
        int seconds = (int)(RoundTime) - minutes * 60;
        string timerText = "Time " + (minutes < 10 ? "0" : "") + minutes + (seconds < 10 ? ":0" : ":") + seconds;
        var style = GUI.skin.FindStyle("timer");
        style.CalcFontSize(new GUIContent(timerText), 300, 70, 50, 10);
        GUI.Label(new Rect(20, 20, 300, 70), timerText, style);

        Vector2 penaltyMessageOffset = style.CalcSize(new GUIContent("Time 00"));
        string penaltyMessageText = "+" + (int)UndoTimePenalty;

        style = GUI.skin.FindStyle("penalty");
        style.CalcFontSize(new GUIContent(_penaltyMessageText), 200, 60, 50, 20);
        for(int i=0; i<_penaltyMessages.Count; ++i)
        {
            _penaltyMessages[i] += Time.deltaTime * _penaltyMessageSpeed;
            if (_penaltyMessages[i] > 1)
            {
                _penaltyMessages.RemoveAt(i);
                --i; continue;
            }

            float height = Mathf.Lerp(_penaltyMessageStartHeight, _penaltyMessageEndHeight, _penaltyMessages[i]);

            GUI.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Clamp01(Mathf.Sin(_penaltyMessages[i] * Mathf.PI * 1.2f)));
            GUI.Label(new Rect(penaltyMessageOffset.x, height, 200, 60), penaltyMessageText, style);
            GUI.color = Color.white;
        }
    }

    public void Reset()
    {
        RoundTime = 0;
    }

    public void AddUndoPenalty()
    {
        RoundTime += UndoTimePenalty;
        _penaltyMessages.Add(0.0f);
    }
}
