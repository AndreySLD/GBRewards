using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardController
{
    private readonly DailyRewardView _rewardView;
    private List<SlotRewardView> _slots;

    private bool _rewardReceived = false;

    public DailyRewardController(DailyRewardView rewardView)
    {
        _rewardView = rewardView;
        InitSlots();
        RefreshUi();
        _rewardView.StartCoroutine(UpdateCoroutine());
        SubscribeButtons();
    }

    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            Update();
            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {
        RefreshRewardState();
        RefreshUi();
    }

    private void RefreshRewardState()
    {
        _rewardReceived = false;
        if (_rewardView.LastRewardTime.HasValue)
        {
            var timeSpan = DateTime.UtcNow - _rewardView.LastRewardTime.Value;
            if (timeSpan.Seconds > _rewardView.TimeDeadline)
            {
                _rewardView.LastRewardTime = null;
                _rewardView.CurrentActiveSlot = 0;
            }
            else if(timeSpan.Seconds < _rewardView.TimeCooldown)
            {
                _rewardReceived = true;
            }
        }
    }

    private void RefreshUi()
    {
        _rewardView.GetRewardButton.interactable = !_rewardReceived;

        for (var i = 0; i < _rewardView.Rewards.Count; i++)
        {
            _slots[i].SetData(_rewardView.Rewards[i], i+1, i <= _rewardView.CurrentActiveSlot);
        }

        DateTime nextDailyBonusTime =
            !_rewardView.LastRewardTime.HasValue
                ? DateTime.MinValue
                : _rewardView.LastRewardTime.Value.AddSeconds(_rewardView.TimeCooldown);
        var delta = nextDailyBonusTime - DateTime.UtcNow;
        if (delta.TotalSeconds < 0)
            delta = new TimeSpan(0);

        _rewardView.RewardTimer.text = delta.ToString();
    }

    private void InitSlots()
    {
        _slots = new List<SlotRewardView>();
        for (int i = 0; i < _rewardView.Rewards.Count; i++)
        {
            var reward = _rewardView.Rewards[i];
            var slotInstance = GameObject.Instantiate(_rewardView.SlotPrefab, _rewardView.SlotsParent, false);
            slotInstance.SetData(reward, i+1, false);
            _slots.Add(slotInstance);
        }
    }

    private void SubscribeButtons()
    {
        _rewardView.GetRewardButton.onClick.AddListener(ClaimReward);
        _rewardView.ResetButton.onClick.AddListener(ResetReward);
    }

    private void ResetReward()
    {
        _rewardView.LastRewardTime = null;
        _rewardView.CurrentActiveSlot = 0;
    }

    private void ClaimReward()
    {
        if (_rewardReceived)
            return;
        var reward = _rewardView.Rewards[_rewardView.CurrentActiveSlot];
        switch (reward.Type)
        {
            case RewardType.None:
                break;
            case RewardType.Wood:
                CurrencyWindow.Instance.AddWood(reward.Count);
                break;
            case RewardType.Diamond:
                CurrencyWindow.Instance.AddDiamond(reward.Count);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _rewardView.LastRewardTime = DateTime.UtcNow;
        _rewardView.CurrentActiveSlot = (_rewardView.CurrentActiveSlot + 1) % _rewardView.Rewards.Count;
        RefreshRewardState();
    }
}
