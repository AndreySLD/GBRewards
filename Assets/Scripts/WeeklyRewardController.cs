using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeeklyRewardController
{
    private readonly RewardView _rewardView;
    private List<SlotRewardView> _slots;

    private bool _rewardReceived = false;

    public WeeklyRewardController(RewardView rewardView)
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
        if (_rewardView.LastWeeklyRewardTime.HasValue)
        {
            var timeSpan = DateTime.UtcNow - _rewardView.LastWeeklyRewardTime.Value;
            if (timeSpan.Seconds > _rewardView.WeeklyTimeDeadline)
            {
                _rewardView.LastWeeklyRewardTime = null;
                _rewardView.CurrentActiveWeeklySlot = 0;
            }
            else if (timeSpan.Seconds < _rewardView.WeeklyTimeCooldown)
            {
                _rewardReceived = true;
            }
        }
    }

    private void RefreshUi()
    {

        _rewardView.GetWeeklyRewardButton.interactable = !_rewardReceived;

        for (var i = 0; i < _rewardView.WeeklyRewards.Count; i++)
        {
            _slots[i].SetData(_rewardView.WeeklyRewards[i], i + 1, i <= _rewardView.CurrentActiveWeeklySlot);
        }

        DateTime nextWeekBonusTime =
            !_rewardView.LastWeeklyRewardTime.HasValue
                ? DateTime.MinValue
                : _rewardView.LastWeeklyRewardTime.Value.AddSeconds(_rewardView.WeeklyTimeCooldown);
        var delta = nextWeekBonusTime - DateTime.UtcNow;
        if (delta.TotalSeconds < 0)
            delta = new TimeSpan(0);

        _rewardView.WeeklyRewardTimer.text = delta.ToString();
        _rewardView.ProgressBarWeekly.value = (float)delta.TotalSeconds;
    }

    private void InitSlots()
    {
        _slots = new List<SlotRewardView>();
        for (int i = 0; i < _rewardView.WeeklyRewards.Count; i++)
        {
            var reward = _rewardView.WeeklyRewards[i];
            var slotInstance = GameObject.Instantiate(_rewardView.SlotPrefab, _rewardView.SlotsParent, false);
            slotInstance.SetData(reward, i + 1, false);
            _slots.Add(slotInstance);
        }

        _rewardView.ProgressBarWeekly.maxValue = _rewardView.WeeklyTimeCooldown;
    }

    private void SubscribeButtons()
    {
        _rewardView.ResetButton.onClick.AddListener(ResetReward);
        _rewardView.GetWeeklyRewardButton.onClick.AddListener(ClaimReward);
    }

    private void ResetReward()
    {
        _rewardView.LastWeeklyRewardTime = null;
        _rewardView.CurrentActiveWeeklySlot = 0;
    }

    private void ClaimReward()
    {
        if (_rewardReceived)
        {
            return;
        }

        var reward = _rewardView.WeeklyRewards[_rewardView.CurrentActiveWeeklySlot];
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

        _rewardView.LastWeeklyRewardTime = DateTime.UtcNow;
        _rewardView.CurrentActiveWeeklySlot = (_rewardView.CurrentActiveWeeklySlot + 1) % _rewardView.WeeklyRewards.Count;
        RefreshRewardState();
    }
}