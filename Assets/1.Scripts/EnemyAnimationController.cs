using UnityEngine;
using System;

/// <summary>
/// Điều khiển Animator Enemy
/// Ưu tiên: Die > Hit > Shoot > Idle
/// Dùng bool + Any State (chuẩn production)
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    private Animator anim;

    // Cache hash để tối ưu
    private readonly int isShotHash = Animator.StringToHash("isShot");
    private readonly int isHitHash = Animator.StringToHash("isHit");
    private readonly int isDieHash = Animator.StringToHash("isDie");

    private bool isDead;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /* ===================== PUBLIC API ===================== */

    /// <summary>
    /// Gọi khi enemy bắn
    /// </summary>
    public void PlayShoot()
    {
        //bool temp = anim.GetBool(isShotHash);
        if (isDead) return;
        
        ResetCombatStates();
        //anim.Play("ShootSingleshot_AR_Anim", 0, 0f);
        anim.SetBool(isShotHash, true);
    }
    public void StopShoot()
    {
        anim.SetBool(isShotHash, false);
    }

    /// <summary>
    /// Gọi khi enemy bị trúng đạn
    /// </summary>
    public void PlayHit()
    {
        if (isDead) return;

        //ResetCombatStates();
        anim.SetBool(isHitHash, true);
    }

    /// <summary>
    /// Gọi khi enemy chết
    /// </summary>
    public void PlayDie()
    {
        if (isDead) return;

        isDead = true;
        ResetAllStates();
        anim.SetBool(isDieHash, true);
    }

    /* ===================== RESET ===================== */

    /// <summary>
    /// Reset shoot + hit (KHÔNG reset die)
    /// </summary>
    private void ResetCombatStates()
    {
        anim.SetBool(isShotHash, false);
        anim.SetBool(isHitHash, false);
    }

    /// <summary>
    /// Reset toàn bộ state
    /// </summary>
    private void ResetAllStates()
    {
        anim.SetBool(isShotHash, false);
        anim.SetBool(isHitHash, false);
        anim.SetBool(isDieHash, false);
    }

    /* ===================== ANIMATION EVENT ===================== */

    /// <summary>
    /// Gắn Animation Event cuối clip Shoot / Hit
    /// </summary>
    public void OnActionAnimationEnd()
    {
        Debug.Log("AAAAAAAAA");
        if (isDead) return;

        ResetCombatStates();
    }
}
