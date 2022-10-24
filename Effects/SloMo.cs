using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SloMo : MonoBehaviour
{
    public float sloMoFactor, timeToSlo;
    public PostProcessVolume volume;

    Vignette ppv;
    LensDistortion ppln;
    ChromaticAberration ppch;

    private float tempTime = 1;
    private float vv, lnv, chv;
    public float[] maxValues, minValues;

    void Start()
    {
        ppv = volume.profile.GetSetting<Vignette>();
        ppln = volume.profile.GetSetting<LensDistortion>();
        ppch = volume.profile.GetSetting<ChromaticAberration>();
    }
    void Update()
    {
        if (Input.GetButtonDown("Fire2")) startSloMo();
        if (Input.GetButtonUp("Fire2")) stopSloMo();

        UpdateIntensities();
    }

    void UpdateIntensities()
    {
        float v = maxValues[0] * (1 - tempTime);
        float ln = maxValues[1] * (1 - tempTime);
        float ch = maxValues[2] * (1 - tempTime);

        ppv.intensity.value = Mathf.SmoothDamp(ppv.intensity.value, v, ref vv, timeToSlo);
        ppln.intensity.value = Mathf.SmoothDamp(ppln.intensity.value, ln, ref lnv, timeToSlo);
        ppch.intensity.value = Mathf.SmoothDamp(ppch.intensity.value, ch, ref chv, timeToSlo);

        if (ppv.intensity.value < minValues[0]) ppv.intensity.value = minValues[0];
        if (ppln.intensity.value < minValues[1]) ppln.intensity.value = minValues[1];
        if (ppch.intensity.value < minValues[2]) ppch.intensity.value = minValues[2];

        if (tempTime == 1)
        {
            ppv.intensity.value = minValues[0];
            ppln.intensity.value = minValues[1];
            ppch.intensity.value = minValues[2];
        }
    }
    void startSloMo()
    {
        if (tempTime == sloMoFactor) return;

        float vel = 0;
        tempTime = Mathf.SmoothDamp(Time.timeScale, sloMoFactor, ref vel, timeToSlo * Time.deltaTime);

        Time.timeScale = tempTime;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    void stopSloMo()
    {
        if (tempTime == 1) return;

        float vel = 0;
        tempTime = Mathf.SmoothDamp(Time.timeScale, 1, ref vel, timeToSlo * Time.deltaTime);

        Time.timeScale = tempTime;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
