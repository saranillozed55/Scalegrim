using UnityEngine;

public static class CardRotations
{

    public static Quaternion _cardFacePlayerVertical = Quaternion.Euler(-90f, 180f, 0);
    public static Quaternion _cardFaceFlatUp = Quaternion.Euler(180f, -180f, 0);
    public static Quaternion _cardFaceFlatDown = Quaternion.identity;

}
