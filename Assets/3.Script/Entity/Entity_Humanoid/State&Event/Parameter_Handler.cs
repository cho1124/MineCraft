using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameter_Handler : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        TryGetComponent(out animator);
    }
    private void Update()
    {
        if (animator.GetBool("LR_Attack"))
            StartCoroutine(Parameter_Set_False_Co("LR_Attack", 0.1f));

        if (animator.GetBool("L_Attack"))
            StartCoroutine(Parameter_Set_False_Co("L_Attack", 0.1f));

        if (animator.GetBool("R_Attack"))
            StartCoroutine(Parameter_Set_False_Co("R_Attack", 0.1f));
    }
    public IEnumerator Parameter_Set_False_Co(string parameter_name, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(parameter_name, false);
    }
}