using System.Collections.Generic;
using UnityEngine;

public enum PastaNoodle
{
    SPAGHETTI,
    MACARONI,
    RAVIOLI,
    NONE
}
public enum PastaSauce
{
    TOMATO,
    ALFREDO,
    PESTO,
    NONE
}
public enum PastaTopping
{
    FISH,
    BIRD,
    RAT,
    NONE
}
public class Pasta : MonoBehaviour
{
    public PastaNoodle noodle = PastaNoodle.NONE;
    public PastaSauce sauce = PastaSauce.NONE;
    public PastaTopping topping = PastaTopping.NONE;
    private SpriteRenderer plateRenderer;
    private SpriteRenderer noodleRenderer;
    private SpriteRenderer sauceRenderer;
    private SpriteRenderer toppingRenderer;
    // Add a header here to separate in the inspector
    public PastaSprites pastaSprites;

    public void ClearPastaPlate()
    {
        SetNoodles(PastaNoodle.NONE);
        SetSauce(PastaSauce.NONE);
        SetTopping(PastaTopping.NONE);
        sauce = PastaSauce.NONE;
        topping = PastaTopping.NONE;
    }

    public void SetNoodles(PastaNoodle noodle)
    {
        this.noodle = noodle;
        switch(noodle)
        {
            case PastaNoodle.SPAGHETTI:
                noodleRenderer.sprite = pastaSprites.spaghettiSprite;
                break;
            case PastaNoodle.MACARONI:
                noodleRenderer.sprite = pastaSprites.macaroniSprite;
                break;
            case PastaNoodle.RAVIOLI:
                noodleRenderer.sprite = pastaSprites.ravioliSprite;
                break;
            case PastaNoodle.NONE:
                noodleRenderer.sprite = pastaSprites.blankSprite;
                break;
        }
    }
    public void SetSauce(PastaSauce sauce)
    {
        this.sauce = sauce;
        switch(sauce)
        {
            case PastaSauce.TOMATO:
                sauceRenderer.sprite = pastaSprites.tomatoSprite;
                break;
            case PastaSauce.ALFREDO:
                sauceRenderer.sprite = pastaSprites.alfredoSprite;
                break;
            case PastaSauce.PESTO:
                sauceRenderer.sprite = pastaSprites.pestoSprite;
                break;
            case PastaSauce.NONE:
                sauceRenderer.sprite = pastaSprites.blankSprite;
                break;
        }
    }
    public void SetTopping(PastaTopping topping)
    {
        this.topping = topping;
        switch(topping)
        {
            case PastaTopping.FISH:
                toppingRenderer.sprite = pastaSprites.fishSprite;
                break;
            case PastaTopping.BIRD:
                toppingRenderer.sprite = pastaSprites.birdSprite;
                break;
            case PastaTopping.RAT:
                toppingRenderer.sprite = pastaSprites.ratSprite;
                break;
            case PastaTopping.NONE:
                toppingRenderer.sprite = pastaSprites.blankSprite;
                break;
        }
    }
    void Start()
    {
        if (!GameObject.Find("PastaPlate").TryGetComponent<SpriteRenderer>(out plateRenderer))
        {
            Debug.LogError("PastaPlate is missing a SpriteRenderer component.");
        }
        if (!GameObject.Find("PastaNoodle").TryGetComponent<SpriteRenderer>(out noodleRenderer))
        {
            Debug.LogError("PastaNoodle is missing a SpriteRenderer component.");
        }
        if (!GameObject.Find("PastaSauce").TryGetComponent<SpriteRenderer>(out sauceRenderer))
        {
            Debug.LogError("PastaSauce is missing a SpriteRenderer component.");
        }
        if (!GameObject.Find("PastaTopping").TryGetComponent<SpriteRenderer>(out toppingRenderer))
        {
            Debug.LogError("PastaTopping is missing a SpriteRenderer component.");
        }
        ClearPastaPlate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
