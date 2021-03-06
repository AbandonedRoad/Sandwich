using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LevelCreation;
using Misc;
using Menu;
using Assets.Scripts.Campaign;
using System.Linq;
using Blocks;

namespace Singleton
{
	public class PrefabSingleton
	{
		private static PrefabSingleton _instance;

        // -- Vert Blocks
        // Red Bricks
		public GameObject VertRedBricks {get; private set;}
		public GameObject VertRedBricksDoor {get; private set;}
		public GameObject VertRedBricksDoorFloor {get; private set;}
		public GameObject VertRedBricksFloor {get; private set;}
		public GameObject VertRedBricksExit {get; private set;}

        public GameObject VertSewer { get; private set; }
        public GameObject VertSewerDoor { get; private set; }
        public GameObject VertSewerDoorFloor { get; private set; }
        public GameObject VertSewerFloor { get; private set; }
        public GameObject VertSewerExit { get; private set; }

        // -- Horz Blocks
        // Red Bricks
		public List<GameObject> HorzRedBricks {get; private set;}
        public List<GameObject> HorzRedBricksSpec { get; private set; }
		public GameObject HorzRedBricksDoorPrefab {get; private set;}
		public GameObject HorzRedBricksExitPrefab {get; private set;}
		public GameObject HorzRedBricksStart {get; private set;}
        public GameObject HorzRedBricksCornerPrefab { get; private set; }
        public GameObject HorzRedBricksCrossingPrefab { get; private set; }
        public GameObject HorzRedBricksTCrossingPrefab { get; private set; }

        public List<GameObject> HorzSewer { get; private set; }
        public GameObject HorzSewerSpec1 { get; private set; }
        public GameObject HorzSewerDoorPrefab { get; private set; }
        public GameObject HorzSewerExitPrefab { get; private set; }
        public GameObject HorzSewerEnd { get; private set; }
        public GameObject HorzSewerCornerPrefab { get; private set; }

        // -- Enemies
        public GameObject Roller { get; private set; }

		public GameObject MetalPipeRoof {get; private set;}
		public GameObject RectStandBlock {get; private set;}
		public GameObject Torch {get; private set;}
        public GameObject TorchEmpty { get; private set; }
        public GameObject Coin {get; private set;}
		public GameObject HeartFull {get; private set;}
		public GameObject HeartEmpty {get; private set;}
		public Dictionary<int, AudioClip> Screams;
		public Dictionary<int, AudioClip> BoneBreak;
		public AudioClip CoinPickup {get; private set;}
		public Transform PickupParent {get; private set;}
		public Transform LevelParent {get; private set;}
		public Transform StandBlockParent {get; private set;}
		public Transform CeillingParent {get; private set;}
        public Transform DebugParent {get; private set;}
		public SceneFadeInOut ScreenFader {get; private set;}
		public YouAreDeadHandler DeadHandler {get; private set;}
		public SceneStatisticsHandler SceneStatistics {get; private set;}
		public InputHandler InputHandler {get; private set;}
		public MenuHandler MenuHandler {get; private set;}
        public DebugHandler DebugHandler { get; private set; }
        public CampaignHandler CampaignHandler { get; private set; }
		public LevelStartup LevelStartup {get; private set;}
        public LevelGenerator LevelGenerator { get; private set; }
        public LevelGeneratorAftermath LevelGeneratorAftermath { get; private set; }
        public Wait Waiter { get; private set; }
        public InventoryHandler InventoryHandler { get; private set; }

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
            // Red Bricks
			HorzRedBricks = new List<GameObject> { Resources.Load("Prefabs/LevelBlocks/HorzRedBricksPrefab") as GameObject };
            HorzRedBricks.Add(Resources.Load("Prefabs/LevelBlocks/HorzRedBricksGatePrefab") as GameObject);
            HorzRedBricksSpec = new List<GameObject> {  Resources.Load("Prefabs/LevelBlocks/HorzSpecRoom1Prefab") as GameObject };
            HorzRedBricksSpec.Add(Resources.Load("Prefabs/LevelBlocks/HorzSpecRoom2Prefab") as GameObject);
            HorzRedBricksStart = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksStartPrefab") as GameObject;
			HorzRedBricksDoorPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksDoorPrefab") as GameObject;
			HorzRedBricksExitPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksExitPrefab") as GameObject;
            HorzRedBricksCornerPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksCornerPrefab") as GameObject;
            HorzRedBricksCrossingPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksCrossingPrefab") as GameObject;
            HorzRedBricksTCrossingPrefab = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksTCrossingPrefab") as GameObject;

