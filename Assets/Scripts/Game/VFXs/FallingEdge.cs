using UnityEngine;

public class FallingEdge : MonoBehaviour {
    void Awake() {
        var Particle = GetComponent<ParticleSystem>();
        var shape = Particle.shape;
        shape.radius = Camera.main.orthographicSize * Camera.main.aspect;
    }
}
