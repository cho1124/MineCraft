using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stance_Change_To_L : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("Moveset_Number", -2);
    }
}
