using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    [SerializeField]
    protected GameObject explosionEffect;
    [SerializeField]
    protected int _damage = 20;
    [SerializeField]
    [Range(1, 20)]
    protected float _range;
   
    public float _Range
    {
        get
        {
            return _range;
        }
    }

    public float _Damage
    {
        get
        {
            return _damage;
        }
    }

    public static System.Action<Damage> ExplosionEvent;

    public virtual void Active()
    {
        if (explosionEffect)
        {
            GameObject obj = Factory.Instance.GetObject(explosionEffect.name);
            obj.transform.position = transform.position;
            obj.SetActive(true);
        }
    }

    public void Explosion()
    {
        if (ExplosionEvent != null)
            ExplosionEvent(this);
    }
}

