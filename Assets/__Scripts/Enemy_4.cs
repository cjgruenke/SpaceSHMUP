using UnityEngine;

// One destructible piece of the boss.
[System.Serializable]
public class Part {
    public string name;              // Must match a child object's name exactly
    public float health;
    public string[] protectedBy;     // Parts that must be destroyed before this one can be damaged

    [HideInInspector] public GameObject go;
    [HideInInspector] public Material mat;

    public Part() { }

    public Part(string n, float h, params string[] prot) {
        name = n;
        health = h;
        protectedBy = prot;
    }
}

// Enemy_4 is the boss. It glides between random positions, and its parts must be destroyed in order.
public class Enemy_4 : Enemy {
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts = new Part[] {
        new Part("Shield", 6),
        new Part("Wing_L", 6),
        new Part("Wing_R", 6),
        new Part("Arm_L", 6, "Wing_L"),
        new Part("Arm_R", 6, "Wing_R"),
        new Part("Core", 12, "Shield", "Arm_L", "Arm_R")
    };
    public float duration = 4f;   // Seconds to glide to each new position

    private Vector3 p0, p1;
    private float timeStart;

    void Start() {
        // Find each part's child object and its material
        foreach (Part prt in parts) {
            Transform t = transform.Find(prt.name);
            if (t != null) {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            } else {
                Debug.LogWarning("Enemy_4: no child object named " + prt.name);
            }
        }

        p1 = pos;
        InitMovement();
    }

    // Pick a new random position in the top half of the screen
    void InitMovement() {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1 = new Vector3(Random.Range(-widMinRad, widMinRad), Random.Range(0, hgtMinRad), 0);
        timeStart = Time.time;
    }

    public override void Move() {
        float u = (Time.time - timeStart) / duration;
        if (u >= 1) {
            InitMovement();
            u = 0;
        }

        // Ease out: fast at first, slowing as it arrives
        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(GameObject go) {
        foreach (Part prt in parts) {
            if (prt.go == go) return prt;
        }
        return null;
    }

    bool Destroyed(string partName) {
        foreach (Part prt in parts) {
            if (prt.name == partName) {
                return prt.go == null || !prt.go.activeSelf;
            }
        }
        return true;
    }

    protected override void OnCollisionEnter(Collision coll) {
        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if (p == null) return;

        coll.gameObject.SetActive(false);
        Destroy(coll.gameObject);

        if (!bndCheck.isOnScreen) return;

        // Which part of the boss was hit
        GameObject goHit = coll.GetContact(0).thisCollider.gameObject;
        Part prtHit = FindPart(goHit);
        if (prtHit == null) return;

        // A protected part takes no damage
        if (prtHit.protectedBy != null) {
            foreach (string s in prtHit.protectedBy) {
                if (!Destroyed(s)) return;
            }
        }

        prtHit.health -= p.damage;
        ShowLocalizedDamage(prtHit.mat);
        if (prtHit.health <= 0) {
            prtHit.go.SetActive(false);
        }

        // The boss is destroyed once every part is gone
        foreach (Part prt in parts) {
            if (!Destroyed(prt.name)) return;
        }
        Die();
    }
}