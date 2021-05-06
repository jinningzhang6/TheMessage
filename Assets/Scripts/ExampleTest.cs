using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTest : MonoBehaviour
{
    private float rotZ;
    private float RotationSpeed = 200;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotZ += Time.deltaTime * RotationSpeed;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
