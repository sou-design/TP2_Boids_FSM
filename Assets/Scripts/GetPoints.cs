using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetPoints
{
    public static List<Vector3> GetPoint(this Transform transform, int direct)
    {

        List<Vector3> directions = new List<Vector3>();

        float angle = 2 * Mathf.PI * 1.5f;

        for (int i = 0; i < direct; i++)
        {
            float distance = (float)i / direct;
            float phi = Mathf.Acos(1 - 2 * distance);
            float theta = angle * i;

            directions.Add(transform.TransformDirection(new Vector3(Mathf.Sin(phi) * Mathf.Cos(theta), Mathf.Sin(phi) * Mathf.Sin(theta), Mathf.Cos(phi))));
        }
        return directions;
    }
}

