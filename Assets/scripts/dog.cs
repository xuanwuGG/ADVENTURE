using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class dog : MonoBehaviour
{
    public GameObject starEffect;
    public Animator dogAnimator;
    public AudioClip petSound; // 宠物声音
    // Start is called before the first frame update
    public void happy()
    {
        GameObject effect = Instantiate(starEffect, transform.position, Quaternion.identity);
        Debug.Log("生成了特效");
        dogAnimator.CrossFade("happy", 0);
        gameManager.instance.hasPetTheDog = true;
        gameManager.instance.dialogInfoIndex++;
        // 延迟销毁：优先根据 ParticleSystem 的时长计算销毁时间，兜底为 2 秒
        float destroyDelay = 2f;
        var ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            var startLifetime = main.startLifetime;
            float maxLifetime = startLifetime.constant;
            if (startLifetime.constantMax > maxLifetime) maxLifetime = startLifetime.constantMax;
            destroyDelay = main.duration + maxLifetime;
        }
        Destroy(effect, destroyDelay);
        gameManager.instance.playSound(petSound);
        Invoke("canControlLuna", 1.75f);   
    }

    private void canControlLuna()
    {
        gameManager.instance.canControlLuna = true;
    }
    void Start()
    {
        dogAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
