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

	private Vector2 _firstPressPos;
	private Vector2 _secondPressPos;
	private float _firstTimePoint;
	private float _secondTimePoint;
	private Vector2 _currentSwipe = Vector2.zero;

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
		Swipe();
		transform.rotation = Quaternion.Lerp(_rotationGoal, transform.rotation, Mathf.Exp(-Time.deltaTime * _rotationSpeed));
	}

	
	public void Swipe()
	{
		if(Input.GetMouseButtonDown(0))
		{
			//save began touch 2d point
			_firstPressPos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			_firstTimePoint = Time.timeSinceLevelLoad;
		}
		if(Input.GetMouseButtonUp(0))
		{
			//save ended touch 2d point
			_secondPressPos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			_secondTimePoint = Time.timeSinceLevelLoad;

			//create vector from the two points
			_currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

			if(_secondTimePoint - _firstTimePoint <0.1f)
			{
				return;
			}

			//normalize the 2d vector
			_currentSwipe.Normalize();
			
			//swipe upwards
			if(_currentSwipe.y > 0  && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
			{
				_rotationGoal = Quaternion.Euler(_rotationSteps, 0.0f, 0.0f) * _rotationGoal;
			}
			//swipe down
			else if(_currentSwipe.y < 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
			{
				_rotationGoal = Quaternion.Euler(-_rotationSteps, 0.0f, 0.0f) * _rotationGoal;
			}
			//swipe left
			else if(_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
			{
				_rotationGoal = Quaternion.Euler(0.0f, _rotationSteps, 0.0f) * _rotationGoal;
			}
			//swipe right
			else if(_currentSwipe.x > 0 && _currentSwipe.y > -0.5f  && _currentSwipe.y < 0.5f)
			{
				_rotationGoal = Quaternion.Euler(0.0f, -_rotationSteps, 0.0f) * _rotationGoal;
			}
		}
	}

}
