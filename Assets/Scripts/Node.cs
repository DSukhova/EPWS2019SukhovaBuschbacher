using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node
    {
        public Vector2Int position;
        public List<Node> connected_nodes;

        public Node(Vector2Int position, List<Node> list)
        {
            this.position = position;
            connected_nodes = list;
        }

        public Node(Vector2Int position)
        {
            this.position = position;
        }

        public void Cout()
        {
            Debug.Log(position);
        }
    }
}
