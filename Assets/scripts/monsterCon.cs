using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class monsterCon : MonoBehaviour
{
    public bool vertical = true;
    public float moveSpeed = 3f;
    private Rigidbody2D rigidbody2d;
    private int direction = 1;
    public float changeTime = 5.0f;
    private float timer;
    private Animator animator;
    // removed monsterId support

    private bool canControlMonster()
    {
        return gameManager.instance == null || gameManager.instance.canControlMonster;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 之前的 monsterId / 持久化逻辑已移除
        

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!canControlMonster())
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canControlMonster())
        {
            return;
        }

        Vector3 pos = rigidbody2d.position;
        if (vertical)
        {
            animator.SetFloat("lookX", 0);
            animator.SetFloat("lookY", direction);
            pos.y += Time.fixedDeltaTime * moveSpeed * direction;
        }
        else
        {
            animator.SetFloat("lookX", direction);
            animator.SetFloat("lookY", 0);
            pos.x += Time.fixedDeltaTime * moveSpeed * direction;
        }
        rigidbody2d.position = pos;
    }

    // 检测碰撞器是否与Player碰撞
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canControlMonster())
        {
            return;
        }

            if (collision.gameObject.CompareTag("Player"))
            {
                // 记录当前遭遇的主场景怪物引用，并进入战斗
                if (gameManager.instance != null)
                {
                    gameManager.instance.currentEncounteredMonster = this.gameObject;
                    gameManager.instance.EnterorExitBattle(true);
                }
            }
    }
}
