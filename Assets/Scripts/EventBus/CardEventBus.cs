using System;

namespace Cards.Events
{
    public static class CardEventBus
    {
        public static event Action<int> OnDirectPlayerDamage;
        public static event Action<int> OnDirectEnemyDamage;

        public static void RaiseOnDirectPlayerDamage(int val)
        {
            OnDirectPlayerDamage?.Invoke(val);
        }
        public static void RaiseOnDirectEnemyDamage(int val)
        {
            OnDirectEnemyDamage?.Invoke(val);
        }
    }
}
