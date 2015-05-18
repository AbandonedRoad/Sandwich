using UnityEngine;
using System.Collections;
using Enums;
using Singleton;
using Assets.Scripts.Blocks;
using System;
using System.Linq;
using Assets.Scripts.Enums;

namespace LevelCreation
{
    /// <summary>
    /// The actual Creation scope.
    /// </summary>
    public class CreationScope
    {
        private LevelOrientation _actualLevelOrienation;
        private HorzDirection _actualHorzDirection;
        private VertDirection _actualVertDirection;
        private GameObject _actualCreatedLevelBlock;
        private TransitionInfo _actualTransitionInfo;

        /// <summary>
        /// The Actual Level Orientation.
        /// </summary>
        public LevelOrientation ActualLevelOrientation 
        { 
            get 
            {
                return _actualLevelOrienation;
            }
            set
            {
                PreviouslyLevelOrientation = _actualLevelOrienation;
                _actualLevelOrienation = value;
            }        
        }
        public LevelOrientation PreviouslyLevelOrientation { get; private set; }

        /// <summary>
        /// The Actual Horizontzal Direction.
        /// </summary>
        public HorzDirection ActualHorizontalDirection
        {
            get
            {
                return _actualHorzDirection;
            }
            set
            {
                PreviousHorizontalDirection = _actualHorzDirection;
                _actualHorzDirection = value;
            }
        }
        public HorzDirection PreviousHorizontalDirection { get; private set; }

        /// <summary>
        /// The Actual Vertical Direction.
        /// </summary>
        public VertDirection ActualVerticalDirection
        {
            get
            {
                return _actualVertDirection;
            }
            set
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
        public TransitionInfo ActualTransitionInfo
        {
            get
            {
                return _actualTransitionInfo;
            }
            set
            {
                PreviouslyTransitionInfo = _actualTransitionInfo;
                _actualTransitionInfo = value;
            }
        }
        public TransitionInfo PreviouslyTransitionInfo { get; set; }

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

        public Vector3 CalculatePositionForHorizontalPosition()
        {
            Vector3 result = new Vector3(0, 0, 0);

            return result;
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        public CreationScope()
        {
            ActualTransitionInfo = new TransitionInfo();
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
    }
}