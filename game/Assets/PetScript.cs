using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
