using System;
using System.Linq;
using UnityEngine;
using Enums;
using Random = UnityEngine.Random;
using Blocks;
using LevelCreation;
using Singletons;
using System.Collections.Generic;
using Assets.Scripts.Blocks;
using Assets.Scripts.Enums;

namespace Singleton
{
	public class CalculationSingleton
	{
		private static CalculationSingleton _instance;

		public float JumpDistance = 2.25f;

        public CreationScope ActualCreationScope { get; set; }

		/// <summary>
		/// Gets instance
		/// </summary>
		public static CalculationSingleton Instance
		{
			get 
			{
				if (_instance == null) 
				{
					_instance = new CalculationSingleton();
                    _instance.ActualCreationScope = new CreationScope();
				}
				
				return _instance;
			}
		}

		/// <summary>
		/// Gets the position for object.
		/// </summary>
		/// <returns>The position for object.</returns>
		/// <param name="parentObject">Parent object.</param>
		/// <param name="objectToPlace">Object to place.</param>
		/// <param name="getForWall">Get for wall.</param>
		public BlockPlacement GetPositionForObject(GameObject parentObject, GameObject childToPlace, int getForWall, ObjectOrientation orientation)
		{
			Renderer parentRenderer = parentObject.tag == "RenderObject" 
				? parentObject.GetComponent<Renderer>() 
				: parentObject.GetComponentsInChildren<Renderer>().ToList().FirstOrDefault(rend => rend.gameObject.tag == "RenderObject");
			Renderer childRenderer = childToPlace.tag == "RenderObject" 
				? childToPlace.GetComponent<Renderer>() 
				: childToPlace.GetComponentsInChildren<Renderer>().ToList().FirstOrDefault(rend => rend.gameObject.tag == "RenderObject");

			if (parentRenderer == null) Debug.LogError("No RenderObject found for: " + parentObject.name);
			if (childRenderer == null) Debug.LogError("No RenderObject found for: " + childToPlace.name);

			var parentSize = parentRenderer.bounds.size;
            var childSize = childRenderer.bounds.size;

            var descriptor = HelperSingleton.Instance.GetWallDescription(parentObject);
            float middleXObject = (parentSize.x - (2 * descriptor.WallStrength)) / 2;
			float childLongSide = childSize.x > childSize.z ? childSize.x : childSize.z;
			float childShortSide = childSize.x > childSize.z ? childSize.z : childSize.x;
            float middleZObject = (parentSize.z - (2 * descriptor.WallStrength)) / 2;
			float xPos = parentObject.transform.position.x;
			float zPos = parentObject.transform.position.z;
			float yRot = 0;

			switch (getForWall) 
			{
				case 0:
				case 2:
				xPos += getForWall == 0 ? (middleXObject - (childShortSide / 2)) * -1 : middleXObject - (childShortSide / 2);
				zPos += orientation == ObjectOrientation.Center ? 0 : Random.Range((middleZObject - childLongSide), (middleZObject - childLongSide)*-1);
					yRot = getForWall == 0 ? 180 : 0;
					break;
				case 1:
				case 3:
				xPos += orientation == ObjectOrientation.Center ? 0 : Random.Range((middleXObject - childLongSide), (middleXObject - childLongSide)*-1);
				zPos += getForWall == 1 ? (middleZObject - (childShortSide / 2)) * -1 : middleZObject - (childShortSide / 2);
					yRot = getForWall == 1 ? 90 : 270;
					break;
				default:
					break;
			}

			childToPlace.name = "Wall: " + getForWall.ToString();

			return new BlockPlacement
			{
				Position = new Vector3(xPos, 0, zPos),
				Rotation = Quaternion.Euler(0, yRot, 0)
			};		
		}

		/// <summary>
		/// Gets the position for transition.
		/// </summary>
		/// <returns>The position for transition.</returns>
		/// <param name="actualBlock">Actual block.</param>
		/// <param name="transitionToBeCreate">Transition to be create.</param>
		public GameObject GetStartForVerticalArea()
		{
			var parent = GameObject.Find("_Levels").transform;
            GameObject lastTransition = CalculationSingleton.Instance.ActualCreationScope.ActualTransitionObject;

			if (lastTransition == null)
			{
				// We have no transition - this is the first area
				return null;
			}

            var wallDoors = HelperSingleton.Instance.GetAllDoorWalls(lastTransition);

            GameObject instance;
            if (CalculationSingleton.Instance.ActualCreationScope.PreviouslyLevelOrientation == LevelOrientation.Vertical)
            {
                instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.VFloorDoor);
                
                // Rotate last vertical transition into correct rotation.
                var doorWall = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Door);
                doorWall.RotateTo(HorzDirection.Backwards);

