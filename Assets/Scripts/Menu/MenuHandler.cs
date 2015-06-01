//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.18444
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Linq;
using UnityEngine;
using Singletons;
using UnityEngine.UI;
using System.Reflection.Emit;
using Singleton;
using LevelCreation;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Menu
{
	public class MenuHandler : MonoBehaviour
	{
		public GameObject Panel 
		{
			get { return _menuPanel; }
		}
		
		private GameObject _menuPanel;
		private Button _exitButton;
		private Button _campaignButton;
		private Button _skirmishButton;
		private Button _optionsButton;
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		void Awake()
		{
			_menuPanel = GameObject.Find("StartPanel");
			
			_campaignButton = _menuPanel.GetComponentsInChildren<Button>().First(btn => btn.name == "CampaignButton");
			_skirmishButton = _menuPanel.GetComponentsInChildren<Button>().First(btn => btn.name == "SkirmishButton");
			_optionsButton = _menuPanel.GetComponentsInChildren<Button>().First(btn => btn.name == "OptionsButton");
			_exitButton = _menuPanel.GetComponentsInChildren<Button>().First(btn => btn.name == "ExitButton");

			// This is a setup for a button that grabs the field value when pressed
			_campaignButton.onClick.AddListener(() => StartCampaign());
			_skirmishButton.onClick.AddListener(() => StartSkirmish());
			_optionsButton.onClick.AddListener(() => ShowOptions());
			_exitButton.onClick.AddListener(() => ExitGame());
		}
		
		/// <summary>
		/// Switchs the scene end panel.
		/// </summary>
		public void SwitchStartPanel()
		{
			_menuPanel.SetActive(!_menuPanel.activeSelf);
			if (_menuPanel.activeSelf)
			{
				_menuPanel.transform.SetAsLastSibling();
			}
		}
		
		/// <summary>
		/// Adapts the worker speed.
		/// </summary>
		/// <param name="newValue">New value.</param>
		private void StartCampaign()
		{			
			SwitchStartPanel();
		}

		/// <summary>
		/// Adapts the worker speed.
		/// </summary>
		/// <param name="newValue">New value.</param>
		private void StartSkirmish()
		{
			SwitchStartPanel();

            // Destroy, if needed
            HelperSingleton.Instance.DestroyLevel();

			// int seed = DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            var seed = 102;
            Debug.Log("Last seed: " + seed.ToString());
			PrefabSingleton.Instance.LevelStartup.Seed = seed;
			PrefabSingleton.Instance.LevelStartup.StartLevel();
		}

		/// <summary>
		/// Adapts the worker speed.
		/// </summary>
		/// <param name="newValue">New value.</param>
		private void ShowOptions()
		{
			SwitchStartPanel();
			PrefabSingleton.Instance.InputHandler.SwitchInputPanel();
		}
		
		/// <summary>
		/// Adapts the worker speed.
		/// </summary>
		/// <param name="newValue">New value.</param>
		private void ExitGame()
		{
			
		}
	}
}