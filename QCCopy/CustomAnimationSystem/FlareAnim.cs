using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlareAnim : MonoBehaviour
{

    public Light light;
    public float changeSpeed = 1;
    public float targetIntensityPower = 500;
    public float pingPongPower = 50;

    private float power;
    private bool isUp;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    private void Update()
    {
        if (power > pingPongPower)
        {
            isUp = false;
        }
        else if (power < 0)
        {
            isUp = true;
        }

        int value = isUp ? 1 : -1;
        power += changeSpeed * Time.deltaTime * 1000f * value;

        light.intensity = targetIntensityPower + power;
    }

    
}
