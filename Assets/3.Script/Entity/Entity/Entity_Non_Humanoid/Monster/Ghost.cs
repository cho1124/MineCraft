using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Ghost : MonoBehaviour
{
    [SerializeField] private GameObject explosion_particle;
    [SerializeField] private Part head;

    public void Explosion()
    {
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        Vector3 original_scale = transform.localScale;
        WaitForSeconds wait_for_one_frame = new WaitForSeconds(1f/60f);
        int frame_count = 0;

        while (frame_count < 60)
        {
            Debug.Log("Ä¿Áü");
            transform.localScale = transform.localScale * 1.005f;
            frame_count++;
            yield return wait_for_one_frame;
        }

        Collider[] victim_colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (var victim_collider in victim_colliders)
        {
            Debug.Log($"{victim_collider.gameObject.name}°¡ Æø¹ß ¹üÀ§ ³»¿¡ ÀÖÀ½");
            head.Set_Value_Melee(100f, 1.0f, 0.5f, 1.0f, 1.0f, 1);
            head.Force_Trigger(victim_collider);
        }
        head.Collider_On_Off(false);
        Destroy(gameObject);

        // Æø¹ß ÆÄÆ¼Å¬ ÀÌÆåÆ® »ý¼º
        //Instantiate(explosion_particle, transform.position, Quaternion.identity);
    }
}
