using UnityEngine;

// Enemy_1 weaves left and right in a sine wave while moving down.
public class Enemy_1 : Enemy {
    [Header("Set in Inspector: Enemy_1")]
    public float waveFrequency = 2f;   // Seconds for one full left-right cycle
    public float waveWidth = 6f;       // How far it moves to each side
    public float waveRotY = 45f;       // How much it banks as it weaves

    private float x0;
    private float birthTime;

    void Start() {
        x0 = pos.x;
        birthTime = Time.time;
    }

    public override void Move() {
        Vector3 tempPos = pos;

        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2f * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        transform.rotation = Quaternion.Euler(0, sin * waveRotY, 0);

        // The base class handles moving down
        base.Move();
    }
}