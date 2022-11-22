using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    //Stores a reference to the waypoint system this object will use
    [SerializeField] private Waypoints waypoints;

    //Temp Variable
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float distanceThreshold = 0.1f;

    bool breathOut;
    bool delayMovement;

    float timer;
    [SerializeField] float delayTime;

    //Default Values
    float breathValue;
    [SerializeField] AnimationCurve breathCurve;

    //The curent waypoint target that the object is moving towards
    private Transform currentWaypoint;

    //Reference to the Breath Propierties for anothe Scripts
    public float BreathValue { get => breathValue; set => breathValue = value; }
    public AnimationCurve BreathCurve { get => breathCurve; set => breathCurve = value; }

    // Start is called before the first frame update
    void Start()
    {
        // Set initial position to first waypoint
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;

        //Set the next waypoint target
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.LookAt(currentWaypoint);
        breathOut = false;
        delayMovement = true;
    }

    // Update is called once per frame
    void Update()
    {
        //When BreathValue Changes, the player will move to the next waypoint.
        if (breathValue != 0f && !delayMovement)
        {
            Debug.Log("[Breath Value Changed] Starting Movement");
            BreathMovement(BreathCurve.Evaluate(Mathf.Abs(BreathValue)));
        }

        if (delayMovement)
        {
            timer += Time.deltaTime;
            if(timer >= delayTime)
            {
                Debug.Log("[Delay Finished] Movement Ready");
                delayMovement = false;
                timer = 0f;
            }
        }

        //Debuging Mode > This code Only Runs in the Unity Editor
#if UNITY_EDITOR
        if (breathOut && !delayMovement)
        {
            BreathMovement(moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            breathOut = true;
#endif
    }

    public void BreathMovement(float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            transform.LookAt(currentWaypoint);
            Debug.Log("[Waypoint Check] Finish Movement - Starting Delay");
            breathValue = 0f;
            delayMovement = true;

#if UNITY_EDITOR
            breathOut = false;
#endif
        }
    }
}
