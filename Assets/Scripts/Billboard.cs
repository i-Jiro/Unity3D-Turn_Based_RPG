using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform CameraTransform;
    public Material material;
    public bool toggleShadows = true;

    void Start()
    {
        CameraTransform = Camera.main.transform;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (toggleShadows)
            {
                GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                GetComponent<Renderer>().receiveShadows = true;
                renderer.material = material;
            }
        }
    }

    void LateUpdate()
    {
        transform.forward = CameraTransform.forward;
    }
}