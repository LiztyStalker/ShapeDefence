namespace SDefence.Manager
{
    using Actor;
    using Attack;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class TargetSystem
    {
        private Dictionary<IAttackable, IActor> _targetDic;
       

        public void Initialize()
        {
            _targetDic = new Dictionary<IAttackable, IActor>();
        }

        public void CleanUp()
        {
            Clear();
            _targetDic = null;
        }

        public void Clear()
        {
            _targetDic.Clear();
        }

        public IActor SearchTarget(IAttackable attackable, IActor[] arr)
        {
            IActor tActor = null;
            if (_targetDic.ContainsKey(attackable))
            {
                //이미 타겟이 있으면 가져오기
                tActor = _targetDic[attackable];
            }
            else
            {
                //타겟이 없으면 가장 가까운 타겟 가져오기 (타겟 우선순위에 따라서 달라질 수 있음)
                IActor targetActor = arr[0];
                for(int i = 1; i < arr.Length; i++)
                {
                    if (Vector2.Distance(attackable.AttackPos, targetActor.NowPosition) > Vector2.Distance(attackable.AttackPos, arr[i].NowPosition))
                    {
                        targetActor = arr[i];
                    }
                }
                _targetDic.Add(attackable, targetActor);
                tActor = targetActor;
            }

            if (tActor != null && attackable.AttackUsableData.Range < Vector2.Distance(attackable.AttackPos, tActor.NowPosition))
            {
                tActor = null;
            }
            return tActor;
        }

        public void RemoveTarget(IActor targetActor)
        {
            if (_targetDic.ContainsValue(targetActor))
            {
                var arr = _targetDic.Where(val => val.Value == targetActor).ToArray();
                for(int i = 0; i < arr.Length; i++)
                {
                    RemoveAttackable(arr[i].Key);
                }
            }
        }

        public void RemoveAttackable(IAttackable attackable)
        {
            if (_targetDic.ContainsKey(attackable))
                _targetDic.Remove(attackable);
        }
    }
}