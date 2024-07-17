using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    public bool isObstacleDeteched;
    public bool isPlayerDeteched;
    public bool isAnimalDeteched;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerDeteched = true;
        }
        else if(other.CompareTag("Animals"))
        {
            isAnimalDeteched = true;
        }
        else
        {
            isObstacleDeteched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerDeteched = false;
        }
        else if(other.CompareTag("Animals"))
        {
            isAnimalDeteched = false;
        }
        else
        {
            isObstacleDeteched = false;
        }   
    }
}
