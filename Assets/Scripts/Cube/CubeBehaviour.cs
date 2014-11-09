using UnityEngine;
using System.Collections;

public class CubeBehaviour : MonoBehaviour {

	public delegate void ClickAction(CubeBehaviour cubeBehaviour);
	public event ClickAction OnClicked;

	public AudioClip FailSound;
	public AudioClip DisappearSound;
	public AudioClip ReappearSound;

	private Vector3 _pos = Vector3.zero;

	private bool selected = false;
	// Use this for initialization
	public void Start() 
	{
        Active = true;
	}

	public void Update()
	{
		if ( Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if(Physics.Raycast(ray,out hit,100)) 
			{
				if(hit.transform.Equals(this.transform))
				{
					//add here a delegate
					if(OnClicked != null)
						OnClicked(this);
				}
			}
		}
	}

	public Vector3 GridPosition 
	{
        set { _pos = value; }
		get { return _pos; }
	}

    public int TypeIndex { get; set; }

    public bool Active { get; private set; }
	

	public void Select()
	{
		//renderer.material.shader = Shader.Find("Self-Illumin/Parallax Diffuse");
		renderer.material.color = Color.yellow;
		selected = true;
		StartCoroutine("ChangeSizeCoroutine");
	}

	public void Unselect()
	{
		//renderer.material.shader = Shader.Find("Diffuse");
		renderer.material.color = Color.white;
		selected = false;
	}

	public void Disappear()
	{
        Active = false;
		Unselect();
		if(DisappearSound != null)
		{
			audio.PlayOneShot(DisappearSound,0.5f);
		}
		selected = false;
		StartCoroutine("DisappearCoroutine");
	}

	public void Reappear()
	{
        Active = true;
		if(ReappearSound != null)
		{
			audio.PlayOneShot(ReappearSound, 2);
		}
		StartCoroutine("ReappearCoroutine");
	}

	public void Fail()
	{
		if(FailSound != null)
		{
			audio.PlayOneShot(FailSound);
		}
	}

	private IEnumerator DisappearCoroutine() 
	{

		while(this.transform.localScale.x >= 0 || audio.isPlaying)
		{
            float waitAppearSpeed = Time.deltaTime * 6;
            this.transform.localScale -= new Vector3(waitAppearSpeed, waitAppearSpeed, waitAppearSpeed);
			yield return new WaitForEndOfFrame();
		}
        this.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        this.gameObject.SetActive(false);
	}

	private IEnumerator ReappearCoroutine() 
	{
		if(!this.gameObject.activeInHierarchy)
		{
			this.gameObject.SetActive(true);
		}

		while(this.transform.localScale.x < 1)
		{
            float waitAppearSpeed = Time.deltaTime * 6;
            this.transform.localScale += new Vector3(waitAppearSpeed, waitAppearSpeed, waitAppearSpeed);
			yield return new WaitForEndOfFrame();
		}
		this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

	private IEnumerator ChangeSizeCoroutine() 
	{
		float waitAppearSpeed = Time.deltaTime * 0.05f;
		while(selected)
		{
			if( (Time.timeSinceLevelLoad)%2 <1 )
			{
				this.transform.localScale += new Vector3(waitAppearSpeed, waitAppearSpeed, waitAppearSpeed);
			}
			else
			{
				this.transform.localScale -= new Vector3(waitAppearSpeed, waitAppearSpeed, waitAppearSpeed);
			}
			yield return new WaitForEndOfFrame();
		}
		this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}
}
