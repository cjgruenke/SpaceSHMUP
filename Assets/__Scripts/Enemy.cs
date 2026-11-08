using UnityEngine;

// Base class for every enemy. Used directly by Enemy_0, which flies straight down.
// Enemy_1 to Enemy_4 inherit from it and change how they move.
[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour {
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;              // Units per second, moving down
    public float health = 3f;
    public int score = 100;                // Points for destroying this enemy
    public float showDamageDuration = 0.1f;

    protected BoundsCheck bndCheck;
    protected Material[] materials;
    protected Color[] originalColors;
    protected bool showingDamage = false;
    protected float damageDoneTime;
    protected bool notifiedOfDestruction = false;

    protected virtual void Awake() {
        bndCheck = GetComponent<BoundsCheck>();

        // Remember every material's color so damage flashes can be undone
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        materials = new Material[rends.Length];
        originalColors = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++) {
            materials[i] = rends[i].material;
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos {
        get { return transform.position; }
        set { transform.position = value; }
    }

    void Update() {
        Move();

        if (showingDamage && Time.time > damageDoneTime) {
            UnShowDamage();
        }

        // Remove enemies that fly off the bottom of the screen
        if (bndCheck.offDown) {
            Destroy(gameObject);
        }
    }

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    protected virtual void OnCollisionEnter(Collision coll) {
        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if (p == null) return;

        // Remove the projectile right away so it can't hit twice
        coll.gameObject.SetActive(false);
        Destroy(coll.gameObject);

        // Enemies can't be damaged before they're on screen
        if (!bndCheck.isOnScreen) return;

        ShowDamage();
        health -= p.damage;
        if (health <= 0) {
            Die();
        }
    }

    protected void Die() {
        if (!notifiedOfDestruction) {
            notifiedOfDestruction = true;
            Main.ENEMY_DESTROYED(this);
        }
        Destroy(gameObject);
    }

    // Flash the whole enemy red
    protected void ShowDamage() {
        foreach (Material m in materials) {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    // Flash just one part red (used by the boss)
    protected void ShowLocalizedDamage(Material m) {
        m.color = Color.red;
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    protected void UnShowDamage() {
        for (int i = 0; i < materials.Length; i++) {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}