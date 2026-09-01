using UnityEngine.UIElements;

namespace Core.UI
{
    public enum UIType
    {
        Main,
        Popup,
        Overlay
    }

    public interface IUIDocumentController<T> : IUIDocumentController
    {
        public UIType UIType { get; }
        public UIDocument UIDocument { get; }
        public void Show(T data);
    }

    public interface IUIDocumentController
    {
        public void Hide();
    }
}