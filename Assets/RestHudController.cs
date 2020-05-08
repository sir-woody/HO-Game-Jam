using UnityEngine;

public class RestHudController : MonoBehaviour
{
    [SerializeField] GameObject statBarPrefab;
    [Space]
    [SerializeField] GameObject[] statContainers;

    void Awake()
    {
        foreach (var container in statContainers)
        {
            Clear(container);
        }
    }

    public void BindStats(Character character, int statContainerId)
    {
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
}
