using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Graph
    {
        public List<Edge> edges;
        public List<Node> nodes;
        
        public float a_min;
        public float a_max;
        public float max_Alpha;

        public Graph(List<Edge> edges)
        {
            this.edges = edges;
        }

        public Graph(){
        }

        public void Set_Alpha()
        {
            float temp_Angle;
            float min_Angle = Mathf.Infinity;
            float max_Angle = Mathf.NegativeInfinity;

            foreach (Edge e_1 in edges)
            {
                foreach (Edge e_2 in edges)
                {
                    if (e_1.start.position == e_2.start.position && !e_2.IsEqualTo(e_1))
                    {
                        temp_Angle = Vector2.Angle(e_1.vector, e_2.vector);
                        
                        if (temp_Angle < min_Angle)
                        {
                            min_Angle = temp_Angle;
                        }

                        if (temp_Angle > max_Angle)
                        {
                            max_Angle = temp_Angle;
                        }
                        
                    } else if (e_1.end.position == e_2.end.position && !e_1.IsEqualTo(e_2))
                    {
                        temp_Angle = Vector2.Angle(e_1.vector, e_2.vector);
                        
                        if (temp_Angle < min_Angle)
                        {
                            min_Angle = temp_Angle;
                        }

                        if (temp_Angle > max_Angle)
                        {
                            max_Angle = temp_Angle;
                        }
                    }
                }
            }

            a_max = max_Angle;
            a_min = min_Angle;
        }

        public void Set_Max_Alpha()
        {
            float Alpha = (a_max - a_min) / 2;

            while (false)
            {
                
            }
        }

        public void Generate_Paths()
        {
            // generiert aus Nav_Nodes Pfade:
            // Kegelbasierter Algorithmus nach Kneidl

            Graph output = new Graph();
            List<Node> node_list = new List<Node>(nodes);
            List<Edge> edge_list = new List<Edge>();
            List<Triangle> cut_cones_list = new List<Triangle>();

            Triangle Inf_Triangle = new Triangle(new Vector2(Mathf.Infinity, Mathf.Infinity), new Vector2(Mathf.Infinity, Mathf.Infinity), new Vector2(Mathf.Infinity, Mathf.Infinity));

            // Suchbereich

            Vector2 p1;
            Vector2 p2;
            Vector2 p3;
            float distance;
            float alpha = (a_max - a_min) / 2;
            float direction_angle;

            foreach (Node n in node_list)
            {
                cut_cones_list.Clear();                
                
                // Liste aller sichtbaren Knoten im Suchbereich
                // Ordnen der Liste

                n.connected_nodes = Get_Search_Area_And_VC_Nodes(n, node_list, Inf_Triangle);             
                n.connected_nodes = n.connected_nodes.OrderBy(o => Vector2.Distance(o.position, n.position)).ToList();                               

                foreach (Node neighbor in n.connected_nodes)
                {                    
                    if (!cut_cones_list.Exists(o => o.IsNodeInTriangle(neighbor)))
                    {                        
                        // Kante hinzufügen

                        if (!edge_list.Exists(o => o.IsEqualTo(new Edge(n, neighbor))))
                        {
                            edge_list.Add(new Edge(n, neighbor));
                        }

                        // Kegelförmigen Bereich ausschneiden

                        p1 = n.position;

                        distance = 100;

                        direction_angle = Mathf.Atan2(neighbor.position.y - n.position.y, neighbor.position.x - n.position.x);
                        
                        p2 = new Vector2((n.position.x + Mathf.Cos(direction_angle + alpha/2) * distance), (n.position.y + Mathf.Sin(direction_angle + alpha/2) * distance));
                        p3 = new Vector2((n.position.x + Mathf.Cos(direction_angle - alpha/2) * distance), (n.position.y + Mathf.Sin(direction_angle - alpha/2) * distance));

                        Triangle cut_cone = new Triangle(p1,p2,p3);
                        cut_cones_list.Add(cut_cone);
                    }
                }
            }

            edges = edge_list;
        }

        List<Node> Get_VC_of_Node(Node node, List<Node> nodes)
        {
            RaycastHit2D hit;
            List<Node> list = new List<Node>();

            foreach (Node p in nodes)
            {
                if (node != p)
                {
                    hit = Physics2D.Raycast(node.position, new Vector3(p.position.x - node.position.x, p.position.y - node.position.y, 0), Vector2.Distance(node.position, p.position));

                    if (hit.collider != null)
                    {
                        continue;
                    }

                    list.Add(p);
                }
            }
            return list;
        }

        List<Node> Get_Search_Area_And_VC_Nodes(Node node, List<Node> nodes, Triangle t)
        {
            RaycastHit2D hit;
            List<Node> list = new List<Node>();

            foreach (Node p in nodes)
            {
                if (node != p)
                {
                    hit = Physics2D.Raycast(node.position, new Vector3(p.position.x - node.position.x, p.position.y - node.position.y, 0), Vector2.Distance(node.position, p.position));

                    if (hit.collider != null || !t.IsNodeInTriangle(node))
                    {                        
                        continue;
                    }

                    list.Add(p);
                }
            }
            return list;
        }

        List<Node> Get_Search_Area_And_VC_Nodes(Node node, List<Node> nodes)
        {
            RaycastHit2D hit;
            List<Node> list = new List<Node>();

            foreach (Node p in nodes)
            {
                if (node != p)
                {
                    hit = Physics2D.Raycast(node.position, new Vector3(p.position.x - node.position.x, p.position.y - node.position.y, 0), Vector2.Distance(node.position, p.position));

                    if (hit.collider != null)
                    {
                        continue;
                    }

                    list.Add(p);
                }
            }
            return list;
        }

        public void Generate_Graph(List<Node> nodes)
        {
            List<Node> node_list = new List<Node>(nodes);
            List<Edge> edge_list = new List<Edge>();

            foreach (Node n in nodes)
            {
                n.connected_nodes = Get_VC_of_Node(n, nodes).OrderBy(o => Vector2Int.Distance(n.position, o.position)).ToList();

                foreach (Node nvc in n.connected_nodes)
                {
                    Edge edge = new Edge(n, nvc);

                    if (!edge_list.Exists(e => e.IsEqualTo(edge)))
                    {
                        edge_list.Add(edge);
                    }
                }
            }

            this.nodes = node_list;
            this.edges = edge_list;
        }
    }
}
