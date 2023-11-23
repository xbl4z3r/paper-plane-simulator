using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PaperController : MonoBehaviour
{
    public float initialThrust = 10f;
    public float launchAngle = 70f;
    private Rigidbody _rb;
    private Vector3 _startPosition;
    private bool _onGround = true;
    private CinemachineFollowZoom _zoom;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        _zoom = FindFirstObjectByType<CinemachineFollowZoom>();
    }

    private void Update()
    {
        ListenForInput();
        
        // Update the zoom level based on whether we're on the ground or not
        _onGround = Physics.Raycast(transform.position, Vector3.down, 1f);
        _zoom.m_MinFOV = Mathf.Lerp(_zoom.m_MinFOV, _onGround ? 10f : 30f, Time.deltaTime * 10f);
        
        // Rotate the paper to face the direction of the velocity
        if(_rb.velocity.magnitude > 3.5f && !_onGround) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_rb.velocity), Time.deltaTime * 10f);
        
        if(_onGround) Debug.DrawRay(transform.position, Quaternion.Euler(-launchAngle, 0f, 0f) * Vector3.forward * 5f, Color.magenta);
        if (_onGround) return;
        
        // Draw the trajectory
        var velocity = _rb.velocity;
        var position = transform.position;
        for (var i = 0; i < 100; i++)
        {
            // Calculate the next position and velocity
            velocity += Physics.gravity * Time.fixedDeltaTime;
            position += velocity * Time.fixedDeltaTime;
            
            // Stop drawing the trajectory if we hit the ground game object
            var hits = new RaycastHit[10];
            Physics.RaycastNonAlloc(position, velocity, hits, velocity.magnitude * Time.fixedDeltaTime);
            if (hits.Where(hit => hit.collider != null).Any(hit => hit.collider.gameObject.CompareTag("Ground"))) break;
            
            // Draw the trajectory
            Debug.DrawRay(position, velocity * Time.fixedDeltaTime, Color.black);
        }
    }
    
    private void ListenForInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var rotation = transform.rotation;
            transform.rotation = Quaternion.Euler(-launchAngle, rotation.eulerAngles.y, rotation.eulerAngles.z);
            _rb.AddForce(transform.rotation * Vector3.forward * initialThrust, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;
        }
    }
}