using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUser : MonoBehaviour
{
    public int producingPower;
    public int consumingPower;
    public bool isProducer;

    public void PowerOn()
    {
        Debug.Log("PowerOn() dipanggil di " + gameObject.name);

        if (PowerManager.Instance == null)
        {
            Debug.LogWarning("[PowerUser] PowerManager.Instance tidak tersedia saat PowerOn.");
            return;
        }

        if (isProducer)
        {
            PowerManager.Instance.AddPower(producingPower);
        }
        else
        {
            PowerManager.Instance.ConsumePower(consumingPower);
        }
    }

    private void OnDestroy()
    {
        if (GetComponent<Constructable>()?.inPreviewMode == true)
            return;

        if (PowerManager.Instance == null)
        {
            Debug.LogWarning("[PowerUser] PowerManager.Instance sudah null saat OnDestroy. Dilewati.");
            return;
        }

        if (isProducer)
        {
            PowerManager.Instance.RemovePower(producingPower);
        }
        else
        {
            PowerManager.Instance.ReleasePower(consumingPower);
        }
    }
}
