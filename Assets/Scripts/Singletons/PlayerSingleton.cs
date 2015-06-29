using System;
using System.Linq;
using UnityEngine;
using Enums;
using Random = UnityEngine.Random;
using Blocks;

namespace Singleton
{
	public class PlayerSingleton
	{
		public KeyCode Jump {get; private set;}
		public KeyCode Crouch {get; private set;}
		public KeyCode Forward {get; private set;}
		public KeyCode Backwards {get; private set;}
		public KeyCode Left {get; private set;}
		public KeyCode Right {get; private set;}
        public GameObject Player { get; private set; }

		private static PlayerSingleton _instance;

		/// <summary>
		/// Gets or sets the total health.
		/// </summary>
		/// <value>The total health.</value>
		public float TotalHealth {get; set;}
		
		/// <summary>
		/// Gets or sets the player health.
		/// </summary>
		/// <value>The player health.</value>
		public float PlayerHealth {get; set;}

		/// <summary>
		/// Gets or sets the coin amount.
		/// </summary>
		/// <value>The coin amount.</value>
		public int CoinAmount {get; set;}

		/// <summary>
		/// Gets or sets the difficulty of the game
		/// </summary>
		/// <value>The difficulty.</value>
		public Difficulty Difficulty {get; set;}

		/// <summary>
		/// Gets instance
		/// </summary>
		public static PlayerSingleton Instance
		{
			get 
			{
				if (_instance == null) 
				{
					_instance = new PlayerSingleton();
					_instance.Init();
				}
				
				return _instance;
			}
		}
		
		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init ()
		{
			TotalHealth = 5;
			PlayerHealth = 5;

			Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JUMP"));
			Crouch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CROUCH"));
			Forward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("FORWARD"));
			Backwards = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BACKWARD"));
			Left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LEFT"));
			Right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RIGHT"));

            Player = GameObject.Find("Player");
		}	
	}
}