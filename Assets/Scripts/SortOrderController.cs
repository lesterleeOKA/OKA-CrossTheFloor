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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
