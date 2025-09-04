using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Scriptable Objects/Projectile")]
public class ProjectileSO : ScriptableObject
{
    public float lifetime;
    public virtual void Launch(Vector3 direction, float speed) { }
    public virtual void OnImpact() { }

}
