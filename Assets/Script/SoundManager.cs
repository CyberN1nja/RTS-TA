using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("Infantry")]
    private AudioSource infantryAttackChannel;
    public AudioClip infantryAttackClip;

    [Header("Unit")]
    public int poolSize = 2;
    private int unitCurrentPoolIndex = 0;
    private int constructionCurrentPoolIndex = 0;

    private AudioSource[] unitVoiceChannelPool;

    public AudioClip[] unitCommandSounds;
    public AudioClip[] unitSelectionSounds;

    [Header("Buildings")]
    private AudioSource destructionBuildingChannel;
    private AudioSource[] constructionBuildingChannelPool;
    private AudioSource extraBuildingChannel;

    public AudioClip sellingSound;
    public AudioClip buildingConstructionSound;
    public AudioClip buildingDestructionSound;

    [Header("Unit Death")]
    public AudioClip unitDeathSound;
    private AudioSource unitDeathChannel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Infantry Attack
        infantryAttackChannel = CreateAudioSource("InfantryAttackChannel");

        // Building channels
        destructionBuildingChannel = CreateAudioSource("DestructionBuildingChannel");
        extraBuildingChannel = CreateAudioSource("ExtraBuildingChannel");

        constructionBuildingChannelPool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            constructionBuildingChannelPool[i] = CreateAudioSource($"ConstructionChannel_{i}");
        }

        // Unit voice pool
        unitVoiceChannelPool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            unitVoiceChannelPool[i] = CreateAudioSource($"UnitVoiceChannel_{i}");
        }

        // Dedicated death channel
        unitDeathChannel = CreateAudioSource("UnitDeathChannel");
    }

    private AudioSource CreateAudioSource(string name)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0f; // 2D
        source.playOnAwake = false;
        source.name = name;
        return source;
    }

    public void PlayInfantryAttackSound()
    {
        if (infantryAttackChannel != null && infantryAttackClip != null)
        {
            infantryAttackChannel.PlayOneShot(infantryAttackClip);
        }
    }

    public void PlayBuildingSellingSound()
    {
        if (extraBuildingChannel != null && sellingSound != null)
        {
            extraBuildingChannel.PlayOneShot(sellingSound);
        }
    }

    public void PlayBuildingConstructionSound()
    {
        if (buildingConstructionSound == null) return;

        constructionBuildingChannelPool[constructionCurrentPoolIndex].PlayOneShot(buildingConstructionSound);
        constructionCurrentPoolIndex = (constructionCurrentPoolIndex + 1) % poolSize;
    }

    public void PlayBuildingDestructionSound()
    {
        if (buildingDestructionSound == null) return;

        if (!destructionBuildingChannel.isPlaying)
        {
            destructionBuildingChannel.PlayOneShot(buildingDestructionSound);
        }
    }

    public void PlayUnitSelectionSound()
    {
        if (unitSelectionSounds.Length == 0) return;

        AudioClip randomClip = unitSelectionSounds[Random.Range(0, unitSelectionSounds.Length)];
        unitVoiceChannelPool[unitCurrentPoolIndex].PlayOneShot(randomClip);
        unitCurrentPoolIndex = (unitCurrentPoolIndex + 1) % poolSize;
    }

    public void PlayUnitCommandSound()
    {
        if (unitCommandSounds.Length == 0) return;

        AudioClip randomClip = unitCommandSounds[Random.Range(0, unitCommandSounds.Length)];
        unitVoiceChannelPool[unitCurrentPoolIndex].PlayOneShot(randomClip);
        unitCurrentPoolIndex = (unitCurrentPoolIndex + 1) % poolSize;
    }

    public void PlayUnitDeathSound()
    {
        if (unitDeathSound == null)
        {
            Debug.LogWarning("unitDeathSound belum di-assign!");
            return;
        }

        Debug.Log("Memutar unitDeathSound melalui channel khusus");
        unitDeathChannel.PlayOneShot(unitDeathSound);
    }
}
