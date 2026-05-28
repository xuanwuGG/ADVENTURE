using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 确保使用 DOTween

public class JumpArea : MonoBehaviour
{
    public Transform jumppointA; // 跳跃点 A
    public Transform jumppointB; // 跳跃点 B

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameManager.instance != null && !gameManager.instance.canControlLuna)
        {
            return;
        }

        lunaMovement luna = collision.GetComponent<lunaMovement>();
        if (luna != null) // 检查是否是玩家
        {
            Rigidbody2D rigidbody2d = luna.GetComponent<Rigidbody2D>();
            Animator lunaAnimator = luna.transform.GetChild(0).GetComponent<Animator>(); // 获取子对象的 Animator
            if (lunaAnimator == null)
            {
                Debug.LogError("JumpArea: 子对象的 Animator 未找到！");
                return;
            }

            if (rigidbody2d == null || lunaAnimator == null || jumppointA == null || jumppointB == null)
            {
                Debug.LogError("JumpArea: 必要组件或跳跃点未正确分配！");
                return;
            }

            lunaAnimator.SetBool("jump", true); // 触发跳跃动画
            Debug.Log("触发跳跃动画");
            rigidbody2d.simulated = false; // 禁止物理模拟，使角色悬空
            Transform showLunaTrans = luna.transform.GetChild(0); 
            showLunaTrans.DOLocalMoveY(1.5f, 0.25f);
            // 计算最近的跳跃点
            float disA = Vector3.Distance(luna.transform.position, jumppointA.position);
            float disB = Vector3.Distance(luna.transform.position, jumppointB.position);
            Transform targetTrans = disA < disB ? jumppointB : jumppointA;

            showLunaTrans.DOLocalMoveY(0f, 0.25f).SetDelay(0.15f); // 先上升后下降，模拟跳跃效果
            // 执行跳跃移动
            luna.transform.DOMove(targetTrans.position, 0.45f).OnComplete(() => {
                rigidbody2d.simulated = true; // 恢复物理模拟
                lunaAnimator.SetBool("jump", false); // 取消跳跃动画触发
            });
        }
    }
}