using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField]
    private RewardView _rewardView;

    private DailyRewardController _dailyRewardController;
    private WeeklyRewardController _weeklyRewardController;


    void Start()
    {
        _dailyRewardController = new DailyRewardController(_rewardView);
        _weeklyRewardController = new WeeklyRewardController(_rewardView);
    }
}
