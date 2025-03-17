using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        float xinput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dr = new Vector3(xinput, 0,zInput).normalized;

        Vector3 move = dr * speed * Time.deltaTime;

        transform.position += move;
    }
}