            // Sewer Parts
            HorzSewer = new List<GameObject> { Resources.Load("Prefabs/LevelBlocks/HorzSewerPrefab") as GameObject };
            HorzSewerSpec1 = Resources.Load("Prefabs/LevelBlocks/HorzSpecRoom1Prefab") as GameObject;
            HorzSewerEnd = Resources.Load("Prefabs/LevelBlocks/HorzRedBricksEndPrefab") as GameObject;
            HorzSewerDoorPrefab = Resources.Load("Prefabs/LevelBlocks/HorzSewerDoorPrefab") as GameObject;
            HorzSewerExitPrefab = Resources.Load("Prefabs/LevelBlocks/HorzSewerExitPrefab") as GameObject;
            HorzSewerCornerPrefab = Resources.Load("Prefabs/LevelBlocks/HorzSewerCornerPrefab") as GameObject;

            // Sewer Parts
            VertSewer = Resources.Load("Prefabs/LevelBlocks/VertSewerPrefab") as GameObject;
            VertSewerDoor = Resources.Load("Prefabs/LevelBlocks/VertSewerDoorPrefab") as GameObject;
            VertSewerDoorFloor = Resources.Load("Prefabs/LevelBlocks/VertSewerDoorFloorPrefab") as GameObject;
            VertSewerFloor = Resources.Load("Prefabs/LevelBlocks/VertRedBricksFloorPrefab") as GameObject;
            VertSewerExit = Resources.Load("Prefabs/LevelBlocks/VertSewerExitPrefab") as GameObject;

            // Enemies
            Roller = Resources.Load("Prefabs/Enemies/RollerPrefab") as GameObject;

			// Roof
			MetalPipeRoof = Resources.Load("Prefabs/LevelBlocks/FliesenCeilingPrefab") as GameObject;

			// Stand Blocks
			RectStandBlock = Resources.Load("Prefabs/StandBlocks/RectStandBlockPrefab") as GameObject;

			// GUI
			HeartFull = Resources.Load("Prefabs/GUI/HeartFullPrefab") as GameObject;
			HeartEmpty = Resources.Load("Prefabs/GUI/HeartEmptyPrefab") as GameObject;

			Torch = Resources.Load("Prefabs/Lights/TorchPrefab") as GameObject;
            TorchEmpty = Resources.Load("Prefabs/Lights/TorchEmptyPrefab") as GameObject;

            // Parents
            PickupParent = GameObject.Find ("_Pickups").transform;
			LevelParent = GameObject.Find ("_Levels").transform;
			StandBlockParent = GameObject.Find ("_Stands").transform;
			CeillingParent = GameObject.Find ("_Ceilling").transform;
            DebugParent = GameObject.Find("_Debug").transform;

			// Scripts
			var handlingPrefab = GameObject.Find ("HandlingObjectPrefab");
			DeadHandler = handlingPrefab.GetComponent<YouAreDeadHandler>(); 
			SceneStatistics = handlingPrefab.GetComponent<SceneStatisticsHandler>(); 
			LevelStartup = handlingPrefab.GetComponent<LevelStartup>();
			MenuHandler = handlingPrefab.GetComponent<MenuHandler>();
            DebugHandler = handlingPrefab.GetComponent<DebugHandler>();
            CampaignHandler = handlingPrefab.GetComponent<CampaignHandler>(); 
			InputHandler = handlingPrefab.GetComponent<InputHandler>();
            Waiter = handlingPrefab.GetComponent<Wait>();
            LevelGenerator = handlingPrefab.GetComponent<LevelGenerator>();
            LevelGeneratorAftermath = handlingPrefab.GetComponent<LevelGeneratorAftermath>();
            InventoryHandler = handlingPrefab.GetComponent<InventoryHandler>();

