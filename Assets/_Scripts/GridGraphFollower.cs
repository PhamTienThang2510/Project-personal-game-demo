using Pathfinding;
using UnityEngine;

public class GridGraphFollower : MonoBehaviour
{
    public Transform target;

    //void LateUpdate()
    //{
    //    if (AstarPath.active == null || target == null) return;

    //    var grid = AstarPath.active.data.gridGraph;
    //    if (grid == null) return;

    //    Vector3 center = target.position;
    //    center.z = 0;

    //    grid.center = center;
    //    AstarPath.active.Scan(grid);
    //}
}
