using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Data.Text;
using CPE.Zephyr;
using CPE.CV;
using GK;

public class Main : MonoBehaviour
{
    Mesh mesh;

    Vector3[] MeshVertices;
    Color[] MeshColors;

    void Start()
    {
        PoseEstimation pe = new();
        PointCloud pc = new PointCloud();

        Matrix<double> Points2D = DelimitedReader.Read<double>("Assets/Csv/points2D.csv", false, ",");
        Matrix<double> Points3D = DelimitedReader.Read<double>("Assets/Csv/points3D.csv", false, ",");
        Matrix<double> K = DelimitedReader.Read<double>("Assets/Csv/K.csv", false, ",");

        CameraPose cp = pe.EstimatePose(Points2D.SubMatrix(0, 100, 0, Points2D.ColumnCount), Points3D.SubMatrix(0, 100, 0, Points3D.ColumnCount), K);

        //pc.LoadPlyFromFile("Assets/Ply/SamPointCloud.ply");
        pc.LoadPlyFromFile("Assets/Ply/Beth.ply");
        Debug.Log(pc.ply.ToPCacheFormat());

        var x = pc.ply.GetDoubleVector("x");
        var y = pc.ply.GetDoubleVector("y");
        var z = pc.ply.GetDoubleVector("z");
        var r = pc.ply.GetUCharVector("red");
        var g = pc.ply.GetUCharVector("green");
        var b = pc.ply.GetUCharVector("blue");

        MeshVertices = new Vector3[x.Count];
        MeshColors = new Color[x.Count];

        var m_ParticleSystem = GetComponent<ParticleSystem>();

        var m_Particles = new ParticleSystem.Particle[x.Count];

        for (int i = 0; i < x.Count; ++i)
        {
            MeshVertices[i].x = (float)x[i];
            MeshVertices[i].y = (float)y[i];
            MeshVertices[i].z = (float)z[i];

            MeshColors[i] = new Color();
            MeshColors[i].r = r[i];
            MeshColors[i].g = g[i];
            MeshColors[i].b = b[i];
            MeshColors[i].a = 0.8f;

            m_Particles[i].startColor = MeshColors[i];
            m_Particles[i].startSize = 0.1f;
            m_Particles[i].position = MeshVertices[i];
            m_Particles[i].remainingLifetime = 10000000f;
        }

        m_ParticleSystem.SetParticles(m_Particles, m_Particles.Length);

        m_ParticleSystem.Stop();

        var renderer = m_ParticleSystem.GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = true;


        // mesh = this.GetComponent<MeshFilter>().mesh;
        //mesh.colors = MeshColors;

        // var CHCalc = new ConvexHullCalculator();
        // var vertices = new List<Vector3>(){};
        // vertices.AddRange(MeshVertices);
        // Debug.Log(vertices.Count);

        // var verts = new List<Vector3>();
        // var triangs = new List<int>();
        // var norms = new List<Vector3>();
        // CHCalc.GenerateHull(vertices, true, ref verts, ref triangs, ref norms);

        // mesh.SetVertices(verts);
        // mesh.SetTriangles(triangs, 0);
        // //mesh.SetNormals(norms);
        // mesh.RecalculateNormals();

        //mesh.vertices = MeshVertices;
        //mesh.triangles = ConvexHull.Generate(MeshVertices);
    }

    // Update is called once per frame
    void Update()
    {
        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
    }
}
