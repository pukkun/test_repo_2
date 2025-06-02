using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    public static float TILE = 1;
    public static float SCALE_TILE = 1f;
    public static float SPACE = 0;
    public static float GetX(int col, int maxWidth)
    {
        float x = (col - (maxWidth - 1) / 2f) * (TILE * SCALE_TILE + SPACE);
        return x;
    }

    public static float GetY(int row, int maxHeight)
    {
        float y = (row - (maxHeight - 1) / 2f) * (TILE * SCALE_TILE + SPACE);
        return y;
    }
    public class PathNode
    {
        public Vector2Int position;
        public PathNode parent;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

        public PathNode(Vector2Int pos)
        {
            position = pos;
        }
    }
    public static (bool, List<Vector2Int>) FindSuccessPath(GridManager gridManager, Snake snake, PortalTarget portalTarget, bool isGhosting)
    {
        Vector2Int start = snake.GetHeadCoor();
        Vector2Int target = portalTarget.PortalData.Coord[0];
        int height = gridManager.Grids.Length;
        int width = gridManager.Grids[0].Widths.Length;
        if (start == target) return (false, new List<Vector2Int> { start });

        PathNode startNode = new PathNode(start);
        PathNode targetNode = new PathNode(target);

        List<PathNode> openSet = new List<PathNode> { startNode };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.position);
            if (currentNode.position == target)
            {
                return RetracePath(currentNode);
            }
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int newX = currentNode.position.x + directions[i, 0];
                int newY = currentNode.position.y + directions[i, 1];
                Vector2Int neighborPos = new Vector2Int(newX, newY);
                CellData cellData = gridManager.Get(newX, newY);
                if (cellData == null)
                {
                    continue;
                }
                //if (newX == 3 && newY == 4)
                //{
                //    Debug.LogError("");
                //}
                //if (newX == 3 && newY == 5)
                //{
                //    Debug.LogError("");
                //}
                Enums.GridValue valueGrid = cellData.GridValue;
                bool isTruePortal = (valueGrid == Enums.GridValue.Portal && portalTarget.TruePortal(cellData.BlockColor) && portalTarget.TruePortal(snake.SnakeData.SelectedColor));
                if (isTruePortal)
                {
                    if (target != new Vector2Int(newX, newY)) isTruePortal = false;
                }
                bool isTrueCell = valueGrid == Enums.GridValue.None || isTruePortal;
                if (valueGrid == Enums.GridValue.Available && isGhosting && target != new Vector2Int(newX, newY)) isTrueCell = true;
                if (newX >= 0 && newX < width && newY >= 0 && newY < height && isTrueCell && !closedSet.Contains(neighborPos))
                {
                    PathNode neighborNode = new PathNode(neighborPos);
                    int newGCost = currentNode.gCost + 1; // Giả sử chi phí di chuyển giữa các ô là 1
                    if (newGCost < neighborNode.gCost || !openSet.Contains(neighborNode))
                    {
                        neighborNode.gCost = newGCost;
                        neighborNode.hCost = CalculateHeuristic(neighborPos, target);
                        neighborNode.parent = currentNode;

                        if (!openSet.Contains(neighborNode))
                        {
                            openSet.Add(neighborNode);
                        }
                    }
                }
            }
        }

        return (false, null); // Không tìm thấy đường đi
    }


    public static (bool, List<Vector2Int>) FindPathInorgeSnake(GridManager gridManager, List<Vector2Int> listCoord, Vector2Int target)
    {
        int height = gridManager.Grids.Length;
        int width = gridManager.Grids[0].Widths.Length;
        if (listCoord[0] == target) return (false, new List<Vector2Int> { listCoord[0] });

        PathNode startNode = new PathNode(listCoord[0]);
        PathNode targetNode = new PathNode(target);

        List<PathNode> openSet = new List<PathNode> { startNode };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.position);
            if (currentNode.position == target)
            {
                return RetracePath(currentNode);
            }
            int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int newX = currentNode.position.x + directions[i, 0];
                int newY = currentNode.position.y + directions[i, 1];
                Vector2Int neighborPos = new Vector2Int(newX, newY);

                Enums.GridValue valueGrid = gridManager.GetGridValue(newX, newY);

                bool isTrueCell = valueGrid == Enums.GridValue.None || valueGrid == Enums.GridValue.Available || valueGrid == Enums.GridValue.Portal;
                if (!checkExist(listCoord, newX, newY) && newX >= 0 && newX < width && newY >= 0 && newY < height && isTrueCell && !closedSet.Contains(neighborPos))
                {
                    PathNode neighborNode = new PathNode(neighborPos);
                    int newGCost = currentNode.gCost + 1; // Giả sử chi phí di chuyển giữa các ô là 1
                    if (newGCost < neighborNode.gCost || !openSet.Contains(neighborNode))
                    {
                        neighborNode.gCost = newGCost;
                        neighborNode.hCost = CalculateHeuristic(neighborPos, target);
                        neighborNode.parent = currentNode;

                        if (!openSet.Contains(neighborNode))
                        {
                            openSet.Add(neighborNode);
                        }
                    }
                }
            }
        }

        return (false, null); // Không tìm thấy đường đi
    }
    private static bool checkExist(List<Vector2Int> list, int x, int y)
    {
        foreach (var item in list)
        {
            if (item.x == x && item.y == y) return true;
        }
        return false;
    }
    static int CalculateHeuristic(Vector2Int a, Vector2Int b)
    {
        // Sử dụng Manhattan distance làm heuristic (ước tính khoảng cách theo chiều ngang + chiều dọc)
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    static (bool, List<Vector2Int>) RetracePath(PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return (true, path);
    }
}
