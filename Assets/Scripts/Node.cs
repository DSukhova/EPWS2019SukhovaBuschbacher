using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node
    {
        public Vector2Int position;
        public List<Node> connected_nodes;
        public List<Node> A_Star_Neighbors;

        // A* Shortest Path Algorithmus
        public float g = 0; // movement cost
        public float h = 0; // estimated cost
        public float f = 0; // g+h
        public Node parent = null;
        public int type = 0;

        public Node(Vector2Int position, List<Node> list)
        {
            this.position = position;
            connected_nodes = list;
        }

        public Node(Vector2Int position)
        {
            this.position = position;
        }        
    }
}
