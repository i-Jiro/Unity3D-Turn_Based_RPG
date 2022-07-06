using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform CameraTransform;
    private Transform MyTransform;
    public Material material;

    void Start()
    {
        MyTransform = this.transform;
        CameraTransform = Camera.main.transform;
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            Debug.LogError("Renderer is empty");
        GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        GetComponent<Renderer>().receiveShadows = true;
        renderer.material = material;
    }

    void LateUpdate()
    {
        MyTransform.forward = CameraTransform.forward;
    }
}