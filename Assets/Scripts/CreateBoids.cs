using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoids : MonoBehaviour
{
    public GameObject boid;
    public int nbMax;

    void Start() {
        for(int i=0; i<nbMax; i++){
            Instantiate(boid, transform.position + Random.insideUnitSphere * 5, Quaternion.identity);
        }
    }
}
