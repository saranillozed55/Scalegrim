using System;
using UnityEngine;

public class CardAttackCommand : ICommand
{
    public bool IsFinished { get; private set; }
    private Card _card;
    private Lane _lane;
    private Action _onImpactCallback;

    public CardAttackCommand(Card card, Lane lane, Action onImpactCallback)
    {
        _card = card;
        _lane = lane;
        _onImpactCallback = onImpactCallback;
    }

    public void Execute()
    {
        bool isPlayerCard = _lane.PlayerActiveCard == _card;
        Vector3 attackDirection = isPlayerCard ? Vector3.forward : Vector3.back;
        _card.PlayAttack(
                onImpact: () => _onImpactCallback?.Invoke(),
                onComplete: () => IsFinished = true,
                attackDirection: attackDirection
            );
    }
}
