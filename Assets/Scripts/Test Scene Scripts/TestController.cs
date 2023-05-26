using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public float explosionForce = 100;
    public float movementSpeed = 5; 
    private Animator _animator;
    private Rigidbody _rb;
    private float LEFT = 90;
    private float RIGHT = 270;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = movementSpeed * h;
        _animator.SetFloat("Speed", Mathf.Abs(h));
        _rb.velocity = new Vector3(v, _rb.velocity.y, 0);

        if (h == 0)
        {
            // Idle
        } else if (h > 0)
        {
            // Left (from camera perspective)
            transform.rotation = Quaternion.Euler(0, LEFT, 0);
        }
        else
        {
            // Right (from camera perspective)
            transform.rotation = Quaternion.Euler(0, RIGHT, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _rb.AddExplosionForce(explosionForce, new Vector3(transform.position.x, transform.position.y-1, transform.position.z), 4);
        }
    }

    private void FixedUpdate()
    {
        
    }
}
