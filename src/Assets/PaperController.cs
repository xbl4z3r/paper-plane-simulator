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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
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
        
        _onGround = Physics.Raycast(transform.position, Vector3.down, 1f);
        _zoom.m_MinFOV = Mathf.Lerp(_zoom.m_MinFOV, _onGround ? 10f : 30f, Time.deltaTime * 10f);
        
        // Rotate the paper to face the direction of the velocity
        if(_rb.velocity.magnitude > 3.5f && !_onGround) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_rb.velocity), Time.deltaTime * 10f);
        
        // Draw the trajectory
        var velocity = _rb.velocity;
        var position = transform.position;
        for (var i = 0; i < 100; i++)
        {
            velocity += Physics.gravity * Time.fixedDeltaTime;
            position += velocity * Time.fixedDeltaTime;
            Debug.DrawRay(position, velocity * Time.fixedDeltaTime, Color.black);
        }
        
        // Draw the launch angle
        var launchDirection = Quaternion.Euler(-launchAngle, 0f, 0f) * Vector3.forward;
        Debug.DrawRay(transform.position, launchDirection * 5f, Color.magenta);
    }
}