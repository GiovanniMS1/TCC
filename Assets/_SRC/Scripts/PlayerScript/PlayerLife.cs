using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerLife : MonoBehaviour
{
    [Header("Player Life")]
    public int maxLife;
    [SerializeField] private int actualLife;
    [SerializeField] private GameObject blood;
    public static BoxCollider2D bc2d;
    public event EventHandler playerDeath;
    public UnityEvent<int> changeLife;
    private Rigidbody2D playerRb2d;
    private PlayerBehaviour playerScript;
    private Animator anim;
    public bool takingDamage, isDeath;

    private void Start()
    {
        actualLife = maxLife;
        changeLife.Invoke(actualLife);
        playerScript = GetComponent<PlayerBehaviour>();
        playerRb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bc2d = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        AnimationState();
    }

    public bool PlayerIsDeath()
    {
        if(actualLife <= 0)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
            return isDeath = true;
        }
        else
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            return isDeath = false;
        }
    }

    public void TakeDamage(Vector2 direction, float reboundPower, int damage)
    {
        if(!takingDamage)
        {
            takingDamage = true;
            actualLife -= damage;
            changeLife.Invoke(actualLife);
            if (actualLife >= 1) SoundManager.Instance.PlaySound2D("Hit");
            playerRb2d.velocity = Vector2.zero;

            if(!PlayerIsDeath())
            {
                Vector2 rebound = new Vector2(transform.position.x - direction.x, 0.4f).normalized;
                playerRb2d.AddForce(rebound * reboundPower, ForceMode2D.Impulse);
                playerScript.DisableAttack();
                playerScript.DisableBlock();
                StartCoroutine(DisableDamage());
                Instantiate(blood, transform.position, Quaternion.identity);
            }
            else
            {
                playerScript.DisablePlayerControl();
                playerDeath?.Invoke(this, EventArgs.Empty);
                playerRb2d.velocity = Vector2.zero;
                SoundManager.Instance.PlaySound2D("Dying");
            }
        }
    }

    private IEnumerator DisableDamage()
    {
        yield return new WaitForSeconds(0.6f);
        takingDamage = false;
    }

    public void HealLife(int heal)
    {
        int tempLife = actualLife + heal;

        if(actualLife > maxLife)
        {
            actualLife = maxLife;
        }
        else
        {
            actualLife = tempLife;
        }

        changeLife.Invoke(actualLife);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("DeathZone"))
        {
            actualLife = 0;
            isDeath = true;
            playerRb2d.velocity = Vector2.zero;
            changeLife.Invoke(actualLife);
            playerDeath?.Invoke(this, EventArgs.Empty);
            SoundManager.Instance.PlaySound2D("Dying");
        }
    }

    private void AnimationState()
    {  
        anim.SetBool("takeDamage", takingDamage);
        anim.SetBool("isDying", isDeath);
    }
}
