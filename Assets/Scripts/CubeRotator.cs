using UnityEngine;
using System.Collections;
using Assets.Scripts.Helper;

public class CubeRotator : MonoBehaviour {

    public FancyButtonScript UpButton;
    public FancyButtonScript DownButton;
    public FancyButtonScript RightButton;
    public FancyButtonScript LeftButton;
    public Camera GUICamera;

    private WindowResizeWatcher _resizeWatcher = new WindowResizeWatcher();
    private Quaternion _rotationGoal;
    private float _rotationSteps = 45.0f;
    private float _rotationSpeed = 5.0f;


	// Use this for initialization
	void Start ()
    {
        UpButton.ButtonClickedEvent += () => _rotationGoal = Quaternion.Euler(_rotationSteps, 0.0f, 0.0f) * _rotationGoal;
        DownButton.ButtonClickedEvent += () => _rotationGoal = Quaternion.Euler(-_rotationSteps, 0.0f, 0.0f) * _rotationGoal;
        RightButton.ButtonClickedEvent += () => _rotationGoal = Quaternion.Euler(0.0f, -_rotationSteps, 0.0f) * _rotationGoal;
        LeftButton.ButtonClickedEvent += () => _rotationGoal = Quaternion.Euler(0.0f, _rotationSteps, 0.0f) * _rotationGoal;

        _resizeWatcher.ResizeEvent += (int width, int height) =>
        {
            float camHalfHeight = GUICamera.orthographicSize;
            float camHalfWidth = GUICamera.aspect * camHalfHeight;


            UpButton.transform.position = new Vector3(0.0f, -UpButton.ActiveSprite.bounds.size.y * 0.5f, 0.5f) +
                                          new Vector3(0.0f, camHalfHeight, 0.0f) + GUICamera.transform.position;
            DownButton.transform.position = new Vector3(0.0f, DownButton.ActiveSprite.bounds.size.y * 0.5f, 0.5f) +
                                            new Vector3(0.0f, -camHalfHeight, 0.0f) + GUICamera.transform.position;
            RightButton.transform.position = new Vector3(-UpButton.ActiveSprite.bounds.size.x * 0.5f, 0.0f, 0.5f) +
                                             new Vector3(camHalfWidth, 0.0f, 0.0f) + GUICamera.transform.position;
            LeftButton.transform.position = new Vector3(LeftButton.ActiveSprite.bounds.size.x * 0.5f, 0.0f, 0.5f) +
                                            new Vector3(-camHalfWidth, 0.0f, 0.0f) + GUICamera.transform.position;
        };

        StartCoroutine(_resizeWatcher.CheckForResize());
    }

    public void Reset()
    {
        transform.rotation = Quaternion.Euler(30.0f, 30.0f, 0.0f);
        _rotationGoal = transform.rotation;
    }

    void OnDestroy()
    {
        _resizeWatcher.Dispose();
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = Quaternion.Lerp(_rotationGoal, transform.rotation, Mathf.Exp(-Time.deltaTime * _rotationSpeed));
	}
}
