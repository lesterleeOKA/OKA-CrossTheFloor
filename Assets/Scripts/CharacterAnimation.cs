using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimation : MonoBehaviour
{
    public CharacterSet characterSet;
    public float frameRate = 10f; // Frame rate for the animation

    private RawImage characterImage;
    public int currentFrame = 0;
    private Coroutine walkingCoroutine;

    void Start()
    {
        this.characterImage = GetComponent<RawImage>();
        this.setIdling();
    }

    public void PlayWalking()
    {
        // If the walking coroutine is already running, do nothing
        if (this.walkingCoroutine != null) return;

        // Start the walking animation coroutine
        this.walkingCoroutine = StartCoroutine(this.walkingAnimation());
    }
    private IEnumerator walkingAnimation()
    {
        int currentFrame = 0;

        while (true) // Loop indefinitely while walking
        {
            if (this.characterImage != null)
            {
                this.characterImage.texture = this.characterSet.walkingAnimationTextures[currentFrame];
            }

            currentFrame = (currentFrame + 1) % this.characterSet.walkingAnimationTextures.Length;
            yield return new WaitForSeconds(1f / this.frameRate); // Wait for the frame duration
        }
    }

    // Call this method to switch animation sets
    public void setIdling()
    {
        // Stop the walking coroutine if it's running
        if (this.walkingCoroutine != null)
        {
            StopCoroutine(this.walkingCoroutine);
            this.walkingCoroutine = null; // Clear the reference
        }

        if (this.characterImage != null)
            this.characterImage.texture = this.characterSet.idlingTexture;
    }
}