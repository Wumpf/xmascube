using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Collider2D ) )]
[RequireComponent( typeof( SpriteRenderer ) )]
public class FancyButtonScript : MonoBehaviour
{
	public interface IButtonClickHandler
	{
		void OnButtonClicked();
	}

	public IButtonClickHandler ClickHandler;

	public string ButtonText;

	public GUISkin GuiSkin;
	public string TextStyle = "FancyButtonText";

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
		if( ClickHandler == null )
			Debug.LogWarning( string.Format("{0}: Missing clickHandler", this) );
		else
			ClickHandler.OnButtonClicked();
	}

	void OnGUI()
	{
		GUI.skin = GuiSkin;

		Rect worldRect = Rect.MinMaxRect( renderer.bounds.min.x, renderer.bounds.max.y, renderer.bounds.max.x, renderer.bounds.min.y );
		Rect screenRect = worldRect.WorldToScreenRect();
		GUI.Label( screenRect, ButtonText, TextStyle );
	}
}
