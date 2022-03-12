using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    private bool _bRunScript = false;
    
    private IEnumerator InitScript()
    {
        JointPoint pInitPos = JointPoint.FromPosition("Init", 135.0F, 0.0F, 90.0F, 0.0F, 90.0F, 0.0F, 0.0F);
        pInitPos.FrameCount = 10;
        yield return _pRobotControl.Move(pInitPos);
    }

    // Initialize is called for ready to episode
    public override void Initialize()
    {
        base.Initialize();
        _pRobotControl = robot.GetComponent<RobotController>();
        _pGripperControl = robot.GetComponent<GripperController>();
        _pRobotControl.ControlMode = OperationMode.Auto;
        _pGripperControl.ControlMode = OperationMode.Auto;
    }

    // OnEpisodeBegin is called before the episode start
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        Debug.Log("[AGENT] Episode Start");
        StartCoroutine(InitScript());
    }

    // CollectObservations is collected the information for the policy update
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation(_pRobotControl.JointAngles); // 7 Points (J1 - J7)
        sensor.AddObservation(_pGripperControl.EndPoint); // 3 Points (x, y, z)
        sensor.AddObservation(target.transform.position); // 3 Points (x, y, z)
    }

    // OnActionReceived does an action received from the policy
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        float[] pActions = actions.ContinuousActions.Array;
        JointPoint pActionPos = _pRobotControl.JointPos + JointPoint.FromPosition("agent_pos", pActions);
        pActionPos.FrameCount = 1;
        StartCoroutine(_pRobotControl.Move(pActionPos));
    }

    // Heuristic is called for direct action
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(in actionsOut);
    }
}
