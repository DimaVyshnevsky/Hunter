using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class Explosion : MonoBehaviour, IPoolObj
{
    [SerializeField]
    private float LightMult = 2;
    private Light currentLight;
    private float startIntensity;
    private bool init;

    void Update ()
    {
		if(!currentLight)
			return;
        currentLight.intensity -= LightMult * Time.deltaTime;
	}

    public void Init()
    {
        init = true;
        currentLight = GetComponent<Light>();
        startIntensity = currentLight.intensity;
    }

    public void Activate()
    {
        if (!init)
            Init();
        gameObject.SetActive(true);
        Audio_Manager.Instance.Play(GameClips._ExplosionRocket);
        DOVirtual.DelayedCall(3, () => Deactivate());
    }

    public void ResetObj()
    {
        if (!currentLight)
            return;
        currentLight.intensity = startIntensity;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        ResetObj();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
