using System.Collections;
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
        _pGripperControl = gripper.GetComponent<GripperController>();
        _pRobotControl.ControlMode = OperationMode.Forced;
        _pGripperControl.ControlMode = OperationMode.Forced;
    }

    // OnEpisodeBegin is called before the episode start
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        Debug.Log("[AGENT] Episode Start");
        JointPoint pInitPos = JointPoint.FromPosition("Init", 135.0F, 0.0F, 90.0F, 0.0F, 90.0F, 0.0F, 0.0F);
        _pRobotControl.ForcedMove(pInitPos);
    }

    // CollectObservations is collected the information for the policy update
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_pRobotControl.JointAngles); // 7 Points (J1 - J7)
        sensor.AddObservation(_pGripperControl.EndPoint); // 3 Points (x, y, z)
        sensor.AddObservation(target.transform.position); // 3 Points (x, y, z)
        Debug.Log($"[AGENT] Collect Observations ({sensor.ObservationSize()})");
        base.CollectObservations(sensor);
    }

    // OnActionReceived does an action received from the policy
    public override void OnActionReceived(ActionBuffers actions)
    {
        float[] pActions = actions.ContinuousActions.Array; // Spaces : Joint points (Continuous)
        // Data Normalization
        for (int i = 0; i < pActions.Length; i++)
            pActions[i] = Mathf.Clamp(pActions[i], -360.0F, 360.0F);
        JointPoint pActionPos = JointPoint.FromPosition("agent_pos", pActions);
        Debug.Log($"[AGENT] Input {pActionPos.Print()}");
        JointPoint pNextPos = _pRobotControl.JointPos + pActionPos;
        _pRobotControl.ForcedMove(pNextPos);
        // Reward for moving continuously
        SetReward(-0.0001F);
        
        base.OnActionReceived(actions);
    }

    // Call the manual action from a developer
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> pActionSegments = actionsOut.ContinuousActions;
        for (int i = 0; i < _pRobotControl.joints.Length; i++)
        {
            pActionSegments[i] = Input.GetKeyDown($"{i+1}") ? Input.GetAxis("Horizontal") : 0.0F;
        }
        Debug.Log(pActionSegments.ToString());
    }
}
