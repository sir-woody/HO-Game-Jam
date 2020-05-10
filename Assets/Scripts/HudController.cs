using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] GameObject statBarPrefab;
    [Space]
    [SerializeField] GameObject[] statContainers;
    [SerializeField] bool displayCharactersNames;

    void Awake()
    {
        foreach (var container in statContainers)
        {
            Clear(container);
        }
    }

    public void BindStats(Character character, int statContainerId)
    {
        if (displayCharactersNames)
        {
            var characterNameTextField = statContainers[statContainerId].transform.parent.GetComponentInChildren<TMP_Text>();
            if (characterNameTextField != null)
            {
                characterNameTextField.text = character.name;
            }
        }

        var stats = character.GetComponent<StatController>().stats;

        foreach (var stat in stats)
        {
            var statBar = Instantiate(statBarPrefab, statContainers[statContainerId].transform);
            statBar.GetComponent<StatBar>().Initialize(stat);
        }
    }

    private void Clear(GameObject statContainer)
    {
        foreach (RectTransform child in statContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
