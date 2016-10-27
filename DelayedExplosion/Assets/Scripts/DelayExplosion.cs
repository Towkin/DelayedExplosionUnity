using UnityEngine;
using System.Collections;

public class DelayExplosion : MonoBehaviour {
    [SerializeField]
    private float m_ExplosionDelayTime = 1.0f;
    private float m_ExplosionTriggerTime;
    public float ExplosionTriggerTime {
        get { return m_ExplosionTriggerTime; }
    }
    private float m_ExplosionEndTime = 0.0f;
    public float ExplosionEndTime {
        get { return m_ExplosionEndTime; }
    }
    [SerializeField]
    private Vector3 m_ExplosionOffset = Vector3.zero;
    public Vector3 ExplosionLocation {
        get { return transform.position + transform.rotation * m_ExplosionOffset; }
    }
    
    [Tooltip("Radius of the force effect over time.")]
    [SerializeField]
    private AnimationCurve m_ExplosionRadius;
    public AnimationCurve ExplosionRadius {
        get { return m_ExplosionRadius; }
    }
    [Tooltip("Force applied to the object in radius over time (note: uses falloff as multiplier).")]
    [SerializeField]
    private AnimationCurve m_ExplosionForce;
    public AnimationCurve ExplosionForce {
        get { return m_ExplosionForce; }
    }
    [Tooltip("Force multiplier over distance from epicenter, defined by radius. Time-/x-axis should cover range [0...1].")]
    [SerializeField]
    private AnimationCurve m_ForceFalloff;
    public AnimationCurve ForceFalloff {
        get { return m_ForceFalloff; }
    }
    
    void Start () {
        if(ExplosionForce.length == 0 || ExplosionRadius.length == 0) {
            Destroy(this);
        }

        m_ExplosionTriggerTime = Time.time + m_ExplosionDelayTime;

        foreach (Keyframe Key in ExplosionRadius.keys) {
            m_ExplosionEndTime = Mathf.Max(ExplosionEndTime, ExplosionTriggerTime + Key.time);
        }
        foreach (Keyframe Key in ExplosionForce.keys) {
            m_ExplosionEndTime = Mathf.Max(ExplosionEndTime, ExplosionTriggerTime + Key.time);
        }
    }
	
    void FixedUpdate() {
        if(Time.time > ExplosionTriggerTime) {
            if(Time.time > ExplosionEndTime) {
                Destroy(this);
            }

            float ExplosionTime = Time.time - ExplosionTriggerTime;
            float Radius = ExplosionRadius.Evaluate(ExplosionTime);
            float Force = ExplosionForce.Evaluate(ExplosionTime);

            Collider[] HitColliders = Physics.OverlapSphere(transform.position, Radius);
            
            foreach(Collider HitCollider in HitColliders) {
                if(HitCollider.gameObject == this.gameObject) {
                    continue;
                }
                Rigidbody Body = HitCollider.GetComponent<Rigidbody>();
                if (Body == null) {
                    continue;
                }
                float BodyDistance = Vector3.Distance(Body.transform.position, transform.position);

                Body.AddExplosionForce(Force * ForceFalloff.Evaluate(BodyDistance / Radius), ExplosionLocation, 0.0f, 0.0f, ForceMode.Force);
            }
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (Time.time > ExplosionTriggerTime) {
            float ExplosionTime = Time.time - ExplosionTriggerTime;
            float Radius = ExplosionRadius.Evaluate(ExplosionTime);
            float Force = ExplosionForce.Evaluate(ExplosionTime);

            Gizmos.color = new Color(0.5f + Force / 500, 0.5f - Force / 500, 0, Mathf.Clamp(Mathf.Abs(Force) / 1000, 0.1f, 0.75f));
            Gizmos.DrawSphere(transform.position, Radius);
        }
    }
#endif
}

