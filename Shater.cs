using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Shater : MonoBehaviour
{
    public int NumberOfTrianglesPerObject = 3;
    public float minExplosionSpeed = 0.0f;
    public float maxExplosionSpeed = 40.0f;
    public float minAngulerExplosionSpeed = 0.0f;
    public float maxAngulerExplosionSpeed = 40.0f;
    public Vector3 explosionVector;
    public int meshCounthPerObje =3;
    public bool gravity = false;
    public float tranparencyTime = 4f;
    public float newAlpha = 0f;
    public float destroyTime = 5f;
    public bool randomDestroyTime = false;
    public float minDestroyTime = 1f;
    public float maxDestroyTime = 5f;
    public bool isColiderSet = false;

    private int meshCounth = 0;



    
    //objelere mesh ekliyip tranparencynin deðiþmesini baþlatýyor
    private void addGameobject(Mesh mesh , Material material)
    {
        GameObject obje = new GameObject("Copy of " + transform.parent.name + " Mesh " + meshCounth );
        meshCounth++;

        obje.AddComponent<MeshFilter>().mesh = mesh;
        obje.AddComponent<MeshRenderer>().material = material;
        obje.AddComponent<Rigidbody>().useGravity = gravity;
        obje.transform.position = transform.position;
        obje.transform.rotation = transform.rotation;
        obje.AddComponent<TranparencyChange>().setData(tranparencyTime, newAlpha);

        if (randomDestroyTime)
        {
            Destroy(gameObject, UnityEngine.Random.Range(minDestroyTime, maxDestroyTime));
        }
        else
        {
            Destroy(gameObject, destroyTime);
        }

        if (isColiderSet)
            obje.AddComponent<BoxCollider>();

        if (explosionVector == Vector3.zero)
        {
            obje.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-maxExplosionSpeed, maxExplosionSpeed),
                UnityEngine.Random.Range(-maxExplosionSpeed, maxExplosionSpeed), UnityEngine.Random.Range(-maxExplosionSpeed, maxExplosionSpeed));

            obje.GetComponent<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(-maxAngulerExplosionSpeed, maxAngulerExplosionSpeed),
                UnityEngine.Random.Range(-maxAngulerExplosionSpeed, maxAngulerExplosionSpeed), UnityEngine.Random.Range(-maxAngulerExplosionSpeed, maxAngulerExplosionSpeed));

        }
        else
        {
            
        }
    }

    //yeni meshler yapýlýp obje oluþturma scriptine gönderiyor
    //Bu methodu çalýþtýrýlmasý lazým
    public void shatere()
    {

        if (gameObject.GetComponent<MeshFilter>() == null && gameObject.GetComponent<SkinnedMeshRenderer>() == null)
        {
            return;
        }


        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }

        Mesh M = new Mesh();
        if (GetComponent<MeshFilter>())
        {
            M = GetComponent<MeshFilter>().mesh;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if (GetComponent<MeshRenderer>())
        {
            materials = GetComponent<MeshRenderer>().materials;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        List<Vector3> newVerts = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUvs = new List<Vector2>();

        foreach (var material in materials)
        {
            material.SetFloat("_Mode", 2);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.EnableKeyword("_ALPHABLEND_ON");
            material.renderQueue = 3000;
            //material.SetInt("_ZWrite", 0);
            //material.DisableKeyword("_ALPHATEST_ON");
            //material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        int loopCounth = 0;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            
            int[] indices = M.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 3)
            {
                
                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts.Add(verts[index]);
                    newUvs.Add(uvs[index]);
                    newNormals.Add(normals[index]);
                }

                loopCounth++;
                if (loopCounth == meshCounthPerObje)
                {
                    loopCounth = 0;

                    Mesh mesh = new Mesh();
                    mesh.vertices = newVerts.ToArray();
                    mesh.normals = newNormals.ToArray();
                    mesh.uv = newUvs.ToArray();

                    List<int> triangles= new List<int>();
                    for (int j = 0; j < newVerts.Count; j++)
                    {
                        triangles.Add(j);
                    }
                    for (int j = newVerts.Count - 1; j >= 0 ; j--)
                    {
                        triangles.Add(j);
                    }
                    mesh.triangles = triangles.ToArray();

                    addGameobject(mesh , materials[submesh]);

                    newVerts.Clear();
                    newUvs.Clear();
                    newNormals.Clear();


                }
                else if(i+3>= indices.Length )
                {
                    loopCounth = 0;

                    Mesh mesh = new Mesh();
                    mesh.vertices = newVerts.ToArray();
                    mesh.normals = newNormals.ToArray();
                    mesh.uv = newUvs.ToArray();

                    List<int> triangles = new List<int>();
                    for (int j = 0; j < newVerts.Count; j++)
                    {
                        triangles.Add(j);
                    }
                    for (int j = newVerts.Count - 1; j >= 0; j--)
                    {
                        triangles.Add(j);
                    }
                    mesh.triangles = triangles.ToArray();

                    addGameobject(mesh, materials[submesh]);

                    newVerts.Clear();
                    newUvs.Clear();
                    newNormals.Clear();
                }
            }
        }

        GetComponent<Renderer>().enabled = false;
    }
    private void Start()
    {
        //shatere();
    }

}

