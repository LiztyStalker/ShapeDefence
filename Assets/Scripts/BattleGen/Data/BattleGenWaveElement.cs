namespace SDefence.BattleGen.Data
{
    using UnityEngine;

    [System.Serializable]
    public class BattleGenWaveElement
    {
        [SerializeField]
        private string _enemyDataKey;
        [SerializeField]
        private int _appearCount;
        [SerializeField]
        private int _weight;
        [SerializeField]
        private float _waveAppearDelay;
        //private TYPE_BATTLEGEN_ACTION TypeEnemyAppearAction;
        //private TYPE_BATTLEGEN_POSITION TypeEnemyAppearPosition;
        //[SerializeField]
        //private float _enemyAppearDelay;



        public string EnemyDataKey => _enemyDataKey;
        public int AppearCount => _appearCount;
        public int Weight => _weight;
        public float WaveAppearDelay => _waveAppearDelay;
        //public TYPE_BATTLEGEN_ACTION TypeEnemyAppearAction;
        //public TYPE_BATTLEGEN_POSITION TypeEnemyAppearPosition;
        //public float EnemyAppearDelay => _enemyAppearDelay;



#if UNITY_EDITOR

        public static BattleGenWaveElement Create_Test() => new BattleGenWaveElement();

        private BattleGenWaveElement()
        {
            _enemyDataKey = "Test";
            _appearCount = 1;
            _weight = 1;
            _waveAppearDelay = 0f;
            //_enemyAppearDelay = 0f;
        }
#endif
    }
    
}