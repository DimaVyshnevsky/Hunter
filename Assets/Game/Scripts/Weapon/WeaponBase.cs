using UnityEngine;
using System.Collections;

public class WeaponBase : MonoBehaviour
{
    [Header("-----Settings-----")]
    [SerializeField]
    protected float Speed = 80f;
    [SerializeField]
    protected float SpeedMax = 80f;
    [SerializeField]
    protected float SpeedMult = 1f;
    [SerializeField]
    protected float LifeTime = 5f;
    //[SerializeField]
    //protected int DistanceLock = 70;

    protected float startSpeed;
    protected GameObject target;
    protected Damage damage;
    protected bool init;
    protected bool fire;

    #region interface

    public virtual void Init()
    {
    }

    public virtual void Activate()
    {
    }

    public virtual void ResetObj()
    {
    }

    #endregion

}