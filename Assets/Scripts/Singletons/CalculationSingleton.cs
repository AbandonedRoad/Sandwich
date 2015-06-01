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
        /// An instance used to determine where a object is to be placed.
        /// </summary>
        public OrienationCalc OrientationCalculation { get; set; }

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
                    _instance.OrientationCalculation = new OrienationCalc();
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
        public void GetPositionForObject(HorzOrientation horzOrientation = HorzOrientation.Center, VertOrientation vertOrientation = VertOrientation.Center)
		{
            float childShortSide = OrientationCalculation.ChildSize.x > OrientationCalculation.ChildSize.z ? OrientationCalculation.ChildSize.z : OrientationCalculation.ChildSize.x;

            // Set object
            OrientationCalculation.Child.transform.SetParent(OrientationCalculation.Wall.transform);
            OrientationCalculation.Child.transform.localPosition = new Vector3((OrientationCalculation.Wall.WallStrength + (childShortSide / 2)) * -1,
                OrientationCalculation.YPos,
                OrientationCalculation.ZPos);
            OrientationCalculation.Child.transform.localRotation = Quaternion.Euler(new Vector3(0, OrientationCalculation.Wall.transform.localRotation.y, 0));
        }

		/// <summary>
		/// Gets the position for transition.
		/// </summary>
		/// <returns>The position for transition.</returns>
		/// <param name="actualBlock">Actual block.</param>
		/// <param name="transitionToBeCreate">Transition to be create.</param>
		public GameObject GetStartForVerticalArea()
		{
            if (CalculationSingleton.Instance.ActualCreationScope.PreviouslyLevelOrientation == LevelOrientation.Horizontal)
            {
                // A Transition is not necessary here - we can use the already created block from the horizontal lane
                return CalculationSingleton.Instance.ActualCreationScope.ActualTransitionObject;
            }

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
                + "Act Level: " + CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation + Environment.NewLine
                + "Nxt Level: " + CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation + Environment.NewLine
                + "Prv Horz: " + CalculationSingleton.Instance.ActualCreationScope.PreviousHorizontalDirection + Environment.NewLine
                + "Act Horz: " + CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection + Environment.NewLine
                + "Nxt Horz: " + CalculationSingleton.Instance.ActualCreationScope.NextHorizontalDirection + Environment.NewLine
                + "Prv Vert: " + CalculationSingleton.Instance.ActualCreationScope.PreviousVerticalDirection + Environment.NewLine
                + "Act Vert: " + CalculationSingleton.Instance.ActualCreationScope.ActualVerticalDirection + Environment.NewLine
                + "Nxt Vert: " + CalculationSingleton.Instance.ActualCreationScope.NextVerticalDirection + Environment.NewLine
                + "Last Trans: " + lastTransition.name + Environment.NewLine
                + "New block: " + instance.name, instance.transform.position);

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
                instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.HBlock);
                CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();

                /*
                if (CalculationSingleton.Instance.ActualCreationScope.PreviousHorizontalDirection == CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection)
                {
                    // Direction did not change - create a regular block.
                    instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.HBlock);
                    instance.transform.rotation = CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
                }
                else
                {
                    // Direction changed - but crner
                    instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.HCorner);
                    instance.transform.rotation = CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForHorizontalCorner();
                }
                */
            }

            instance.transform.parent = PrefabSingleton.Instance.LevelParent;

            Vector3 size = HelperSingleton.Instance.GetSize(instance);
            float zValue = lastTransition.transform.position.z;
            float xValue = lastTransition.transform.position.x;

            instance.transform.position = CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForHorizontalStart();

            HelperSingleton.Instance.CreateDebugGOAtPosition(
                  "Prv Level: " + CalculationSingleton.Instance.ActualCreationScope.PreviouslyLevelOrientation + Environment.NewLine
                + "Act Level: " + CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation + Environment.NewLine
                + "Nxt Level: " + CalculationSingleton.Instance.ActualCreationScope.NextLevelOrientation + Environment.NewLine
                + "Prv Horz: " + CalculationSingleton.Instance.ActualCreationScope.PreviousHorizontalDirection + Environment.NewLine
                + "Act Horz: " + CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection + Environment.NewLine
                + "Nxt Horz: " + CalculationSingleton.Instance.ActualCreationScope.NextHorizontalDirection + Environment.NewLine
                + "Prv Vert: " + CalculationSingleton.Instance.ActualCreationScope.PreviousVerticalDirection + Environment.NewLine
                + "Act Vert: " + CalculationSingleton.Instance.ActualCreationScope.ActualVerticalDirection + Environment.NewLine
                + "Nxt Vert: " + CalculationSingleton.Instance.ActualCreationScope.NextVerticalDirection + Environment.NewLine
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
        public GameObject PlaceOnTopOfObject(GameObject onTopOf, GameObject objectToPlace, HorzOrientation orientation)
		{
            var onTopOfSize = HelperSingleton.Instance.GetSize(onTopOf);
            var objectToPlaceSize = HelperSingleton.Instance.GetSize(objectToPlace);
			var center = HelperSingleton.Instance.GetCenterOfGameObject(onTopOf);			
			Vector3 pos;

            if (orientation == HorzOrientation.Center)
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

            area = area.OrderByDescending(bk => bk.transform.position.y);
            result = area.FirstOrDefault(bk => Math.Round(y, 1) >= Math.Round(bk.transform.position.y, 1));

            return result;
        }
    }
}