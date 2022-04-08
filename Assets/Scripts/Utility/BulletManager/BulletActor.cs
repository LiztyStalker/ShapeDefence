namespace Utility.Bullet
{
    using UnityEngine;
    using PoolSystem;
    using Data;
    using Effect;
    using SDefence.Actor;
    using SDefence.Attack;
    using SDefence.Movement;
    using SDefence.Attack.Usable;

    public class BulletActor : MonoBehaviour, IPoolElement, IMoveable
    {

        private const float ARRIVE_DISTANCE = 0.1f;

        private IAttackable _attackable;

        private GameObject _graphicObject;
        private BulletData _data;

        private Vector2 _startPos;
        private Vector2 _arrivePos;

        private IMovementUsableData _movementData;
        private IMovementActionUsableData _movementActionData;
        private AttackActionUsableData _attackActionData;

        private float _nowTime = 0f;

        private bool _isAttack = false;

        private SpriteRenderer _spriteRenderer { get; set; }
        private ParticleSystem[] _particles { get; set; }

        public Vector2 NowPosition => transform.position;

        private bool _isEffectActivate = false;

        private void SetName()
        {
            gameObject.name = $"BulletActor_{_data.name}";
        }

        public void SetData(BulletData data)
        {
            _data = data;
            _movementData = _data.GetMovementUsableData();
            _movementActionData = _data.GetMovementActionUsableData();
            _attackActionData = _data.GetAttackActionUsableData();

            _attackActionData.SetOnAttackActionListener(OnDamageEvent);

            _movementActionData.SetOnEndedActionListener(OnArriveEvent);
            SetName();
        }

        public void SetData(IAttackable attackable)
        {
            _attackable = attackable;
        }

        public void SetGraphicObject(GameObject graphicObject)
        {
            if (_graphicObject != null) DestroyImmediate(_graphicObject);

            _graphicObject = Instantiate(graphicObject);
            _graphicObject.transform.SetParent(transform);
            _graphicObject.transform.localPosition = Vector3.zero;
            _graphicObject.transform.localScale = Vector3.one;
        }

        public void SetPosition(Vector2 startPos, Vector2 arrivePos)
        {
            _startPos = startPos;
            _arrivePos = arrivePos;
        }


        public bool Contains(BulletData data) => _data == data;

        public void Activate()
        {
            _nowTime = 0f;
            _isEffectActivate = false;
            gameObject.SetActive(true);

        }

        public void Inactivate()
        {
            gameObject.SetActive(false);
            _attackable = null;
            _movementActionData = null;
            _attackActionData = null;
            _startPos = Vector2.zero;
            _arrivePos = Vector2.zero;
            _nowTime = 0f;
            _inactiveEvent?.Invoke(this);
        }

        public void CleanUp()
        {
            _data = null;
            _arrivedEvent = null;
            _inactiveEvent = null;
            _startPos = Vector2.zero;
            _arrivePos = Vector2.zero;
        }

        public void SetPosition(Vector2 pos) => transform.position = pos;

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }
        public void RunProcess(float deltaTime)
        {
            if (!_isAttack)
                _attackActionData.RunProcess(deltaTime);
            else
                _movementActionData.RunProcess(this, _movementData, deltaTime, _arrivePos);
        }             

        private void ReadyInactivate()
        {

            for (int i = 0; i < _particles.Length; i++)
            {
                var main = _particles[i].main;
                main.loop = false;
            }

            if (_spriteRenderer != null)
                _spriteRenderer.enabled = false;

            if (_particles == null)
            {
                Inactivate();
            }
            else
            {
                int cnt = 0;
                for (int i = 0; i < _particles.Length; i++)
                {
                    if (!_particles[i].isPlaying)
                    {
                        cnt++;
                    }
                }
                if (cnt == _particles.Length)
                {
                    Inactivate();
                }
            }
        }


        public void OnTriggerEnter2D(Collider2D col)
        {
            var damagable = col.GetComponent<IDamagable>();
            if (damagable != null)
            {
                if (damagable != _attackable)
                {
                    if (damagable.IsDamagable)
                    {
                        //damagable.SetDamage(_attackable.AttackUsableData);
                        OnCollisionEvent();
                        _isAttack = true;
                    }
                }
            }

            //if(count == 0) Inactivate();
        }



        #region ##### Listener #####

        private System.Action<float, bool> _damageEvent;
        private System.Action<BulletActor> _collisionEvent;
        private System.Action<BulletActor> _arrivedEvent;
        private System.Action<BulletActor> _inactiveEvent;

        public void SetOnArrivedListener(System.Action<BulletActor> act) => _arrivedEvent = act;
        public void SetOnInactiveListener(System.Action<BulletActor> act) => _inactiveEvent = act;
        public void SetOnCollisionListener(System.Action<BulletActor> act) => _collisionEvent = act;
        public void SetOnDamageListener(System.Action<float, bool> act) => _damageEvent = act;

        private void OnArriveEvent()
        {
            Debug.Log("Arrive");
            _arrivedEvent?.Invoke(this);
            Inactivate();
        }

        private void OnCollisionEvent()
        {
            Debug.Log("Collision");
            _collisionEvent?.Invoke(this);
        }

        private void OnDamageEvent(float range, bool isOverlap)
        {
            _damageEvent?.Invoke(range, isOverlap);
        }

        #endregion


    }
}