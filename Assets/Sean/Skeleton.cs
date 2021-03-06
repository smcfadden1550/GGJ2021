using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{

    public float moveSpeed;

    private int moveDir = 1;

    public GameObject skeletonDeathPrefab;

    private GameObject _playerTarget;

    [Header("Attacks")]
    public float playerDetectRange;

    public float attackCooldown;
    private float _currentAttackCooldown;

    public float _shotForce;
    public float boneDuration;

    public GameObject enemyProjectile;

    public Transform enemyShootPoint;

    public Vector2 projectileOrientation;

    public int projDamage;
    
    public bool isMoving = true;

    private bool canMove;

    private Animator _anim;

    [Header("Animations")]
    public string walkAnimationName;
    public string attackAnimationName;

    private int layer;

    protected override void Awake()
    {
        base.Awake();
        this._renderer = this.GetComponent<SpriteRenderer>();
        this._anim = this.GetComponent<Animator>();
        layer = LayerMask.GetMask("Player");
        _currentAttackCooldown = attackCooldown;
        canMove = true;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        // flash colors
        if(base._enemyHealth > 0)
        {
        }
    }

    public void StopMoving()
    {
        this.isMoving = false;
    }

    public void StartMoving()
    {
        this.isMoving = true;
    }

    private void FlipCharacter()
    {
        Vector3 skellyScale = this.transform.localScale;
        skellyScale.x *= -1;
        this.transform.localScale = skellyScale;
        moveDir *= -1;
    }

    void Attack()
    {
        // -1 = left
        // 1 = right
        // if p.x is less than, player is to the left
        // if p.x is greater than, player is to the right
        float distance = _playerTarget.transform.position.x - this.transform.position.x;
        float direction = distance / Mathf.Abs(distance);
        if (direction != moveDir)
        {
            this.FlipCharacter();
        }

        Debug.Log("do the attack");
        _anim.Play(attackAnimationName);
        this._currentAttackCooldown = attackCooldown;

    }

    public void LaunchProjectile()
    {
        GameObject proj = GameObject.Instantiate(enemyProjectile, enemyShootPoint.position, Quaternion.identity);
        Projectile projComp = proj.GetComponent<Projectile>();
        
        projComp.speed = _shotForce;
        projComp.hasDuration = true;
        projComp.duration = boneDuration;
        
        float xDir = projectileOrientation.x * moveDir;
        float yDir = projectileOrientation.y;
        
        projComp.direction = new Vector3(xDir, yDir, 0.0f);
        projComp.damage = projDamage;
        
        projComp.OnShoot();
    }

    void Update()
    {
        if (!canMove)
        {
            return;
        }

        if (isMoving)
        {
            _anim.Play(walkAnimationName);
            this.transform.position = transform.position + new Vector3(moveDir, 0, 0) * moveSpeed * Time.deltaTime;
        }
        RaycastHit2D[] hits = Physics2D.CircleCastAll(this.transform.position, playerDetectRange, new Vector2(0.0f, 0.0f), 0.0f, layer); 
        if (hits.Length > 0)
        {
            if(this._playerTarget == null) 
            {
                this._playerTarget = hits[0].collider.gameObject;  
            }
        }
        else
        {
            if(this._playerTarget != null)
            {
                this._playerTarget = null;
                _currentAttackCooldown = attackCooldown;
                // float distance = _playerTarget.transform.position.x - this.transform.position.x;
                // float direction = distance / Mathf.Abs(distance);
                // if (direction != moveDir)
                // {
                //     this.FlipCharacter();
                // }
            }
        }
        if (this._playerTarget != null) 
        {
            _currentAttackCooldown -= Time.deltaTime;
            if (_currentAttackCooldown <= 0.0f) 
            {
                this.Attack();
            }
        }
    }

    public override void Die()
    {
        StopAllCoroutines();
        base.Die();
    }


    public void CreateDeathParticles()
    {
        Instantiate(this.skeletonDeathPrefab, this.transform.position, Quaternion.identity);
    }

    public void DestroyOnDeath()
    {
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.layer);
        if(other.gameObject.layer == LayerMask.NameToLayer("Default") || other.gameObject.layer == LayerMask.NameToLayer("EnemyBoundary"))
        {
           this.FlipCharacter();
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();
            if(player != null)
            {
                player.TakeDamage(this._enemyContactDamage);
            }
        }
    }

    public void CanMove(bool moveThatGearUp)
    {
        canMove = moveThatGearUp;
    }
}