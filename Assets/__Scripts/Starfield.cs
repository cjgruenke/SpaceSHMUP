using UnityEngine;

// Attached to each starfield quad. Scrolls the star texture downward forever.
public class Starfield : MonoBehaviour {
    [Header("Set in Inspector")]
    public float scrollSpeed = 0.05f;   // Higher = faster (use a higher value for the near layer)

    private Material mat;

    void Start() {
        mat = GetComponent<Renderer>().material;
    }

    void Update() {
        Vector2 offset = mat.mainTextureOffset;
        offset.y = (Time.time * scrollSpeed) % 1f;
        mat.mainTextureOffset = offset;
    }
}