using UnityEngine;
using System.Collections;

public static class Extensions
{
	#region TRANSFORM

	public static void SetPositionXY(this Transform t, float newX, float newY)
	{
		t.position = new Vector3(newX, newY, t.position.z);
	}

	public static void SetPositionX(this Transform t, float newX)
	{
		t.position = new Vector3(newX, t.position.y, t.position.z);
	}
	
	public static void SetPositionY(this Transform t, float newY)
	{
		t.position = new Vector3(t.position.x, newY, t.position.z);
	}
	
	public static void SetPositionZ(this Transform t, float newZ)
	{
		t.position = new Vector3(t.position.x, t.position.y, newZ);
	}

	public static void SetLocalPositionXY(this Transform t, float newX, float newY)
	{
		t.localPosition = new Vector3(newX, newY, t.localPosition.z);
	}
	
	public static float GetPositionX(this Transform t)
	{
		return t.position.x;
	}
	
	public static float GetPositionY(this Transform t)
	{
		return t.position.y;
	}
	
	public static float GetPositionZ(this Transform t)
	{
		return t.position.z;
	}

	public static float GetLocalPositionX(this Transform t)
	{
		return t.localPosition.x;
	}
	
	public static float GetLocalPositionY(this Transform t)
	{
		return t.localPosition.y;
	}
	
	public static float GetLocalPositionZ(this Transform t)
	{
		return t.localPosition.z;
	}

	public static void RotateTo(this Transform t, Vector3 newEulerAngles)
	{
		Quaternion newRotation = t.transform.rotation;
		newRotation.eulerAngles = newEulerAngles;
		t.transform.rotation = newRotation;
	}
	
	public static void RotateZ(this Transform t, float newAngle)
	{
		Quaternion newRotation = t.transform.rotation;
		newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newAngle);
		t.transform.rotation = newRotation;
	}
	
	public static void RotateClockwise(this Transform t)
	{
		Quaternion newRotation = t.transform.rotation;
		newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z - 90);
		t.transform.rotation = newRotation;
	}
	
	public static void RotateAntiClockwise(this Transform t)
	{
		Quaternion newRotation = t.transform.rotation;
		newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z + 90);
		t.transform.rotation = newRotation;
	}

	#endregion

	#region GAMEOBJECT

	public static float GetPositionX(this GameObject go)
	{
		return go.transform.position.x;
	}

	public static float GetPositionY(this GameObject go)
	{
		return go.transform.position.y;
	}

	public static float GetPositionZ(this GameObject go)
	{
		return go.transform.position.z;
	}

	public static float GetLocalPositionX(this GameObject go)
	{
		return go.transform.localPosition.x;
	}
	
	public static float GetLocalPositionY(this GameObject go)
	{
		return go.transform.localPosition.y;
	}
	
	public static float GetLocalPositionZ(this GameObject go)
	{
		return go.transform.localPosition.z;
	}

	public static void SetPositionXY(this GameObject go, float newX, float newY)
	{
		go.transform.position = new Vector3(newX, newY, go.transform.position.z);
	}
	
	public static void SetPositionX(this GameObject go, float newX)
	{
		go.transform.position = new Vector3(newX, go.transform.position.y, go.transform.position.z);
	}
	
	public static void SetPositionY(this GameObject go, float newY)
	{
		go.transform.position = new Vector3(go.transform.position.x, newY, go.transform.position.z);
	}
	
	public static void SetPositionZ(this GameObject go, float newZ)
	{
		go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, newZ);
	}

	public static void SetLocalPositionXY(this GameObject go, float newX, float newY)
	{
		go.transform.localPosition = new Vector3(newX, newY, go.transform.localPosition.z);
	}
	
	public static void SetLocalPositionX(this GameObject go, float newX)
	{
		go.transform.localPosition = new Vector3(newX, go.transform.localPosition.y, go.transform.localPosition.z);
	}
	
	public static void SetLocalPositionY(this GameObject go, float newY)
	{
		go.transform.localPosition = new Vector3(go.transform.localPosition.x, newY, go.transform.localPosition.z);
	}
	
	public static void SetLocalPositionZ(this GameObject go, float newZ)
	{
		go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, newZ);
	}

	#endregion

	#region RIGIDBODY2D

	public static void SetSpeedX(this Rigidbody2D rb, float newSpeed)
	{
		rb.velocity = new Vector2(newSpeed, rb.velocity.y);
	}

	public static void SetSpeedY(this Rigidbody2D rb, float newSpeed)
	{
		rb.velocity = new Vector2(rb.velocity.x, newSpeed);
	}

	public static float GetSpeedX(this Rigidbody2D rb)
	{
		return rb.velocity.x;
	}

	public static float GetSpeedY(this Rigidbody2D rb)
	{
		return rb.velocity.y;
	}

	#endregion	
}