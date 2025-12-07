using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
class PlayerAnimationController : Singleton<PlayerAnimationController>
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    readonly Dictionary<Direction, Tuple<float, float>> dirXY = new Dictionary<Direction, Tuple<float, float>>
    {
        [Direction.NW] = Tuple.Create(-1f, 1f),
        [Direction.N] = Tuple.Create(0f, 1f),
        [Direction.NE] = Tuple.Create(1f, 1f),
        [Direction.E] = Tuple.Create(1f, 0f),
        [Direction.SE] = Tuple.Create(1f, -1f),
        [Direction.S] = Tuple.Create(0f, -1f),
        [Direction.SW] = Tuple.Create(-1f, -1f),
        [Direction.W] = Tuple.Create(-1f, 0f),
    };
    enum Direction
    {
        NW,
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W
    }

    void OnEnable()
    {
        PlayerAim.Instance.onFacingDirectionChanged.AddListener(UpdateSprite);
    }

    void OnDisable()
    {
        PlayerAim.Instance.onFacingDirectionChanged.RemoveListener(UpdateSprite);
    }

    public void SetIsMoving(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    public void SetIsSliding(bool isSliding)
    {
        animator.SetBool("isSliding", isSliding);
    }

    public void SetIsRolling(bool isRolling)
    {
        animator.SetBool("isRolling", isRolling);
    }

    public void UpdateSprite(PlayerAim.FacingDirection facingDirection)
    {
        switch (facingDirection)
        {
            case PlayerAim.FacingDirection.NW:
                animator.SetFloat("DirX", dirXY[Direction.NW].Item1);
                animator.SetFloat("DirY", dirXY[Direction.NW].Item2);
                spriteRenderer.flipX = true;
                break;
            case PlayerAim.FacingDirection.N:
                animator.SetFloat("DirX", dirXY[Direction.N].Item1);
                animator.SetFloat("DirY", dirXY[Direction.N].Item2);
                spriteRenderer.flipX = false;
                break;
            case PlayerAim.FacingDirection.NE:
                animator.SetFloat("DirX", dirXY[Direction.NE].Item1);
                animator.SetFloat("DirY", dirXY[Direction.NE].Item2);
                spriteRenderer.flipX = false;
                break;
            case PlayerAim.FacingDirection.E:
                animator.SetFloat("DirX", dirXY[Direction.E].Item1);
                animator.SetFloat("DirY", dirXY[Direction.E].Item2);
                spriteRenderer.flipX = false;
                break;
            case PlayerAim.FacingDirection.SE:
                animator.SetFloat("DirX", dirXY[Direction.SE].Item1);
                animator.SetFloat("DirY", dirXY[Direction.SE].Item2);
                spriteRenderer.flipX = false;
                break;
            case PlayerAim.FacingDirection.S:
                animator.SetFloat("DirX", dirXY[Direction.S].Item1);
                animator.SetFloat("DirY", dirXY[Direction.S].Item2);
                spriteRenderer.flipX = false;
                break;
            case PlayerAim.FacingDirection.SW:
                animator.SetFloat("DirX", dirXY[Direction.SW].Item1);
                animator.SetFloat("DirY", dirXY[Direction.SW].Item2);
                spriteRenderer.flipX = true;
                break;
            case PlayerAim.FacingDirection.W:
                animator.SetFloat("DirX", dirXY[Direction.W].Item1);
                animator.SetFloat("DirY", dirXY[Direction.W].Item2);
                spriteRenderer.flipX = true;
                break;
        }
    }
    public void FlashRed()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRedCoroutine());
    }

    IEnumerator FlashRedCoroutine(float interval = 0.1f)
    {
        Color originalColor = spriteRenderer.color;
        int times = PlayerController.Instance.PlayerSO.iFrameDuration % (interval * 2) == 0 ?
            (int)(PlayerController.Instance.PlayerSO.iFrameDuration / (interval * 2)) :
            (int)(PlayerController.Instance.PlayerSO.iFrameDuration / (interval * 2)) + 1;
        
        for (int i = 0; i < times; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(interval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(interval);
        }
    }
}