using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Rigidbody m_body;
    private WheelController[] m_wheels;

    [Tooltip("Offset rigidbody center of mass for better handling")]
    public Vector3 m_centerOfMass = Vector3.zero;

    // Vehicle base settings
    [Header("Driving settings")]
    public float m_maxTorque = 100f;
    public float m_maxBrake = 1000f;
    public float m_maxSteerAngle = 30f;

    // Leaning
    [Header("Leaning")]
    [Tooltip("Leaning will offset center of mass toward expected direction")]
    public float m_maxLean = .5f;
    public float m_leaningInertia = 1f;

    private Vector3 m_leaning = Vector3.zero;

    public void Reset()
    {
        m_body   = GetComponent<Rigidbody>();
        m_wheels = GetComponentsInChildren<WheelController>();
    }

    private void Awake()
    {
        if( !m_body )
            m_body = GetComponent<Rigidbody>();

        if( m_wheels == null )
            m_wheels = GetComponentsInChildren<WheelController>();
    }

    private void FixedUpdate()
    {
        Vector2 steeringInput = Vector2.zero;
        float throttleInput     = 0f;
        float brakeInput = 0f;

        if ( Gamepad.current != null )
        {
            steeringInput = Gamepad.current.leftStick.ReadValue();
            throttleInput = Gamepad.current.rightTrigger.ReadValue();
            brakeInput = Gamepad.current.leftTrigger.ReadValue();
        }

        if ( Keyboard.current.upArrowKey.isPressed )
            throttleInput += 1f;
        
        if ( Keyboard.current.downArrowKey.isPressed )
            brakeInput += 1f;

        if ( Keyboard.current.leftArrowKey.isPressed )
            steeringInput.x -= 1f;
        if ( Keyboard.current.rightArrowKey.isPressed )
            steeringInput.x += 1f;

        // Clamp input values to avoid overpassed limits
        steeringInput.x = Mathf.Clamp(steeringInput.x, -1f, 1f);
        throttleInput = Mathf.Clamp01(throttleInput);
        brakeInput    = Mathf.Clamp01(brakeInput);

        float handBrake = Mathf.Clamp01(Gamepad.current.leftShoulder.ReadValue() + Gamepad.current.leftShoulder.ReadValue());

        foreach ( WheelController wheel in m_wheels )
        {
            if ( wheel.m_motor )
            {
                wheel.wheelCollider.motorTorque = throttleInput * m_maxTorque;

                if( handBrake > 0f )
                {
                    wheel.wheelCollider.brakeTorque = handBrake * m_maxBrake;
                }
            }

            if ( wheel.m_brake && (handBrake <= 0f || !wheel.m_motor) )
            {
                wheel.wheelCollider.brakeTorque = brakeInput * m_maxBrake;
            }

            if (wheel.m_steer)
            {
                wheel.wheelCollider.steerAngle = steeringInput.x * m_maxSteerAngle;
            }
        }

        m_leaning = Vector3.Lerp(m_leaning, new Vector3(steeringInput.x, 0f, -steeringInput.y) * m_maxLean, Time.deltaTime / m_leaningInertia);

        m_body.centerOfMass = m_centerOfMass + m_leaning;
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        Vector3 worldCenterOfMass = transform.TransformPoint( m_centerOfMass + m_leaning );
        Gizmos.DrawLine(worldCenterOfMass - transform.right * .2f, worldCenterOfMass + transform.right * .2f );
        Gizmos.DrawLine( worldCenterOfMass - transform.up * .2f, worldCenterOfMass + transform.up * .2f);
        Gizmos.DrawLine( worldCenterOfMass - transform.forward * .2f, worldCenterOfMass + transform.forward * .2f);
    }
#endif // UNITY_EDITOR
}
