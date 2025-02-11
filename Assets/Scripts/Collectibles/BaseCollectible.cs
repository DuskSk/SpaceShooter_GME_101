using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCollectible : MonoBehaviour
{
    [SerializeField] protected float _speed = 6f;
    [SerializeField] protected float _yLimitToDestroy = -8f;
    [SerializeField] protected AudioClip _audioClip;
    
    protected virtual void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < _yLimitToDestroy)
        {
            Destroy(this.gameObject);
        }
    }

    protected abstract void ApplyEffect(Player player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                ApplyEffect(player);
                Destroy(this.gameObject);
            }
        }
    }



}
