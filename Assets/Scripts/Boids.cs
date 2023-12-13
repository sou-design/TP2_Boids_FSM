using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Boids: MonoBehaviour
{
    Vector3 offset;
    public ParticleSystem Fire;
    //movement
    [HideInInspector]
    public Vector3 velocity;

    //layer mask
    int obstacle;
    float timeUsed = 0f;
    public bool target;
    Transform destination;
    static Boids[] boidsList;
    static Vector3 pos;
    static Vector3 Align;
    static Vector3 Avoid;
    public float vitesse = 3f;
    List<KeyCode> keys;
    public float PercepRadius = 4f;
    void Start() {
        obstacle = ~(LayerMask.NameToLayer("Obstacle"));

        velocity = transform.forward;
        switch (target)
        {
            case true:
                destination = GameObject.FindWithTag("Target").transform;
                break;
            case false:
                destination = GameObject.FindWithTag("Home").transform;
                break;
        }
    }
    IEnumerator Fuir()
    {
        destination = null;
        yield return new WaitForSeconds(2);
        destination = GameObject.FindWithTag("Target").transform;
    }
    private void Update() {
        timeUsed += 1 ;
        var inputValue = Input.inputString; Vector3 direction = Vector3.zero;
        switch (inputValue)
        {//state repos
            case ("R"):
                destination = GameObject.FindWithTag("Home").transform;
                break;
        //state courrir
            case ("C"):
                destination = GameObject.FindWithTag("Target").transform;
                break;
        //state random fly
            case ("K"):
                target=false;
                destination = null;
                break;
        //state fuir
            case ("F"):
                var fire =Instantiate(Fire, GameObject.FindWithTag("Player").transform.position + Vector3.one, Quaternion.identity);
                StartCoroutine(Fuir());
                break;
        }

       
        if (target || destination!=null){
            offset = destination.position - transform.position;
            offset= Vector3.ClampMagnitude(offset, 5f);
            direction += (offset - velocity);
        }
        Vector3[] boids = GetInsects(this);
        Vector3 AlignDirection = CustomClampMagnitude(boids[1], 5f).normalized;

        direction += (CustomClampMagnitude(boids[0], 5f) - velocity) * 0.5f;
        direction += (AlignDirection - velocity) * 0.125f;
        direction += (CustomClampMagnitude(boids[2], 5f) -velocity);
        //check for collision
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit, 5, obstacle))
        { 
            direction += CustomClampMagnitude(GetDirection(), 3f) * 100;
        }
        velocity += direction * vitesse * Time.deltaTime;
        float tempVitesse = Mathf.Clamp(velocity.magnitude, 3, 5);
        velocity = velocity.normalized * tempVitesse;
        transform.position += velocity * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        } 
    }
    Vector3 GetDirection()
    {
        Vector3 dir, other = transform.forward;
        List<Vector3> directions = transform.GetPoint(300);
        foreach (var direct in directions)
        {
             Ray ray = new Ray(transform.position, direct);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 5f, obstacle))
            {
                return direct.normalized;
            }
        }
        return other;
    }
    //Get other insects
    public static Vector3[] GetInsects(Boids boid_)
    {
        boidsList = MonoBehaviour.FindObjectsOfType<Boids>();
        Vector3 offset= new Vector3(0,0,0);
        int nb = 0;
        foreach (var boid in boidsList)
        {
            if (boid == boid_) continue;
            
                float boidDistance = Vector3.Distance(boid_.transform.position, boid.transform.position);
                if (boidDistance < boid.PercepRadius)
                {
                    nb++;
                    pos += boid.transform.position;
                    Align += boid.velocity;
                    if (boidDistance < 1)
                    {
                        offset -= boid.transform.position - boid_.transform.position;
                    }
                }
        }
        int all = Mathf.Max(nb, 1);
        Align = ((Align / all) - boid_.transform.position) / all - boid_.velocity;
        Avoid = offset;

        Vector3[] returnValues = { pos, Align, Avoid };
        return returnValues;
    }
    //fonction clampMagnitude
    public static Vector3 CustomClampMagnitude(Vector3 vector, float max)
    {
        if (vector != Vector3.zero)
        {
            float magnitude = vector.magnitude;
            vector = vector.normalized * Mathf.Min(magnitude, max);
        }

        return vector;
    }
}
