using UnityEngine;
using System.Collections;
using Singleton;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using Enums;
using Blocks;
using Singletons;

namespace LevelCreation
{
	public class LevelGenerator
	{
		private List<GameObject> _actualArea;
		private LevelOrientation _actualLevelCreation;

		/// <summary>
		/// Initializes a new instance of the <see cref="LevelCreation"/> class.
		/// </summary>
		/// <param name="seed">Seed.</param>
		public LevelGenerator (int seed)
		{
			Random.seed = seed;
		}

		/// <summary>
		/// Creates the new level.
		/// </summary>
		public void CreateNewLevel()
		{
			int levelAreaAmount = Random.Range(3, 10);
			TransitionInfo transInfo = new TransitionInfo();
			AreaInfos areaInfos = null;
			LevelOrientation lastOrientation = LevelOrientation.Vertical;
			for (int i = 0; i < levelAreaAmount; i++) 
			{
				LevelOrientation orientation = (LevelOrientation)Random.Range(0, 2);
				areaInfos = PrefabSingleton.Instance.GetNewAreaInfo(null);

				int areaCount = Random.Range(5, 7);
				if (orientation == LevelOrientation.Vertical)
				{
					// This is the object, where the last transtion to a next level begins.
					GameObject transitionObject = CalculationSingleton.Instance.GetStartForVerticalTransition(transInfo.TransitionObject, areaInfos, lastOrientation);

                    // If the level is just starting, we need to go up here!
					var direction = transInfo.TransitionObject == null
						? VertDirection.Up
						: (VertDirection)Random.Range(0, 2);
					transInfo = CreateVerticalArea(areaCount, areaInfos, transitionObject, i == levelAreaAmount - 1, direction);
					lastOrientation = LevelOrientation.Vertical;
				}
				else
				{
					GameObject transitionObject = CalculationSingleton.Instance.GetStartForHorintzalTransition(transInfo.TransitionObject, areaInfos, lastOrientation);
					var direction = (HorzDirection)Random.Range(0, 2);
					transInfo = CreateHorizontalArea(areaCount, areaInfos, transitionObject, i == levelAreaAmount - 1, direction);
					lastOrientation = LevelOrientation.Horizontal;
				}
			}
		}
		
		/// <summary>
		/// Creates the level blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private TransitionInfo CreateHorizontalArea(int areaCount, AreaInfos areaInfos, GameObject transitonBlock, bool isLastAreaBlock, HorzDirection direction)
		{
			int dirMulti = direction == HorzDirection.Left ? 1 : -1;
			TransitionInfo result = new TransitionInfo();
			GameObject endBlock = null;
			_actualArea = new List<GameObject>();
			if (transitonBlock != null)
			{
				// If we got a transition block already, add it to the actual area.
				_actualArea.Add(transitonBlock);
			}
			
			// If we gave a block into this function, we already have transition - skip the first one.
			int start = (transitonBlock == null ? 0 : 1) ;
			var blockSize = CalculationSingleton.Instance.GetSize(areaInfos.HBlock);
			for (int i = start; i < areaCount; i++)
			{
				// Create block
				Vector3 pos = new Vector3(transitonBlock == null ? (i * blockSize.x) * dirMulti : (transitonBlock.transform.position.x + (i * blockSize.x)) * dirMulti,
				                          transitonBlock == null ? 0 : transitonBlock.transform.position.y,
				                          transitonBlock == null ? 0 : transitonBlock.transform.position.z);
				
				GameObject levelBlock = null;
				if (i >= (areaCount - 3))
				{
					// Determine if we build a transition block
					int range = Random.Range(0, 2);
					range = i == areaCount - 1 && result.TransitionObject == null ? 1 : 0;
					
					// Create a Transition block, if random fits. If in the last loop no transition was created yet, create one in every case.
					levelBlock = range == 1
						? isLastAreaBlock ? PrefabSingleton.Instance.Create(areaInfos.HExit, pos) : PrefabSingleton.Instance.Create(areaInfos.HTransition, pos)
						: PrefabSingleton.Instance.Create(areaInfos.HBlock, pos);
					result.TransitionObject = range == 1 ? levelBlock : result.TransitionObject;
				}
				else
				{
					// Create a floor when doing the first one.
					levelBlock = i == 0 
						? transitonBlock != null 
							? PrefabSingleton.Instance.Create(areaInfos.HFloor, pos)
							: PrefabSingleton.Instance.Create(areaInfos.HFloor, pos)
							: PrefabSingleton.Instance.Create(areaInfos.HBlock, pos);
				}
				
				levelBlock.transform.parent = PrefabSingleton.Instance.LevelParent;
				_actualArea.Add(levelBlock);
				
				// Remind the first and teh last block in order to create lights nad plates afterwards
				transitonBlock = i == 0 ? levelBlock : transitonBlock;
				endBlock = i == areaCount -1 ? levelBlock : endBlock;
			}

			// Decide which object to place - do not place on ground floor.
			PlaceHorinzalContent(transitonBlock, endBlock, direction);

			// Return result
			return result;
		}

