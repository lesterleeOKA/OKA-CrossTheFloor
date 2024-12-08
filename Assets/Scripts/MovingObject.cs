using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MovingObject : MonoBehaviour
{
    public int sortLayer = 0;
    public Texture[] objectTextures;
    public float startPosX;
    public RawImage objectImage; // Reference to the RawImage UI element
    public float minSpeed = 1f; // Minimum speed
    public float maxSpeed = 5f; // Maximum speed
    private RectTransform rectTransform = null; // RectTransform of the RawImage
    // Start is called before the first frame update
    void Start()
    {
        if (this.objectImage == null) this.objectImage = this.GetComponent<RawImage>();
        if (this.rectTransform == null) this.rectTransform = this.GetComponent<RectTransform>();
        this.StartNewMovement();
    }

    public enum MovingDirection
    {
        None,
        Left,
        Right
    }

    private void StartNewMovement()
    {
        if(this.objectImage != null) this.objectImage.texture = this.randomObjectTex;
        // Randomize speed
        float speed = Random.Range(minSpeed, maxSpeed);

        // Determine the target position
        Vector2 targetPosition = Vector2.zero;

        this.rectTransform.anchoredPosition = new Vector2(this.startPosX, this.rectTransform.anchoredPosition.y);
        targetPosition = new Vector2(-(this.startPosX), this.rectTransform.anchoredPosition.y);
        // Use DOTween to move the car
        this.rectTransform.DOAnchorPos(targetPosition, speed).SetEase(Ease.Linear).OnComplete(StartNewMovement);
    }

    public Texture randomObjectTex
    {
        get
        {
            if (this.objectTextures != null && this.objectTextures.Length > 0)
            {
                int randomId = Random.Range(0, this.objectTextures.Length);
                return this.objectTextures[randomId];
            }
            else return null;
        }

    
    }
}
