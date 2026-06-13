using UnityEngine;

public class LaneView : MonoBehaviour
{

    [Header("Lane Index")]
    public int laneIndex;

    [Header("Physical Slot References")]
    [SerializeField] private EnemyPrepArea enemyPrepArea; // Queue slot
    [SerializeField] private CardDropArea enemyActiveArea; // Front slot
    [SerializeField] private CardDropArea playerActiveArea;

    public EnemyPrepArea EnemyPrepArea => enemyPrepArea;
    public CardDropArea EnemyActiveArea => enemyActiveArea;
    public CardDropArea PlayerActiveArea => playerActiveArea;
}
