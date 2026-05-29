using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerController pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
 
    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
 
    private Vector3 grapplePoint;
 
    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;
 
    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;
 
    private bool grappling;
    private SpringJoint joint; // added for swinging
 
    private void Start()
    {
        pm = GetComponent<PlayerController>();
    }
 
    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();
        if (Input.GetKeyUp(grappleKey) && grappling) StopGrapple(); // release to stop swing
 
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }
 
    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);
    }
 
    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;
 
        grappling = true;
        pm.freeze = true;
 
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
 
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }
 
    private void ExecuteGrapple()
    {
        pm.freeze = false;
 
        // spring joint swing
        joint = pm.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;
 
        float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }
 
    public void StopGrapple()
    {
        pm.freeze = false;
        pm.activeGrapple = false; // FIX: was leaving activeGrapple true, blocking movement after landing
 
        grappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;
 
        if (joint != null) Destroy(joint); // clean up spring joint
    }
 
    public bool IsGrappling()
    {
        return grappling;
    }
 
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
 