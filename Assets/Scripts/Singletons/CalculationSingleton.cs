using System;
using System.Linq;
using UnityEngine;
using Enums;
using LevelCreation;
using Blocks;

namespace Singleton
{
	public class CalculationSingleton
	{
		private static CalculationSingleton _instance;

        /// <summary>
        /// Distance how high te player may jump
        /// </summary>
		public float JumpDistance = 2.25f;

        /// <summary>
        /// Size of one block
        /// </summary>
        public float BlockSize = 8f;

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

            var wallDoors = HelperSingleton.Instance.GetAllWallsOfType(lastTransition, WallDescription.Door);

            GameObject instance;
            if (CalculationSingleton.Instance.ActualCreationScope.PreviouslyLevelOrientation == LevelOrientation.Vertical)
            {
                instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.VFloorDoor);
                
                // Rotate last vertical transition into correct rotation.
                var doorWall = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().FirstOrDefault(dsc => dsc.Descriptor == WallDescription.Door);
                if (doorWall == null)
                {
                    doorWall = null;
                }
                doorWall.RotateTo(HorzDirection.Backwards);

                // Rotate to actual be created block into correct rotation.
                doorWall = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Door);
                doorWall.RotateTo(HorzDirection.Forward);
            }
            else
            {
                instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.GetHBlock());
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
                  "Prv Level: " + Instance.ActualCreationScope.PreviouslyLevelOrientation + Environment.NewLine
                + "Act Level: " + Instance.ActualCreationScope.ActualLevelOrientation + Environment.NewLine
                + "Nxt Level: " + Instance.ActualCreationScope.NextLevelOrientation + Environment.NewLine
                + "Prv Horz: " + Instance.ActualCreationScope.PreviousHorizontalDirection + Environment.NewLine
                + "Act Horz: " + Instance.ActualCreationScope.ActualHorizontalDirection + Environment.NewLine
                + "Nxt Horz: " + Instance.ActualCreationScope.NextHorizontalDirection + Environment.NewLine
                + "Prv Vert: " + Instance.ActualCreationScope.PreviousVerticalDirection + Environment.NewLine
                + "Act Vert: " + Instance.ActualCreationScope.ActualVerticalDirection + Environment.NewLine
                + "Nxt Vert: " + Instance.ActualCreationScope.NextVerticalDirection + Environment.NewLine
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
                var doorWall = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().FirstOrDefault(dsc => dsc.Descriptor == WallDescription.Door);
                if (doorWall == null)
                {
                    doorWall = null;
                }
                doorWall.RotateTo(CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection);

                // Rotate to actual be created block into correct rotation.
                doorWall = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Door);
                doorWall.RotateTo(HelperSingleton.Instance.GetOpposite(CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection));
            }
            else
            {
                instance = PrefabSingleton.Instance.Create(ActualCreationScope.AreaInfos.GetHBlock());
                CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
            }

            instance.transform.parent = PrefabSingleton.Instance.LevelParent;
            instance.transform.position = CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForNextHorizontal();
            HelperSingleton.Instance.AdaptPositonForExit();

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
            var onTopOfSize = HelperSingleton.Instance.GetSize(onTopOf, false);
            var objectToPlaceSize = HelperSingleton.Instance.GetSize(objectToPlace, false);
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
        /// Re - Assigns all Predecessor and Succeors for the whole Level.
        /// </summary>
        public void ReAssignAllPredecessorsSuccessors()
        {
            for (int i = 0; i < ActualCreationScope.ActualLevel.Count; i++)
            {
                // Get Blocks.
                var prvOne = i > 0
                    ? ActualCreationScope.ActualLevel[i - 1]
                    : null;
                var block = this.ActualCreationScope.ActualLevel[i];
                var nextOne = i < ActualCreationScope.ActualLevel.Count - 1
                    ? ActualCreationScope.ActualLevel[i + 1]
                    : null;

                // Assign
                block.CollisionInfo.Predecessor = prvOne != null ? prvOne.LevelBlock : null;
                block.CollisionInfo.Successor = nextOne != null ? nextOne.LevelBlock : null;
            }
        }
    }
}