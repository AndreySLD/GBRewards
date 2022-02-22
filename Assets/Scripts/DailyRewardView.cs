using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardView : MonoBehaviour
{
    private const string LastTimeKey = "LastRewardTime";
    private const string ActiveSlotKey = "ActiveSlot";

    #region Fields
    [Header("Time settings")]
    [SerializeField]
    public int TimeCooldown = 86400;
    [SerializeField]
    public int TimeDeadline = 172800;
    [Space]
    [Header("RewardSettings")]
    public List<Reward> Rewards;
    [Header("UI")]
    [SerializeField]
    public TMP_Text RewardTimer;
    [SerializeField]
    public Transform SlotsParent;
    [SerializeField]
    public SlotRewardView SlotPrefab;
    [SerializeField]
    public Button ResetButton;
    [SerializeField]
    public Button GetRewardButton;
    #endregion

    public int CurrentActiveSlot
    {
        get => PlayerPrefs.GetInt(ActiveSlotKey);
        set => PlayerPrefs.SetInt(ActiveSlotKey, value);
    }

    public DateTime? LastRewardTime
    {
        get
        {
            var data = PlayerPrefs.GetString(LastTimeKey);
            if (string.IsNullOrEmpty(data))
                return null;
            return DateTime.Parse(data);
        }
        set
        {
            if (value != null)
                PlayerPrefs.SetString(LastTimeKey, value.ToString());
            else
                PlayerPrefs.DeleteKey(LastTimeKey);
        }
    }


    private void OnDestroy()
    {
        GetRewardButton.onClick.RemoveAllListeners();
        ResetButton.onClick.RemoveAllListeners();
    }

}
