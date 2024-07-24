using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return_Hand_And_Victim : MonoBehaviour
{
    [SerializeField] private bool L_or_R;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Entity"))
        {
            gameObject.GetComponentInParent<Player_Control>().On_Attack(L_or_R, other.gameObject);
        }
    }
}
