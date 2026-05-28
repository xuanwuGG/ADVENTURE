using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rabbitMovement : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator rabbitAnimator;

    [Header("Auto Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float minMoveDuration = 1.2f;
    [SerializeField] private float maxMoveDuration = 3.0f;
    [SerializeField] private float minIdleDuration = 0.6f;
    [SerializeField] private float maxIdleDuration = 1.8f;

    private Vector2 lookDirection = Vector2.down;
    private Vector2 currentDirection = Vector2.zero;
    private bool isMoving;
    private float stateTimer;

    private static readonly Vector2[] directions8 = new Vector2[]
    {
        Vector2.up,
        new Vector2(1f, 1f).normalized,
        Vector2.right,
        new Vector2(1f, -1f).normalized,
        Vector2.down,
        new Vector2(-1f, -1f).normalized,
        Vector2.left,
        new Vector2(-1f, 1f).normalized
    };

    void Start()
    {
        if (rabbitAnimator == null)
        {
            rabbitAnimator = GetComponentInChildren<Animator>();
        }

        if (rabbitAnimator == null)
        {
            rabbitAnimator = GetComponent<Animator>();
        }

        if (rabbitAnimator == null)
        {
            Debug.LogError("rabbitMovement: Animator 未找到");
            enabled = false;
            return;
        }

        BeginRandomState();
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            SwitchState();
        }

        UpdateAnimator();
    }

    void LateUpdate()
    {
        if (!isMoving)
        {
            return;
        }

        transform.Translate(currentDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void BeginRandomState()
    {
        if (Random.value > 0.5f)
        {
            BeginMove();
        }
        else
        {
            BeginIdle();
        }
    }

    private void SwitchState()
    {
        if (isMoving)
        {
            BeginIdle();
        }
        else
        {
            BeginMove();
        }
    }

    private void BeginMove()
    {
        isMoving = true;
        currentDirection = directions8[Random.Range(0, directions8.Length)];
        lookDirection = currentDirection;
        stateTimer = Random.Range(minMoveDuration, maxMoveDuration);
    }

    private void BeginIdle()
    {
        isMoving = false;
        currentDirection = Vector2.zero;
        stateTimer = Random.Range(minIdleDuration, maxIdleDuration);
    }

    private void UpdateAnimator()
    {
        if (rabbitAnimator == null)
        {
            return;
        }

        rabbitAnimator.SetFloat("loX", lookDirection.x);
        rabbitAnimator.SetFloat("loY", lookDirection.y);
        rabbitAnimator.SetFloat("moveValue", isMoving ? 1f : 0f);
    }
}
