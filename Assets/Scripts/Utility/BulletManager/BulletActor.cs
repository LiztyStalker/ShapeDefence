namespace Utility.Bullet
{
    using UnityEngine;
    using PoolSystem;
    using Data;
    using Effect;

    public class BulletActor : MonoBehaviour, IPoolElement
    {

        private const float ARRIVE_DISTANCE = 0.1f;

        private GameObject _prefab;
        private BulletData _data;


        private Vector2 _startPos;
        private Vector2 _arrivePos;

        private float _nowTime = 0f;

        private SpriteRenderer _spriteRenderer { get; set; }
        private ParticleSystem[] _particles { get; set; }

        private bool _isEffectActivate = false;

        private void SetName()
        {
            gameObject.name = $"BulletActor_{_data.name}";
        }

        public void SetData(BulletData data)
        {
            _data = data;
            SetName();
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

            if (_prefab == null)
            {
                //flyweight ÇÊ¿ä
                _prefab = Instantiate(_data.prefab);
                _prefab.transform.SetParent(transform);
                _prefab.transform.localPosition = Vector3.zero;
                _prefab.transform.localScale = Vector3.one;
                _prefab.gameObject.SetActive(true);
            }

            transform.position = _startPos;

            _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            if (_spriteRenderer != null)
            {
                //_spriteRenderer.sortingLayerName = "FrontEffect";
                //_spriteRenderer.sortingOrder = (int)-transform.position.y - 5;
                _spriteRenderer.enabled = true;
            }

            _particles = _prefab.GetComponentsInChildren<ParticleSystem>(true);
            //if (_particles != null)
            //{
            //    for (int i = 0; i < _particles.Length; i++)
            //    {
            //        var psRenderer = _particles[i].GetComponent<ParticleSystemRenderer>();
            //        if(psRenderer != null)
            //        {
            //            psRenderer.sortingLayerName = "FrontEffect";
            //            psRenderer.sortingOrder = (int)-transform.position.y + 10;
            //        }
            //    }
            //}
            gameObject.SetActive(true);

        }

        public void Inactivate()
        {
            gameObject.SetActive(false);
            _startPos = Vector2.zero;
            _arrivePos = Vector2.zero;
            _nowTime = 0f;
            _inactiveEvent?.Invoke(this);
        }

        public void CleanUp()
        {
            DestroyImmediate(_prefab);
            _data = null;
            _arrivedEvent = null;
            _inactiveEvent = null;
            _startPos = Vector2.zero;
            _arrivePos = Vector2.zero;
            _prefab = null;
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }
        private void Update()
        {

            _nowTime = CalculateTime(_nowTime);
            transform.position = GetPosition(_startPos, _arrivePos, _nowTime);

            if (_data.IsRotate) transform.eulerAngles = GetEuler(_startPos, _arrivePos, _nowTime);

            //Debug.Log(transform.position + " " + _startPos + " " + _arrivePos + " " + _nowTime);

            if (Vector2.Distance(transform.position, _arrivePos) < ARRIVE_DISTANCE)
            {
                if (!_isEffectActivate)
                {
                    OnArriveEvent();
                    EffectManager.Current.Activate(_data.ArriveEffectData, transform.position);
                    _isEffectActivate = true;
                }
            }

            if (_isEffectActivate)
            {
                ReadyInactivate();
            }

        }

        private float CalculateTime(float nowTime)
        {
            return nowTime += Time.deltaTime * _data.MovementSpeed;
        }

        private Vector3 GetEuler(Vector3 startPos, Vector3 arrivePos, float nowTime)
        {
            var nowPos = startPos;
            switch (_data.TypeBulletAction)
            {
                case BulletData.TYPE_BULLET_ACTION.Curve:
                    nowPos = Ditzel.Parabola.MathParabola.Parabola(startPos, arrivePos, 0.5f, nowTime);
                    arrivePos = Ditzel.Parabola.MathParabola.Parabola(startPos, arrivePos, 0.5f, nowTime + 0.1f);
                    break;
                case BulletData.TYPE_BULLET_ACTION.Drop:
                    nowPos = new Vector2(arrivePos.x, arrivePos.y + 10f);
                    break;
            }

            var direction = arrivePos - nowPos;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Vector3.forward * angle;
        }

        private Vector2 GetPosition(Vector3 startPos, Vector3 arrivePos, float nowTime)
        {
            switch (_data.TypeBulletAction)
            {
                case BulletData.TYPE_BULLET_ACTION.Curve:
                    return Ditzel.Parabola.MathParabola.Parabola(startPos, arrivePos, 0.5f, nowTime);
                case BulletData.TYPE_BULLET_ACTION.Move:
                    return Vector2.MoveTowards(startPos, arrivePos, nowTime);
                case BulletData.TYPE_BULLET_ACTION.Drop:
                    var pos = new Vector2(arrivePos.x, arrivePos.y + 10f);
                    return Vector2.MoveTowards(pos, arrivePos, nowTime);
                case BulletData.TYPE_BULLET_ACTION.Direct:
                    return arrivePos;
            }
            return Vector2.zero;
        }

        private Vector2 GetSlerp(Vector3 startPos, Vector3 arrivePos, float nowTime)
        {
            var center = (startPos + arrivePos) * 8f;
            center -= Vector3.up;

            var startRelPos = startPos - center;
            var arriveRelPos = arrivePos - center;
            var nowPos = Vector3.Slerp(startRelPos, arriveRelPos, nowTime);
            nowPos += center;
            return nowPos;
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




        #region ##### Listener #####

        private System.Action<BulletActor> _arrivedEvent;
        private System.Action<BulletActor> _inactiveEvent;

        public void SetOnArrivedListener(System.Action<BulletActor> act) => _arrivedEvent = act;
        public void SetOnInactiveListener(System.Action<BulletActor> act) => _inactiveEvent = act;


        private void OnArriveEvent()
        {
            _arrivedEvent?.Invoke(this);
        }
        #endregion


    }
}