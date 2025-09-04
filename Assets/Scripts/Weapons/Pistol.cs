using UnityEngine;

public class Pistol : Weapon
{
    public override void PrimaryAction()
    {
        if (TryShoot())
        {
            Debug.Log("Pistol fired!");
            Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 myPos = new(transform.position.x, transform.position.y + 1);
            Vector2 direction = target - myPos;
            direction.Normalize();
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f);
            Instantiate(projectilePrefab, myPos, rotation);
        }
    }

    public override void SecondaryAction()
    {
        TryReload();
    }

    public override void TertiaryAction()
    {
        // Implement any tertiary action for the pistol here
    }

    public override void HandleShoot()
    {
        CurrentAmmo--;
    }
}