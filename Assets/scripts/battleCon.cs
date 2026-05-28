using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Analytics;


public class battleCon : MonoBehaviour
{
    public AudioClip monsterHitSound;
    public AudioClip attackSound;
    public AudioClip LunaAttack;
    public AudioClip monsterAttackSound;
    public AudioClip skillSound;
    public AudioClip healSound;
    public AudioClip hitSound;
    public AudioClip dieSound;
    public Animator playerAnimator;
    public Transform playerTransform;
    public Transform monsterTransform;
    private Vector3 monsterOriginalPosition;
    private Vector3 playerOriginalPosition;
    private bool originalPositionsSet = false;

    public SpriteRenderer playerSpriteRenderer;

    public SpriteRenderer monsterSpriteRenderer;
    public GameObject skilleffect;
    public GameObject healEffect;
    private void Awake()
    {
        monsterOriginalPosition = monsterTransform.localPosition;
        playerOriginalPosition = playerTransform.localPosition;
        originalPositionsSet = true;
    }

    private void OnEnable()
    {
        if (!originalPositionsSet)
        {
            monsterOriginalPosition = monsterTransform.localPosition;
            playerOriginalPosition = playerTransform.localPosition;
            originalPositionsSet = true;
        }
        monsterSpriteRenderer.DOFade(1f, 0.01f);
        playerSpriteRenderer.DOFade(1f, 0.01f);
        playerTransform.localPosition = playerOriginalPosition;
        monsterTransform.localPosition = monsterOriginalPosition;
        UImanager.instance.showorhideBattleUI(true);

    }
    public void playerEscape()
    {
        UImanager.instance.showorhideBattleUI(false);
        playerTransform.DOLocalMove(playerOriginalPosition + new Vector3(5, 0, 0), 0.5f).OnComplete(() =>
        {
            gameManager.instance.EnterorExitBattle(false);
        });
        playerAnimator.SetBool("moveState", true);
        playerAnimator.SetFloat("moveValue", 1);
    }
    IEnumerator performHeal()
    {
        UImanager.instance.showorhideBattleUI(false);
        gameManager.instance.playSound(healSound);
        playerAnimator.CrossFade("heal", 0);
        GameObject effect = Instantiate(healEffect, playerTransform.position, Quaternion.identity);
        applyEffectSorting(effect, playerSpriteRenderer.sortingOrder + 1);
        yield return new WaitForSeconds(1.2f);
        gameManager.instance.addorDecreaseHP(40);
        Destroy(effect);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(monsterAttack());
    }
    public void heal()
    {
        if (!gameManager.instance.IsHealthFull())
        {
            StartCoroutine(performHeal());
        }
    }
    public void useSkill()
    {
        if (gameManager.instance.canUseMP(80))
        {
            StartCoroutine(performSkill());

        }

    }
    IEnumerator performSkill()
    {

        UImanager.instance.showorhideBattleUI(false);
        playerAnimator.CrossFade("skill", 0);
        gameManager.instance.playSound(skillSound);
        gameManager.instance.addorDecreaseMP(-80);
        yield return new WaitForSeconds(0.5f);
        GameObject effect = Instantiate(skilleffect, monsterTransform.position, Quaternion.identity);
        applyEffectSorting(effect, monsterSpriteRenderer.sortingOrder + 1);
        yield return new WaitForSeconds(0.5f);
        monsterSpriteRenderer.DOFade(0.3f, 0.2f).OnComplete(() =>
        {
            judgeMonsterHp(100);
            Destroy(effect);
        });
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(monsterAttack());
    }
    public void attack()
    {
        StartCoroutine(playerAttack());
    }

    public void defense()
    {
        StartCoroutine(performDefense());
    }

    IEnumerator performDefense()
    {
        UImanager.instance.showorhideBattleUI(false);
        playerAnimator.SetBool("defense", true);
        StartCoroutine(monsterAttack());
        yield return new WaitForSeconds(2.5f);
        playerAnimator.SetBool("defense", false);
    }


