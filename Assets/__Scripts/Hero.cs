using UnityEngine;

// Attached to _Hero. Handles movement, shooting, and taking damage from enemies.
public class Hero : MonoBehaviour {
    static public Hero S;   // The single hero in the scene

    [Header("Set in Inspector")]
    public float speed = 30f;
    public float rollMult = -45f;          // How much the ship tilts when moving left/right
    public float pitchMult = 30f;          // How much the ship tilts when moving up/down
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;
    public float fireDelay = 0.2f;         // Seconds between shots while Space is held
    public float invulnerableTime = 0.5f;  // Brief protection after being hit

    [Header("Set Dynamically")]
    [SerializeField] private int _shieldLevel = 3;

    private float nextFireTime = 0f;
    private float invulnerableUntil = 0f;
    private bool dead = false;

    void Awake() {
        S = this;
    }

    void Update() {
        // Movement (arrow keys or WASD)
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Tilt the ship in the direction it's moving
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);

        // Hold Space to fire
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime) {
            Fire();
            nextFireTime = Time.time + fireDelay;
        }
    }

    void Fire() {
        GameObject projGO = Instantiate(projectilePrefab);
        projGO.transform.position = transform.position + Vector3.up * 2f;
        Rigidbody rb = projGO.GetComponent<Rigidbody>();
        rb.velocity = Vector3.up * projectileSpeed;
    }

    // Called when the Shield trigger touches an enemy
    void OnTriggerEnter(Collider other) {
        GameObject go = other.transform.root.gameObject;
        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy == null) return;

        // Ignore extra hits right after being damaged
        if (Time.time < invulnerableUntil) return;
        invulnerableUntil = Time.time + invulnerableTime;

        // Regular enemies are destroyed when they ram the hero; the boss survives
        if (!(enemy is Enemy_4)) {
            Destroy(go);
        }

        shieldLevel--;
    }

    public int shieldLevel {
        get { return _shieldLevel; }
        private set {
            _shieldLevel = Mathf.Min(value, 4);
            // Getting hit with no shield left destroys the hero
            if (value < 0 && !dead) {
                dead = true;
                Destroy(gameObject);
                Main.HERO_DIED();
            }
        }
    }
}