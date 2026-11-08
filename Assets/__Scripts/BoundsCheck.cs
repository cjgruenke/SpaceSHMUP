using UnityEngine;

// Checks whether a GameObject is on screen, and can keep it on screen.
// Used by the hero, projectiles, and every enemy.
public class BoundsCheck : MonoBehaviour {
    // center: the object's center must stay on screen
    // inset:  the whole object must stay on screen (used by the hero)
    // outset: the object counts as off screen only once it has fully left (used by enemies and projectiles)
    public enum eType { center, inset, outset }

    [Header("Set in Inspector")]
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set Dynamically")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;

    [HideInInspector] public bool offRight, offLeft, offUp, offDown;

    void Awake() {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate() {
        float checkRadius = 0f;
        if (boundsType == eType.inset) checkRadius = -radius;
        if (boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;
        offRight = offLeft = offUp = offDown = false;

        if (pos.x > camWidth + checkRadius) {
            pos.x = camWidth + checkRadius;
            offRight = true;
        }
        if (pos.x < -camWidth - checkRadius) {
            pos.x = -camWidth - checkRadius;
            offLeft = true;
        }
        if (pos.y > camHeight + checkRadius) {
            pos.y = camHeight + checkRadius;
            offUp = true;
        }
        if (pos.y < -camHeight - checkRadius) {
            pos.y = -camHeight - checkRadius;
            offDown = true;
        }

        isOnScreen = !(offRight || offLeft || offUp || offDown);

        if (keepOnScreen && !isOnScreen) {
            transform.position = pos;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }
}