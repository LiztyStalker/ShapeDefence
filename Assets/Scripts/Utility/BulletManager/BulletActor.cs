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
            _movementActionData.RunProcess(this, _movementData, deltaTime, _arrivePos);
        }
              

        //private Vector3 GetEuler(Vector3 startPos, Vector3 arrivePos, float nowTime)
        //{
        //    var nowPos = startPos;
        //    var direction = arrivePos - nowPos;
        //    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //    return Vector3.forward * angle;
        //}

        //private Vector2 GetPosition(Vector3 startPos, Vector3 arrivePos, float nowTime)
        //{
        //    switch (_data.TypeBulletAction)
        //    {
        //        case BulletData.TYPE_BULLET_ACTION.Curve:
        //            return Ditzel.Parabola.MathParabola.Parabola(startPos, arrivePos, 0.5f, nowTime);
        //        case BulletData.TYPE_BULLET_ACTION.Move:
        //            return Vector2.MoveTowards(startPos, arrivePos, nowTime);
        //        case BulletData.TYPE_BULLET_ACTION.Drop:
        //            var pos = new Vector2(arrivePos.x, arrivePos.y + 10f);
        //            return Vector2.MoveTowards(pos, arrivePos, nowTime);
        //        case BulletData.TYPE_BULLET_ACTION.Direct:
        //            return arrivePos;
        //    }
        //    return Vector2.zero;
        //}

        //private Vector2 GetSlerp(Vector3 startPos, Vector3 arrivePos, float nowTime)
        //{
        //    var center = (startPos + arrivePos) * 8f;
        //    center -= Vector3.up;

        //    var startRelPos = startPos - center;
        //    var arriveRelPos = arrivePos - center;
        //    var nowPos = Vector3.Slerp(startRelPos, arriveRelPos, nowTime);
        //    nowPos += center;
        //    return nowPos;
        //}


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
                        damagable.SetDamage(_attackable.AttackUsableData);
                        OnArriveEvent();
                        //count--;
                    }
                }
            }

            //if(count == 0) Inactivate();
        }



        #region ##### Listener #####

        private System.Action<BulletActor> _arrivedEvent;
        private System.Action<BulletActor> _inactiveEvent;

        public void SetOnArrivedListener(System.Action<BulletActor> act) => _arrivedEvent = act;
        public void SetOnInactiveListener(System.Action<BulletActor> act) => _inactiveEvent = act;


        private void OnArriveEvent()
        {
            Debug.Log("Arrive");
            _arrivedEvent?.Invoke(this);
            Inactivate();
        }

        #endregion


    }
}