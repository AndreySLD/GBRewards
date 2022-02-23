using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardView : MonoBehaviour
{
    private const string LastTimeDailyKey = "LastDailyRewardTime";
    private const string ActiveDailySlotKey = "ActiveDailySlot";

    private const string LastTimeWeeklyKey = "LastWeekRewardTime";
    private const string ActiveWeeklySlotKey = "ActiveWeeklySlot";

    #region Fields
    [Header("Time settings")]
    [SerializeField]
    public int DailyTimeCooldown = 86400;
    [SerializeField]
    public int DailyTimeDeadline = 172800;
    [SerializeField]
    public int WeeklyTimeCooldown;
    [SerializeField]
    public int WeeklyTimeDeadline;
    [Space]
    [Header("RewardSettings")]
    public List<Reward> DailyRewards;
    public List<Reward> WeeklyRewards;
    [Header("UI")]
    [SerializeField]
    public TMP_Text DailyRewardTimer;
    [SerializeField]
    public TMP_Text WeeklyRewardTimer;
    [SerializeField]
    public Transform SlotsParent;
    [SerializeField]
    public SlotRewardView SlotPrefab;
    [SerializeField]
    public Button ResetButton;
    [SerializeField]
    public Button GetDailyRewardButton;
    [SerializeField]
    public Button GetWeeklyRewardButton;
    [SerializeField]
    public Slider ProgressBarDaily;
    [SerializeField]
    public Slider ProgressBarWeekly;
    #endregion

    public int CurrentActiveDailySlot
    {
        get => PlayerPrefs.GetInt(ActiveDailySlotKey);
        set => PlayerPrefs.SetInt(ActiveDailySlotKey, value);
    }

    public int CurrentActiveWeeklySlot
    {
        get => PlayerPrefs.GetInt(ActiveWeeklySlotKey);
        set => PlayerPrefs.GetInt(ActiveWeeklySlotKey, value);
    }

    public DateTime? LastDailyRewardTime
    {
        get
        {
            var data = PlayerPrefs.GetString(LastTimeDailyKey);
            if (string.IsNullOrEmpty(data))
                return null;
            return DateTime.Parse(data);
        }
        set
        {
            if (value != null)
                PlayerPrefs.SetString(LastTimeDailyKey, value.ToString());
            else
                PlayerPrefs.DeleteKey(LastTimeDailyKey);
        }
    }

    public DateTime? LastWeeklyRewardTime
    {
        get
        {
            var data = PlayerPrefs.GetString(LastTimeWeeklyKey);
            if (string.IsNullOrEmpty(data))
                return null;
            return DateTime.Parse(data);
        }
        set
        {
            if (value != null)
                PlayerPrefs.SetString(LastTimeWeeklyKey, value.ToString());
            else
                PlayerPrefs.DeleteKey(LastTimeWeeklyKey);
        }
    }

    private void OnDestroy()
    {
        GetDailyRewardButton.onClick.RemoveAllListeners();
        GetWeeklyRewardButton.onClick.RemoveAllListeners();
        ResetButton.onClick.RemoveAllListeners();
    }

}
