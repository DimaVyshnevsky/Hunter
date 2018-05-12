using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunter_Base : MonoBehaviour
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

    public virtual void Restart()
    {
        Health = 100;
        UpdateHealtBar();
    }

    public State GetState()
    {
        return currentState;
    }

    public virtual void SetState(State state)
    {
        currentState = state;
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

    #endregion

    #region Handlers

    protected virtual void ExplosionHandler(Damage damage)
    {
        //расчет урона от близости взрыва
        float distance = Vector3.Distance(damage.transform.position, transform.position);
        if (distance < damage._Range)
        {
            float d = (1f - (1f / damage._Range * distance)) * damage._Damage;
            MakeDamage(d);
        }
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
