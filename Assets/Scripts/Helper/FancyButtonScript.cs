using UnityEngine;
using System.Collections;
using System;

[RequireComponent( typeof( Collider2D ) )]
[RequireComponent( typeof( SpriteRenderer ) )]
public class FancyButtonScript : MonoBehaviour
{
    public event Action ButtonClickedEvent;

	public string ButtonText;

	public Sprite NormalSprite;
	public Sprite HoverSprite;
	public Sprite ActiveSprite;

	private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = renderer as SpriteRenderer;
	}

	void OnMouseOver()
	{
		if( Input.GetMouseButton( 0 ) )
			spriteRenderer.sprite = ActiveSprite;
		else
			spriteRenderer.sprite = HoverSprite;
	}

	void OnMouseExit()
	{
		spriteRenderer.sprite = NormalSprite;
	}

	void OnMouseUp()
	{
        if (ButtonClickedEvent == null)
            Debug.LogWarning(string.Format("{0}: Missing ButtonClicked event", this));
        else
            ButtonClickedEvent();
	}

	void OnGUI()
	{
		Rect worldRect = Rect.MinMaxRect( renderer.bounds.min.x, renderer.bounds.max.y, renderer.bounds.max.x, renderer.bounds.min.y );
		Rect screenRect = worldRect.WorldToScreenRect();
	}
}
