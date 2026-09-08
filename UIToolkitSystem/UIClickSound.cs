using UnityEngine.UIElements;
using SoundManagerService = Core.SoundManager.SoundManager;

namespace Core.UI
{
    public static class UIClickSound
    {
        // Registered on a document root during trickle-down so every ClickEvent inside
        // that panel plays the shared UI click sound, even if a child stops bubbling.
        public static void Bind(VisualElement root)
        {
            if (root == null) return;
            root.RegisterCallback<ClickEvent>(OnClick, TrickleDown.TrickleDown);
        }

        private static void OnClick(ClickEvent evt)
        {
            if (!IsClickable(evt.target as VisualElement)) return;
            if (ServiceManager.TryGet<SoundManagerService>(out var sound))
                sound.PlaySound(SoundEnum.UI_Click);
        }

        // The ClickEvent target is the deepest picked element, which may be a Label/Image
        // inside a Button, so walk up to find an interactive control.
        private static bool IsClickable(VisualElement element)
        {
            for (var current = element; current != null; current = current.parent)
            {
                if (current is Button || current is Toggle) return true;
            }
            return false;
        }
    }
}
