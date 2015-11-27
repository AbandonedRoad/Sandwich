using System;
using UnityEngine;
using Enums;
using Assets.Scripts.Player;
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
        public PlayerTorch PlayersTorch { get; private set; }
        public HorzDirection FacingDirection { get; set; }
        public CardinalDirection FacingCardinalDirection { get; set; }
        public BlockInfo ActualBlock { get; set; }

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
        /// This is the time, the level was opened. At the end, the level duration may be shown.
        /// </summary>
        public DateTime LevelStartDate { get; set;}

		/// <summary>
		/// Gets or sets the difficulty of the game
		/// </summary>
		/// <value>The difficulty.</value>
		public Difficulty Difficulty {get; set;}

        /// <summary>
        /// The Position of the player.
        /// </summary>
        public Vector2 PlayerPosition { get; set; }

        /// <summary>
        /// The length of one step by the player.
        /// </summary>
        public float PlayerStepLength { get; private set; }

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

			Jump = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JUMP"));
			Crouch = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CROUCH"));
			Forward = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("FORWARD"));
			Backwards = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BACKWARD"));
			Left = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LEFT"));
			Right = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RIGHT"));

            Player = GameObject.Find("Player");
            PlayersTorch = GameObject.Find("PlayersTorch").GetComponent<PlayerTorch>();

            PlayerStepLength = 4f;
        }

        /// <summary>
        /// Checks if the current move is allowed
        /// </summary>
        public bool IsMoveAllowed(HorzDirection desiredDirection)
        {
            Vector3 rayDirection = Vector3.one;
            switch (desiredDirection)
            {
                case HorzDirection.Left:
                    rayDirection = PlayerSingleton.Instance.Player.transform.right * -1;
                    break;
                case HorzDirection.Right:
                    rayDirection = PlayerSingleton.Instance.Player.transform.right;
                    break;
                case HorzDirection.Forward:
                    rayDirection = PlayerSingleton.Instance.Player.transform.forward;
                    break;
                case HorzDirection.Backwards:
                    rayDirection = PlayerSingleton.Instance.Player.transform.forward * -1;
                    break;
            }

            var pos = new Vector3(Player.transform.position.x, Player.transform.position.y + 1.5f, Player.transform.position.z);
            var ray = new Ray(pos, rayDirection);
            RaycastHit info;
            Physics.Raycast(ray, out info, 4.1f);

            if (info.transform == null
                || info.transform.gameObject.tag == "Passable")
            {
                // Nothing in the way, or something is in the way, but it is on the white list.
                return true;
            }
            else
            {
                if (info.transform.gameObject.tag == "RenderObject")
                {
                    // Seems to be an unpassble zone. But check if it is on the same height as we are!
                    var size = HelperSingleton.Instance.GetSize(info.transform.gameObject, false);

                    if ((info.transform.position.y - (size.z / 2)) > Player.transform.position.y  // <-- It' above us.
                        || (size.z + info.transform.position.y - (size.z / 2)) < Player.transform.position.y)  // <-- It' below  us.
                    {
                        // Its either below us, or above us
                        return true;
                    }

                    // Its in the way.
                    return false;
                }

                if (HelperSingleton.Instance.GetTopMostGO(info.transform.gameObject, true).tag == "LevelBlock"
                    || HelperSingleton.Instance.GetTopMostGO(info.transform.gameObject, true).tag == "Unpassable")
                {
                    // End of level reached, or object is on teh black list.
                    return false;
                }

                return true;
            }
        }
    }
}