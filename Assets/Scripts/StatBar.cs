using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    Stat _stat;
    [SerializeField] Slider _slider;
    [SerializeField] TMP_Text statNameField;
    [SerializeField] Image statIcon;

    internal void Initialize(Stat stat)
    {
        _stat = stat;
        statIcon.sprite = stat.GetSprite();
        _stat.OnChange += Stat_OnChange;
        _stat.Refresh();
        statNameField.text = stat.name;
    }

    private void Stat_OnChange(float value, float maxValue)
    {
        _slider.maxValue = maxValue;
        _slider.value = value;
    }

    void OnDestory()
    {
        _stat.OnChange -= Stat_OnChange;
    }
}
