using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    /// Get the appropriate Waypoint using the provided index.
    /// <param name="waypointIndex"> Index of the waypoint to find. </param>
    /// <returns> The transform position of the waypoint. </returns>
    public Transform GetWaypoint(int waypointIndex)
    {
        // The waypoint parent has 3 children waypoints, get the child that has the current index
        return transform.GetChild(waypointIndex);
    }

    /// Get the index of the next waypoint in the path.
    /// <param name="currentWaypointIndex"> Index of the current waypoint in the path. </param>
    /// <returns> The index of the next waypoint in the path. </returns>
    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        var nextWaypointIndex = currentWaypointIndex + 1;

        // If we've reached the end of the path, loop back around to the start
        if (nextWaypointIndex == transform.childCount)
        {
            nextWaypointIndex = 0;
        }

        return nextWaypointIndex;
    }
}