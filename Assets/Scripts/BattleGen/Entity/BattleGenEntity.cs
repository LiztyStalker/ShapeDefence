namespace SDefence.BattleGen.Entity
{
    using Data;
    using SDefence.Data;
    using System.Collections.Generic;

    public class BattleGenEntity
    {

        private const float BATTLE_GEN_LOBBY_DELAY = .5f;

        private BattleGenLevelData _battleGenLevelData;
        private BattleGenWaveData _battleGenWaveData;
        private List<BattleGenWaveElement> _list;

        private float _nowTime = 0;
        private int _nowIndex = 0;

        public static BattleGenEntity Create() => new BattleGenEntity();

        private BattleGenEntity()
        {
            _list = new List<BattleGenWaveElement>();
        }

        public bool HasBattleGenLevelData() => _battleGenLevelData != null;
        public bool HasBattleGenWaveData() => _battleGenWaveData != null;

        public void SetData(BattleGenLevelData battleGenLevelData)
        {
            _battleGenLevelData = battleGenLevelData;
            _battleGenWaveData = null;
        }

        /// <summary>
        /// 프로세스 진행
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="levelWaveData"></param>
        public void RunProcessBattle(float deltaTime)
        {
            _nowTime += deltaTime;
            //Wave에 도달
            if (HasBattleGenWaveData())
            {
                if (_battleGenWaveData.HasWaveData(_nowIndex, _nowTime))
                {
                    _list.Add(_battleGenWaveData.GetBattleGenWaveElement(_nowIndex));
                    _nowIndex++;
                }
            }

            for(int i = _list.Count - 1; i >= 0; i--)
            {
                var waveElement = _list[i];
                for (int j = 0; j < waveElement.AppearCount; j++) 
                {
                    OnAppearEnemyEvent(waveElement.EnemyDataKey);
                }
                _list.Remove(waveElement);
            }
        }

        public void RunProcessLobby(float deltaTime)
        {
            if(_battleGenWaveData == null)
                _battleGenWaveData = _battleGenLevelData.GetLobbyBattleGenWaveData();

            _nowTime += deltaTime;

            if (_battleGenWaveData.HasWaveData(_nowIndex, _nowTime))
            {
                _list.Add(_battleGenWaveData.GetBattleGenWaveElement(_nowIndex));
                _nowIndex++;
                //Delay
                _nowTime -= BATTLE_GEN_LOBBY_DELAY;
            }
            else
            {
                _nowIndex = 0;
                _nowTime = -BATTLE_GEN_LOBBY_DELAY;
            }

            for (int i = _list.Count - 1; i >= 0; i--)
            {
                var waveElement = _list[i];
                for (int j = 0; j < waveElement.AppearCount; j++)
                {
                    OnAppearEnemyEvent(waveElement.EnemyDataKey);
                }
                _list.Remove(waveElement);
            }
        }

        /// <summary>
        /// 레벨 웨이브 적용
        /// </summary>
        /// <param name="levelWaveData"></param>
        public void SetLevelWave(LevelWaveData levelWaveData)
        {
            _battleGenWaveData = _battleGenLevelData.GetBattleGenWaveData(levelWaveData.GetWave());
            _battleGenWaveData = null;
            _nowTime = 0;
            _nowIndex = 0;
        }

        #region ##### Listener #####

        private System.Action<string> _appearEvent;
        public void SetOnAppearEnemyListener(System.Action<string> act) => _appearEvent = act;
        private void OnAppearEnemyEvent(string data) => _appearEvent?.Invoke(data);
        #endregion
    }
}