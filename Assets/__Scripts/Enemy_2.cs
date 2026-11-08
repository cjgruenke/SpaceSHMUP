using UnityEngine;

// Enemy_2 streaks across the screen from one side to the other, swerving as it goes.
public class Enemy_2 : Enemy {
    [Header("Set in Inspector: Enemy_2")]
    public float lifeTime = 6f;            // Seconds to cross the screen
    public float sinEccentricity = 0.3f;   // How much it speeds up, slows down, and doubles back

    private Vector3 p0, p1;
    private float birthTime;

    void Start() {
        float yMin = -bndCheck.camHeight * 0.3f;
        float yMax = bndCheck.camHeight * 0.8f;

        // Start just off the left edge and end just off the right edge
        p0 = new Vector3(-bndCheck.camWidth - bndCheck.radius, Random.Range(yMin, yMax), 0);
        p1 = new Vector3(bndCheck.camWidth + bndCheck.radius, Random.Range(yMin, yMax), 0);

        // Half the time, go right to left instead
        if (Random.value > 0.5f) {
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;
        pos = p0;
    }

    public override void Move() {
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1) {
            Destroy(gameObject);
            return;
        }

        // Barrel roll twice while crossing
        transform.rotation = Quaternion.Euler(0, u * 720f, 0);

        // Easing makes it swerve back and forth instead of moving at a steady speed
        u = u + sinEccentricity * Mathf.Sin(u * Mathf.PI * 2f);
        pos = (1 - u) * p0 + u * p1;
    }
}