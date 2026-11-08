using UnityEngine;

// Attached to the hero's Shield. Fades the shield bubble as the shield level drops.
public class Shield : MonoBehaviour {
    [Header("Set in Inspector")]
    public float maxAlpha = 0.5f;   // How see-through the shield is at full strength

    private Material mat;

    void Start() {
        mat = GetComponent<Renderer>().material;
    }

    void Update() {
        if (Hero.S == null) return;

        Color c = mat.color;
        c.a = maxAlpha * Mathf.Clamp01(Hero.S.shieldLevel / 4f);
        mat.color = c;
    }
}