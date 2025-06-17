using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }

    public int totalPower;
    public int powerUsage;

    [SerializeField] private Image sliderFill;
    [SerializeField] private Slider powerSlider;
    //[SerializeField] private TextMeshProUGUI powerText;

    public AudioClip powerAddedClip;
    public AudioClip powerInsufficientClip;

    private AudioSource powerAudioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // <- ini mencegah objek hancur saat pindah scene
        }

        powerAudioSource = gameObject.AddComponent<AudioSource>();
    }


    public void AddPower(int amount)
    {
        PlayPowerAddedSound();
        totalPower += amount;
        UpdatePowerUI();
    }

    public void ConsumePower(int amount)
    {
        powerUsage += amount;
        Debug.Log("Power used: " + powerUsage);
        UpdatePowerUI();

        if (IsInsufficientPower())
        {
            PlayPowerInsufficientSound();
        }
    }

    public void RemovePower(int amount)
    {
        totalPower -= amount;
        UpdatePowerUI();

        if (IsInsufficientPower())
        {
            PlayPowerInsufficientSound();
        }
    }

    public void ReleasePower(int amount)
    {
        powerUsage -= amount;
        UpdatePowerUI();
    }

    private bool IsInsufficientPower()
    {
        return (totalPower - powerUsage) <= 0;
    }

    private void UpdatePowerUI()
    {
        int availablePower = totalPower - powerUsage;

        if (sliderFill != null)
        {
            sliderFill.gameObject.SetActive(availablePower > 0);
        }

        if (powerSlider != null)
        {
            powerSlider.maxValue = totalPower;
            powerSlider.value = availablePower;
        }

        //if (powerText != null)
        //{
        //    powerText.text = $"{availablePower}/{totalPower}";
        //}
    }


    public int CalculateAvailablePower()
    {
        return totalPower - powerUsage;
    }

    public void PlayPowerAddedSound()
    {
        powerAudioSource.PlayOneShot(powerAddedClip);
    }


    public void PlayPowerInsufficientSound()
    {
        if (this == null || powerAudioSource == null)
        {
            Debug.LogWarning("[PowerManager] Instance or AudioSource sudah null. Tidak bisa memainkan suara.");
            return;
        }

        if (powerInsufficientClip == null)
        {
            Debug.LogWarning("[PowerManager] powerInsufficientClip is not assigned.");
            return;
        }

        powerAudioSource.PlayOneShot(powerInsufficientClip);
    }




}
