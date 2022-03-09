using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RobotAgent : Agent
{
    public GameObject robot;
    public GameObject gripper;
    public GameObject target;

    private RobotController _pRobotControl;
    private GripperController _pGripperControl;

    // Initialize is called for ready to episode
    public override void Initialize()
    {
        base.Initialize();
        _pRobotControl = robot.GetComponent<RobotController>();
        _pGripperControl = robot.GetComponent<GripperController>();
    }

    // OnEpisodeBegin is called before the episode start
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        
    }

    // CollectObservations is collected the information for the policy update
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    // OnActionReceived does an action received from the policy
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
    }

    // Heuristic is called for direct action
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(in actionsOut);
    }
}
