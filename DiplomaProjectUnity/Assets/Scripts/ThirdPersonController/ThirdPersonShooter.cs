using Cinemachine;
using StarterAssets;
using UnityEngine;

namespace DiplomaProject.ThirdPersonController
{
    public class ThirdPersonShooter : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _aimCamera;
        [SerializeField] private StarterAssetsInputs _inputs;
        [SerializeField] private StarterAssets.ThirdPersonController _thirdPersonController;
        [SerializeField] private Animator _animator;

        [SerializeField] private float _aimSensitivity = 0.5f;
        [SerializeField] private float _defaultSensitivity = 1f;

        [SerializeField] private LayerMask _aimColliderMask;

        [SerializeField] private Transform _shootPosition;
        [SerializeField] private Projectile _projectilePrefab;

        private void Update()
        {
            Vector3 mouseWorldPosition = Vector3.zero;
            
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, _aimColliderMask))
            {
                mouseWorldPosition = hit.point;
            }

            _aimCamera.gameObject.SetActive(_inputs.aim);
            _thirdPersonController.SetSensitivity(_inputs.aim ? _aimSensitivity : _defaultSensitivity);
            _thirdPersonController.SetRotateOnMove(!_inputs.aim);

            if (_inputs.aim)
            {
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1, Time.deltaTime * 10));
                
                var worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                var aimDirection = (worldAimTarget - transform.position).normalized;
                
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20);
                
                if (_inputs.shoot)
                {
                    aimDirection = (mouseWorldPosition - _shootPosition.position).normalized;
                    Instantiate(_projectilePrefab, _shootPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    _inputs.shoot = false;
                }
            }
            else
            {
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0, Time.deltaTime * 10));
            }
        }
    }
}
