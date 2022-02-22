using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyWindow : MonoBehaviour
{
    private const string WoodKey = "Wood";
    private const string DiamondKey = "Diamond";

    public static CurrencyWindow Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI _diamondText;
    [SerializeField]
    private TextMeshProUGUI _woodText;

    private void Start()
    {
        RefreshText();
    } 

    private int Wood
    {
        get => PlayerPrefs.GetInt(WoodKey);
        set => PlayerPrefs.SetInt(WoodKey, value);
    }

    private int Diamond
    {
        get => PlayerPrefs.GetInt(DiamondKey);
        set => PlayerPrefs.SetInt(DiamondKey, value);
    }

    public void AddDiamond(int count)
    {
        Diamond += count;
        RefreshText();
    }

    public void AddWood(int count)
    {
        Wood += count;
        RefreshText();
    }

    private void RefreshText()
    {
        if (_diamondText != null)
            _diamondText.text = Diamond.ToString();
        if (_woodText != null)
            _woodText.text = Wood.ToString();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
