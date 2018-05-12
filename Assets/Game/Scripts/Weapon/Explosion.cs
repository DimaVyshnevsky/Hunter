using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float LightMult = 2;
    private Light currentLight;
    private float startIntensity;

    private void Awake()
    {
        currentLight = GetComponent<Light>();
        startIntensity = currentLight.intensity;
    }

    private void OnEnable()
    {
        AudioManager.Instance.Play(GameClips._ExplosionRocket);
        currentLight.intensity = startIntensity;
        DOVirtual.DelayedCall(3, () => gameObject.SetActive(false));
    }

    void Update ()
    {
		if(!currentLight)
			return;
        currentLight.intensity -= LightMult * Time.deltaTime;
	}
}
