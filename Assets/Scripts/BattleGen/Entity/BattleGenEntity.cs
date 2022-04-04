namespace SDefence.BattleGen.Entity
{
    using Data;
    using SDefence.Data;

    public class BattleGenEntity
    {
        private BattleGenLevelData _battleGenLevelData;
        private BattleGenWaveData? _battleGenWaveData;

        private float _nowTime = 0;
        private int _nowIndex = 0;

        public static BattleGenEntity Create() => new BattleGenEntity();


        public bool HasBattleGenLevelData() => _battleGenLevelData != null;
        public bool HasBattleGenWaveData() => _battleGenWaveData != null;

        public void SetData(BattleGenLevelData battleGenLevelData)
        {
            _battleGenLevelData = battleGenLevelData;
        }

        /// <summary>
        /// 프로세스 진행
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="levelWaveData"></param>
        public void RunProcess(float deltaTime)
        {
            _nowTime += deltaTime;
            //Wave에 도달
            if (HasBattleGenWaveData())
            {
                if (_battleGenWaveData.Value.HasWaveData(_nowIndex, _nowTime))
                {
                    OnBattleGenWaveEvent(_battleGenWaveData.Value.GetBattleGenWaveElement(_nowIndex));
                    _nowIndex++;
                }
            }
        }

        /// <summary>
        /// 레벨 웨이브 적용
        /// </summary>
        /// <param name="levelWaveData"></param>
        public void SetLevelWave(LevelWaveData levelWaveData)
        {
            _battleGenWaveData = _battleGenLevelData.GetBattleGenWaveData(levelWaveData.GetWave());
            _nowTime = 0;
            _nowIndex = 0;
        }


        #region ##### Listener #####

        private System.Action<BattleGenWaveElement?> _waveEvent;
        public void SetOnBattleGenWaveElementListener(System.Action<BattleGenWaveElement?> act) => _waveEvent = act;
        private void OnBattleGenWaveEvent(BattleGenWaveElement? data) => _waveEvent?.Invoke(data);

        #endregion
    }
}