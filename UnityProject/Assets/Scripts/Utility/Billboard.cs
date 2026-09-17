using UnityEngine;

namespace SUBR.Utility
{
    /// <summary>Nameplates / icons hamesha camera face karein.</summary>
    public sealed class Billboard : MonoBehaviour
    {
        [SerializeField] bool reverse;
        Transform _cam;

        void LateUpdate()
        {
            if (_cam == null && Camera.main) _cam = Camera.main.transform;
            if (_cam == null) return;
            Vector3 dir = transform.position - _cam.position;
            if (reverse) dir = -dir;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
