using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.UI
{
    public class UIToolkitSystem : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<UIType, PanelSettings> _uiPanelSettingDict = new();

        private Dictionary<string, IUIDocumentController> _loadedUI = new();

        void Awake()
        {
            ServiceManager.Register(this).As<UIToolkitSystem>();
        }

        public async UniTask<IUIDocumentController<T>> LoadUI<T>(string uiName, T uiData) where T : struct
        {
            try
            {
                if (_loadedUI.ContainsKey(uiName))
                {
                    var controller = _loadedUI[uiName] as IUIDocumentController<T>;
                    controller.Show(uiData);
                    return controller;
                }
                else
                {
                    var uiAsset = await SmartAddressable.LoadAsync<GameObject>(uiName);
                    var spawnAsset = Instantiate(uiAsset, this.transform);
                    var controller = spawnAsset.GetComponent<IUIDocumentController<T>>();
                    controller.Show(uiData);
                    _loadedUI[uiName] = controller;
                    controller.UIDocument.panelSettings = _uiPanelSettingDict[controller.UIType];
                    return controller;
                }
            }
            catch
            {
                Debug.Log("Do popup with name");
                // LoadUI<EmptyUIData>("ErrorPopup", default).Forget();
            }
            return null;
        }

        public async void UnloadUI(string uiName)
        {
            if (_loadedUI.ContainsKey(uiName))
            {
                var ui = _loadedUI[uiName];
                ui.Hide();
            }
            else
            {
                Debug.LogError($"Can't find ui {uiName} to close");
            }
        }
    }
}
