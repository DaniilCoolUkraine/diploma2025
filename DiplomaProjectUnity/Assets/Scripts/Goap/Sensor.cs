using System;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DiplomaProject.Goap
{
    public class Sensor : MonoBehaviour
    {
        [SerializeField, Required] private Collider _collider;
        
        [SerializeField] private float _detectionRange;
        [SerializeField] private float _timerInterval;

        public event Action OnTargetChanged = delegate { };

        public Vector3 TargetPosition => _target ? _target.transform.position : Vector3.zero;
        public bool IsInRange => TargetPosition != Vector3.zero;
        public float DetectionRange => _detectionRange;
        
        private GameObject _target;
        private Vector3 _lastPosition;

        private CountdownTimer _timer;

        private void Start()
        {
            _timer = new CountdownTimer(_timerInterval);
            _timer.OnTimerStop += () =>
            {
                UpdateTargetPosition(_target);
                _timer.Start();
            };

            _timer.Start();
        }

        private void Update()
        {
            _timer.Tick();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            UpdateTargetPosition(other.gameObject);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            UpdateTargetPosition();
        }

        private void UpdateTargetPosition(GameObject target = null)
        {
            _target = target;

            if (IsInRange && (_lastPosition != TargetPosition || _lastPosition != Vector3.zero))
            {
                _lastPosition = TargetPosition;
                OnTargetChanged.Invoke();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.Undo.RecordObject(gameObject, "Set Collider");

            _collider = GetComponent<Collider>();

            if (_collider != null)
            {
                _collider.isTrigger = true;
                if (_collider is SphereCollider sphereCollider)
                    sphereCollider.radius = _detectionRange;

                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
        }
#endif
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Sensor))]
    public class SensorEditor : UnityEditor.Editor
    {
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.Selected)]
        static void DrawGizmo(Sensor sensor, UnityEditor.GizmoType gizmoType)
        {
            Gizmos.color = sensor.IsInRange ? new Color(1f, 0f, 0f, 0.5f) : new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawSphere(sensor.transform.position, sensor.DetectionRange);
        }
    }
#endif
}