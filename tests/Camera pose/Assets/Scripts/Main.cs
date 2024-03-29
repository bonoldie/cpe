using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPE.Zephyr;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PointCloud pc = new PointCloud();

        pc.LoadPlyFromFile("Assets/Ply/SamPointCloud.ply");
        Debug.Log(pc.ply.getDoublePropertyArray("x").Length);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
