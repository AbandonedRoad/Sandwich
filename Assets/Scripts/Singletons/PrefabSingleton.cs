using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LevelCreation;
using Enums;
using Misc;
using Menu;

namespace Singleton
{
	public class PrefabSingleton
	{
		private static PrefabSingleton _instance;

		public GameObject Player {get; private set;}
		public GameObject VertRedBricks {get; private set;}
		public GameObject VertRedBricksDoor {get; private set;}
		public GameObject VertRedBricksDoorFloor {get; private set;}
		public GameObject VertRedBricksFloor {get; private set;}
		public GameObject VertRedBricksExit {get; private set;}

		public GameObject HorzRedBricks {get; private set;}
		public GameObject HorzRedBricksDoorPrefab {get; private set;}
		public GameObject HorzRedBricksExitPrefab {get; private set;}
		public GameObject HorzRedBricksEnd {get; private set;}
        public GameObject HorzRedBricksCornerPrefab { get; private set; }

		public GameObject MetalPipeRoof {get; private set;}
		public GameObject RectStandBlock {get; private set;}
		public GameObject Torch {get; private set;}
		public GameObject Coin {get; private set;}
		public GameObject HeartFull {get; private set;}
		public GameObject HeartEmpty {get; private set;}
		public Dictionary<int, AudioClip> Screams;
		public Dictionary<int, AudioClip> BoneBreak;
		public AudioClip CoinPickup {get; private set;}
		public Transform PickupParent {get; private set;}
		public Transform LevelParent {get; private set;}
		public Transform StandBlockParent {get; private set;}
		public Transform LightsParent {get; private set;}
		public Transform CeillingParent {get; private set;}
		public SceneFadeInOut ScreenFader {get; private set;}
		public YouAreDeadHandler DeadHandler {get; private set;}
		public SceneStatisticsHandler SceneStatistics {get; private set;}
		public InputHandler InputHandler {get; private set;}
		public MenuHandler MenuHandler {get; private set;}
		public LevelStartup LevelStartup {get; private set;}

		/// <summary>
		/// Gets instance
		/// </summary>
		public static PrefabSingleton Instance
		{
			get 
			{
				if (_instance == null) 
				{
					_instance = new PrefabSingleton();
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
			Player = GameObject.Find("Player");

			Screams = new Dictionary<int, AudioClip>();
			Screams.Add(0, Resources.Load("Sounds/Scream1") as AudioClip);
			Screams.Add(1, Resources.Load("Sounds/Scream2") as AudioClip);

			BoneBreak = new Dictionary<int, AudioClip>();
			BoneBreak.Add(0, Resources.Load("Sounds/BoneBreak1") as AudioClip);
			BoneBreak.Add(1, Resources.Load("Sounds/BoneBreak2") as AudioClip);
			BoneBreak.Add(2, Resources.Load("Sounds/BoneBreak3") as AudioClip);

			// Pickups
			CoinPickup = Resources.Load("Sounds/CoinPickup") as AudioClip;
			Coin = Resources.Load("Prefabs/Pickups/CoinPrefab") as GameObject;

			// Level Parts - Vertical
			VertRedBricks = Resources.Load("Prefabs/LevelBlocks/VertRedBricksPrefab") as GameObject;
			VertRedBricksDoor = Resources.Load("Prefabs/LevelBlocks/VertRedBricksDoorPrefab") as GameObject;
			VertRedBricksDoorFloor = Resources.Load("Prefabs/LevelBlocks/VertRedBricksDoorFloorPrefab") as GameObject;
			VertRedBricksFloor = Resources.Load("Prefabs/LevelBlocks/VertRedBricksFloorPrefab") as GameObject;
			VertRedBricksExit = Resources.Load("Prefabs/LevelBlocks/VertRedBricksExitPrefab") as GameObject;

			// Level Parts - Horizontal
			HorzRedBricks = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksPrefab") as GameObject;
			HorzRedBricksEnd = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksEndPrefab") as GameObject;
			HorzRedBricksDoorPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksDoorPrefab") as GameObject;
			HorzRedBricksExitPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksExitPrefab") as GameObject;
            HorzRedBricksCornerPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksCornerPrefab") as GameObject;

			// Roof
			MetalPipeRoof = Resources.Load("Prefabs/LevelBlocks/FliesenCeilingPrefab") as GameObject;

			// Stand Blocks
			RectStandBlock = Resources.Load("Prefabs/StandBlocks/RectStandBlockPrefab") as GameObject;

			// GUI
			HeartFull = Resources.Load("Prefabs/GUI/HeartFullPrefab") as GameObject;
			HeartEmpty = Resources.Load("Prefabs/GUI/HeartEmptyPrefab") as GameObject;

			Torch = Resources.Load("Prefabs/Lights/TorchPrefab") as GameObject;

			// Parents
			PickupParent = GameObject.Find ("_Pickups").transform;
			LevelParent = GameObject.Find ("_Levels").transform;
			StandBlockParent = GameObject.Find ("_Stands").transform;
			LightsParent = GameObject.Find ("_Lights").transform;
			CeillingParent = GameObject.Find ("_Ceilling").transform;

			// Scripts
			var handlingPrefab = GameObject.Find ("HandlingObjectPrefab");
			DeadHandler = handlingPrefab.GetComponent<YouAreDeadHandler>(); 
			SceneStatistics = handlingPrefab.GetComponent<SceneStatisticsHandler>(); 
			LevelStartup = handlingPrefab.GetComponent<LevelStartup>();
			MenuHandler = handlingPrefab.GetComponent<MenuHandler>(); 
			InputHandler = handlingPrefab.GetComponent<InputHandler>();

			var guiPrefab = GameObject.Find ("GUIPrefab");
			ScreenFader = guiPrefab.GetComponent<SceneFadeInOut>();
		}
	
		/// <summary>
		/// Gets the new area info, this is the stuff we need to construct an area.
		/// </summary>
		/// <returns>The new area info.</returns>
		/// <param name="orientation">Orientation inwhich the area is planned. Horizontal or vertical</param>
		/// <param name="lastAreaInfos">The Last area infos - used to match textures in transition</param>
		public AreaInfos GetNewAreaInfo(AreaInfos lastAreaInfos)
		{
			int value = Random.Range(0, 1);
			if (value == 0)
			{
				return new AreaInfos()
				{
					VFloorDoor = VertRedBricksDoorFloor,
					VFloor = VertRedBricksFloor,
					VBlock = VertRedBricks,
					VTransition = VertRedBricksDoor,
					VRoof = MetalPipeRoof,
					VExit = VertRedBricksExit,

					HFloor = HorzRedBricks,
					HBlock = HorzRedBricks,
					HTransition = HorzRedBricksDoorPrefab,
					HExit = HorzRedBricksExitPrefab,
                    HCorner = HorzRedBricksCornerPrefab
				};
			}

			return null;
		}

		/// <summary>
		/// Create the specified toBeCreated and pos.
		/// </summary>
		/// <param name="toBeCreated">To be created.</param>
		/// <param name="pos">Position.</param>
		public GameObject Create(GameObject toBeCreated, Vector3? pos = null, Vector3? rot = null)
		{
			GameObject result;
			result = GameObject.Instantiate(toBeCreated);
			result.transform.position = pos.HasValue ? pos.Value : result.transform.position;

            if (rot.HasValue)
            {
                result.transform.rotation = Quaternion.Euler(rot.Value);
            }
            
			result.name = result.name.Substring(0, result.name.Length - 7);

            CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock = result;

			return result;		
		}
	}
}