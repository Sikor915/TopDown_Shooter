using UnityEngine.UI;
using UnityEngine;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text titleText;
    [SerializeField] TMPro.TMP_Text descriptionText;
    [SerializeField] TMPro.TMP_Text statsText;
    [SerializeField] Button chooseButton;

    void OnEnable()
    {
        chooseButton.onClick.AddListener(() => { UpgradeUIManager.Instance.onUpgradeChosen?.Invoke(); });
    }

    public void SetCard(string title, string description, string stats, StatUpgradeSO upgradeSO)
    {
        titleText.text = title;
        descriptionText.text = description;
        statsText.text = stats;
        chooseButton.onClick.AddListener(upgradeSO.ApplyUpgrade);
    }
}