using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardController
{
    private readonly RewardView _rewardView;
    private List<SlotRewardView> _slots;

    private bool _rewardReceived = false;

    public DailyRewardController(RewardView rewardView)
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
        if (_rewardView.LastDailyRewardTime.HasValue)
        {
            var timeSpan = DateTime.UtcNow - _rewardView.LastDailyRewardTime.Value;
            if (timeSpan.Seconds > _rewardView.DailyTimeDeadline)
            {
                _rewardView.LastDailyRewardTime = null;
                _rewardView.CurrentActiveDailySlot = 0;
            }
            else if(timeSpan.Seconds < _rewardView.DailyTimeCooldown)
            {
                _rewardReceived = true;
            }
        }
    }

    private void RefreshUi()
    {
        _rewardView.GetDailyRewardButton.interactable = !_rewardReceived;

        for (var i = 0; i < _rewardView.DailyRewards.Count; i++)
        {
            _slots[i].SetData(_rewardView.DailyRewards[i], i+1, i <= _rewardView.CurrentActiveDailySlot);
        }

        DateTime nextDailyBonusTime =
            !_rewardView.LastDailyRewardTime.HasValue
                ? DateTime.MinValue
                : _rewardView.LastDailyRewardTime.Value.AddSeconds(_rewardView.DailyTimeCooldown);
        var delta = nextDailyBonusTime - DateTime.UtcNow;
        if (delta.TotalSeconds < 0)
            delta = new TimeSpan(0);

        _rewardView.DailyRewardTimer.text = delta.ToString();
        _rewardView.ProgressBarDaily.value = (float)delta.TotalSeconds;
    }

    private void InitSlots()
    {
        _slots = new List<SlotRewardView>();
        for (int i = 0; i < _rewardView.DailyRewards.Count; i++)
        {
            var reward = _rewardView.DailyRewards[i];
            var slotInstance = GameObject.Instantiate(_rewardView.SlotPrefab, _rewardView.SlotsParent, false);
            slotInstance.SetData(reward, i+1, false);
            _slots.Add(slotInstance);
        }

        _rewardView.ProgressBarDaily.maxValue = _rewardView.DailyTimeCooldown;
    }

    private void SubscribeButtons()
    {
        _rewardView.GetDailyRewardButton.onClick.AddListener(ClaimReward);
        _rewardView.ResetButton.onClick.AddListener(ResetReward);
    }

    private void ResetReward()
    {
        _rewardView.LastDailyRewardTime = null;
        _rewardView.CurrentActiveDailySlot = 0;
    }

    private void ClaimReward()
    {
        if (_rewardReceived)
            return;
        var reward = _rewardView.DailyRewards[_rewardView.CurrentActiveDailySlot];
        switch (reward.TypeByCur)
        {
            case RewardTypeByCurrency.None:
                break;
            case RewardTypeByCurrency.Wood:
                CurrencyWindow.Instance.AddWood(reward.Count);
                break;
            case RewardTypeByCurrency.Diamond:
                CurrencyWindow.Instance.AddDiamond(reward.Count);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _rewardView.LastDailyRewardTime = DateTime.UtcNow;
        _rewardView.CurrentActiveDailySlot = (_rewardView.CurrentActiveDailySlot + 1) % _rewardView.DailyRewards.Count;
        RefreshRewardState();
    }
}
