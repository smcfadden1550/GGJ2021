using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Vector2 direction;

    public float damage;

    public bool hasDuration; 

    public float duration;

    public float rotation;

    public bool pierceWalls;

    public bool pierceTargets;

    void OnCollisionEnter() 
    {
        GameObject.Destroy(this);
    }

    void OnBecameInvisible()
    {
        // GameObject.Destroy(this);
        this.Dispose();
        // this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemyCandidate = other.GetComponent<Enemy>();
        if (enemyCandidate != null)
        {
            enemyCandidate.TakeDamage(this.damage);
            if (!pierceTargets)
            {
                this.Dispose();
            }
            return;
        }
        if (other.gameObject.layer != LayerMask.NameToLayer("Default") && !pierceWalls)
        {
            this.Dispose();
            return;
        }
    }

    protected void Dispose() 
    {
        Destroy(this.gameObject);
    }

    void Update()
    {
        Vector2 target = direction * speed * Time.deltaTime;
        this.transform.position += new Vector3(target.x, target.y, 0.0f);
        if (this.hasDuration) 
        {
            this.duration -= Time.deltaTime;
            if (this.duration <= 0.0f)
            {
                this.Dispose();
            }
        }
    }
}