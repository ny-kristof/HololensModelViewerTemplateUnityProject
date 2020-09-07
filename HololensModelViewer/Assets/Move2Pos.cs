using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2Pos : MonoBehaviour
{
    bool startmove = false;
    bool assemblycanstart = false;
    Vector3 endpos = new Vector3(0.0f,0.0f,0.0f);
    Vector3 startpos = new Vector3(0.0f, 0.0f, 0.0f);
    Rigidbody rb;
    Transform tr;
    MeshCollider mc;
    float speed = 2;

    public Vector3 Endpos
    {
        get => endpos;
        set => endpos = value;
    }
    public bool Startmove
    {
        get => startmove;
        set => startmove = value;
    }
    public bool AssemblyCanStart
    {
        get => assemblycanstart;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.startpos = gameObject.GetComponent<Transform>().position;
        rb = gameObject.GetComponent<Rigidbody>();
        tr = gameObject.GetComponent<Transform>();
        mc = gameObject.GetComponent<MeshCollider>();
        //as tr.localPosition = endpos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (startmove)
        {
            //rb.MovePosition(rb.position + (endpos-rb.position)*Time.fixedDeltaTime);
            tr.localPosition = tr.localPosition + (endpos - tr.localPosition) * Time.fixedDeltaTime* speed;
            if ((tr.localPosition - endpos).magnitude < 0.01)
            {
                startmove = false;
                assemblycanstart = true;
                mc.enabled = true;
            }
        }
        
    }
}
