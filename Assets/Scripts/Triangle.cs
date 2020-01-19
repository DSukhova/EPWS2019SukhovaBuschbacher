using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Triangle
    {
        public Vector2 p0;
        public Vector2 p1;
        public Vector2 p2;

        public Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
        }

        public bool IsNodeInTriangle(Node node)
        {
            if (p0.x == Mathf.Infinity || p0.y == Mathf.Infinity || p1.x == Mathf.Infinity || p1.y == Mathf.Infinity || p2.x == Mathf.Infinity || p2.y == Mathf.Infinity)
            {
                return true;
            }

            //http://jsfiddle.net/PerroAZUL/zdaY8/1/

            float A = 1 / 2 * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
            int sign = A < 0 ? -1 : 1;
            float s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * node.position.x + (p0.x - p2.x) * node.position.y) * sign;
            float t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * node.position.x + (p1.x - p0.x) * node.position.y) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * A * sign;
        }
    }
}
