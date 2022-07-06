using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBird : Enemy
{
    [SerializeField] float _startPositionOffset = 3f;
    protected override void Start()
    {
        transform.position += new Vector3(0f, _startPositionOffset, 0f);
        base.Start();
    }
}