            var guiPrefab = GameObject.Find ("GUIPrefab");
			ScreenFader = guiPrefab.GetComponent<SceneFadeInOut>();
		}
	
		/// <summary>
		/// Gets the new area info, this is the stuff we need to construct an area.
		/// </summary>
        public AreaInfos GetNewAreaInfo()
		{
			int value = Random.Range(0, 1);
			if (value == 0)
			{
                var infos = new AreaInfos();
                if (CalculationSingleton.Instance.ActualCreationScope.ActualCampaign == Campaigns.TheCellar)
                {
                    infos.VFloorDoor = VertRedBricksDoorFloor;
                    infos.VFloor = VertRedBricksFloor;
                    infos.VBlock = VertRedBricks;
                    infos.VTransition = VertRedBricksDoor;
                    infos.VRoof = MetalPipeRoof;
                    infos.VExit = VertRedBricksExit;

                    infos.HStart = HorzRedBricksStart;
                    infos.HBlock = new List<GameObject>();
                    infos.HBlock.AddRange(HorzRedBricks);
                    infos.HBlock.AddRange(HorzRedBricksSpec);
                    infos.HTransition = HorzRedBricksDoorPrefab;
                    infos.HExit = HorzRedBricksExitPrefab;
                    infos.HCorner = HorzRedBricksCornerPrefab;
                    infos.HCrossing = HorzRedBricksCrossingPrefab;
                    infos.HTCrossing = HorzRedBricksTCrossingPrefab;
                }
                else if (CalculationSingleton.Instance.ActualCreationScope.ActualCampaign == Campaigns.TheSewers)
                {
                    infos.VFloorDoor = VertSewerDoorFloor;
                    infos.VFloor = VertSewerFloor;
                    infos.VBlock = VertSewer;
                    infos.VTransition = VertSewerDoor;
                    infos.VRoof = MetalPipeRoof;
                    infos.VExit = VertSewerExit;

                    infos.HStart = HorzSewer.First();
                    infos.HBlock = new List<GameObject>();
                    infos.HBlock.AddRange(HorzSewer);
                    infos.HTransition = HorzSewerDoorPrefab;
                    infos.HExit = HorzSewerExitPrefab;
                    infos.HCorner = HorzSewerCornerPrefab;
                    infos.HCrossing = HorzRedBricksCrossingPrefab;
                    infos.HTCrossing = HorzRedBricksTCrossingPrefab;
                }

                return infos;
			}

			return null;
		}

		/// <summary>
		/// Create the specified toBeCreated and pos.
		/// </summary>
		/// <param name="toBeCreated">To be created.</param>
		/// <param name="pos">Position.</param>
		public GameObject Create(GameObject toBeCreated, Vector3? pos = null)
		{
			GameObject result;
			result = GameObject.Instantiate(toBeCreated);
			result.transform.position = pos.HasValue ? pos.Value : result.transform.position;         
			result.name = result.name.Substring(0, result.name.Length - 7);

            if (toBeCreated.tag == "LevelBlock" || toBeCreated.tag == "Exit")
            {
                // Remeber, if this is a level block
                CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock = result;

                var lastInfo = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock != null
                    ? CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.GetComponentInChildren<CollisionCheck>()
                    : null;

                var actualinfo = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponentInChildren<CollisionCheck>();
                if (actualinfo == null)
                {
                    Debug.LogError("CollisionCheck is NULL for: " + CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.name);
                }
                else
                {
                    actualinfo.CreationId = lastInfo == null
                        ? 1
                        : lastInfo.CreationId + 1;
                }

                // Set Successor and Predecessor entries!
                var predecessor = CalculationSingleton.Instance.ActualCreationScope.ActualArea.FirstOrDefault(bl => bl.CollisionInfo.CreationId == (actualinfo.CreationId - 1));
                if (predecessor != null)
                {
                    predecessor.CollisionInfo.Successor = result;
                    actualinfo.Predecessor = predecessor.LevelBlock;
                }

                result.name = String.Concat(result.name, " ID: ", actualinfo.CreationId.ToString());
            }

            return result;		
		}
	}
}