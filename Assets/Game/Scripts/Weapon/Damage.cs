using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    [SerializeField]
    protected GameObject explosionEffect;
    [SerializeField]
    protected int damageEffect = 20;
    [SerializeField]
    [Range(1, 20)]
    protected float explosionRange;
   
    public float ExplosionRange
    {
        get
        {
            return explosionRange;
        }
    }

    public float DamageEffect
    {
        get
        {
            return damageEffect;
        }
    }

    public static System.Action<Damage> ExplosionEvent;

    public virtual void Active()
    {
        if (explosionEffect)
        {
            Explosion obj = Factory.Instance.GetObject<Explosion>(explosionEffect.name);
            obj.GetGameObject().transform.position = transform.position;
            obj.Activate();
        }
    }

    public void Explosion()
    {
        if (ExplosionEvent != null)
            ExplosionEvent(this);
    }
}

