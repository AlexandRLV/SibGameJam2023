using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    private int VisionConeResolution = 50;
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;

    public void Init(float viewAngle)
    {
        MeshFilter_ = GetComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        transform.rotation = Quaternion.Euler (0.0f, viewAngle/2, 0.0f);
    }

    public void SetColor(Color newColor)
    {
        GetComponent<MeshRenderer>().material.color = newColor;
    }

    public void DrawFOV(float viewDistance, float viewAngle, LayerMask layerMask)
    {
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        viewAngle *= Mathf.Deg2Rad;
        float Currentangle = -viewAngle;
        float angleIcrement = viewAngle / (VisionConeResolution - 1);
        float Sin, Cos;
        

        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sin = Mathf.Sin(Currentangle);
            Cos = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cos) + (transform.right * Sin);
            Vector3 VertForward = (Vector3.forward * Cos) + (Vector3.right * Sin);
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, viewDistance, layerMask))
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * viewDistance;
            }


            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}
