using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShoot : FlyingAI
{
    [SerializeField] private float projSpeed;
    [SerializeField] private int projDamage;
    [SerializeField] private float projDuration;
    [SerializeField] private bool pierceTargets;
    [SerializeField] private bool pierceWalls;

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform projectileOrigin;

    [SerializeField] private string attackAnimationName;

    protected override IEnumerator Attack()
    {
        GameObject boolet = GameObject.Instantiate(_projectilePrefab, projectileOrigin.position, Quaternion.identity);
        EnemyProjectile proj = boolet.GetComponent<EnemyProjectile>();

        proj.speed = projSpeed;
        proj.damage = projDamage;
        proj.hasDuration = (projDuration > 0.0f);
        proj.duration = projDuration;
        proj.pierceTargets = pierceTargets;
        proj.pierceWalls = pierceWalls;
        proj.direction = player.transform.position - proj.transform.position;
        proj.direction.Normalize();

        proj.OnShoot(); //...what does this do..?

        if (this.animator != null)
        {
            animator.Play(attackAnimationName);
        }

        inAction = false;
        yield break;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();
            if(player != null)
            {
                player.TakeDamage(this._enemyContactDamage);
            }
        }
    }
}
