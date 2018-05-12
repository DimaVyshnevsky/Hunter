using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base_Behavior : MonoBehaviour
{
    public enum State
    {
        waiting,
        hunting,
        attack,
        dead,
        _null
    }

    [Header("-----Characteristics-----")]
    [SerializeField][Range(0, 100)]
    protected float Health;
    [SerializeField][Range(0, 1)]
    protected float Protection_Level;
    [SerializeField][Range(1, 100)]
    protected float Damage;
    [Header("-----Components-----")]
    [SerializeField]
    protected Slider healthBar;

    protected State currentState;

    public static System.Action<string> DeadEvent;

    protected virtual void OnEnable()
    {
        global::Damage.ExplosionEvent += ExplosionHandler;
    }

    protected virtual void OnDisable()
    {
        global::Damage.ExplosionEvent -= ExplosionHandler;
    }

    #region Interface

    public virtual void ResetObj()
    {
        Health = 100;
        UpdateHealtBar();
        SetState(State.hunting);
    }

    public virtual void MakeDamage(float damage)
    {
        if (Health <= 0)
            return;
        //расчет урона с учетом защиты 
        Health -= (1f - Protection_Level) * damage;// для удобства моего восприятия: 0 - минимальная защита или ее отсутствие, 1 - неуязвимость, поэтому отнимаю от 1f 
        UpdateHealtBar();
        if (Health <= 0)
            SetState(State.dead);
    }

    public virtual void SetState(State state)
    {
        if (currentState == state)
            return;
        currentState = State._null;

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

        currentState = state;
    }

    public State GetState()
    {
        return currentState;
    }

    #endregion

    #region Handlers

    protected virtual void ExplosionHandler(Damage damage)
    {
        //расчет урона от близости взрыва
        //TODO: move bodies in opposit direction from explosion point
        float distance = Vector3.Distance(damage.transform.position, transform.position);
        if (distance < damage._Range)
        {
            float d = (1f - (1f / damage._Range * distance)) * damage._Damage;
            MakeDamage(d);
        }
    }

    #endregion 

    #region States

    protected virtual void Attack()
    {
    }

    protected virtual void Hunting()
    {
    }

    protected virtual void Waiting()
    {
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
        if (DeadEvent != null)
            DeadEvent(gameObject.tag);
    }

    #endregion

    #region Methods

    protected virtual void UpdateHealtBar()
    {
        if (healthBar != null)
            healthBar.value = Health;
    }

    #endregion


}
