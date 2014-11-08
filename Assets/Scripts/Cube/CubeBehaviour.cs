using UnityEngine;
using System.Collections;

public class CubeBehaviour : MonoBehaviour {
	
	private Material _mat;
	private Color _originalColor;
	// Use this for initialization
	void Start () 
	{
		_mat = this.renderer.material;
		_originalColor = _mat.color;
	}


	public void OnGUI()
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
					OnClicked();
				}
			}
		}
	}

	public void OnClicked()
	{
		Debug.LogError("I was clicked!");
	}

	public void Select()
	{
		if(_mat == null)
		{
			return;
		}
		_mat.color = new Color(0, 0, 0, 255);
	}

	public void Unselect()
	{
		if(_mat == null)
		{
			return;
		}
		_mat.color = _originalColor;
	}

	public void Disappear()
	{
		StartCoroutine("DisappearCoroutine");
	}

	public void Reappear()
	{
		StartCoroutine("ReappearCoroutine");
	}


	private IEnumerator DisappearCoroutine() 
	{
		while(this.transform.localScale.x >= 0)
		{
			this.transform.localScale -= new Vector3(0.1f,0.1f, 0.1f);
			yield return new WaitForEndOfFrame();
		}
		this.gameObject.SetActive(false);	
	}

	private IEnumerator ReappearCoroutine() 
	{
		if(!this.gameObject.activeInHierarchy)
		{
			this.gameObject.SetActive(true);
		}

		while(this.transform.localScale.x <= 1)
		{
			this.transform.localScale += new Vector3(0.1f,0.1f, 0.1f);
			yield return new WaitForEndOfFrame();
		}
	}
}
