using UnityEngine;
using System.Collections;

public class SortOrderController : MonoBehaviour
{
    public static SortOrderController Instance = null;
    public SortRoad[] roads;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void startMovingObjects()
    {
        StartCoroutine(startMovingItem());
    }


    private IEnumerator startMovingItem(float delay = 0.5f)
    {
        if (LoaderConfig.Instance != null && LoaderConfig.Instance.gameSetup.maxRoadNumber == 2)
        {
            this.roads[2].direction = SortRoad.Direction.none;
            this.roads[3].direction = SortRoad.Direction.toLeft;
        }
        for (int i = 0; i < this.roads.Length; i++)
        {
            if (this.roads[i] != null && this.roads[i].direction != SortRoad.Direction.none)
            {
                this.roads[i].InitRoad();
                yield return new WaitForSeconds(delay);
                this.roads[i].startMovingItems(i);
            }
        }
    }
}
