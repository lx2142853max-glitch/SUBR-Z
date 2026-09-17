using SUBR.Player;
using SUBR.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace SUBR.AI
{
    /// <summary>
    /// Simple bot: wander / chase player / shoot. NavMesh optional.
    /// Agar NavMesh nahi hai to transform move fallback.
    /// </summary>
    public sealed class BotController : MonoBehaviour
    {
        [SerializeField] float detectRange = 40f;
        [SerializeField] float attackRange = 22f;
        [SerializeField] float moveSpeed = 3.8f;
        [SerializeField] float repathInterval = 0.5f;
        [SerializeField] WeaponController weapon;

        Transform _target;
        Health _health;
        NavMeshAgent _agent;
        float _repathAt;
        Vector3 _wanderPoint;
        bool _hasNav;

        void Awake()
        {
            _health = GetComponent<Health>();
            _agent = GetComponent<NavMeshAgent>();
            _hasNav = _agent != null;
            if (weapon == null) weapon = GetComponentInChildren<WeaponController>();
            weapon?.ConfigureAsBot();
            _wanderPoint = transform.position;
        }

        public void BindLocalPlayer(Transform player) => _target = player;

        void Update()
        {
            if (_health != null && _health.IsDead)
            {
                if (_hasNav) _agent.isStopped = true;
                return;
            }

            if (_target == null)
            {
                var pc = FindObjectOfType<PlayerController>();
                if (pc) _target = pc.transform;
            }

            float dist = _target ? Vector3.Distance(transform.position, _target.position) : float.MaxValue;

            if (_target && dist <= detectRange)
            {
                Face(_target.position);
                if (dist > attackRange) MoveTowards(_target.position);
                else
                {
                    StopMove();
                    weapon?.SetBotFire(true);
                }
            }
            else
            {
                Wander();
            }
        }

        void Wander()
        {
            if (Time.time >= _repathAt)
            {
                _repathAt = Time.time + repathInterval + Random.Range(0f, 0.5f);
                Vector2 r = Random.insideUnitCircle * 12f;
                _wanderPoint = transform.position + new Vector3(r.x, 0f, r.y);
            }
            MoveTowards(_wanderPoint);
        }

        void MoveTowards(Vector3 pos)
        {
            if (_hasNav && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
                _agent.speed = moveSpeed;
                _agent.SetDestination(pos);
            }
            else
            {
                Vector3 dir = pos - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.1f)
                {
                    transform.position += dir.normalized * moveSpeed * Time.deltaTime;
                    Face(pos);
                }
            }
        }

        void StopMove()
        {
            if (_hasNav && _agent.isOnNavMesh) _agent.isStopped = true;
        }

        void Face(Vector3 world)
        {
            Vector3 d = world - transform.position;
            d.y = 0f;
            if (d.sqrMagnitude < 0.001f) return;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(d), 10f * Time.deltaTime);
        }
    }
}
