using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candle : MonoBehaviour
{
    public GameObject Effect;
    public AudioClip collectSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameManager.instance.candleNum++;
        GameObject eff = Instantiate(Effect, transform.position, Quaternion.identity);
        float destroyDelay = 2f;
        var ps = eff.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            var startLifetime = main.startLifetime;
            float maxLifetime = startLifetime.constant;
            if (startLifetime.constantMax > maxLifetime) maxLifetime = startLifetime.constantMax;
            destroyDelay = main.duration + maxLifetime;
        }
        Destroy(eff, destroyDelay);
        gameManager.instance.playSound(collectSound);
        if(gameManager.instance.candleNum >= 5)
        {
            gameManager.instance.SetContentIndex();
        }
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
