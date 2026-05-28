using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class lunaMovement : MonoBehaviour
{
    Transform jumppointA;
    Transform jumppointB;
    public float speed = 5f;
    private Animator lunaAnimator;
    private Rigidbody2D rigidbody2d;
    private Vector2 lookDirection = new Vector2(0, -1);
    private Transform childObject; // 子对象引用

    // Start is called before the first frame update
    public void Pet()
    {

            rigidbody2d.position = new Vector2(3.1f, -4.2f);
            lunaAnimator.CrossFade("pet", 0);

    }
    public void talk()
    {
        Collider2D cd = Physics2D.OverlapCircle(rigidbody2d.position, 0.8f, LayerMask.GetMask("NPC"));
        if (cd != null)
        {
            if(cd.name == "Nala")
            {
                gameManager.instance.canControlLuna = false;
                lunaAnimator.SetFloat("moveValue", 0f);
                cd.GetComponent<NPCDialog>().DisplayDialog();
            }else if(cd.name == "dog"  && gameManager.instance.dialogInfoIndex >= 2)
            {
                gameManager.instance.canControlLuna = false;
                Pet();
                cd.GetComponent<dog>().happy();
            }
        }
    }
    void Start()
    {
        var _ = UImanager.instance; // 确保 UImanager 实例已创建

        // 获取子对象
        childObject = transform.GetChild(0);
        if (childObject == null)
        {
            Debug.LogError("子对象未找到！");
            return;
        }

        // 获取子对象的 Animator
        lunaAnimator = childObject.GetComponent<Animator>();
        if (lunaAnimator == null)
        {
            Debug.LogError("子对象的 Animator 未找到！");
        }

        // 获取 Rigidbody2D
        rigidbody2d = GetComponent<Rigidbody2D>();
        gameManager.instance.canControlLuna = true; 
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.instance.canControlLuna == false)
        {
            lunaAnimator.SetFloat("moveValue", 0f);
            return;
        }

        // 获取输入
        float loX = Input.GetAxis("Horizontal");
        float loY = Input.GetAxis("Vertical");

        // 计算移动向量
        Vector2 movement = new Vector2(loX, loY);

        // 更新 lookDirection，仅在有输入时更新方向
        if (!Mathf.Approximately(movement.x, 0f) || !Mathf.Approximately(movement.y, 0f))
        {
            lookDirection.Set(movement.x, movement.y);
            lookDirection.Normalize();
        }

        // 设置 Animator 参数
        lunaAnimator.SetFloat("loX", lookDirection.x);
        lunaAnimator.SetFloat("loY", lookDirection.y);

        // 检查是否按住 LeftShift
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // 根据移动向量的长度设置 moveValue
        float moveValue = movement.magnitude > 0 ? (isRunning ? 2 : 1) : 0;
        lunaAnimator.SetFloat("moveValue", moveValue);

        // 移动角色
        float currentSpeed = isRunning ? speed * 1.5f : speed; // 跑步时速度增加 1.5 倍
        transform.Translate(movement.normalized * currentSpeed * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("按下了空格键，尝试与NPC交互");
            talk();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("vine")) // 检查是否是绿藤
        {
            lunaAnimator.SetTrigger("climb"); // 触发爬行动画
            Debug.Log("触发爬行动画");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("vine")) // 检查是否是绿藤
        {
            lunaAnimator.SetBool("climb", false); // 取消爬行动画触发
            Debug.Log("取消爬行动画触发");
        }
    }
}
