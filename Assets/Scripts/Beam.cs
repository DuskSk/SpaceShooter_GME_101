using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _rotationLimit = 120f;
    [SerializeField] private float _damageCooldown = 1f;
    private float _rotationStep;

    private bool _isRotatinClockwise = true;
    private Dictionary<GameObject, float> _lastDamageTime = new Dictionary<GameObject, float>();
    private Collider2D _myCollider;

    private void Start()
    {
        _myCollider = GetComponent<Collider2D>();
    }
    

    public void Initialize(bool clockwise)
    {
        _isRotatinClockwise = clockwise;
        StartCoroutine(SweepBeam());

    }

    IEnumerator SweepBeam()
    {
        float currentAngle = transform.eulerAngles.z;
        float targetAngle = _rotationLimit;

        while (true)
        {
            _rotationStep = _rotationSpeed * Time.deltaTime;
            currentAngle += _isRotatinClockwise ? _rotationStep : -_rotationStep;
            transform.rotation = UnityEngine.Quaternion.Euler(0, 0, currentAngle);

            yield return null;
        }

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (!_lastDamageTime.ContainsKey(other.gameObject) || Time.time - _lastDamageTime[other.gameObject] >= _damageCooldown)
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.DamagePlayer();
                    _lastDamageTime[other.gameObject] = Time.time;
                }
            }
        }
        
    }
}
