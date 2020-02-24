using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Unit
    {
        public Unit(Vector2Int pos)
        {
            position = pos;
        }

        public Unit parent;
        public Vector2Int position;
        public float f = 0;
        public float g = 0;
        public float h = 0;
        
    }
}
