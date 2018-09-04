using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Make_Grid : MonoBehaviour {

    public bool onlyDisplayPathGizmos;
    public Transform player;
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] private float nodeRadius;
    [SerializeField] private LayerMask unwalkableMask;

    private Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    // Use this for initialization
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius));
                }
            }
        }
        else
        {
            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(player.position);
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (playerNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    if (path != null)
                    {
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius));

                }
            }
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        //Debug.Log(grid[x, y].worldPosition);
        return grid[x, y];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX  < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
}
