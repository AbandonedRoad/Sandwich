using UnityEngine;
using Singleton;
using System.Linq;
using Random = UnityEngine.Random;
using Enums;
using Blocks;
using System.Collections;

namespace LevelCreation
{
	public class LevelGenerator : MonoBehaviour
	{
        private readonly ContentGenerator _contentGenerator = new ContentGenerator();

    	/// <summary>
		/// Creates the new level.
		/// </summary>
		public IEnumerator CreateNewLevel(int seed)
		{
            Random.seed = seed;

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

            // Wait for .5f seconds to allow everything to initialize.
            yield return new WaitForSeconds(.5f);
        }
		
		/// <summary>
		/// Creates the level blocks.
		/// </summary>
		/// <param name="blockAmount">Block amount.</param>
		private GameObject CreateHorizontalArea(int areaCount, GameObject transitonBlock)
		{
            GameObject result = null;
			GameObject endBlock = null;
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
						? PrefabSingleton.Instance.Create(CalculationSingleton.Instance.ActualCreationScope.AreaInfos.HStart, pos)
                        : PrefabSingleton.Instance.Create(blockToBeCreated, pos);
                    CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
                    HelperSingleton.Instance.AdaptPositonForExit();
				}

                levelBlock.transform.parent = PrefabSingleton.Instance.LevelParent;
                CalculationSingleton.Instance.ActualCreationScope.ActualArea.Add(new BlockInfo(levelBlock));
				
				// Remind the first and teh last block in order to create lights nad plates afterwards
				transitonBlock = i == 0 ? levelBlock : transitonBlock;
				endBlock = i == areaCount -1 ? levelBlock : endBlock;

                _contentGenerator.AddEnemey(levelBlock);
			}

            // Add actual area to whole leve.
            CalculationSingleton.Instance.ActualCreationScope.ActualLevel.AddRange(CalculationSingleton.Instance.ActualCreationScope.ActualArea);

            // Decide which object to place - do not place on ground floor.
            _contentGenerator.PlaceHorinzalContent(transitonBlock, endBlock);

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
            _contentGenerator.PlaceVerticalContent(transitonBlock, endBlock);

            // Add area to level
            CalculationSingleton.Instance.ActualCreationScope.ActualLevel.AddRange(CalculationSingleton.Instance.ActualCreationScope.ActualArea);

            // Return result
            return transitionInfo;
		}
	}
}