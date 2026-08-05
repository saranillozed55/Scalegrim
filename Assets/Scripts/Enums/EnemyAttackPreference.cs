using UnityEngine;

public enum EnemyAttackPreference
{
    None,
    PreferAttackEmptyLane,
    PreferDefendLane,
    PreferAttackLaneWithLowestHealth,
}
