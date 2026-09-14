using UnityEngine;

[ExecuteAlways]
public class CameraRotate : MonoBehaviour
{
    [SerializeField] private bool _lockX = true;
    [SerializeField] private bool _lockZ = true;
    [SerializeField] private bool _flip = true;

    private Transform _cameraTransform;

    private void Awake()
    {
        if (Camera.main != null)
            _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null)
        {
            if (Camera.main == null) return;
            _cameraTransform = Camera.main.transform;
        }

        Vector3 forward = _flip ? -_cameraTransform.forward : _cameraTransform.forward;

        if (_lockX) forward.x = 0f;
        if (_lockZ) forward.z = 0f;

        if (forward.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(forward, _cameraTransform.up);
    }
}
