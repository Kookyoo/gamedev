using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    public Vector3 m_centerOfMass = Vector3.zero;

    Rigidbody m_body;

    public float m_maxTorque = 100f;
    public float m_maxBrake = 1000f;

    public float m_maxSteerAngle = 30f;

    public float m_maxLean = .5f;

    private WheelController[] m_wheels;

    public float m_leaningInertia = 1f;
    private Vector3 m_leaning = Vector3.zero;

    private void Awake()
    {
        if( !m_body )
            m_body = GetComponent<Rigidbody>();

        if( m_wheels == null )
            m_wheels = GetComponentsInChildren<WheelController>();
    }

    private void FixedUpdate()
    {
        if ( Gamepad.current != null )
        {
            Vector2 steeringInput = Gamepad.current.leftStick.ReadValue();
            float throttleInput = Gamepad.current.rightTrigger.ReadValue();
            float brakeInput = Gamepad.current.leftTrigger.ReadValue();

            float handBrake = Mathf.Clamp01(Gamepad.current.leftShoulder.ReadValue() + Gamepad.current.leftShoulder.ReadValue());

            foreach (WheelController wheel in m_wheels)
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
    }

    public void Reset()
    {
        m_body = GetComponent<Rigidbody>();
        m_wheels = GetComponentsInChildren<WheelController>();
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 worldCenterOfMass = transform.TransformPoint( m_centerOfMass + m_leaning );
        Gizmos.DrawLine(worldCenterOfMass - transform.right * .2f, worldCenterOfMass + transform.right * .2f );
        Gizmos.DrawLine( worldCenterOfMass - transform.up * .2f, worldCenterOfMass + transform.up * .2f);
        Gizmos.DrawLine( worldCenterOfMass - transform.forward * .2f, worldCenterOfMass + transform.forward * .2f);
    }
}
