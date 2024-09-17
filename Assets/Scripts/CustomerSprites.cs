using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CustomerSprites", menuName = "Scriptable Objects/CustomerSprites")]
public class CustomerSprites : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();

    public Sprite getRandomSprite()
    {
        return sprites[Random.Range(0, sprites.Count)];
    }
}
