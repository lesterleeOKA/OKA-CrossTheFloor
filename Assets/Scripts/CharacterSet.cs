using System;
using UnityEngine;

[Serializable]
public class CharacterSet
{
    public string name;
    public int playerNumber;
    public Texture idlingTexture;
    public Texture[] walkingAnimationTextures;
    public Vector3 positionRangeX;
    public Vector3 positionRangeY;
}
