using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpiderAI : MonoBehaviour
{
    public Transform[] upperLegTargets; // ���ٸ� ��ǥ ��ġ
    public Transform[] lowerLegTargets; // �Ʒ��ٸ� ��ǥ ��ġ
    public Transform body; // �Ź� ��ü

    void Start()
    {
        // ���ٸ��� Two Bone IK Constraint ����
        for (int i = 0; i < upperLegTargets.Length; i++)
        {
            var upperLegJoint = transform.Find($"Leg{i}/UpperLeg/Joint");

            var upperLegIK = upperLegJoint.gameObject.AddComponent<TwoBoneIKConstraint>();
            var upperLegData = upperLegIK.data;

            upperLegData.root = transform.Find($"Leg{i}/UpperLeg/Root");
            upperLegData.mid = transform.Find($"Leg{i}/UpperLeg/Mid");
            upperLegData.tip = transform.Find($"Leg{i}/UpperLeg/Tip");
            upperLegData.target = upperLegTargets[i];

            // �ʿ信 ���� ���� ���� (optional)
            upperLegData.hint = transform.Find($"Leg{i}/UpperLeg/Hint");
        }

        // �Ʒ��ٸ��� Two Bone IK Constraint ����
        for (int i = 0; i < lowerLegTargets.Length; i++)
        {
            var lowerLegJoint = transform.Find($"Leg{i}/LowerLeg/Joint");

            var lowerLegIK = lowerLegJoint.gameObject.AddComponent<TwoBoneIKConstraint>();
            var lowerLegData = lowerLegIK.data;

            lowerLegData.root = transform.Find($"Leg{i}/LowerLeg/Root");
            lowerLegData.mid = transform.Find($"Leg{i}/LowerLeg/Mid");
            lowerLegData.tip = transform.Find($"Leg{i}/LowerLeg/Tip");
            lowerLegData.target = lowerLegTargets[i];

            // �ʿ信 ���� ���� ���� (optional)
            lowerLegData.hint = transform.Find($"Leg{i}/LowerLeg/Hint");
        }
    }
}
