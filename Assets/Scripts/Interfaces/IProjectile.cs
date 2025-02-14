using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void Move();
    void Initialize(Vector3 direction, bool isEnemy);


}
