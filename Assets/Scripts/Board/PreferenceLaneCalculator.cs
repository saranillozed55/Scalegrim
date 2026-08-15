using UnityEngine;

public class PreferenceLaneCalculator
{
    private EnemyAttackPreference preference;
    private BoardState boardState;
    public PreferenceLaneCalculator(EnemyAttackPreference pref)
    {
        preference = pref;
        boardState = BoardLaneManager.Instance.CaptureBoardState();
    }

    public float EvaluateLaneWithPreference()
    {
        switch (preference) {
            case EnemyAttackPreference.PreferAttackEmptyLane:
                return FindAttackEmptyLane();
            case EnemyAttackPreference.PreferDefendLane:
                return FindDefendLane();
            default:
                return 0;
        }
    }

    //get boardstate -> find best lane for the current preference then we want to choose that lane index

    private float FindAttackEmptyLane()
    {
        foreach (LaneSnapShot shot in boardState.LanesShot)
        {
            bool hasplayerCard = shot.PlayerCard != null;
            bool hasEnemyCard = shot.EnemyCard != null;

            if (hasEnemyCard)
            {
                
            }

            if(hasplayerCard)
            {

            }

        }
        return 0;
    }
    private float FindDefendLane()
    {
        return 0;
    }
}
