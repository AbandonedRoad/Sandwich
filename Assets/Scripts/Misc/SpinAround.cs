using UnityEngine;
using System.Collections;

public class SpinAround : MonoBehaviour 
{	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () 
	{
		this.transform.RotateAround (this.transform.position, Vector3.up, 30 * Time.deltaTime);
	}
}
