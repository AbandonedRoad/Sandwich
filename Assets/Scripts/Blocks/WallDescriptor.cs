using Assets.Scripts.Enums;
using Enums;
using Singleton;
using Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Blocks
{
    public class WallDescriptor : MonoBehaviour
    {
        public WallDescription Descriptor;
        
        public float WallStrength = 0.19f;
        
        public int WallNumber
        {
            get 
            { 
                int number = Convert.ToInt32(this.gameObject.name.Substring(4, 1));
                return (number - 1); 
            }
        }

        /// <summary>
        /// The Length of one Wall
        /// </summary>
        public float OuterWallSize = 8f;

        /// <summary>
        /// The Length of one Wall within the block
        /// </summary>
        public float InnerWallSize
        {
            get { return OuterWallSize - (2 * WallStrength); }
        }

        /// <summary>
        /// Rotates a specific wall that it is facing a certain direction.
        /// This means, if a raycast would be done between the wall and the target, it would NEVER cross the floor
        /// </summary>
        /// <param name="rotateTo"></param>
        public void RotateTo(HorzDirection rotateTo)
        {
            switch (rotateTo)
            {
                case HorzDirection.Left:
                    if (WallNumber == 0) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    if (WallNumber == 1) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    if (WallNumber == 2) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    if (WallNumber == 3) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case HorzDirection.Right:
                    if (WallNumber == 0) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    if (WallNumber == 1) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    if (WallNumber == 2) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    if (WallNumber == 3) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    break;
                case HorzDirection.Forward:
                    if (WallNumber == 0) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    if (WallNumber == 1) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    if (WallNumber == 2) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    if (WallNumber == 3) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    break;
                case HorzDirection.Backwards:
                    if (WallNumber == 0) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    if (WallNumber == 1) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    if (WallNumber == 2) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    if (WallNumber == 3) this.gameObject.transform.parent.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// If this is called, the Wall is rotated that if faces a certain direction, but on the long side
        /// This means, if a raycast would be done between the wall and the target, it would always cross the floor
        /// </summary>
        /// <param name="rotateTo"></param>
        public void RotateWallFacesDirection(HorzDirection rotateTo)
        {
            RotateTo(HelperSingleton.Instance.GetOpposite(rotateTo));
        }
    }
}