using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // agar musik tidak mati saat ganti scene
    }

    public void StopMusic()
    {
        Destroy(gameObject); // dipanggil saat klik START
    }
}
