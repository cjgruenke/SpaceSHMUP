using UnityEngine;

// Attached to the ProjectileHero prefab. Removes the projectile once it leaves the top of the screen.
[RequireComponent(typeof(BoundsCheck))]
public class ProjectileHero : MonoBehaviour {
    [Header("Set in Inspector")]
    public float damage = 1f;

    private BoundsCheck bndCheck;

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();
    }

    void Update() {
        if (bndCheck.offUp) {
            Destroy(gameObject);
        }
    }
}