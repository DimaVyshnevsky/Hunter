using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Enemy : Hunter_Base, IPoolObj
{
    [SerializeField]
    private float attack_disctance = 2;

    private NavMeshAgent agent;
    private Animator anim;
    private Rigidbody rig;
    private Tween tweenDamage;
    private Tween tweenDieing;

    public static System.Action<float> MakeDamageEvent;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Update()
    {
        if (currentState == State.waiting || currentState == State.dead || currentState == State._null)
            return;

        if (Vector3.Distance(transform.position, Player.PlayerTransform.position) < attack_disctance)
        {
            SetState(State.attack);
        }
        else
        {
            SetState(State.hunting);
            agent.SetDestination(Player.PlayerTransform.position);
        }   
    }

    #region Interface

    public override void Restart()
    {
        rig.detectCollisions = true;
        healthBar.gameObject.SetActive(true);
        base.Restart();
    }  

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        SetState(State.hunting);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ResetObj()
    {
        Restart();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #endregion

    #region States

    public override void SetState(State state)
    {
        if (currentState == state)
            return;
        currentState = State._null;

        if (tweenDamage != null && tweenDamage.IsActive())
            tweenDamage.Kill();

        switch (state)
        {
            case State.waiting:
                Waiting();
                break;
            case State.hunting:
                Hunting();
                break;
            case State.dead:
                Die();
                break;
            case State.attack:
                Attack();
                break;
        }

        base.SetState(state);
    }

    protected  void Attack()
    {
        if (tweenDieing != null && tweenDieing.IsActive())
            return;
        anim.SetTrigger(State.attack.ToString());
        tweenDamage = DOVirtual.DelayedCall(0.5f, MakingDamage).SetLoops(-1, LoopType.Restart);
    }

    public  void Die()
    {
        anim.SetTrigger(State.dead.ToString());
        rig.detectCollisions = false;
        agent.SetDestination(transform.position);
        healthBar.gameObject.SetActive(false);
        tweenDieing = DOVirtual.DelayedCall(4, () =>
        {
            tweenDieing = null;
            gameObject.SetActive(false);
            if (DeadEvent != null)
                DeadEvent(gameObject.tag);
        });
    }

    protected  void Hunting()
    {
        if (tweenDieing != null && tweenDieing.IsActive())       
            return;

        anim.SetTrigger(State.hunting.ToString());
    }

    protected  void Waiting()
    {
        if (tweenDieing != null && tweenDieing.IsActive())
            return;
        anim.SetTrigger(State.waiting.ToString());
        agent.SetDestination(transform.position);
    }

    #endregion

    #region Methods

    private void MakingDamage()
    {
        if (MakeDamageEvent != null)
            MakeDamageEvent(Damage);
    }

    #endregion
}
