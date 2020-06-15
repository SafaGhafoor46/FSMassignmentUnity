using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckMyVision : MonoBehaviour
{
    public enum enumSensitivity { High, Low };

    public enumSensitivity sensitivity = enumSensitivity.High;
    public bool targetInSight = false;
    public float fieldOfVision = 45f;
    private Transform target = null;
    public Transform myEyes = null;
    public Transform npcTransform = null;
    private SphereCollider SphereCollider = null;
    public Vector3 lastKnownSighting = Vector3.zero;

    private void Awake() 
    {
        npcTransform = GetComponent<Transform>();
        SphereCollider = GetComponent<SphereCollider>();
        lastKnownSighting = npcTransform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    bool InMyFieldOfVision() 
    {
        Vector3 dirToTarget = target.position - myEyes.position;
        float angle = Vector3.Angle(myEyes.forward, dirToTarget);
        if (angle <= fieldOfVision)
        {
            return true;
        }
        else
            return false;
    }

    bool ClearLineOfSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(myEyes.position, (target.position - myEyes.position).normalized, out hit, SphereCollider.radius))
        {
            if (hit.transform.CompareTag("Player"))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    void UpdateSight()
    {
        switch (sensitivity) 
        { 
            case enumSensitivity.High:
                targetInSight = InMyFieldOfVision() && ClearLineOfSight();
                break;
            case enumSensitivity.Low:
                targetInSight = InMyFieldOfVision() || ClearLineOfSight();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateSight();
        if (targetInSight)
        {
            lastKnownSighting = target.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Pslayer"))
            return;
        targetInSight = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
