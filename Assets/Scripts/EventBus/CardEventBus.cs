using System;

namespace Cards.Events
{
    public static class CardEventBus
    {
        public static event Action<int> OnDirectDamage;

        public static void RaiseOnDirectDamage(int val)
        {
            OnDirectDamage?.Invoke(val);
        }
    }
}
