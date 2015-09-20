﻿using UnityEngine;
using Singleton;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using Enums;
using Assets.Scripts.Blocks;
using Assets.Scripts.Enums;

namespace LevelCreation
{
	public class LevelGenerator
	{
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
            CalculationSingleton.Instance.ActualCreationScope.AreaInfos = PrefabSingleton.Instance.GetNewAreaInfo();
            // Prepare Actual Scope - set the next Orienation twice, so Actual and gets filled - 
            CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation = (LevelOrientation)Random.Range(0, 1);
            CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation = (LevelOrientation)Random.Range(0, 1);

			for (int i = 0; i < levelAreaAmount; i++) 
			{
                CalculationSingleton.Instance.ActualCreationScope.IsLastArea = i == levelAreaAmount - 1;

                if (CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation == LevelOrientation.Vertical)
				{
                    int areaCount = Random.Range(3, 6);
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
                    int areaCount = Random.Range(3, 7);
                    if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.NotSet)
                    {
                        // Preset, if not done yet.
                        CalculationSingleton.Instance.ActualCreationScope.GetNewHorzDirection();
                    }
                    CalculationSingleton.Instance.ActualCreationScope.GetNewHorzDirection();

                    GameObject transitionObject = CalculationSingleton.Instance.GetStartForHorintzalArea();
                    CalculationSingleton.Instance.ActualCreationScope.ActualTransitionObject = CreateHorizontalArea(areaCount, transitionObject);
				}

                CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation = (LevelOrientation)Random.Range(0, 1);
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
            CalculationSingleton.Instance.ActualCreationScope.ActualLevel.AddRange(CalculationSingleton.Instance.ActualCreationScope.ActualArea);
            CalculationSingleton.Instance.ActualCreationScope.ActualArea.Clear();
			if (transitonBlock != null)
			{
				// If we got a transition block already, add it to the actual area.
                CalculationSingleton.Instance.ActualCreationScope.ActualArea.Add(new BlockInfo(transitonBlock));
			}
			
			// If we gave a block into this function, we already have transition - skip the first one.
			int start = (transitonBlock == null ? 0 : 1) ;
			for (int i = start; i < areaCount; i++)
			{
                // Gets the next position for the next horizontal block
                GameObject blockToBeCreated = null;
                Vector3 pos;
				GameObject levelBlock = null;
				if (i >= (areaCount - 3))
				{
					// Determine if we build a transition block
					int range = Random.Range(0, 2);
					range = i == areaCount - 1 && result == null ? 1 : 0;
					
					// Create a Transition block, if random fits. If in the last loop no transition was created yet, create one in every case.
                    if (range == 1)
                    {
                        blockToBeCreated = CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HTransition;
                        pos = CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForNextHorizontal(blockToBeCreated);

                        levelBlock = CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation == LevelOrientation.Horizontal
                            ? CalculationSingleton.Instance.ActualCreationScope.GetHorizontalTranstion(pos)
                            : CalculationSingleton.Instance.ActualCreationScope.GetVerticalTransition(pos); 
                        result = levelBlock;
                    }
                    else
                    {
                        blockToBeCreated = CalculationSingleton.Instance.ActualCreationScope.AreaInfos.GetHBlock();
                        pos = CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForNextHorizontal(blockToBeCreated);

                        pos = CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForNextHorizontal(blockToBeCreated);
                        levelBlock = PrefabSingleton.Instance.Create(blockToBeCreated, pos);
                        CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
                    }
                    HelperSingleton.Instance.AdaptPositonForExit();
				}
				else
				{
					// Create a floor when doing the first one.
                    blockToBeCreated = CalculationSingleton.Instance.ActualCreationScope.AreaInfos.GetHBlock();
                    pos = CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForNextHorizontal(blockToBeCreated);
					levelBlock = i == 0 
						? PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HFloor, pos)
                        : PrefabSingleton.Instance.Create(blockToBeCreated, pos);
                    CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
                    HelperSingleton.Instance.AdaptPositonForExit();
				}
				
				levelBlock.transform.parent = PrefabSingleton.Instance.LevelParent;
                CalculationSingleton.Instance.ActualCreationScope.ActualArea.Add(new BlockInfo(levelBlock));
				
