using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private int _maxCards = 5; // enemy shouldnt have _maxCards unless we want it to
    [SerializeField] private Card _cardPrefab;

    [SerializeField] private List<EnemyPrepArea> _prepArea;

    private List<Card> _enemyHand = new();

    private UtilityAI _utilityAI = new UtilityAI();

    private void Start()
    {
        InitalizeEnemyDeck();
        BoardLaneManager.Instance.PlaceEnemyCardsInQueue(_cardPrefab, 0);
        //BoardLaneManager.Instance.PlaceEnemyCardsInQueue(_cardPrefab, 1);
        BoardLaneManager.Instance.PlaceEnemyCardsInQueue(_cardPrefab, 2);
    }

    private void InitalizeEnemyDeck()
    {
        _enemyHand.Clear();
        for (int i = 0; i < _maxCards; i++)
        {
            _enemyHand.Add(_cardPrefab);
        }
    }
    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            BoardLaneManager.Instance.AdvanceEnemyCardsFromQueue();
        }
    }

    //Might want this to be like PlayEnemyStartingHand instead and place it in BoardLaneManger
    //private void PlayStartingHand(List<Card> cards, int laneIndex) // We want parameters for this later on because we want to determine which lane we want to put it in and 
    //{
    //    for (int i = 0; i < _prepArea.Count; i++)
    //    {
    //        int localIndex = i;
    //        GameObject instance = Instantiate(_enemyHand[i].gameObject, _prepArea[i]._cardSpawnLocation.position, _prepArea[i]._cardSpawnLocation.rotation);
    //        Card cardInstance = instance.GetComponent<Card>();

    //        // can add delay when this work
    //        // have to check if a card is already there. Can be done in Utility AI I believe. This is just a test code
    //        Vector3 position = _prepArea[i].transform.position;

    //        instance.transform.DOKill();
    //        instance.transform.DOMove(position, 0.3f);//delay here
    //        instance.transform.DORotateQuaternion(CardRotations._cardFaceFlatUp, 0.3f).OnComplete(() =>
    //        {
    //            _prepArea[localIndex].SetCardInArea(cardInstance);
    //        });
    //    }
    //}
}
