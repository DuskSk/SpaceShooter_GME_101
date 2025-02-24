using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void Initialize(Vector3 direction, float speed, bool isEnemy);

}