		/// <summary>
		/// Creates the level blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private TransitionInfo CreateVerticalArea(int areaCount, AreaInfos areaInfos, GameObject transitonBlock, bool isLastAreaBlock, VertDirection direction)
		{
			int dirMulti = direction == VertDirection.Up ? 1 : -1;
			TransitionInfo result = new TransitionInfo();
			GameObject endBlock = null;
			_actualArea = new List<GameObject>();
			if (transitonBlock != null)
			{
				// If we got a transition block already, add it to the actual area.
				_actualArea.Add(transitonBlock);
			}

			// If we gave a block into this function, we already have transition - skip the first one.
			int start = (transitonBlock == null ? 0 : 1) ;
			var blockSize = CalculationSingleton.Instance.GetSize(areaInfos.VBlock);
			for (int i = start; i < areaCount; i++)
			{
				// Create block
				Vector3 pos = new Vector3(transitonBlock == null ? 0 : transitonBlock.transform.position.x,
				                          transitonBlock == null ? (i * blockSize.y) * dirMulti : (transitonBlock.transform.position.y + (i * blockSize.y)) * dirMulti,
				                          transitonBlock == null ? 0 : transitonBlock.transform.position.z);

				GameObject levelBlock = null;
				if (i >= (areaCount - 3))
				{
					// Determine if we build a transition block
					int range = Random.Range(0, 2);
					range = i == areaCount - 1 && result.TransitionObject == null ? 1 : 0;

					// Create a Transition block, if random fits. If in the last loop no transition was created yet, create one in every case.
					levelBlock = range == 1
						? isLastAreaBlock ? PrefabSingleton.Instance.Create(areaInfos.VExit, pos) : PrefabSingleton.Instance.Create(areaInfos.VTransition, pos)
						: PrefabSingleton.Instance.Create(areaInfos.VBlock, pos);
					result.TransitionObject = range == 1 ? levelBlock : result.TransitionObject;
				}
				else
				{
					// Create a floor when doing the first one.
					levelBlock = i == 0 
						? transitonBlock != null 
							? PrefabSingleton.Instance.Create(areaInfos.VFloorDoor, pos)
							: PrefabSingleton.Instance.Create(areaInfos.VFloor, pos)
						: PrefabSingleton.Instance.Create(areaInfos.VBlock, pos);
				}

				levelBlock.transform.parent = PrefabSingleton.Instance.LevelParent;
				_actualArea.Add(levelBlock);

				// Remind the first and teh last block in order to create lights nad plates afterwards
				transitonBlock = i == 0 ? levelBlock : transitonBlock;
				endBlock = i == areaCount -1 ? levelBlock : endBlock;
			}

			// Place a roof on top
			var ceilling = PrefabSingleton.Instance.Create(areaInfos.VRoof, 
				new Vector3(endBlock.transform.position.x, endBlock.transform.position.y + blockSize.y, endBlock.transform.position.z));
			ceilling.transform.parent = PrefabSingleton.Instance.CeillingParent;

			// Decide which object to place - do not place on ground floor.
			PlaceVerticalContent(transitonBlock, endBlock);

			// Return result
			return result;
		}

