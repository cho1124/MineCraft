using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_Handler : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Entity_Humanoid>().Reset_Hand_Collider();
    }
}
