using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : FireSystem
{
    [SerializeField]
    private GameObject explosionEffectPref;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        List<IPoolObj> list = Factory.Instance.CreatePool<MoverMissile>(bulletPref.GetType().ToString(), bulletPref.gameObject, quantityObjsInPool);
        foreach (var item in list)
            item.Init();
        list = Factory.Instance.CreatePool<Explosion>(explosionEffectPref.name, explosionEffectPref, quantityObjsInPool);
        foreach (var item in list)
            item.Init();
    }

    public override void Fire()
    {
        Audio_Manager.Instance.Play(GameClips._Rocket_Fire);

        IPoolObj bullet = PrepareNewBullet<MoverMissile>();

        if (!bullet.GetGameObject())
        {
            print("no bullet!");
            return;
        }

        bullet.ResetObj();
        bullet.Activate();
    }
}