		/// <summary>
		/// Places the stand blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private void PlaceVerticalContent(GameObject startArea, GameObject endArea)
		{
			float y = startArea.transform.position.y;
			int lastLevelBlock = 0;

			while(true)
			{
				if (y > endArea.transform.position.y)
				{
					// Reached the top - break
					break;
				}
				var actualLevelBlock = _actualArea.OrderByDescending(bk => bk.transform.position.y).FirstOrDefault(bk => y >= bk.transform.position.y);
				y += CalculationSingleton.Instance.JumpDistance;
				
				List<int> compareList = new List<int> { 0, 1, 2, 3};
				BlockInfo blockInfos = null;
				int doorWall = -1;
				if (actualLevelBlock != null)
				{
					blockInfos = XMLSingleton.Instance.BlockInfos[actualLevelBlock.name];
					if (actualLevelBlock.transform.rotation.y == 180)
					{
						// If we have turned the object by 180° we need to make sure to adpt the wall where the hole is.
						doorWall = doorWall == 0 && actualLevelBlock.transform.rotation.y == 180 ? 2 : 0;
						doorWall = doorWall == 1 && actualLevelBlock.transform.rotation.y == 180 ? 3 : 1;
					}
				}

				List<int> wallsUsed = new List<int>();
				for (int stand = 0; stand < 3; stand++)
				{
					GameObject standBlock = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.RectStandBlock, null);
					int wall;
					while (true)
					{
						wall = Math.Abs(Random.Range(0, 4));

						if (doorWall != -1 && doorWall == wall)
						{
							// Continue because we have a door in the wall we want to place the object in
							continue;
						}

						if (!wallsUsed.Contains(wall))
						{
							wallsUsed.Add(wall);
							break;
						}
					}
					
					var pos = CalculationSingleton.Instance.GetPositionForObject(startArea, standBlock, wall, ObjectOrientation.Randomize);
					standBlock.transform.position = new Vector3(pos.Position.x, y, pos.Position.z);
					standBlock.transform.rotation = pos.Rotation;
					standBlock.transform.parent = PrefabSingleton.Instance.StandBlockParent;

					CreateCoin(standBlock, ObjectOrientation.Center);
				}	

				if (actualLevelBlock != null && actualLevelBlock.GetHashCode() != lastLevelBlock && lastLevelBlock != 0)
				{
					IEnumerable<int> unusedWall = compareList.Except(wallsUsed);
					CreateLights(actualLevelBlock, unusedWall.First(), LevelOrientation.Vertical);
				}
				lastLevelBlock = actualLevelBlock != null ? actualLevelBlock.GetHashCode() : 0;
			}
		}

		/// <summary>
		/// Places the stand blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private void PlaceHorinzalContent(GameObject startArea, GameObject endArea, HorzDirection horiziontalDirection)
		{
			float x = startArea.transform.position.x;

            /*
			while(true)
			{
				if ((horiziontalDirection == HorzDirection.Left && x > endArea.transform.position.x)
				    || (horiziontalDirection == HorzDirection.Right && x < endArea.transform.position.x))
				{
					// Reached the end - break
					break;
				}
				var actualLevelBlock = _actualArea.OrderByDescending(bk => bk.transform.position.x).FirstOrDefault(bk => x >= bk.transform.position.x);
				x += CalculationSingleton.Instance.GetSize(actualLevelBlock).x;

				CreateLights(actualLevelBlock, 1, LevelOrientation.Horizontal);
				CreateLights(actualLevelBlock, 3, LevelOrientation.Horizontal);
			}
            */
		}

		/// <summary>
		/// Creates the lights.
		/// </summary>
		/// <param name="wall">Wall.</param>
		private void CreateLights(GameObject levelBlock, int wall, LevelOrientation orientation)
		{
			GameObject light = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.Torch, null);

			if (orientation == LevelOrientation.Vertical)
			{
				var pos = CalculationSingleton.Instance.GetPositionForObject(levelBlock, light, wall, ObjectOrientation.Center);
				float y = levelBlock.transform.position.y + CalculationSingleton.Instance.JumpDistance;
				light.transform.position = new Vector3(pos.Position.x, y, pos.Position.z);
				light.transform.rotation = pos.Rotation;
			}
			else
			{
				var pos = CalculationSingleton.Instance.GetPositionForObject(levelBlock, light, wall, ObjectOrientation.Center);
				float y = CalculationSingleton.Instance.GetSize(levelBlock).y / 2;
				light.transform.position = new Vector3(pos.Position.x, y, pos.Position.z);
				light.transform.rotation = pos.Rotation;
			}
			light.transform.parent = PrefabSingleton.Instance.LightsParent;
		}

		/// <summary>
		/// Creates the coin.
		/// </summary>
		/// <param name="standBlock">Stand block.</param>
		private void CreateCoin (GameObject standBlock, ObjectOrientation orientation)
		{
			int coinSpawn = PlayerSingleton.Instance.Difficulty == Difficulty.Hard ? 3 : -1;
			coinSpawn = PlayerSingleton.Instance.Difficulty == Difficulty.VeryHard ? 4 : coinSpawn;
			coinSpawn = PlayerSingleton.Instance.Difficulty == Difficulty.NintendoHard ? 5 : coinSpawn;

			if (Random.Range(0, coinSpawn) == 0)
			{
				// Place a coin, if we are lucky
				var coin = CalculationSingleton.Instance.PlaceOnTopOfObject(standBlock, PrefabSingleton.Instance.Coin, ObjectOrientation.Center);
				coin.transform.parent = PrefabSingleton.Instance.PickupParent;
			}
		}
	}
}