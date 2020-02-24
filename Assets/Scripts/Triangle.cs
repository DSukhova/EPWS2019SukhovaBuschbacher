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

        float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        public bool PointInTriangle(Node pt)
        {
            // Prüft ob sich ein Punkt in einem Dreieck befindet
            // Relevant für die Minimierung von Knotenverbindungen im Graphen
            // Quelle: https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle

            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt.position, p0, p1);
            d2 = Sign(pt.position, p1, p2);
            d3 = Sign(pt.position, p2, p0);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
    }
}
