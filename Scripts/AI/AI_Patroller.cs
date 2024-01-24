using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.AI
{
    public class AI_Patroller : MonoBehaviour
    {
        //public List<Transform> waypoints = new List<Transform>();
        //private int currentWaypointIndex = 0;
        //public float speed = 2f;
        //public float waypointThreshold = 1f;

        //private LineRenderer lineRenderer;

        //void Start()
        //{
        //    lineRenderer = GetComponent<LineRenderer>();
        //    UpdateLineRenderer();
        //}

        //void Update()
        //{
        //    Transform targetWaypoint = waypoints[currentWaypointIndex];
        //    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        //    if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
        //    {
        //        currentWaypointIndex++;
        //        if (currentWaypointIndex >= waypoints.Count)
        //        {
        //            currentWaypointIndex = 0;
        //        }
        //    }
        //}

        //void UpdateLineRenderer()
        //{
        //    lineRenderer.positionCount = waypoints.Count;
        //    for (int i = 0; i < waypoints.Count; i++)
        //    {
        //        lineRenderer.SetPosition(i, waypoints[i].position);
        //    }
        //    lineRenderer.loop = true; // To connect the last waypoint to the first
        //}

        [SerializeField] float wayPointSize = 1f;
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), wayPointSize);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}