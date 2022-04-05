namespace SDefence.Manager
{
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        private GameSystem _system;
        private BattleManager _battleManager;

        private void Awake()
        {
            _battleManager = BattleManager.Create();
            _system = GameSystem.Create();

            _system.AddOnRefreshEntityPacketListener(_battleManager.OnEntityPacketEvent);
        }

        private void Start()
        {
            _system.RefreshAll();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _battleManager.RunProcess(deltaTime);
        }

        private void OnDestroy()
        {
            _system.RemoveOnRefreshEntityPacketListener(_battleManager.OnEntityPacketEvent);

            _battleManager = null;
            _system = null;
        }
    }
}