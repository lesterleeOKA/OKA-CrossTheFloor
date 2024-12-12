using UnityEngine;

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
        foreach(var road in roads)
        {
            if(road != null && road.movingItem != null) {
                road.movingItem.StartNewMovement();
            }
        }
    }
}
