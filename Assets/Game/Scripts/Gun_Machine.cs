using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gun_Machine : FireSystem
{
    [SerializeField]
    private float damping = 1;
    [SerializeField]
    private int DistanceLock = 20;
    [SerializeField]
    private float radiusSeeker = 10f;

    private List<Enemy> enemyList;
    private bool readyForNextShot = true;
    private int step = 3;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        List<IPoolObj> list = Factory.Instance.CreatePool<MoverBullet>(bulletPref.GetType().ToString(), bulletPref.gameObject, quantityObjsInPool);
        foreach (var item in list)
            item.Init();
    }

    void FixedUpdate()
    {
        if (!activate)
            return;

        if(Time.frameCount % step == 0)
            target = FindEnemy();

        if (target != null)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        else      
            transform.rotation = Quaternion.Slerp(transform.rotation, Player.PlayerTransform.rotation, Time.deltaTime * damping);
        

        if (!attack)
            return;

        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 1, transform.position.z), fwd, out hit, DistanceLock))
        {
            if (hit.transform.tag.Equals("Enemy"))
                hit.transform.GetComponent<Enemy>().MakeDamage(bulletPref.GetComponent<Damage>().DamageEffect);        
        }

        attack = false;
    }

    public override void Fire()
    {
        if (!readyForNextShot)
            return;

        Audio_Manager.Instance.Play(GameClips._GunMachine);
        readyForNextShot = false;
        IPoolObj currentBullet = PrepareNewBullet<MoverBullet>();
        currentBullet.ResetObj();
        currentBullet.Activate();
        attack = true;     
        Invoke("CheckForReady", delay);
    }

    private void CheckForReady()
    {
        readyForNextShot = true;
    }

    private Transform FindEnemy()
    {
        if (enemyList == null || enemyList.Count <= 0)
        {
            enemyList = new List<Enemy>();
            List<IPoolObj>list = Factory.Instance.GetList("Enemy");
            foreach (var item in list)
            {
                enemyList.Add(item as Enemy);
            }
        }

        enemyList.Sort(delegate (Enemy obj_1, Enemy obj_2)
        { return Vector3.Distance(obj_1.GetGameObject().transform.position, Player.PlayerTransform.position)
            .CompareTo(Vector3.Distance(obj_2.GetGameObject().transform.position, Player.PlayerTransform.position)); });

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].GetState() == Hunter_Base.State.dead || !enemyList[i].gameObject.activeSelf)
                continue;
            if (Vector3.Distance(enemyList[0].GetGameObject().transform.position, transform.position) < radiusSeeker)
                return enemyList[i].GetGameObject().transform;
        }
        return null;    
    } 
}
