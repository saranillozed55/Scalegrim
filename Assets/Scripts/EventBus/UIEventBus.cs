using System;
using UnityEngine;

namespace UI.Events
{
    public static class UIEventBus
    {
        public static event Action OnCloseOptionsButtonPressed; // do I need to make this return bool? also haven't used this for anything yet

        public static void RaiseOnCloseOptionsButtonPressed()
        {
            OnCloseOptionsButtonPressed?.Invoke();
        }

        public static event Action OnOptionsButtonClicked;
        //Raised in PauseMenu, Subscribed in OptionsMenu

        public static void RaiseOnOptionsButtonClicked()
        {
            OnOptionsButtonClicked?.Invoke();
        }
    }
}
