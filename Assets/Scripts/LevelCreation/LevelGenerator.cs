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
            CalculationSingleton.Instance.ActualCreationScope.AreaInfos = PrefabSingleton.Instance.GetNewAreaInfo(null);             
            // Prepare Actual Scope - set the next Orienation twice, so Actual and gets filled - 
            CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation = (LevelOrientation)Random.Range(0, 2);
            CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation = (LevelOrientation)Random.Range(0, 2);

			for (int i = 0; i < levelAreaAmount; i++) 
			{
                CalculationSingleton.Instance.ActualCreationScope.IsLastArea = i == levelAreaAmount - 1;

				int areaCount = Random.Range(5, 7);
                if (CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation == LevelOrientation.Vertical)
				{
                    if (CalculationSingleton.Instance.ActualCreationScope.ActualVerticalDirection == VertDirection.NotSet)
                    {
                        // Preset, if not done yet - If the level is just starting, we need to go up here!
                        CalculationSingleton.Instance.ActualCreationScope.GetNewVertDirection();
                    }
                    CalculationSingleton.Instance.ActualCreationScope.GetNewVertDirection();

					// This is the object, where the last transtion to a next level begins.
					GameObject transitionObject = CalculationSingleton.Instance.GetStartForVerticalArea();
					CalculationSingleton.Instance.ActualCreationScope.ActualTransitionObject = CreateVerticalArea(areaCount, transitionObject);
				}
				else
				{
                    if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.NotSet)
                    {
                        // Preset, if not done yet.
                        CalculationSingleton.Instance.ActualCreationScope.GetNewHorzDirection();
                    }
                    CalculationSingleton.Instance.ActualCreationScope.GetNewHorzDirection();

                    GameObject transitionObject = CalculationSingleton.Instance.GetStartForHorintzalArea();
                    CalculationSingleton.Instance.ActualCreationScope.ActualTransitionObject = CreateHorizontalArea(areaCount, transitionObject);
				}

                CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation = (LevelOrientation)Random.Range(0, 2);
			}
		}
		
		/// <summary>
		/// Creates the level blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private GameObject CreateHorizontalArea(int areaCount, GameObject transitonBlock)
		{
            GameObject result = null;
			GameObject endBlock = null;
			_actualArea = new List<GameObject>();
			if (transitonBlock != null)
			{
				// If we got a transition block already, add it to the actual area.
				_actualArea.Add(transitonBlock);
			}
			
			// If we gave a block into this function, we already have transition - skip the first one.
			int start = (transitonBlock == null ? 0 : 1) ;
            var blockSize = HelperSingleton.Instance.GetSize(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HBlock);
			for (int i = start; i < areaCount; i++)
			{
				// Create block
                float x = 0;
                float z = 0;
                if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right
                    || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left)
                {
                    int dirMulti = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left ? 1 : -1;
                    x = transitonBlock == null ? (i * blockSize.x) * dirMulti : transitonBlock.transform.position.x + (i * blockSize.x * dirMulti);
                    z = transitonBlock == null ? 0 : transitonBlock.transform.position.z;
                }
                else if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                    || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Backwards)
                {
                    int dirMulti = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward ? 1 : -1;
                    x = transitonBlock == null ? 0 : transitonBlock.transform.position.x;
                    z = transitonBlock == null ? (i * blockSize.z) * dirMulti : transitonBlock.transform.position.z + (i * blockSize.z * dirMulti);
                }

				Vector3 pos = new Vector3(x, transitonBlock == null ? 0 : transitonBlock.transform.position.y, z);		
				GameObject levelBlock = null;
				if (i >= (areaCount - 3))
				{
					// Determine if we build a transition block
					int range = Random.Range(0, 2);
					range = i == areaCount - 1 && result == null ? 1 : 0;
					
					// Create a Transition block, if random fits. If in the last loop no transition was created yet, create one in every case.
                    if (range == 1)
                    {
                        if (CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation == LevelOrientation.Horizontal)
                        {
                            levelBlock = CalculationSingleton.Instance.ActualCreationScope.GetHorizontalTranstion(pos);
                        }
                        else
                        {
                            levelBlock = CalculationSingleton.Instance.ActualCreationScope.GetVerticalTransition(pos);
                        }
                        
                        result = levelBlock; 
                    }
                    else
                    {
                        levelBlock = PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HBlock, pos);
                        CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
                    }
				}
				else
				{
					// Create a floor when doing the first one.
					levelBlock = i == 0 
						? transitonBlock != null
                            ? PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HFloor, pos)
                            : PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HFloor, pos)
                            : PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HBlock, pos);
                    CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
				}
				
				levelBlock.transform.parent = PrefabSingleton.Instance.LevelParent;
				_actualArea.Add(levelBlock);
				
				// Remind the first and teh last block in order to create lights nad plates afterwards
				transitonBlock = i == 0 ? levelBlock : transitonBlock;
				endBlock = i == areaCount -1 ? levelBlock : endBlock;
			}

			// Decide which object to place - do not place on ground floor.
			PlaceHorinzalContent(transitonBlock, endBlock);

			// Return result
			return result;
		}

		/// <summary>
		/// Creates the level blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private GameObject CreateVerticalArea(int areaCount, GameObject transitonBlock)
		{
			int dirMulti = CalculationSingleton.Instance.ActualCreationScope.ActualVerticalDirection == VertDirection.Up ? 1 : -1;
			GameObject endBlock = null;
            GameObject transitionInfo = null;
			_actualArea = new List<GameObject>();
			if (transitonBlock != null)
			{
				// If we got a transition block already, add it to the actual area.
				_actualArea.Add(transitonBlock);
			}

			// If we gave a block into this function, we already have transition - skip the first one.
			int start = (transitonBlock == null ? 0 : 1) ;
            var blockSize = HelperSingleton.Instance.GetSize(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VBlock);
			for (int i = start; i < areaCount; i++)
			{
				// Create block
				Vector3 pos = new Vector3(transitonBlock == null ? 0 : transitonBlock.transform.position.x,
				                          transitonBlock == null ? (i * blockSize.y) * dirMulti : transitonBlock.transform.position.y + (i * blockSize.y * dirMulti),
				                          transitonBlock == null ? 0 : transitonBlock.transform.position.z);

				GameObject levelBlock = null;
				if (i >= (areaCount - 3))
				{
					// Determine if we build a transition block
					int range = Random.Range(0, 2);
					range = i == areaCount - 1 && transitionInfo == null ? 1 : 0;

					// Create a Transition block, if random fits. If in the last loop no transition was created yet, create one in every case.
					levelBlock = range == 1
                        ? CalculationSingleton.Instance.ActualCreationScope.IsLastArea
                            ? PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VExit, pos)
                            : PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VTransition, pos)
                        : PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VBlock, pos);
					transitionInfo = range == 1 ? levelBlock : transitionInfo;
				}
				else
				{
					// Create a floor when doing the first one.
					levelBlock = i == 0 
						? transitonBlock != null
                            ? PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VFloorDoor, pos)
                            : PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VFloor, pos)
                        : PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VBlock, pos);
				}

				levelBlock.transform.parent = PrefabSingleton.Instance.LevelParent;
				_actualArea.Add(levelBlock);

				// Remind the first and teh last block in order to create lights nad plates afterwards
				transitonBlock = i == 0 ? levelBlock : transitonBlock;
				endBlock = i == areaCount -1 ? levelBlock : endBlock;
			}

			// Place a roof on top
            var ceilling = PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VRoof, 
				new Vector3(endBlock.transform.position.x, endBlock.transform.position.y + blockSize.y, endBlock.transform.position.z));
			ceilling.transform.parent = PrefabSingleton.Instance.CeillingParent;

			// Decide which object to place - do not place on ground floor.
			PlaceVerticalContent(transitonBlock, endBlock);

			// Return result
			return transitionInfo;
		}

		/// <summary>
		/// Places the stand blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
        private void PlaceVerticalContent(GameObject startArea, GameObject endArea)
		{
			float y = startArea.transform.position.y;
			int lastLevelBlock = 0;
            int counter = 0;
            
			while(true)
			{
                if (counter > 100)
                {
                    Debug.Log(String.Concat("Counter is more than ", counter, "! Abort"));
                    break;
                }
                counter++;

				if (y > endArea.transform.position.y)
				{
					// Reached the top - break
					break;
				}

                var actualLevelBlock = CalculationSingleton.Instance.GetBlockFromArea(_actualArea, y);
                // y += direction == VertDirection.Up ? CalculationSingleton.Instance.JumpDistance : CalculationSingleton.Instance.JumpDistance * -1;
                y += CalculationSingleton.Instance.JumpDistance;
				
				List<int> compareList = new List<int> { 0, 1, 2, 3};
				int doorWall = -1;
				if (actualLevelBlock != null)
				{
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
					GameObject standBlock = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.RectStandBlock);
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

                    CalculationSingleton.Instance.GetPositionForObject(actualLevelBlock, standBlock, wall, ObjectOrientation.Randomize, y);
					CreateCoin(standBlock, ObjectOrientation.Center);
				}	

				if (actualLevelBlock != null && actualLevelBlock.GetHashCode() != lastLevelBlock && lastLevelBlock != 0)
				{
					IEnumerable<int> unusedWall = compareList.Except(wallsUsed);
					CreateLights(actualLevelBlock, unusedWall.First());
				}
				lastLevelBlock = actualLevelBlock != null ? actualLevelBlock.GetHashCode() : 0;
			}
		}

		/// <summary>
		/// Places the stand blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private void PlaceHorinzalContent(GameObject startArea, GameObject endArea)
		{
            float xOrz = (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left
                || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right)
                ? startArea.transform.position.x
                : startArea.transform.position.z;
            int counter = 0;
            
			while(true)
			{
                if (counter > 100)
                {
                    Debug.Log(String.Concat("Counter is more than ", counter, "! Abort"));
                    break;
                }
                counter++;

                if ((CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left && xOrz > endArea.transform.position.x)
                    || (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right && xOrz < endArea.transform.position.x)
                    || (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward && xOrz > endArea.transform.position.z)
                    || (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Backwards && xOrz < endArea.transform.position.z))
				{
					// Reached the end - break
					break;
				}

                GameObject actualLevelBlock;
                switch (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection)
                {
                    case HorzDirection.Left:
                    case HorzDirection.Right:
                        actualLevelBlock = _actualArea.OrderByDescending(bk => bk.transform.position.x).FirstOrDefault(bk => xOrz >= bk.transform.position.x);
                        xOrz += CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left
                            ? HelperSingleton.Instance.GetSize(actualLevelBlock).x
                            : HelperSingleton.Instance.GetSize(actualLevelBlock).x * -1;
                        break;
                    case HorzDirection.Forward:
                    case HorzDirection.Backwards:
                        actualLevelBlock = _actualArea.OrderByDescending(bk => bk.transform.position.z).FirstOrDefault(bk => xOrz >= bk.transform.position.z);
                        xOrz += CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                            ? HelperSingleton.Instance.GetSize(actualLevelBlock).z
                            : HelperSingleton.Instance.GetSize(actualLevelBlock).z * -1;
                        break;
                    default:
                        throw new ArgumentException("Type not supported");
                }

                CreateLights(actualLevelBlock, -1);
			}
		}

		/// <summary>
		/// Creates the lights.
		/// </summary>
		/// <param name="wall">Wall.</param>
		private void CreateLights(GameObject levelBlock, int wall)
		{
            GameObject light;
            
            if (CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation == LevelOrientation.Vertical)
			{
                light = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.Torch);
                float y = levelBlock.transform.position.y + CalculationSingleton.Instance.JumpDistance;
                CalculationSingleton.Instance.GetPositionForObject(levelBlock, light, wall, ObjectOrientation.Center, y);
			}
			else
			{
                var wallDoors = HelperSingleton.Instance.GetAllRealWalls(levelBlock);
                foreach (var wallNumber in wallDoors)
                {
                    light = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.Torch);
                    CalculationSingleton.Instance.GetPositionForObject(levelBlock, light, wallNumber.WallNumber, ObjectOrientation.Center);
                }
			}
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