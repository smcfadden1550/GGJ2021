using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Enemy class dictates anything that is interactable with projectiles
// chests, barrells, destroyable traps should all count as enemies for the time being
public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _enemyHealth;

    [SerializeField] protected float _enemyMaxHealth;

    [SerializeField] protected int _enemyContactDamage;

    [SerializeField] protected SpriteRenderer _renderer;

    public UnityEvent<GameObject> DieEvents;

    [SerializeField] protected Color flickerColor;

    [SerializeField] protected AudioClip hitSound;
    protected AudioSource aSource;

    protected virtual void Awake() 
    {
        aSource = gameObject.GetComponent<AudioSource>();

        this._enemyHealth = this._enemyMaxHealth;
        this._renderer = this.GetComponent<SpriteRenderer>();
    }

    public virtual void TakeDamage(int damage)
    {
        if(aSource != null && hitSound != null)
        {

            aSource.Stop();
            aSource.loop = false;
            aSource.clip = hitSound;
            aSource.Play();
        }
        else
        {
            Debug.Log("Something wrong playing audio for an enemy");
        }

        this._enemyHealth -= damage;
        if (this._enemyHealth <= 0)
        {
            this.Die();
        }
        if(this._enemyHealth > 0)
        {
            StartCoroutine(DamageFlicker());
        }
    }

    public virtual void Die()
    {
        // Die.
        DieEvents.Invoke(this.gameObject);
    }

    private IEnumerator DamageFlicker()
    {
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.05f);
            _renderer.material.SetColor("_AdditiveTint", flickerColor);
            yield return new WaitForSeconds(0.05f);
            _renderer.material.SetColor("_AdditiveTint", Color.clear);
        }
    }
}
