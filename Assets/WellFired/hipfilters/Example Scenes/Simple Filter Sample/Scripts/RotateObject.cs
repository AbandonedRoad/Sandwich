using UnityEngine;
using System.Collections;

namespace WellFired
{
	public class RotateObject : MonoBehaviour 
	{
		private void Update() 
		{
			transform.Rotate(Vector3.up, 4.0f * Time.deltaTime);
		}
	}
}