				// Remind the first and teh last block in order to create lights nad plates afterwards
				transitonBlock = i == 0 ? levelBlock : transitonBlock;
				endBlock = i == areaCount -1 ? levelBlock : endBlock;

                AddEnemey(levelBlock);
			}

			// Decide which object to place - do not place on ground floor.
			PlaceHorinzalContent(transitonBlock, endBlock);

			// Return result
			return result;
		}

        /// <summary>
        /// Adds an enemy to an Horizinotal block
        /// </summary>
        /// <param name="levelBlock"></param>
        private void AddEnemey(GameObject levelBlock)
        {
            if (Random.Range(0, 5) == 0)
            {
                Vector3 position = new Vector3(levelBlock.transform.position.x, levelBlock.transform.position.y + .5f, levelBlock.transform.position.z);
                
                // Add an enemy if needed
                // PrefabSingleton.Instance.Create(PrefabSingleton.Instance.Roller, position);
            }
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
            CalculationSingleton.Instance.ActualCreationScope.ActualLevel.AddRange(CalculationSingleton.Instance.ActualCreationScope.ActualArea);
            CalculationSingleton.Instance.ActualCreationScope.ActualArea.Clear();
			if (transitonBlock != null)
			{
				// If we got a transition block already, add it to the actual area.
                CalculationSingleton.Instance.ActualCreationScope.ActualArea.Add(new BlockInfo(transitonBlock));
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
				if (i >= (areaCount - 2))
				{
					// Determine if we build a transition block
					int range = Random.Range(0, 2);
					range = i == areaCount - 1 && transitionInfo == null ? 1 : 0;

					// Create a Transition block, if random fits. If in the last loop no transition was created yet, create one in every case.
                    if (range == 1)
                    {
                        levelBlock = CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation == LevelOrientation.Horizontal
                            ? CalculationSingleton.Instance.ActualCreationScope.GetHorizontalTranstion(pos)
                            : CalculationSingleton.Instance.ActualCreationScope.GetVerticalTransition(pos);
                        levelBlock.transform.position = pos;
                        transitionInfo = levelBlock;
                    }
                    else
                    {
                        levelBlock = PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.VBlock, pos);
                        levelBlock.transform.rotation = Quaternion.Euler(new Vector3(0, transitonBlock.transform.rotation.eulerAngles.y + 90, 0));
                    }
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
                CalculationSingleton.Instance.ActualCreationScope.ActualArea.Add(new BlockInfo(levelBlock));

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

            foreach (var blockInfo in CalculationSingleton.Instance.ActualCreationScope.ActualArea)
            {
                List<int> wallsUsed = new List<int>();
                GameObject levelBlock = blockInfo.LevelBlock;

                var walls = levelBlock.transform.GetComponentsInChildren<WallDescriptor>().Where(wl => wl.Descriptor == WallDescription.Wall);
                int wallCount = walls.Count() == 4 ? 3 : walls.Count();
                for (int i = 0; i < 2; i++)
                {
                    var remainingWalls = walls.ToList();
                    VertOrientation verticalOrientation = i == 1
                        ? VertOrientation.Center
                        : VertOrientation.Top;
                    for (int stand = 0; stand < wallCount; stand++)
                    {
                        if (!levelBlock.transform.GetComponentsInChildren<BlockDescriptor>().First().NeedsStairs)
                        {
                            // If no stairs are needed, do not create one.
                            continue;
                        }

                        GameObject standBlock = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.RectStandBlock);
                        int wall;
                        while (true)
                        {
                            wall = Random.Range(0, 4);
                            if (!walls.Any(wl => wl.WallNumber == wall))
                            {
                                // Continue because we have a door/nothing/exit in the wall we want to place the object in
                                continue;
                            }

                            if (remainingWalls.Any(wl => wl.WallNumber == wall))
                            {
                                remainingWalls = remainingWalls.Where(wl => wl.WallNumber != wall).ToList();
                                break;
                            }

                            if (!remainingWalls.Any())
                            {
                                // List is empty
                                break;
                            }
                        }

                        CalculationSingleton.Instance.OrientationCalculation.Init(levelBlock.GetComponentsInChildren<WallDescriptor>().First(wl => wl.WallNumber == wall), standBlock);
                        CalculationSingleton.Instance.OrientationCalculation.SetOrienation(HorzOrientation.Randomize, verticalOrientation);
                        CalculationSingleton.Instance.GetPositionForObject(HorzOrientation.Randomize, verticalOrientation);
                        CreateCoin(standBlock, HorzOrientation.Center);
                    }

                    if (verticalOrientation == VertOrientation.Top)
                    {
                        // Place light
                        if (remainingWalls.Any())
                        {
                            CreateLights(levelBlock, remainingWalls.First().WallNumber);
                        }
                    }
                }			
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
                        actualLevelBlock = CalculationSingleton.Instance.ActualCreationScope.ActualArea
                            .OrderByDescending(bk => bk.LevelBlock.transform.position.x).FirstOrDefault(bk => xOrz >= bk.LevelBlock.transform.position.x).LevelBlock;
                        xOrz += CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left
                            ? HelperSingleton.Instance.GetSize(actualLevelBlock).x
                            : HelperSingleton.Instance.GetSize(actualLevelBlock).x * -1;
                        break;
                    case HorzDirection.Forward:
                    case HorzDirection.Backwards:
                        actualLevelBlock = CalculationSingleton.Instance.ActualCreationScope.ActualArea
                            .OrderByDescending(bk => bk.LevelBlock.transform.position.z).FirstOrDefault(bk => xOrz >= bk.LevelBlock.transform.position.z).LevelBlock;
                        xOrz += CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                            ? HelperSingleton.Instance.GetSize(actualLevelBlock).z
                            : HelperSingleton.Instance.GetSize(actualLevelBlock).z * -1;
                        break;
                    default:
                        throw new ArgumentException("Type not supported");
                }

                if (Random.Range(0, 12) == 0)
                {
                    // Create a light, if random fights
                    CreateLights(actualLevelBlock, -1);
                }
			}
		}

		/// <summary>
		/// Creates the lights.
		/// </summary>
		/// <param name="wall">Wall.</param>
		private void CreateLights(GameObject levelBlock, int wall)
		{
            if (CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation == LevelOrientation.Vertical)
			{
                var light = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.Torch);
                CalculationSingleton.Instance.OrientationCalculation.Init(levelBlock.GetComponentsInChildren<WallDescriptor>().First(wl => wl.WallNumber == wall), light);
                CalculationSingleton.Instance.OrientationCalculation.SetOrienation(HorzOrientation.Center, VertOrientation.Center);
                CalculationSingleton.Instance.GetPositionForObject();
                light.SetActive(false);
                light.SetActive(true);
            }
			else
			{
                var wallObject = HelperSingleton.Instance.GetAllRealWalls(levelBlock).First();
                var light = PrefabSingleton.Instance.Create(PrefabSingleton.Instance.Torch);
                CalculationSingleton.Instance.OrientationCalculation.Init(wallObject, light);
                CalculationSingleton.Instance.GetPositionForObject();
                light.SetActive(false);
                light.SetActive(true);
			}
		}

		/// <summary>
		/// Creates the coin.
		/// </summary>
		/// <param name="standBlock">Stand block.</param>
        private void CreateCoin(GameObject standBlock, HorzOrientation orientation)
		{
			int coinSpawn = PlayerSingleton.Instance.Difficulty == Difficulty.Hard ? 3 : -1;
			coinSpawn = PlayerSingleton.Instance.Difficulty == Difficulty.VeryHard ? 4 : coinSpawn;
			coinSpawn = PlayerSingleton.Instance.Difficulty == Difficulty.NintendoHard ? 5 : coinSpawn;

			if (Random.Range(0, coinSpawn) == 0)
			{
				// Place a coin, if we are lucky
                var coin = CalculationSingleton.Instance.PlaceOnTopOfObject(standBlock, PrefabSingleton.Instance.Coin, HorzOrientation.Center);
				coin.transform.parent = PrefabSingleton.Instance.PickupParent;
			}
		}
	}
}