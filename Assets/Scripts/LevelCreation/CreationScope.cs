using UnityEngine;
using System.Collections;
using Enums;
using Singleton;

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
        public GameObject PreviouslyCreatedLevelBlock { get; set; }

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
    }
}