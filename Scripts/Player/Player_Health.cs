using System.Collections;
using System;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{
    [Header("Parameters")]
    public float health;
    private float intHealthvalue;
    [SerializeField] private float healthIncreseSpeed = 2;
    [SerializeField] private float timeToHealthRegen = 7;

    [Header("Images")]
    [SerializeField] private Sprite[] damageSlashes;
    [SerializeField] private Image lightDamageImage;
    [SerializeField] private Image heavyDamageImage;
    [SerializeField] private Image slashImage;
    [SerializeField] private float imageDisolveTime;
    private Color lightDamageColor;
    private Color heavyDamageColor;
    private Color slashDamageColor;
    private float damageLerpInc;
    private float slashlerpInc;

    [Header("Audio")]
    [SerializeField] private AudioClip[] painSoundClips;
    private AudioSource audioSource;

    private float timeSinceLastDamage;
    private event Action OnHitEvent;
    public event Action OnKilledEvent;

    private void Awake()
    {
        OnHitEvent += DisplaySlashDamage;
        intHealthvalue = health;
        if(lightDamageImage == null || heavyDamageImage == null)
        {
            Debug.LogError("Please add light and heavy damage images to Player_Health!!");
        }
        if(damageSlashes.Length == 0)
        {
            Debug.LogError("Please add damage slashes images to Player_Health!!");
        }

        audioSource = GetComponent<AudioSource>();

        slashImage.gameObject.SetActive(false);
        lightDamageImage.gameObject.SetActive(false);
        heavyDamageImage.gameObject.SetActive(false);

        //Caching Colors
        lightDamageColor = lightDamageImage.gameObject.GetComponent<Image>().color;
        heavyDamageColor = heavyDamageImage.gameObject.GetComponent<Image>().color;
        slashDamageColor = heavyDamageImage.gameObject.GetComponent<Image>().color;
    }

    private void OnDestroy()
    {
        OnHitEvent -= DisplaySlashDamage;
    }

    void DisplaySlashDamage()
    {
        slashImage.gameObject.SetActive(true);
        slashDamageColor.a = 255;
        slashImage.color = slashDamageColor;
        int damageIndexToDisplay = UnityEngine.Random.Range(0, damageSlashes.Length);
        Sprite damageimageToDisplay = damageSlashes[damageIndexToDisplay];
        slashImage.sprite = damageimageToDisplay;
    }

    void DisplayDamageOverlay()
    {
        if(health < 100 && health > 50)
        {
            lightDamageImage.gameObject.SetActive(true);
            //255 is the max alpha value
            lightDamageColor.a = 255f;
            lightDamageImage.color = lightDamageColor;
        }
        if (health <= 50)
        {
            heavyDamageImage.gameObject.SetActive(true);
            //255 is the max alpha value
            heavyDamageColor.a = 255f;
            heavyDamageImage.color = heavyDamageColor;
        }
    }

    //Called externally by animation event
    public void TakeDamage(float damage)
    {
        health -= damage;
        timeSinceLastDamage = 0;

        int painSoundIndex = UnityEngine.Random.Range(0, painSoundClips.Length);
        audioSource.PlayOneShot(painSoundClips[painSoundIndex]);
        OnHitEvent?.Invoke();
    }

    private void UpdateHealth()
    {
        if (timeSinceLastDamage > timeToHealthRegen && health < intHealthvalue)
        {
            health += Time.deltaTime * healthIncreseSpeed;
        }
    }

    private void UpdateDamageImages()
    {
        float currentLightAlpha = lightDamageColor.a;
        float currentHeavyAlpha = heavyDamageColor.a;
        float currentSlashAlpha = slashDamageColor.a;

        if (timeSinceLastDamage > timeToHealthRegen)
        {
            if (currentLightAlpha > 0 || currentHeavyAlpha > 0)
            {
                damageLerpInc += Time.deltaTime * imageDisolveTime;
                float lerpFactor = Mathf.Clamp(damageLerpInc / 255f, 0, 1); // Normalize for lerp

                lightDamageColor.a = Mathf.Lerp(currentLightAlpha, 0, lerpFactor);
                heavyDamageColor.a = Mathf.Lerp(currentHeavyAlpha, 0, lerpFactor);
            }

            if (currentSlashAlpha > 0)
            {
                slashlerpInc += Time.deltaTime * imageDisolveTime * 0.75f;
                float slashLerpFactor = Mathf.Clamp(slashlerpInc / 255f, 0, 1); // Normalize for lerp

                slashDamageColor.a = Mathf.Lerp(currentSlashAlpha, 0, slashLerpFactor);
            }
        }

        else
        {
            damageLerpInc = 0;
            slashlerpInc = 0;
        }

        // Apply the color changes
        lightDamageImage.color = lightDamageColor;
        heavyDamageImage.color = heavyDamageColor;
        slashImage.color = slashDamageColor;
    }

    private void Update()
    {
        if (health <= 0)
        {
            OnKilledEvent?.Invoke();
            health = 0;
            return;
        }

        if (health > intHealthvalue)
        {
            health = intHealthvalue;
        }

        timeSinceLastDamage += Time.deltaTime;

        DisplayDamageOverlay();
        UpdateHealth();
        UpdateDamageImages();
    }
}