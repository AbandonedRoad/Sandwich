using Assets.Scripts.Enums;
using Enums;
using Singleton;
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
        /// Rotates a specific wall that it is facing a certain direction.
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
    }
}