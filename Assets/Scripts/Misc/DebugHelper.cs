using UnityEngine;
using System.Collections;
using Singleton;

public class DebugHelper : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		PrefabSingleton.Instance.MenuHandler.SwitchStartPanel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
