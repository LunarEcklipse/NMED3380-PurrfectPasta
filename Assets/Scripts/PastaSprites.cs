using UnityEngine;

[CreateAssetMenu(fileName = "PastaSprites", menuName = "Scriptable Objects/PastaSprites")]
public class PastaSprites : ScriptableObject
{
    public Sprite blankSprite;
    public Sprite plateSprite;
    [Header("Noodle Sprites")]
    public Sprite spaghettiSprite;
    public Sprite macaroniSprite;
    public Sprite ravioliSprite;
    [Header("Sauce Sprites")]
    public Sprite tomatoSprite;
    public Sprite alfredoSprite;
    public Sprite pestoSprite;
    [Header("Topping Sprites")]
    public Sprite fishSprite;
    public Sprite birdSprite;
    public Sprite ratSprite;
}
