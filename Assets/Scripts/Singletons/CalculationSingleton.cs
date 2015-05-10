using System;
using System.Linq;
using UnityEngine;
using Enums;
using Random = UnityEngine.Random;
using Blocks;
using LevelCreation;
using Singletons;

namespace Singleton
{
	public class CalculationSingleton
	{
		private static CalculationSingleton _instance;

		public float JumpDistance = 2.25f;

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
				}
				
				return _instance;
			}
		}

		/// <summary>
		/// Gets the size.
		/// </summary>
		/// <returns>The size.</returns>
		/// <param name="objectToCheck">Object to check.</param>
		public Vector3 GetSize(GameObject objectToCheck)
		{
			Renderer renderer = objectToCheck.tag == "RenderObject" 
				? objectToCheck.GetComponent<Renderer>() 
				: objectToCheck.GetComponentsInChildren<Renderer>(true).ToList().FirstOrDefault(rend => rend.gameObject.tag == "RenderObject");
			if (renderer == null) 
			{
				Debug.LogError("No RenderObject found for: " + objectToCheck.name);
			}

			return renderer != null 
				? renderer.bounds.size
				: Vector3.one;
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
			var blockInfo = XMLSingleton.Instance.BlockInfos[parentObject.name];

			float middleXObject = (parentSize.x - (2*blockInfo.WallSize)) / 2;
			float childLongSide = childSize.x > childSize.z ? childSize.x : childSize.z;
			float childShortSide = childSize.x > childSize.z ? childSize.z : childSize.x;
			float middleZObject = (parentSize.z - (2*blockInfo.WallSize)) / 2;
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
		public GameObject GetStartForVerticalTransition(GameObject lastTransition, AreaInfos areaInfo, LevelOrientation fromOrientation)
		{
			var parent = GameObject.Find("_Levels").transform;

			if (lastTransition == null)
			{
				// We have no transition - this is the first area
				return null;
			}

			var infos = XMLSingleton.Instance.BlockInfos[lastTransition.name];
			var instance = fromOrientation == LevelOrientation.Vertical 
				? PrefabSingleton.Instance.Create(areaInfo.VFloorDoor, null)
				: PrefabSingleton.Instance.Create(areaInfo.HBlock, null);
			instance.transform.parent = parent;

			Debug.Log("From: " + fromOrientation + " To: Vertical   Last Trans: " + lastTransition.name + "   New block: " + instance.name);

			float xValue = lastTransition.transform.position.x;
			Vector3 size = GetSize(instance);
			if (infos.DoorWall == 0 || infos.DoorWall == 2)
			{
				xValue += infos.DoorWall == 0 ? size.x : size.x * -1;
			}
			else
			{
				xValue = lastTransition.transform.position.x;
			}

			float zValue = lastTransition.transform.position.z;
			if (infos.DoorWall == 1 || infos.DoorWall == 3)
			{
				zValue += infos.DoorWall ==  1 ? size.z : size.z * -1;
			}
			else
			{
				zValue = lastTransition.transform.position.z;
			}
			
			instance.transform.position = new Vector3(xValue, lastTransition.transform.position.y, zValue);
			instance.transform.rotation = Quaternion.Euler(0, 180, 0);

			return instance;		
		}
	
		/// <summary>
		/// Gets the position for transition.
		/// </summary>
		/// <returns>The position for transition.</returns>
		/// <param name="actualBlock">Actual block.</param>
		/// <param name="transitionToBeCreate">Transition to be create.</param>
		public GameObject GetStartForHorintzalTransition(GameObject lastTransition, AreaInfos areaInfo, LevelOrientation fromOrientation)
		{
			var parent = GameObject.Find("_Levels").transform;
			
			if (lastTransition == null)
			{
				// We have no transition - this is the first area
				return null;
			}
			
			var infos = XMLSingleton.Instance.BlockInfos[lastTransition.name];
			var instance = fromOrientation == LevelOrientation.Vertical 
				? PrefabSingleton.Instance.Create(areaInfo.HTransition, null)
				: PrefabSingleton.Instance.Create(areaInfo.HBlock, null);
			instance.transform.parent = parent;
			
			Debug.Log("From: " + fromOrientation + " To: Horinzontal   Last Trans: " + lastTransition.name + "   New block: " + instance.name);
			
			float xValue = lastTransition.transform.position.x;
			Vector3 size = GetSize(instance);
			if (infos.DoorWall == 0 || infos.DoorWall == 2)
			{
				xValue += infos.DoorWall == 0 ? size.x : size.x * -1;
			}
			else
			{
				xValue = lastTransition.transform.position.x;
			}
			
			float zValue = lastTransition.transform.position.z;
			if (infos.DoorWall == 1 || infos.DoorWall == 3)
			{
				zValue += infos.DoorWall ==  1 ? size.z : size.z * -1;
			}
			else
			{
				zValue = lastTransition.transform.position.z;
			}
			
			instance.transform.position = new Vector3(xValue, lastTransition.transform.position.y, zValue);
			instance.transform.rotation = Quaternion.Euler(0, 180, 0);
			
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
			var onTopOfSize = CalculationSingleton.Instance.GetSize(onTopOf);
			var objectToPlaceSize = CalculationSingleton.Instance.GetSize(objectToPlace);
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
	}
}