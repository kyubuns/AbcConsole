using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AbcConsole.Internal
{
    public class OnPointerDownButton : EventTrigger
    {
        public class ButtonClickedEvent : UnityEvent {}

        // ReSharper disable once InconsistentNaming
        public ButtonClickedEvent onClick { get; } = new ButtonClickedEvent();

        public override void OnPointerDown(PointerEventData data)
        {
            onClick.Invoke();
        }
    }
}