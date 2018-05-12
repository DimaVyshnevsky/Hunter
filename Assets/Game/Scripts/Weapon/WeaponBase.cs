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

    protected float startSpeed;
    protected Transform target;
    protected Damage damage;
    protected bool init;
    protected bool fire;
}