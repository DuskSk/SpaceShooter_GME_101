
using UnityEngine;

public class Debuff : BaseCollectible
{
    private enum DebuffType { Slow };
    [SerializeField] private DebuffType _debuffType;   

    
    void Update()
    {
        Move();
    }

    protected override void ApplyEffect(Player player)
    {
        switch (_debuffType)
        {
            case DebuffType.Slow:
                player.ApplySlow();
                break;
        }
    }
}
