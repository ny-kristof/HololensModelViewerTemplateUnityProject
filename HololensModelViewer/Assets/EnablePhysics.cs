    using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePhysics : MonoBehaviour
{
    Rigidbody rbody;
    MeshCollider mc;
    ObjectManipulator om;

    public void DropObject(ManipulationEventData e)
    {
        rbody.isKinematic = false;
        if (mc != null)
        {
            //mc.convex = true;
        }
    }
    public void HoldObject(ManipulationEventData e)
    {
        rbody.isKinematic = true;
        if (mc != null)
        {
            //mc.convex = false;
        }
    }
    public void InPlace()
    {
        //om.enabled = false;
        //mc.enabled = false;
        rbody.isKinematic = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        rbody = gameObject.GetComponent<Rigidbody>();
        mc = gameObject.GetComponent<MeshCollider>();
        om = gameObject.GetComponent<ObjectManipulator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rbody.position.y < -5)
        {
            rbody.isKinematic = true;
            rbody.position = new Vector3(0.0f, 0.0f, 1.0f); 
        }
    }
}
