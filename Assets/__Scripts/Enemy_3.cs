using UnityEngine;

// Enemy_3 swoops down and back up along a Bezier curve.
public class Enemy_3 : Enemy {
    [Header("Set in Inspector: Enemy_3")]
    public float lifeTime = 5f;
    public Vector2 midpointYRange = new Vector2(1.5f, 2f);   // Controls how low it dips

    private Vector3[] points;
    private float birthTime;

    void Start() {
        points = new Vector3[3];

        // Start where Main placed it (above the screen)
        points[0] = pos;

        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        // The middle control point is far below the screen, which pulls the curve down
        points[1] = new Vector3(Random.Range(xMin, xMax),
            -bndCheck.camHeight * Random.Range(midpointYRange.x, midpointYRange.y), 0);

        // End back above the screen at a different x
        points[2] = new Vector3(Random.Range(xMin, xMax), pos.y, 0);

        birthTime = Time.time;
    }

    public override void Move() {
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1) {
            Destroy(gameObject);
            return;
        }

        // Flip over as it turns around at the bottom
        transform.rotation = Quaternion.Euler(u * 180f, 0, 0);

        // Slight easing, then a quadratic Bezier curve through the three points
        u = u - 0.1f * Mathf.Sin(u * Mathf.PI * 2f);
        Vector3 p01 = (1 - u) * points[0] + u * points[1];
        Vector3 p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}