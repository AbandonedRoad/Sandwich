using UnityEngine;
using System.Collections;
using Menu;
using Singleton;

public class KeyChecks : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PrefabSingleton.Instance.MenuHandler.SwitchStartPanel();
        }
    }
}
