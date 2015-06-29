using UnityEngine;
using System.Collections;
using Enums;
using Singleton;
using Assets.Scripts.Blocks;
using System;
using System.Linq;
using Assets.Scripts.Enums;
using Random = UnityEngine.Random;
using Singletons;

namespace LevelCreation
{
    /// <summary>
    /// The actual Creation scope.
    /// </summary>
    public class CreationScope
    {
        private LevelOrientation _nextLevelOrienation;
        private LevelOrientation _actualLevelOrienation;
        private HorzDirection _nextHorzDirection;
        private HorzDirection _actualHorzDirection;
        private VertDirection _nextVertDirection = VertDirection.NotSet;
        private VertDirection _actualVertDirection = VertDirection.NotSet;
        private GameObject _actualCreatedLevelBlock;
        private GameObject _actualTransitionInfo;

        /// <summary>
        /// Level Orienation
        /// </summary>
        public LevelOrientation NextLevelOrientation
        {
            get
            {
                return _nextLevelOrienation;
            }
            set
            {
                ActualLevelOrientation = _nextLevelOrienation;
                _nextLevelOrienation = value;
            }
        }
        public LevelOrientation ActualLevelOrientation 
        { 
            get 
            {
                return _actualLevelOrienation;
            }
            private set
            {
                PreviouslyLevelOrientation = _actualLevelOrienation;
                _actualLevelOrienation = value;
            }        
        }
        public LevelOrientation PreviouslyLevelOrientation { get; private set; }

        /// <summary>
        /// Horizontal Orienation
        /// </summary>
        public HorzDirection NextHorizontalDirection
        {
            get
            {
                return _nextHorzDirection;
            }
            private set
            {
                ActualHorizontalDirection = _nextHorzDirection;
                _nextHorzDirection = value;
            }
        }
        public HorzDirection ActualHorizontalDirection
        {
            get
            {
                return _actualHorzDirection;
            }
            private set
            {
                PreviousHorizontalDirection = _actualHorzDirection;
                _actualHorzDirection = value;
            }
        }
        public HorzDirection PreviousHorizontalDirection { get; private set; }

        /// <summary>
        /// The Vertical Direction.
        /// </summary>
        public VertDirection NextVerticalDirection
        {
            get
            {
                return _nextVertDirection;
            }
            private set
            {
                ActualVerticalDirection = _nextVertDirection;
                _nextVertDirection = value;
            }
        }
        public VertDirection ActualVerticalDirection
        {
            get
            {
                return _actualVertDirection;
            }
            private set
            {
                PreviousVerticalDirection = _actualVertDirection;
                _actualVertDirection = value;
            }
        }
        public VertDirection PreviousVerticalDirection { get; private set; }

        /// <summary>
        /// The Game Actual which was created last.
        /// </summary>
        public GameObject ActualCreatedLevelBlock
        {
            get
            {
                return _actualCreatedLevelBlock;
            }
            set
            {
                PreviouslyCreatedLevelBlock = _actualCreatedLevelBlock;
                _actualCreatedLevelBlock = value;
            }
        }
        public GameObject PreviouslyCreatedLevelBlock { get; set; }

        /// <summary>
        /// The last created Transition Info
        /// </summary>
        public GameObject ActualTransitionObject
        {
            get
            {
                return _actualTransitionInfo;
            }
            set
            {
                PreviouslyTransitionObject = _actualTransitionInfo;
                _actualTransitionInfo = value;
            }
        }
        public GameObject PreviouslyTransitionObject { get; set; }

        /// <summary>
        /// The actual Area Info - the kind of blocks we use and stuff.
        /// </summary>
        public AreaInfos AreaInfos { get; set; }

        /// <summary>
        /// Indicates if the area we are creating is the last one - this is imported in order to know where text exit will be 
        /// </summary>
        public bool IsLastArea { get; set; }

        /// <summary>
        /// Creates new instance
        /// </summary>
        public CreationScope()
        {}

        /// <summary>
        /// Returns the correct rotation for the given horizonzal rotations.
        /// </summary>
        /// <returns></returns>
        public void CalculateRotationForHorizontalCorner()
        {
            WallDescriptor wallToUse = null;
            var wall1 = ActualCreatedLevelBlock.transform.GetComponentsInChildren<WallDescriptor>().Where(wl => wl.WallNumber == 0).First();
            var wall2 = ActualCreatedLevelBlock.transform.GetComponentsInChildren<WallDescriptor>().Where(wl => wl.WallNumber == 1).First();

            if (ActualHorizontalDirection == HorzDirection.Backwards)
            {
                wallToUse = NextHorizontalDirection == HorzDirection.Left
                    ? wall2
                    : wall1;
            }
            else if (ActualHorizontalDirection == HorzDirection.Right)
            {
                wallToUse = NextHorizontalDirection == HorzDirection.Backwards
                    ? wall2
                    : wall1;
            }
            else if (ActualHorizontalDirection == HorzDirection.Forward)
            {
                wallToUse = NextHorizontalDirection == HorzDirection.Right
                    ? wall2
                    : wall1;
            }
            else if (ActualHorizontalDirection == HorzDirection.Left)
            {
                wallToUse = NextHorizontalDirection == HorzDirection.Forward
                    ? wall2
                    : wall1;
            }

            wallToUse.RotateWallFacesDirection(NextHorizontalDirection);
            wallToUse.transform.parent.gameObject.name = "Act Rot: " + wallToUse.transform.parent.gameObject.transform.rotation.ToString();
        }

