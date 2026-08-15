//using UnityEngine;

//public class BoardEvaluater
//{
//    public float EvaluateLane(AIPersonality profile, LaneSnapShot lane) // Evaluate lane based on AI personality profile and with EnemyAttackPreference use it to multiply the score of the lane

//    {
//        float laneScore = 0f;

//        bool hasEnemyCard = lane.EnemyCard.HasValue;
//        bool hasPlayerCard = lane.PlayerCard.HasValue;

//        float defense = profile.DefensiveMultiplier;
//        float aggression = profile.AggressionMultipler;


//        //Enemy and Card in Play Areas
//        if (hasEnemyCard && hasPlayerCard)
//        {
//            CardSnapShot playerCard = lane.PlayerCard.Value;
//            CardSnapShot enemyCard = lane.EnemyCard.Value;
//            Debug.Log($"<color=red> Both AI and Player have cards in lane {lane.LaneIndex + 1}. Therefore, shouldn't queue any cards in this lane. </color>");

//            //Scale with defense: higher defense cares more about stopping incoming damage
//            laneScore -= playerCard.Attack * defense;
//            if (enemyCard.Health > playerCard.Attack)
//            {
//                laneScore += profile.BlockPlayerBonus;
//            }
//        }

//        //No Enemy Card but there is Player Card in play
//        else if (!hasEnemyCard && hasPlayerCard)
//        {
//            Debug.Log($"<color=green> Only Player has a card in lane {lane.LaneIndex + 1}! Player can attack enemy directly! </color>");

//            CardSnapShot playerCard = lane.PlayerCard.Value;

//            laneScore += playerCard.Attack * defense;
//        }

//        //no player card in lane
//        else if (hasEnemyCard && !hasPlayerCard)
//        {
//            CardSnapShot enemyCard = lane.EnemyCard.Value;
//            Debug.Log($"<color=green> No cards in lane {lane.LaneIndex + 1}. Enemy can attack player directly! </color>");

//            float potentialDamage = enemyCard.Attack;

//            if (potentialDamage >= Player.CurrentPlayerHealth)
//            {
//                laneScore += profile.KillPlayerBonus;
//            }
//            else
//            {
//                //scaled by agression
//                laneScore += potentialDamage * aggression;
//            }
//        }
//        else
//        {
//            laneScore += profile.EmptyLanePenalty;
//        }
//        return laneScore;
//    }
//}
using System;
using UnityEngine;
public class BoardEvaluater
{
    //Do we even need this? -> Want to evaluate lane based on
    //Don't want to return high values if there is already an enemyCard in that lane because we can't play the card in that lane
    public float EvaluateLane(BoardState boardState)
    {
        
        foreach(LaneSnapShot shot in boardState.LanesShot)
        {
            bool hasplayerCard = shot.PlayerCard != null;
            bool hasEnemyCard = shot.EnemyCard != null;

            if (hasplayerCard && hasEnemyCard)
            {
                // may change later
                return -999;
            }
            else if (hasplayerCard && !hasEnemyCard)
            {
                CardModel playerCardModel = shot.PlayerCard.Value.cardModel;
                int damage = playerCardModel.AttackDamage;
                return damage;
            }
            
        }
        return 0;
    }


}

