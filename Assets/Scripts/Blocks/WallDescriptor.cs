using Assets.Scripts.Enums;
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
    }
}