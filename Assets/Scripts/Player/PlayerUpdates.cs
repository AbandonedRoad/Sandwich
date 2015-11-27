//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.18444
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using Singleton;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Blocks;
using Enums;

namespace Player
{
	public class PlayerUpdates : MonoBehaviour
	{
		private Text _coinsText;
		private AudioSource _audioSourcesScream;
		private AudioSource _audioSourcesBone;
		private AudioSource _audioSourcesMisc;
		private List<GameObject> _playerHearts = new List<GameObject>();
		private float _actualHealth;
		private bool _playerWon;

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start()
		{
			_actualHealth = PlayerSingleton.Instance.PlayerHealth;

			_audioSourcesScream = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			_audioSourcesBone = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			_audioSourcesMisc = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			_coinsText = GameObject.Find("CoinText").GetComponent<Text>();

            CreateHearts();
        }

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update()
		{
			// Player recieved damage
			if (_actualHealth > PlayerSingleton.Instance.PlayerHealth)
			{
				int boneBreak = Random.Range(0, 3);
				int scream = Random.Range(0, 2);

                if (Random.Range(0, 3) == 0)
                {
                    // Play bone break sound only under certain circumstances.
                    _audioSourcesBone.PlayOneShot(PrefabSingleton.Instance.BoneBreak[boneBreak]);
                }
				_audioSourcesScream.PlayOneShot(PrefabSingleton.Instance.Screams[scream]);

                CreateHearts();

                _actualHealth = PlayerSingleton.Instance.PlayerHealth;
			}

			if (_actualHealth <= 0 && !PrefabSingleton.Instance.ScreenFader.IsBlack)
			{
				// Player is dead
				PrefabSingleton.Instance.ScreenFader.PlayerDied();
			}

            // Update Label for Coins.
            _coinsText.text = PlayerSingleton.Instance.CoinAmount.ToString();

            // Check User input.
			if (Input.GetKeyDown(KeyCode.Return))
			{
				// Enter was pressed
				RaycastHit hitInfo;
				if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo))
				{
                    if (HelperSingleton.Instance.GetTopMostGO(hitInfo.transform.gameObject, true).GetComponentInChildren<BlockDescriptor>().BlockType == LevelBlockType.Exit)
                    {
                        _playerWon = true;
                    }
                    else if (hitInfo.transform.gameObject.tag == "Torch")
                    {
                        var wallObject = hitInfo.transform.parent.gameObject.GetComponent<WallDescriptor>();

                        if (wallObject != null)
                        {
                            var light = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.TorchEmpty, hitInfo.transform.position);
                            CalculationSingleton.Instance.OrientationCalculation.Init(wallObject, light);
                            CalculationSingleton.Instance.GetPositionForObject();
                            Destroy(hitInfo.transform.gameObject);
                            PlayerSingleton.Instance.PlayersTorch.Enable(120);
                        }
                    }
                }
			}

			if (_playerWon && !PrefabSingleton.Instance.ScreenFader.IsBlack)
			{
				PrefabSingleton.Instance.ScreenFader.PlayerWon();
			}
			else if (_playerWon && PrefabSingleton.Instance.ScreenFader.IsBlack)
			{
				_playerWon = false;
			}
		}

		/// <summary>
		/// Raises the controller collider hit event.
		/// </summary>
		/// <param name="hit">Hit.</param>
		void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (hit.normal.y > 0.707)
			{
				// Ground collision
			}
			else
			{
				if (hit.gameObject.name == "CoinPrefab")
				{
					_audioSourcesMisc.PlayOneShot(PrefabSingleton.Instance.CoinPickup);
					PlayerSingleton.Instance.CoinAmount++;
					GameObject.Destroy(hit.gameObject);
				}
			}
		}

		/// <summary>
		/// Creates the hearts.
		/// </summary>
		private void CreateHearts()
		{
			// Destroy old ones, if needed.
			foreach (var heart in _playerHearts) 
			{
				GameObject.Destroy(heart);
			}

			var canvas = GameObject.Find("Canvas");
			float distanceBetween = 5;

			int heartNumber = 1;
            int distance = 50;
			for (int loop = 0; loop < PlayerSingleton.Instance.PlayerHealth; loop++) 
			{
				GameObject heartFull = GameObject.Instantiate(PrefabSingleton.Instance.HeartFull) as GameObject;
				heartFull.tag = "Heart";
				var rectTransform = heartFull.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                var pos = new Vector3(((heartNumber * rectTransform.sizeDelta.y) + distanceBetween + distance) ,
				                      (rectTransform.sizeDelta.x + 5) * -1,
                                      0);
				rectTransform.position = pos;
				heartFull.transform.SetParent(canvas.transform, false);
				_playerHearts.Add(heartFull);
				heartNumber++;
			}

			for (int loop = 0; loop < (PlayerSingleton.Instance.TotalHealth - PlayerSingleton.Instance.PlayerHealth); loop++) 
			{
				GameObject heartEmpty = GameObject.Instantiate(PrefabSingleton.Instance.HeartEmpty) as GameObject;
				heartEmpty.tag = "Heart";
				var rectTransform = heartEmpty.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                var pos = new Vector3(((heartNumber * rectTransform.sizeDelta.y) + distanceBetween + distance),
				                      (rectTransform.sizeDelta.x + 5) * -1, 
				                      0);
				rectTransform.position = pos;
				heartEmpty.transform.SetParent(canvas.transform, false);
				_playerHearts.Add(heartEmpty);
				heartNumber++;
			}
		}

        /// <summary>
        /// Applies Player health back.
        /// </summary>
        public void ResetHealth()
        {
            _actualHealth = PlayerSingleton.Instance.PlayerHealth;
            CreateHearts();
        }
    }
}

