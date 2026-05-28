using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public GameObject healEffect; // 特效预制体
    public AudioClip pickSound; // 拾取声音
    private void OnTriggerEnter2D(Collider2D collision)
    {
        lunaMovement luna = collision.gameObject.GetComponent<lunaMovement>();
        if (luna != null) // 如果碰撞对象有 lunaMovement 组件
        {
            Animator lunaAnimator = luna.transform.GetChild(0).GetComponent<Animator>(); // 获取子对象的 Animator
            if (lunaAnimator == null)
            {
                Debug.LogError("Potion: 子对象的 Animator 未找到！");
                return;
            }

            if (!gameManager.instance.IsHealthFull()) // 检查血量是否已满
            {
                gameManager.instance.addorDecreaseHP(1); // 增加血量
                Vector3 effectPosition = lunaAnimator.transform.position; // 在角色身上生成特效
                GameObject effect = Instantiate(healEffect, effectPosition, Quaternion.identity); // 生成特效
                Destroy(effect, GetAnimationLength(effect)); // 动画播放完成后销毁特效
                gameManager.instance.playSound(pickSound); // 播放拾取声音
                Destroy(gameObject); // 销毁药瓶
            }
        }
    }

    private float GetAnimationLength(GameObject effect)
    {
        Animator animator = effect.GetComponent<Animator>();
        if (animator != null)
        {
            return animator.GetCurrentAnimatorStateInfo(0).length; // 获取动画时长
        }
        return 0f; // 如果没有动画，立即销毁
    }

}
