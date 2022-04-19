#if UNITY_EDITOR
namespace SDefence.Test
{
    using SDefence.Manager;
    using Packet;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameIMGUI : MonoBehaviour
    {
        [SerializeField]
        private GameManager _gameManager;

        private string _orbitIndex;
        private string _levelWave;



        public void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, 1920, 1080));
            
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            GUILayout.Label("전투");

            //적등장
            if (GUILayout.Button("적 등장 "))
            {
                var cmd = new EnemyCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }

            //전투
            if (GUILayout.Button("전투 시작"))
            {
                var cmd = new PlayBattleCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("재도전"))
            {
                var cmd = new RetryCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("로비이동"))
            {
                var cmd = new ToLobbyCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("다음전투"))
            {
                var cmd = new NextLevelCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }


            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.Label("로비");

            if (GUILayout.Button($"로비 리스폰 {BattleManager.IS_LOBBY_GEN}"))
            {
                BattleManager.IS_LOBBY_GEN = !BattleManager.IS_LOBBY_GEN;
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("강화"))
            {
                var cmd = new UpgradeCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("확장"))
            {
                var cmd = new ExpandCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("테크열기"))
            {
                var cmd = new OpenTechCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("테크진행"))
            {
                var cmd = new UpTechCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("갱신"))
            {
                var cmd = new RefreshCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("분해열기"))
            {
                var cmd = new OpenDisassembleCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("분해"))
            {
                var cmd = new DisassembleCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }



            GUILayout.Label(_levelWave);

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

    }
}
#endif