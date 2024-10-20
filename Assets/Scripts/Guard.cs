using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;
    public float speed;
    public float timeBetweenWaypoints;
    public Light flashlight;
    public float viewDistance;
    public LayerMask viewMask;
    public float secondToBeCaught = 1f;
    float secondsSeen;
    float viewAngle;
    Transform player;
    Color originalFlashlightColor;
    float greenFactor = 1f;
    static public System.Action OnPlayerSpotted;


    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition,startPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position,transform.forward * viewDistance);
        //Gizmos.color = Color.blue;
        //for (int i = 0; i <= viewAngle; i++)
        //{
        //    float drawAngle = i - (viewAngle / 2);
        //    Vector3 rayDirection = Quaternion.Euler(Vector3.up * drawAngle) * transform.forward;
        //    Gizmos.DrawRay(transform.position, rayDirection * viewDistance);
        //}
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalFlashlightColor = flashlight.color;
        viewAngle = flashlight.spotAngle;
        Vector3[] waypointPositions = new Vector3[pathHolder.childCount];
        for (int i = 0; i < pathHolder.childCount; i++)
        {
            waypointPositions[i] = new Vector3(pathHolder.GetChild(i).position.x,transform.position.y,pathHolder.GetChild(i).position.z);
        }
        transform.position = waypointPositions[0];
        Vector3 initialDirection = (waypointPositions[1] - waypointPositions[0]).normalized;
        transform.rotation = Quaternion.LookRotation(initialDirection);
        StartCoroutine(RoamPath(waypointPositions));
    }

    void Update()
    {
        //DetectPlayer();
        if (IsPlayerDetected())
        {
            secondsSeen += Time.deltaTime/secondToBeCaught;
        }
        else
        {
            secondsSeen -= Time.deltaTime/secondToBeCaught;
        }
        // Abstracted the below lines in the FlashlightColor method.
       //secondsSeen = Mathf.Clamp(secondsSeen, 0f, 1f);
        //greenFactor =  Mathf.Lerp(1f, 0f, secondsSeen);
        //Color currentColor = new Color(1f,greenFactor,0f,1f);
        flashlight.color = FlashlightColor();
        if (secondsSeen == 1f)
        {
            if (OnPlayerSpotted != null)
            {
                OnPlayerSpotted();
            }
        }
    }
    IEnumerator RoamPath(Vector3[] waypoints)
    {
        int targetWaypointIndex = 0;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        while (true)
        {

           transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed* Time.deltaTime);
            if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(timeBetweenWaypoints);
            }
            Vector3 targetDirection = (targetWaypoint - transform.position).normalized;
            float targetAngle  = Mathf.Atan2(targetDirection.x,targetDirection.z) * Mathf.Rad2Deg;
            while (transform.forward != targetDirection)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(Vector3.up *targetAngle), 100 * Time.deltaTime*speed);
                yield return null;
            }
            yield return null;
        }
    }
    bool IsPlayerDetected()
    {
        if (Vector3.Distance(transform.position, player.position) <= viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward,dirToPlayer);
            if (angleToPlayer <= viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    Color FlashlightColor()
    {
        secondsSeen = Mathf.Clamp(secondsSeen, 0f, 1f);
        greenFactor = Mathf.Lerp(1f, 0f, secondsSeen);
        Color currentColor = new Color(1f, greenFactor, 0f, 1f);
        return currentColor;
    }

        // This implamentation works but its alot of rays
        //void DetectPlayer()
        //{
            
        //    for (int i = 0; i <= viewAngle; i++)
        //    {
        //        float rayAngle = i - (viewAngle / 2);
        //        Vector3 rayDirection = Quaternion.Euler(rayAngle*Vector3.up) * transform.forward;
        //        if (Physics.Raycast(transform.position,rayDirection,out RaycastHit hitInfo, viewDistance))
        //        {
        //            if (hitInfo.collider.CompareTag("Player"))
        //            {
        //                print("Player spotted");
        //                break;
        //            }
        //        }
        //    }
        //}
}
