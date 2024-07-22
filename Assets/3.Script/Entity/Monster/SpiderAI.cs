using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpiderAI : MonoBehaviour
{
    public Transform[] upperLegTargets; // 윗다리 목표 위치
    public Transform[] lowerLegTargets; // 아랫다리 목표 위치
    public Transform body; // 거미 몸체

    void Start()
    {
        // 윗다리에 Two Bone IK Constraint 설정
        for (int i = 0; i < upperLegTargets.Length; i++)
        {
            var upperLegJoint = transform.Find($"Leg{i}/UpperLeg/Joint");

            var upperLegIK = upperLegJoint.gameObject.AddComponent<TwoBoneIKConstraint>();
            var upperLegData = upperLegIK.data;

            upperLegData.root = transform.Find($"Leg{i}/UpperLeg/Root");
            upperLegData.mid = transform.Find($"Leg{i}/UpperLeg/Mid");
            upperLegData.tip = transform.Find($"Leg{i}/UpperLeg/Tip");
            upperLegData.target = upperLegTargets[i];

            // 필요에 따라 힌지 설정 (optional)
            upperLegData.hint = transform.Find($"Leg{i}/UpperLeg/Hint");
        }

        // 아랫다리에 Two Bone IK Constraint 설정
        for (int i = 0; i < lowerLegTargets.Length; i++)
        {
            var lowerLegJoint = transform.Find($"Leg{i}/LowerLeg/Joint");

            var lowerLegIK = lowerLegJoint.gameObject.AddComponent<TwoBoneIKConstraint>();
            var lowerLegData = lowerLegIK.data;

            lowerLegData.root = transform.Find($"Leg{i}/LowerLeg/Root");
            lowerLegData.mid = transform.Find($"Leg{i}/LowerLeg/Mid");
            lowerLegData.tip = transform.Find($"Leg{i}/LowerLeg/Tip");
            lowerLegData.target = lowerLegTargets[i];

            // 필요에 따라 힌지 설정 (optional)
            lowerLegData.hint = transform.Find($"Leg{i}/LowerLeg/Hint");
        }
    }
}
