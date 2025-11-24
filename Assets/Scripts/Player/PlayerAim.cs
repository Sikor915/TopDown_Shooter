using UnityEngine;

public class PlayerAim : Singleton<PlayerAim>
{

    [SerializeField] Animator characterAnimator;
    [SerializeField] Transform weaponPivot;

    enum FacingDirection
    {
        NW,
        N,
        NE,
        E,
        S,
        W
    }

    void FixedUpdate()
    {
        RotateWeaponToCursor();
    }

    void RotateWeaponToCursor()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = weaponPivot.position.z;

        Vector3 lookDir = (mousePos - weaponPivot.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        FacingDirection facingDir = GetFacingDirection(angle);

        characterAnimator.SetInteger("Facing", (int)facingDir);
    }

    FacingDirection GetFacingDirection(float angle)
    {
        if (angle >= 67.5f && angle < 112.5f)
            return FacingDirection.N;
        else if (angle >= 22.5f && angle < 67.5f)
            return FacingDirection.NE;
        else if (angle >= -22.5f && angle < 22.5f)
            return FacingDirection.E;
        else if (angle >= -157.5f && angle < -22.5f)
            return FacingDirection.S;
        else if (angle >= 112.5f && angle < 157.5f)
            return FacingDirection.NW;
        else
            return FacingDirection.W;
    }
}