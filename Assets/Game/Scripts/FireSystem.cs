using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FireSystem: MonoBehaviour
{
    public enum WeaponsType
    {
        GunMachine,
        Launcher
    }

    [SerializeField]
    protected WeaponsType currentWeaponType;
    [SerializeField]
    protected WeaponBase bulletPref;
    [SerializeField]
    protected Transform spawnPoint;
    [SerializeField]
    protected float delay = 1;
    [SerializeField]
    protected int quantityObjsInPool = 10;

    protected Transform target;
    protected bool attack;
    protected bool activate;

    public WeaponsType CurrentWeaponType
    {
        get
        {
            return currentWeaponType;
        }
    }

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (delay < 0.1f)
            delay = 0.1f;

        if (spawnPoint == null)
            spawnPoint = transform;
    }

    public virtual void Fire()
    {
        print("Fire!"); 
    }

    public virtual void ActivateFireSystem(bool activate)
    {
        this.activate = activate;
    }

    protected virtual IPoolObj PrepareNewBullet<T>() where T: class, IPoolObj
    {
        T temp = Factory.Instance.GetObject<T>(bulletPref.GetType().ToString());
        if (temp == null)
        {
            print("no bullet!");
            return null;
        }
        temp.GetGameObject().transform.position = spawnPoint.transform.position;
        temp.GetGameObject().transform.rotation = transform.rotation;
        return temp;
    }  
}


