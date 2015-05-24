using UnityEngine;
using System.Collections;
using Enums;
using Singleton;
using Assets.Scripts.Blocks;
using System;
using System.Linq;
using Assets.Scripts.Enums;
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
            set
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
            set
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
        public Quaternion CalculateRotationForHorizontalCorner()
        {
            Vector3 result = new Vector3(0, 0, 0);

            if (this.PreviousHorizontalDirection == HorzDirection.Left)
            {
                result = this.ActualHorizontalDirection == HorzDirection.Forward ? new Vector3(0, 270, 0) : result;
            }

            return Quaternion.Euler(result);
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
        public Quaternion CalculateRotationForNextHorizonzalBlock()
        {
            Vector3 result = Vector3.back;
            GameObject previousBlock = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock;
            GameObject actualBlock = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock;

            WallDescriptor prevWallDesc = previousBlock.GetComponentsInChildren<WallDescriptor>()
                .First(dsc => dsc.Descriptor == WallDescription.Door || dsc.Descriptor == WallDescription.Nothing);
            WallDescriptor actWallDesc = actualBlock.GetComponentsInChildren<WallDescriptor>()
                .First(dsc => dsc.Descriptor == WallDescription.Door || dsc.Descriptor == WallDescription.Nothing);

            int difference = Math.Abs(prevWallDesc.WallNumber - actWallDesc.WallNumber);

            if ((actWallDesc.Descriptor == WallDescription.Door && prevWallDesc.Descriptor == WallDescription.Door)
                || (actWallDesc.Descriptor == WallDescription.Nothing && prevWallDesc.Descriptor == WallDescription.Door)
                || (actWallDesc.Descriptor == WallDescription.Door && prevWallDesc.Descriptor == WallDescription.Nothing))
            {
                // Match both walls together
                // If the wall is the same, we have just to turn the next block by 180°
                if (difference == 0) result = new Vector3(0, 180, 0);
                // If it's the next, turn ny 90°
                else if (difference == 1) result = new Vector3(0, 90, 0);
                // If it's two blocks away, do not turn- it fits.
                else if (difference == 2) result = new Vector3(0, 0, 0);
                // If it's three blocks away, trun by 270°
                else if (difference == 3) result = new Vector3(0, 270, 0);
            }

            return Quaternion.Euler(result);
        }

        /// <summary>
        /// Gets the horizontal transition which is needed.
        /// </summary>
        /// <returns></returns>
        public GameObject GetHorizontalTranstion(Vector3 position, Vector3? rotation)
        {
            GameObject instance = null;

            if (IsLastArea)
            {
                instance = PrefabSingleton.Instance.Create(AreaInfos.HExit, position, rotation);
            }
            else
            {
                instance = PrefabSingleton.Instance.Create(AreaInfos.HTransition, position, rotation);
            }

            return instance;
        }
    }
}