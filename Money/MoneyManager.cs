using System;
using Core.Json;
using UnityEngine;

public class MoneyManager
{
    private class MoneyData
    {
        public long Money;
    }

    public Action<long> OnMoneyChange;
    public long Money => _moneyData.Money;
    private MoneyData _moneyData;
    private int _saveThreshold = 5;
    private int _threshold = 0;

    public static void Init()
    {
        var moneyManager = new MoneyManager();
        moneyManager._moneyData = new MoneyData();
        var savedData = JsonSaveLoad.QuickLoadFromJson<MoneyData>("MoneyData", new());
        if (savedData != null)
        {
            moneyManager._moneyData = savedData;
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
            JsonSaveLoad.QuickSaveToJson(_moneyData, "MoneyData");
        }
    }

    public void ForceSave()
    {
        JsonSaveLoad.QuickSaveToJson(_moneyData, "MoneyData");
    }
}