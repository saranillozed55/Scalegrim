using System.Collections.Generic;
using UnityEngine;

public class LaneView : MonoBehaviour
{

    [Header("Lane Index")]
    public int laneIndex;

    [Header("Physical Slot References")]
    [SerializeField] private EnemyPrepArea enemyPrepArea; // Queue slot
    [SerializeField] private CardDropArea enemyActiveArea; // Front slot
    [SerializeField] private CardDropArea playerActiveArea; // Player slot
    public List<CardDropArea> activeAreas = new();

    private void Awake()
    {
        activeAreas = new List<CardDropArea> { enemyActiveArea, playerActiveArea };
    }

    public EnemyPrepArea EnemyPrepArea => enemyPrepArea;
    public CardDropArea EnemyActiveArea => enemyActiveArea;
    public CardDropArea PlayerActiveArea => playerActiveArea;
}
