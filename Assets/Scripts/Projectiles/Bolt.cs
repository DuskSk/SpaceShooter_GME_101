using UnityEngine;

public class Bolt : BaseWeapon, IProjectile
{
    protected Space _spaceRelativeTo;
    
    void Start()
    {
        InvokeRepeating(nameof(RepeatScreenChecking), 0.1f, 0.5f);
    }

    
    void Update()
    {
        Move();
    }

    protected override void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, _spaceRelativeTo);
        
    }

    private void RepeatScreenChecking()
    {
        this.DestroyOnScreenExit(transform.position, this.gameObject);
    }
    public override void Initialize(Vector3 direction, float speed, bool isEnemy = false)
    {
        _direction = direction;
        _speed = speed;
        _isEnemyWeapon = isEnemy;
    }

    public void Initialize(Vector3 direction, float speed, bool isEnemy = false, Space spaceRelativeTo = Space.Self)
    {
        _direction = direction;
        _speed = speed;
        _isEnemyWeapon = isEnemy;
        _spaceRelativeTo = spaceRelativeTo;
    }

    
}
