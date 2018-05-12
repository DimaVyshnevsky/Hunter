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
    protected WeaponBase bullet;
    [SerializeField]
    protected Transform spawnPoint;
    [SerializeField]
    protected float delay = 1;
    [SerializeField]
    protected int quantityObjsInPool = 10;
   
    protected bool attack;
    protected bool activate;

    public WeaponsType CurrentWeaponType
    {
        get
        {
            return currentWeaponType;
        }
    }

    protected virtual void Awake()
    {
        if (delay < 0.5f)
            delay = 0.5f;

        if (spawnPoint == null)
            spawnPoint = transform;
        bullet.gameObject.SetActive(false);
    }

    protected virtual void Start()
    {
        Factory.Instance.CreatePool(bullet.GetType().ToString(), bullet.gameObject, quantityObjsInPool);
    }

    public virtual void Fire()
    {
        GameObject temp = PrepareNewBullet();

        if (!temp)
        {
            print("no bullet!");
            return;
        }

        WeaponBase obj = temp.GetComponent<WeaponBase>();
        obj.ResetObj();
        temp.SetActive(true);
        obj.Activate(); 
    }

    public virtual void ActivateFireSystem(bool activate)
    {
        this.activate = activate;
    }

    protected virtual GameObject PrepareNewBullet()
    {
        GameObject temp = Factory.Instance.GetObject(bullet.GetType().ToString());
        if (!temp)
            return null;
        temp.transform.position = spawnPoint.transform.position;
        temp.transform.rotation = transform.rotation;
        return temp;
    }  
}


