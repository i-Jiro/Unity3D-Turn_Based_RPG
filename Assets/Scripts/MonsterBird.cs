using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBird : Enemy
{
    [SerializeField] float _startPositionOffsetMax = 3f;
    [SerializeField] float _startPositionOffsetMin = 1f;
    protected override void Start()
    {
        transform.position += new Vector3(0f, Random.Range(_startPositionOffsetMin, _startPositionOffsetMax) , 0f);
        base.Start();
    }
}
