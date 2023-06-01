using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;

public class TestController : MonoBehaviour
{
    public float explosionForce = 35;
    public float movementSpeed = 35;
    public float loseThreshold = -5;
    public GameObject landminePrefab;
    private const float MineCooldown = 0.3f;
    private float _mineAvailableTime = 0;
    private Animator _animator;
    private Rigidbody _rb;
    private const float Left = 90;
    private const float Right = 270;
    private GameObject _landminePlaced;
    private float _lastHorizontalInput = 0;
    private bool _shouldExplode = false;
    private const int FloorLayer = 3;
    private CapsuleCollider _collider;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        _lastHorizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown("space") && Time.time > _mineAvailableTime && isGrounded())
        {
            if (!_landminePlaced)
            {
                _landminePlaced = Instantiate(landminePrefab, transform.position, Quaternion.Euler(90, 0, 0));
                print("Landmine placed at " + _landminePlaced.transform.position);
            }
            else
            {
                _shouldExplode = true;
                _mineAvailableTime = Time.time + MineCooldown;
            }
        }
    }

    private void FixedUpdate()
    {
        ResetPositionIfOutOfBounds();
        if (_shouldExplode)
        {
            ApplyExplosionForce();

            Destroy(_landminePlaced);
            _landminePlaced = null;
            _shouldExplode = false;
        }
        else if (isGrounded())
        {
            ApplyWalkingForce();
        }
        OrientPlayer();
        UpdateAnimatorValues();
    }

    private void UpdateAnimatorValues()
    {
        _animator.SetFloat("Speed", Mathf.Abs(_rb.velocity.x));
    }

    private void OrientPlayer()
    {
        if (Mathf.Approximately(_rb.velocity.x, 0))
        {
            // Idle
        }
        else if (_rb.velocity.x > 0)
        {
            // Left (from camera perspective)
            transform.rotation = Quaternion.Euler(0, Left, 0);
        }
        else
        {
            // Right (from camera perspective)
            transform.rotation = Quaternion.Euler(0, Right, 0);
        }
    }

    private void ApplyWalkingForce()
    {
        _rb.AddForce(new Vector3(movementSpeed * _lastHorizontalInput, 0, 0));
    }

    private void ApplyExplosionForce()
    {
        Vector3 direction = (_rb.worldCenterOfMass - _landminePlaced.transform.position).normalized;
        _rb.velocity = Vector3.zero;
        _rb.AddForce(explosionForce * direction, ForceMode.Impulse);
    }

    private void ResetPositionIfOutOfBounds()
    {
        if (transform.position.y < loseThreshold)
        {
            transform.position = Vector3.zero;
        }
    }

    private bool isGrounded()
    {
        Ray ray = new Ray(transform.TransformPoint(_collider.center), Vector3.down);
        return Physics.SphereCast(ray, _collider.radius, _collider.height/2, 1 << FloorLayer);
    }
}
