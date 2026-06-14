//using System;
//using UnityEngine;

//public class CardAttackCommand : ICommand
//{
//    public bool IsFinished { get; private set; }
//    private Card _card;
//    private Lane _lane;

//    public CardAttackCommand(Card card, Lane lane) 
//    {
//        _card = card;
//        _lane = lane;
//    }

//    public void Execute()
//    {
//        bool isPlayerCard = _lane.PlayerActiveCard == _card;
//        Vector3 attackDirection = isPlayerCard ? Vector3.forward : Vector3.back;
//        _card.PlayAttack(
//                onComplete: () => IsFinished = true,
//                attackDirection: attackDirection
//            );
//    }
//}
