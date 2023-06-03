using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;

public class TestController : MonoBehaviour
{
    public float groundDrag = 5;
    public float explosionForce = 20;
    public float movementSpeed = 55;
    public float loseThreshold = -5;
    public GameObject landminePrefab;
    public float addGravity = 13;
    public float blastCenterRadius = 4; // within blast center, full force; otherwise distance attenuation
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
    private float _minDistance;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _minDistance = (transform.position - _rb.worldCenterOfMass).magnitude;
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
        bool grounded = isGrounded();
        if (grounded)
        {
            _rb.drag = groundDrag;
            ApplyWalkingForce();
        }
        else
        {
            _rb.drag = 0;
        }
        _rb.AddForce(new Vector3(0, -addGravity, 0));
        
        OrientPlayer();
        UpdateAnimatorValues(grounded);
    }

    private void UpdateAnimatorValues(bool grounded)
    {
        _animator.SetFloat("Speed", Mathf.Abs(_rb.velocity.x));
        _animator.SetBool("Grounded", grounded);
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
        Vector3 landmineToCenter = _rb.worldCenterOfMass - _landminePlaced.transform.position;
        Vector3 direction = landmineToCenter.normalized;
        _rb.velocity = Vector3.zero;
        
        // immediate falloff
        float distanceAttenuation = landmineToCenter.magnitude <= blastCenterRadius ? 1 : 0;
        Vector3 finalExplosionForce = distanceAttenuation * explosionForce * direction;
        _rb.AddForce(finalExplosionForce, ForceMode.Impulse);
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
