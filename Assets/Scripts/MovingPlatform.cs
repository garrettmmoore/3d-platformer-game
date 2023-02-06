using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Tooltip("The path of the platform's waypoints.")]
    [SerializeField]
    private WaypointPath waypointPath;

    [Tooltip("The speed of the moving platform.")]
    [SerializeField]
    private float speed;

    /// The time that has elapsed for the moving platform.
    private float _elapsedTime;
    

    private Transform _previousWaypoint;
    private Transform _targetWaypoint;

    /// The index of the waypoint the platform is moving towards.
    private int _targetWaypointIndex;

    /// The time it takes to get to the target waypoint.
    private float _timeToWaypoint;

    private void Start()
    {
        TargetNextWaypoint();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        var elapsedPercentage = _elapsedTime / _timeToWaypoint;
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);

        if (elapsedPercentage >= 1)
        {
            TargetNextWaypoint();
        }
    }

    /// Target the next waypoints in the path.
    private void TargetNextWaypoint()
    {
        // Set previous waypoint to the waypoint of the current target index
        _previousWaypoint = waypointPath.GetWaypoint(_targetWaypointIndex);

        // Set the target index to the index of the next waypoint in the path
        _targetWaypointIndex = waypointPath.GetNextWaypointIndex(_targetWaypointIndex);

        // Set the target waypoint to the waypoint of the target Index
        _targetWaypoint = waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        // The distance between the previous and the target endpoints
        var distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / speed;
    }
}