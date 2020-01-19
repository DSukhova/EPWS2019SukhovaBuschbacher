using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Edge
    {
        public Node start;
        public Node end;
        public Vector2 vector;

        public Edge(Node start, Node end)
        {
            this.start = start;
            this.end = end;
            vector = new Vector2Int(end.position.x - start.position.x, end.position.y - start.position.y);
        }

        public Edge Reversed()
        {
            Node temp;
            temp = start;
            start = end;
            end = temp;
            return this;
        }

        public bool IsEqualTo(Edge edge)
        {
            if (this.start == edge.start && this.end == edge.end || this.end == edge.start && this.start == edge.end)
            {
                return true;
            }

            return false;
        }
    }
}
