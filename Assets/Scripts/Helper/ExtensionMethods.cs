using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    /// <summary>
    /// Method to compute a Rect from a BoxCollider2D in screen coordinates
    /// </summary>
    /// <returns>A rectangle in Screen coordinates.</returns>
    /// <param name="_camera">The camera to compute the screen coordinates.</param>
    public static Rect ConvertToScreenRect(this BoxCollider2D _bc2d, Camera _camera) {
        Vector3 pos = _bc2d.transform.position;
        Vector2 size = _bc2d.size;
        Vector3 screenPosTopLeft = _camera.WorldToScreenPoint (new Vector3 (pos.x - size.x / 2f, pos.y - size.y / 2f, 0f));
        Vector3 screenPosBotRight = _camera.WorldToScreenPoint (new Vector3 (pos.x + size.x / 2f, pos.y + size.y / 2f, 0f));
        float titleWidth = screenPosBotRight.x - screenPosTopLeft.x;
        float titleHeight = screenPosBotRight.y - screenPosTopLeft.y;
        
        // Note: camera coordinates start bottom left --> remeber to swap the y-achsis
        return new Rect (screenPosTopLeft.x, Screen.height - screenPosBotRight.y, titleWidth, titleHeight);
    }

    /// <summary>
    /// Calculates the size of the font.
    /// </summary>
    /// <param name="_content">The gui Content for wich the size should be computet.</param>
    /// <param name="_width">The width of the Box in which the content should be placed.</param>
    /// <param name="_height">The height of the Box in which the content should be placed.</param>
    /// <param name="_maxFontSize">The maximum font size.</param>
    /// <param name="_minFontSize">The minimum font size.</param>
    public static void CalcFontSize(this GUIStyle _guiStyle, GUIContent _content, int _width, int _height, int _maxFontSize, int _minFontSize)
    {
        _guiStyle.fontSize = _maxFontSize;
        bool doesntFitSoLargeOhGodThePain = true;
        while( doesntFitSoLargeOhGodThePain && _guiStyle.fontSize > _minFontSize )
        {
            float textHeight = _guiStyle.CalcHeight( _content, _width );
            
            doesntFitSoLargeOhGodThePain = false;
            if( textHeight >= (_height - _guiStyle.padding.vertical) * (0.75f) )
            {
                _guiStyle.fontSize--;
                doesntFitSoLargeOhGodThePain = true;
            }
        }
    }

	#region Vector3

	public static Vector2 ScreenToImagePoint( this Vector3 _screenPoint, SpriteRenderer _target )
	{
		return _screenPoint.ScreenToImagePoint( _target, Camera.main );
	}
	
	public static Vector2 ScreenToImagePoint( this Vector3 _screenPoint, SpriteRenderer _target, Camera _camera )
	{
		Vector2 screenPoint2D = new Vector2( _screenPoint.x, _screenPoint.y );
		return screenPoint2D.ScreenToImagePoint( _target, _camera );
	}
	
	public static Vector2 WorldToImagePoint( this Vector3 _worldPoint, SpriteRenderer _target )
	{
		Vector2 worldPoint2D = new Vector2( _worldPoint.x, _worldPoint.y );
		return worldPoint2D.WorldToImagePoint( _target );
	}

	#endregion

	#region Vector2

	public static Vector2 ScreenToImagePoint( this Vector2 _screenPoint, SpriteRenderer _target )
	{
		return _screenPoint.ScreenToImagePoint( _target, Camera.main );
	}

	public static Vector2 ScreenToImagePoint( this Vector2 _screenPoint, SpriteRenderer _target, Camera _camera )
	{
		Vector2 worldPoint = _camera.ScreenToWorldPoint( _screenPoint );
		return worldPoint.WorldToImagePoint( _target );
	}
	
	public static Vector2 WorldToImagePoint( this Vector2 _worldPoint, SpriteRenderer _target )
	{
//		Debug.Log( string.Format( "worldPoint: {0}", _worldPoint ) );

		Vector2 targetPixelSize = new Vector2( _target.sprite.texture.width, _target.sprite.texture.height );
//		Debug.Log( string.Format( "targetPixelSize: {0}", targetPixelSize ) );
		
		Vector2 targetWorldSize = new Vector2( _target.bounds.size.x, _target.bounds.size.y );
		Vector2 targetWorldBottomLeft = new Vector2( _target.bounds.min.x, _target.bounds.min.y );
//		Debug.Log( string.Format( "targetWorldSize: {0}", targetWorldSize ) );
//		Debug.Log( string.Format( "targetWorldBottomLeft: {0}", targetWorldBottomLeft ) );
		
		float tx = ( _worldPoint.x - targetWorldBottomLeft.x ) / targetWorldSize.x;
		float ty = ( _worldPoint.y - targetWorldBottomLeft.y ) / targetWorldSize.y;
//		Debug.Log( string.Format( "tx: {0}", tx ) );
//		Debug.Log( string.Format( "ty: {0}", ty ) );
		
		Vector2 pixelPoint = new Vector2( tx * targetPixelSize.x, ( 1 - ty ) * targetPixelSize.y );
//		Debug.Log( string.Format( "pixelPoint: {0}", pixelPoint ) );
		
		return pixelPoint;
	}

	#endregion

	#region string

	/// <summary>
	/// Generates an MD5 hash for an input string. The formatting will match the output of PHP's md5() function.
	/// </summary>
	/// <param name="_input">The string to be converted. Assumed to be UTF8 encoded</param>
	public static string md5( this string _input )
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(_input);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}

	#endregion

    #region IEnumerable<T>

    /// <summary>
    /// Removes an element at the given index and returns the removed elment.
    /// </summary>
    /// <returns>The removed element.</returns>
    /// <param name="_index">The index where the element should be removed at.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    /// <exception cref="System.IndexOutOfRangeException">If the index is out of range</exception>
    public static T RemoveAtAndReturn<T>(this List<T> l, int _index)
    {
        if(_index < 0 || _index > l.Count - 1)
            throw new System.IndexOutOfRangeException();
        T res = l[_index];
        l.RemoveAt(_index);
        return res;
    }

    #endregion

	#region Rect

	public static Rect WorldToScreenRect( this Rect _worldRect, Camera _camera )
	{
		Vector2 screenMin = _camera.WorldToScreenPoint( _worldRect.min );
		Vector2 screenMax = _camera.WorldToScreenPoint( _worldRect.max );
		return Rect.MinMaxRect( screenMin.x, Screen.height - screenMin.y, screenMax.x, Screen.height - screenMax.y );
	}
	public static Rect WorldToScreenRect( this Rect _worldRect )
	{
		return _worldRect.WorldToScreenRect( Camera.main );
	}

	#endregion

    #region int [,,]

    public static int[,,] FillAllWithValue(this int [,,] array, int value)
    {
        for(int i = 0; i< array.GetLength(0); ++i)
            for(int j = 0; j< array.GetLength(1); ++j)
                for(int k = 0; k< array.GetLength(2); ++k)
                    array[i,j,k] = value;

        return array;
    }
    #endregion
}