                // Rotate to actual be created block into correct rotation.
                doorWall = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Door);
                doorWall.RotateTo(HorzDirection.Forward);
            }
            else
            {
                instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.HBlock);
            }

            instance.transform.parent = parent;

			float xValue = lastTransition.transform.position.x;
			Vector3 size = HelperSingleton.Instance.GetSize(instance);
            if (wallDoors.Any(dsc => dsc.WallNumber == 0 || dsc.WallNumber == 2))
			{
                xValue += wallDoors.Any(dsc => dsc.WallNumber == 0) ? size.x : size.x * -1;
			}
			else
			{
				xValue = lastTransition.transform.position.x;
			}

			float zValue = lastTransition.transform.position.z;
            if (wallDoors.Any(dsc => dsc.WallNumber == 1 || dsc.WallNumber == 3))
			{
                zValue += wallDoors.Any(dsc => dsc.WallNumber == 1) ? size.z : size.z * -1;
			}
			else
			{
				zValue = lastTransition.transform.position.z;
			}
			
			instance.transform.position = new Vector3(xValue, lastTransition.transform.position.y, zValue);

            HelperSingleton.Instance.CreateDebugGOAtPosition(
                  "Prv Level: " + CalculationSingleton.Instance.ActualCreationScope.PreviouslyLevelOrientation + Environment.NewLine
                + "  Act Level: " + CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation + Environment.NewLine
                + "  Prv Horz: " + CalculationSingleton.Instance.ActualCreationScope.PreviousHorizontalDirection + Environment.NewLine
                + "  Act Horz: " + CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection + Environment.NewLine
                + "  Prv Vert: " + CalculationSingleton.Instance.ActualCreationScope.PreviousVerticalDirection + Environment.NewLine
                + "  Act Vert: " + CalculationSingleton.Instance.ActualCreationScope.ActualVerticalDirection + Environment.NewLine
                + "  Last Trans: " + lastTransition.name + Environment.NewLine
                + "  New block: " + instance.name, instance.transform.position);

			return instance;		
		}
	
		/// <summary>
		/// Gets the position for transition.
		/// </summary>
		/// <returns>The position for transition.</returns>
		/// <param name="actualBlock">Actual block.</param>
		/// <param name="transitionToBeCreate">Transition to be create.</param>
		public GameObject GetStartForHorintzalArea()
		{
            GameObject lastTransition = CalculationSingleton.Instance.ActualCreationScope.ActualTransitionObject;

			if (lastTransition == null)
			{
				// We have no transition - this is the first area
				return null;
			}

            GameObject instance;
            if (CalculationSingleton.Instance.ActualCreationScope.PreviouslyLevelOrientation == LevelOrientation.Vertical)
            {
                instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.HTransition);

                // Rotate last vertical transition into correct rotation.
                var doorWall = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Door);
                doorWall.RotateTo(CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection);

                // Rotate to actual be created block into correct rotation.
                doorWall = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Door);
                doorWall.RotateTo(HelperSingleton.Instance.GetOpposite(CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection));
            }
            else
            {
                if (CalculationSingleton.Instance.ActualCreationScope.PreviousHorizontalDirection == CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection)
                {
                    // Direction did not change - create a regular block.
                    instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.HBlock);
                    instance.transform.rotation = CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
                }
                else
                {
                    // Direction changed - create corner.
                    instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.HCorner);
                    instance.transform.rotation = CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForHorizontalCorner();
                }
            }

            instance.transform.parent = PrefabSingleton.Instance.LevelParent;

            Vector3 size = HelperSingleton.Instance.GetSize(instance);
            float zValue = lastTransition.transform.position.z;
            float xValue = lastTransition.transform.position.x;

            instance.transform.position = CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForHorizontalStart();

            HelperSingleton.Instance.CreateDebugGOAtPosition(
                  "Prv Level: " + CalculationSingleton.Instance.ActualCreationScope.PreviouslyLevelOrientation + Environment.NewLine
                + "Act Level: " + CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation + Environment.NewLine
                + "Prv Horz: " + CalculationSingleton.Instance.ActualCreationScope.PreviousHorizontalDirection + Environment.NewLine
                + "Act Horz: " + CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection + Environment.NewLine
                + "Prv Vert: " + CalculationSingleton.Instance.ActualCreationScope.PreviousVerticalDirection + Environment.NewLine
                + "Act Vert: " + CalculationSingleton.Instance.ActualCreationScope.ActualVerticalDirection + Environment.NewLine
                + "Last Trans: " + lastTransition.name + Environment.NewLine
                + "New block: " + instance.name, instance.transform.position);
			
			return instance;		
		}

		/// <summary>
		/// Placesan object on top of an other object.
		/// </summary>
		/// <returns>The on top of object.</returns>
		/// <param name="onTopOf">On top of.</param>
		/// <param name="objectToPlace">Object to place.</param>
		/// <param name="orientation">Orientation.</param>
		public GameObject PlaceOnTopOfObject(GameObject onTopOf, GameObject objectToPlace, ObjectOrientation orientation)
		{
            var onTopOfSize = HelperSingleton.Instance.GetSize(onTopOf);
            var objectToPlaceSize = HelperSingleton.Instance.GetSize(objectToPlace);
			var center = HelperSingleton.Instance.GetCenterOfGameObject(onTopOf);			
			Vector3 pos;

			if (orientation == ObjectOrientation.Center)
			{
				pos = new Vector3(center.x - (objectToPlaceSize.x / 2),
				                      onTopOf.transform.position.y + onTopOfSize.y + 0.1f,
				                      center.z - (objectToPlaceSize.z / 2));
			}
			else
			{
				pos = new Vector3(center.x - (objectToPlaceSize.x / 2),
				                  onTopOf.transform.position.y + onTopOfSize.y + 0.1f,
				                  center.z - (objectToPlaceSize.z / 2));
			}

			return PrefabSingleton.Instance.Create(objectToPlace, pos);
		}

        /// <summary>
        /// Returns the block from an area which matches the y pos.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GameObject GetBlockFromArea(IEnumerable<GameObject> area, float y)
        {
            GameObject result;

            area = area.OrderBy(bk => bk.transform.position.y);
            result = area.FirstOrDefault(bk => Math.Round(y, 1) >= Math.Round(bk.transform.position.y, 1));

            return result;
        }
    }
}