using UnityEngine;
using UnityEngine.Events;

public class PlayerAim : Singleton<PlayerAim>
{

    [SerializeField] Animator characterAnimator;
    [SerializeField] Transform weaponPivot;
    [SerializeField] float throwForce;

    Vector3 mousePosition;

    public enum FacingDirection
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

    FacingDirection currentFacingDirection;
    public FacingDirection CurrentFacingDirection => currentFacingDirection;

    FacingDirection oldFacingDirection;

    public UnityEvent<FacingDirection> onFacingDirectionChanged = new UnityEvent<FacingDirection>();

    void FixedUpdate()
    {
        RotateWeaponToCursor();
    }

    public void ThrowWeapon()
    {
        if (!PlayerInventory.Instance.HasCurrentWeapon) return;
        // Implement weapon throwing logic here
        Debug.Log("Weapon thrown!");

        PlayerInventory.Instance.ThrowCurrentWeapon(mousePosition, throwForce);

    }

    void RotateWeaponToCursor()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = weaponPivot.position.z;

        Vector3 lookDir = (mousePosition - weaponPivot.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        currentFacingDirection = GetFacingDirection(angle);
        if (currentFacingDirection != oldFacingDirection)
        {
            onFacingDirectionChanged.Invoke(currentFacingDirection);
            oldFacingDirection = currentFacingDirection;
        }

        characterAnimator.SetInteger("Facing", (int)currentFacingDirection);
    }

    FacingDirection GetFacingDirection(float angle)
    {
        if (angle >= 67.5f && angle < 112.5f)
            return FacingDirection.N;
        else if (angle >= 112.5f && angle < 157.5f)
            return FacingDirection.NW;
        else if (angle >= 157.5f || angle < -157.5f)
            return FacingDirection.W;
        else if (angle >= -157.5f && angle < -112.5f)
            return FacingDirection.SW;
        else if (angle >= -112.5f && angle < -67.5f)
            return FacingDirection.S;
        else if (angle >= -67.5f && angle < -22.5f)
            return FacingDirection.SE;
        else if (angle >= -22.5f && angle < 22.5f)
            return FacingDirection.E;
        else // angle >= 22.5f && angle < 67.5f
            return FacingDirection.NE;
    }
}