using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public WheelCollider wheelCollider;

    public bool m_motor;
    public bool m_brake;
    public bool m_steer;

    public void Awake()
    {
        if( !wheelCollider )
            wheelCollider = GetComponentInChildren<WheelCollider>();
    }

    public void Reset()
    {
        wheelCollider = GetComponentInChildren<WheelCollider>();
    }
}
