using System;
using System.Threading.Tasks;
using Core.FileUtil;
using Core.Json;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class MoneyManager
{
    private class MoneyData
    {
        public long Money;
    }

    private const string SAVE_PATH = "money_data.json";

    public Action<long> OnMoneyChange;
    public long Money => _moneyData.Money;
    private MoneyData _moneyData;
    private int _saveThreshold = 5;
    private int _threshold = 0;

    public static async UniTask Init()
    {
        var moneyManager = new MoneyManager();
        moneyManager._moneyData = new MoneyData();
        if (FileUtility.IsFileExist(SAVE_PATH))
        {
            var savedData = JsonSaveLoad.QuickLoadFromJson<MoneyData>(SAVE_PATH, new());
            if (savedData != null)
            {
                moneyManager._moneyData = savedData;
            }
        }
        else
        {
            var json = await SmartAddressable.LoadAsync<TextAsset>("money_data_default");
            moneyManager._moneyData = JsonConvert.DeserializeObject<MoneyData>(json.text);
        }

        ServiceManager.Register(moneyManager).As<MoneyManager>();
    }

    public void AddMoney(int money)
    {
        _moneyData.Money += money;
        OnMoneyChange?.Invoke(_moneyData.Money);
        SaveWhenThresholdReach();
    }

    public bool SpendMoney(int money)
    {
        if (_moneyData.Money < money)
        {
            return false;
        }
        else
        {
            _moneyData.Money -= money;
            OnMoneyChange?.Invoke(_moneyData.Money);
            SaveWhenThresholdReach();
            return true;
        }
    }

    private void SaveWhenThresholdReach()
    {
        _threshold++;
        if (_threshold >= _saveThreshold)
        {
            _threshold = 0;
            JsonSaveLoad.QuickSaveToJson(_moneyData, SAVE_PATH);
        }
    }

    public void ForceSave()
    {
        JsonSaveLoad.QuickSaveToJson(_moneyData, SAVE_PATH);
    }
}