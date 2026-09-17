using UnityEngine;

namespace SUBR.CameraSys
{
    /// <summary>Simple late-update follow. Main Camera pe lagao, target = player CameraPivot.</summary>
    public sealed class TpsCameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 shoulderOffset = new Vector3(0.45f, 0.2f, -3.2f);
        [SerializeField] float followLerp = 18f;
        [SerializeField] float collisionRadius = 0.2f;
        [SerializeField] LayerMask collisionMask = ~0;

        public void SetTarget(Transform t) => target = t;

        void LateUpdate()
        {
            if (!target) return;

            Vector3 desired = target.TransformPoint(shoulderOffset);
            Vector3 origin = target.position + Vector3.up * 0.5f;
            Vector3 dir = desired - origin;
            float dist = dir.magnitude;
            if (dist > 0.01f &&
                Physics.SphereCast(origin, collisionRadius, dir.normalized, out var hit, dist, collisionMask))
            {
                desired = origin + dir.normalized * Mathf.Max(0.2f, hit.distance - 0.05f);
            }

            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followLerp * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 1f - Mathf.Exp(-followLerp * Time.deltaTime));
        }
    }
}
