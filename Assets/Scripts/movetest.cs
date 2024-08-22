using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movetest : MonoBehaviour
{
    private new Rigidbody rigidbody;

    private float v;
    private float h;
    private float r;

    RaycastHit hit;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        
    }

    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        r = Input.GetAxis("Mouse X");

        if (Input.GetKeyDown("space"))
        {
            if(Physics.Raycast(transform.position, -transform.up, out hit, 0.6f))
            {
                rigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
        transform.Translate(dir.normalized * Time.deltaTime * 8f, Space.Self);
        transform.Rotate(Vector3.up * Time.smoothDeltaTime * r);
    }
}