    private IEnumerator playerAttack()
    {
        UImanager.instance.showorhideBattleUI(false);
        playerSpriteRenderer.sortingOrder = 12;
        playerAnimator.SetBool("moveState", true);
        playerAnimator.SetFloat("moveValue", -1);
        playerTransform.DOLocalMove(monsterOriginalPosition + new Vector3(1f, 0.1f, 0), 0.5f).OnComplete(() =>
        {
            playerAnimator.SetBool("moveState", false);
            playerAnimator.SetFloat("moveValue", 0);
            playerAnimator.CrossFade("attack", 0);
            gameManager.instance.playSound(attackSound);
            gameManager.instance.playSound(LunaAttack);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                monsterSpriteRenderer.DOFade(0.3f, 0.2f).OnComplete(() =>
                {
                    judgeMonsterHp(50);
                });
            });
        });
        yield return new WaitForSeconds(1f);
        playerSpriteRenderer.sortingOrder = 11;
        playerAnimator.SetBool("moveState", true);
        playerAnimator.SetFloat("moveValue", 1);
        playerTransform.DOLocalMove(playerOriginalPosition, 0.5f).OnComplete(() =>
        {
            playerAnimator.SetBool("moveState", false);
            playerAnimator.SetFloat("moveValue", 0);
        });
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(monsterAttack());
    }

    private IEnumerator monsterAttack()
    {
        monsterSpriteRenderer.sortingOrder = 12;
        monsterTransform.DOLocalMove(playerOriginalPosition - new Vector3(2f, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.playSound(monsterAttackSound);
        monsterTransform.DOLocalMove(playerOriginalPosition - new Vector3(1f, 0, 0), 0.5f).OnComplete(() =>
        {
            if (!playerAnimator.GetBool("defense"))
            {
                gameManager.instance.playSound(hitSound);
                playerAnimator.CrossFade("damage", 0);
                playerSpriteRenderer.DOFade(0.3f, 0.2f).OnComplete(() =>
                {
                    playerSpriteRenderer.DOFade(1f, 0.2f);
                    judgePlayerHp(-20);
                });
            }
            else
            {
                playerTransform.DOLocalMove(playerOriginalPosition + new Vector3(1, 0, 0), 0.3f).OnComplete(() =>
                {
                    playerTransform.DOLocalMove(playerOriginalPosition, 0.3f);
                });
            }
        });
        yield return new WaitForSeconds(0.6f);
        monsterSpriteRenderer.sortingOrder = 11;
        monsterTransform.DOLocalMove(monsterOriginalPosition, 0.5f).WaitForCompletion();
        UImanager.instance.showorhideBattleUI(true);
    }


    private void judgePlayerHp(int value)
    {

        gameManager.instance.addorDecreaseHP(value);
        if (gameManager.instance.playerCurrentHP <= 0)
        {
            gameManager.instance.playSound(dieSound);
            playerAnimator.CrossFade("die", 0);
            playerSpriteRenderer.DOFade(0f, 0.5f).OnComplete(() =>
            {
                gameManager.instance.EnterorExitBattle(false);
            });
        }
    }

    private void judgeMonsterHp(int value)
    {
        gameManager.instance.playSound(monsterHitSound);
        monsterSpriteRenderer.DOFade(1f, 0.2f).OnComplete(() =>
        {
            if (gameManager.instance.addorDecreaseMonsterHP(-value) <= 0)
            {
                gameManager.instance.killNum++;
                monsterSpriteRenderer.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    // 隐藏主场景对应的怪物（如果有的话），然后退出战斗
                    gameManager.instance.RemoveCurrentEncounteredMonster();
                    gameManager.instance.EnterorExitBattle(false);
                });
            }
        });
    }

    private void applyEffectSorting(GameObject effect, int sortingOrder)
    {
        Renderer[] renderers = effect.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            renderer.sortingLayerID = monsterSpriteRenderer.sortingLayerID;
            renderer.sortingOrder = sortingOrder;
        }
    }
}
