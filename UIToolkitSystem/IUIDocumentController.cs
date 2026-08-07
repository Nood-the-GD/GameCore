using UnityEngine.UIElements;

namespace Core.UI
{
    public enum UIType
    {
        Main,
        Popup,
        Overlay
    }

    public interface IUIDocumentController<T> : IUIDocumentController  where T : struct
    {
        public UIType UIType { get; }
        public UIDocument UIDocument { get; }
        public void Show(T data);
        public void Hide();
    }

    public interface IUIDocumentController{}
}