        /// <summary>
        /// Gets the next position for the actual Horizontal Block
        /// </summary>
        /// <returns></returns>
        public Vector3 CalculatePositionForHorizontalStart()
        {
            Vector3 result = new Vector3(0, 0, 0);

            GameObject previousInstance = this.PreviouslyCreatedLevelBlock;
            GameObject actualInstace = this.ActualCreatedLevelBlock;
            Vector3 actualSize = HelperSingleton.Instance.GetSize(actualInstace);

            switch (this.ActualHorizontalDirection)
            {
                case HorzDirection.Backwards:
                    result = new Vector3(previousInstance.transform.position.x, previousInstance.transform.position.y, previousInstance.transform.position.z - actualSize.z);
                    break;
                case HorzDirection.Forward:
                    result = new Vector3(previousInstance.transform.position.x, previousInstance.transform.position.y, previousInstance.transform.position.z + actualSize.z);
                    break;
                case HorzDirection.Left:
                    result = new Vector3(previousInstance.transform.position.x + actualSize.x, previousInstance.transform.position.y, previousInstance.transform.position.z);
                    break;
                case HorzDirection.Right:
                    result = new Vector3(previousInstance.transform.position.x - actualSize.x, previousInstance.transform.position.y, previousInstance.transform.position.z);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns correct rotatation for next block
        /// </summary>
        /// <returns></returns>
        public void CalculateRotationForNextHorizonzalBlock()
        {
            // Rotate by 90° in we are going backwards or forwards
            Vector3 rot = (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Backwards)
                ? new Vector3(0, 90, 0)
                : new Vector3(0, 0, 0);

            this.ActualCreatedLevelBlock.transform.rotation = Quaternion.Euler(rot);
        }

        /// <summary>
        /// Gets the horizontal transition which is needed.
        /// </summary>
        /// <returns></returns>
        public GameObject GetHorizontalTranstion(Vector3 position)
        {
            GameObject instance = null;

            if (IsLastArea)
            {
                // Last area - return the exit.
                instance = PrefabSingleton.Instance.Create(AreaInfos.HExit, position);

                // Rotate the Exit correct, if it was created.
                var exitWall = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Exit);
                exitWall.RotateTo(CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection);

                return instance;
            }

            if (CalculationSingleton.Instance.ActualCreationScope.ActualLevelOrientation == LevelOrientation.Horizontal)
            {
                if (CalculationSingleton.Instance.ActualCreationScope.PreviousHorizontalDirection != CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection)
                {
                    // Direction differs - create a corner.
                    instance = PrefabSingleton.Instance.Create(AreaInfos.HCorner, position);
                    CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForHorizontalCorner();
                }
                else
                {
                    // They do not differ - just create a block.
                    instance = PrefabSingleton.Instance.Create(AreaInfos.HBlock, position);
                    CalculationSingleton.Instance.ActualCreationScope.CalculateRotationForNextHorizonzalBlock();
                }              
            }
            else
            {
                instance = PrefabSingleton.Instance.Create(AreaInfos.VTransition, position);
            }

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public GameObject GetVerticalTransition(Vector3 position)
        {
            GameObject instance = null;

            if (IsLastArea)
            {
                // Last area - return the exit.
                instance = PrefabSingleton.Instance.Create(AreaInfos.VExit, position);
                return instance;
            }

            instance = ActualLevelOrientation == LevelOrientation.Vertical
                ? PrefabSingleton.Instance.Create(AreaInfos.VTransition, position)
                : PrefabSingleton.Instance.Create(AreaInfos.VFloorDoor, position);

            if (ActualLevelOrientation == LevelOrientation.Horizontal)
            {
                // Rotate if needed.
                var doorWall = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponentsInChildren<WallDescriptor>().First(dsc => dsc.Descriptor == WallDescription.Door);
                doorWall.RotateWallFacesDirection(ActualHorizontalDirection);
            }

            return instance;
        }

        /// <summary>
        /// Gets a new Horizontal Direction
        /// </summary>
        public void GetNewHorzDirection()
        {
            HorzDirection newHorzDirection = (HorzDirection)Random.Range(0, 4);

            while (newHorzDirection == HelperSingleton.Instance.GetOpposite(CalculationSingleton.Instance.ActualCreationScope.NextHorizontalDirection))
            {
                newHorzDirection = (HorzDirection)Random.Range(0, 4);
            }

            CalculationSingleton.Instance.ActualCreationScope.NextHorizontalDirection = newHorzDirection;
        }

        /// <summary>
        /// Gets a new Horizontal Direction
        /// </summary>
        public void GetNewVertDirection()
        {
            if (CalculationSingleton.Instance.ActualCreationScope.ActualTransitionObject == null)
            {
                CalculationSingleton.Instance.ActualCreationScope.NextVerticalDirection = VertDirection.Up;
            }
            else
            {
                VertDirection newVertDirection = (VertDirection)Random.Range(0, 2);

                while (newVertDirection == HelperSingleton.Instance.GetOpposite(CalculationSingleton.Instance.ActualCreationScope.NextVerticalDirection))
                {
                    newVertDirection = (VertDirection)Random.Range(0, 2);
                }

                CalculationSingleton.Instance.ActualCreationScope.NextVerticalDirection = newVertDirection;
            }
        }
    }
}