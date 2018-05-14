using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MoverMissile : WeaponBase, IPoolObj
{  
    [SerializeField]
    private float Damping = 3f;
    [SerializeField]
    private Vector3 Noise = new Vector3(20f, 20f, 20f);
    [SerializeField]
    private float TargetLockDirection = 0.5f;
    [SerializeField]
    private bool Seeker;
    [Header("-----Rocket_Parts-----")]
    [SerializeField]
    private GameObject lightEffect;
    [SerializeField]
    private GameObject rocketRenderer;

    private Tween tweenTimer;
    private Rigidbody rig;
    private List<Enemy> enemyList;

    private void FixedUpdate()
    {
        if (!fire)
            return;       

        rig.velocity = new Vector3(transform.forward.x * Speed * Time.fixedDeltaTime, transform.forward.y * Speed * Time.fixedDeltaTime, transform.forward.z * Speed * Time.fixedDeltaTime);
        rig.velocity += new Vector3(Random.Range(-Noise.x, Noise.x), Random.Range(-Noise.y, Noise.y), Random.Range(-Noise.z, Noise.z));

        if (Speed < SpeedMax)
            Speed += SpeedMult * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (!fire)
            return;

        foreach (var item in enemyList)
        {
            if (Vector3.Distance(new Vector3(item.GetGameObject().transform.position.x, transform.position.y, item.GetGameObject().transform.position.z), transform.position) < 1f && 
                item.GetState() != Hunter_Base.State.dead)
                Explosion();
        }

        if (target)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Damping);
            Vector3 dir = (new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position).normalized;
            float direction = Vector3.Dot(dir, transform.forward);

            if (direction < TargetLockDirection)
                target = null;
        }

        if (Seeker && !target)
        {
            float distance = int.MaxValue;
            if (enemyList.Count > 0)
            {
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (enemyList[i] != null)
                    {
                        Vector3 dir = (enemyList[i].GetGameObject().transform.position - transform.position).normalized;
                        float direction = Vector3.Dot(dir, transform.forward);
                        float dis = Vector3.Distance(enemyList[i].GetGameObject().transform.position, transform.position);

                        if (direction >= TargetLockDirection)
                        {
                            if (distance > dis)
                            {
                                distance = dis;
                                target = enemyList[i].GetGameObject().transform;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player"))
            Explosion();
        print("Trigger Enter");
    }

    #region Interface

    public  void Activate()
    {
        if (!init)
            Init();

        if (enemyList == null || enemyList.Count <= 0)
        {
            enemyList = new List<Enemy>();
            List<IPoolObj> list = Factory.Instance.GetList("Enemy");
            foreach (var item in list)
            {
                enemyList.Add(item as Enemy);
            }
        }

        lightEffect.SetActive(true);
        gameObject.SetActive(true);

        tweenTimer = DOVirtual.DelayedCall(LifeTime, () =>
        {
            Explosion();
        });

        fire = true;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ResetObj()
    {
        if (!init)
            Init();        
        target = null;
        rig.detectCollisions = true;
        rocketRenderer.SetActive(true);
        if (rig.velocity != Vector3.zero)
            rig.AddForce(-rig.velocity, ForceMode.VelocityChange);
        Speed = startSpeed;
    }

    public void Init()
    {
        init = true;
        if (rig == null)
            rig = GetComponent<Rigidbody>();
        if (damage == null)
            damage = GetComponent<Damage>();
        if (startSpeed == 0)
            startSpeed = Speed;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion

    private void Explosion()
    {
        fire = false;

        if (tweenTimer != null && tweenTimer.IsActive())
            tweenTimer.Kill();

        rig.detectCollisions = false;
        rocketRenderer.SetActive(false);
        lightEffect.SetActive(false);

        DOVirtual.DelayedCall(3, ()=>
        {
            gameObject.SetActive(false);
        });

        if (damage)
        {
            damage.Active();
            damage.Explosion();
        }
    }   
}


