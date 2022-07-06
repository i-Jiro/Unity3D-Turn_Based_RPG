using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSelector : MonoBehaviour
{
    public float speed = 6;
    public float offset = 3;
    private Vector3 _StartingPosition;
    private bool _returnOrigin = false;

    private void Start()
    {
        _StartingPosition = Vector3.zero;  
    }

    void Update()
    {
        if(transform.position.x > _StartingPosition.x - offset && !_returnOrigin)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        else
            _returnOrigin = true;

        if (transform.position.x < _StartingPosition.x + offset && _returnOrigin)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        else
            _returnOrigin = false;
    }
}
