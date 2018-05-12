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
    private ParticleSystem effectsParticle;
    [SerializeField]
    private GameObject lightEffect;
    [SerializeField]
    private GameObject rocketRenderer;

    private Tween tweenTimer;
    private Rigidbody rig;
    private List<IPoolObj> enemyList;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.tag.Equals("Player"))
            Explosion();
    }

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

        if (target)
        {
            if (Vector3.Distance(transform.position, target.position) < 0.2f)
            {
                Explosion();
                return;
            }

            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Damping);
            Vector3 dir = (target.position - transform.position).normalized;
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

    #region Interface

    public  void Activate()
    {
        if (!init)
            Init();

        if (enemyList == null)
            enemyList = Factory.Instance.GetList("Enemy");
        effectsParticle.Stop();
        effectsParticle.Play();
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
        fire = false;
        target = null;
        rig.detectCollisions = true;
        rig.isKinematic = false;
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
        if (tweenTimer != null && tweenTimer.IsActive())
            tweenTimer.Kill();

        ResetObj();
        rocketRenderer.SetActive(false);
        rig.detectCollisions = false;
        rig.isKinematic = true;
        lightEffect.SetActive(false);
        effectsParticle.Stop();

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


