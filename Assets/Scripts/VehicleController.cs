using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public Vector3 m_centerOfMass = Vector3.zero;

    Rigidbody m_body;


    private void Awake()
    {
        m_body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_body.centerOfMass = m_centerOfMass;
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 worldCenterOfMass = transform.TransformPoint( m_centerOfMass );
        Gizmos.DrawLine(worldCenterOfMass - transform.right * .2f, worldCenterOfMass + transform.right * .2f );
        Gizmos.DrawLine( worldCenterOfMass - transform.up * .2f, worldCenterOfMass + transform.up * .2f);
        Gizmos.DrawLine( worldCenterOfMass - transform.forward * .2f, worldCenterOfMass + transform.forward * .2f);
    }
}
