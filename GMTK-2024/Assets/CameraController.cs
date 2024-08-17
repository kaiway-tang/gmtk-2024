using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform camTargetPos;
    [SerializeField] float lerpRate;

    Transform trfm;
    // Start is called before the first frame update
    void Start()
    {
        trfm = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        trfm.position += (camTargetPos.position - trfm.position) * lerpRate;
    }
}
