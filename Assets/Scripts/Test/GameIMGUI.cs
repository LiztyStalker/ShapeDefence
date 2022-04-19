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

            GUILayout.Label("����");

            //������
            if (GUILayout.Button("�� ���� "))
            {
                var cmd = new EnemyCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }

            //����
            if (GUILayout.Button("���� ����"))
            {
                var cmd = new PlayBattleCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("�絵��"))
            {
                var cmd = new RetryCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("�κ��̵�"))
            {
                var cmd = new ToLobbyCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("��������"))
            {
                var cmd = new NextLevelCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }


            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.Label("�κ�");

            if (GUILayout.Button($"�κ� ������ {BattleManager.IS_LOBBY_GEN}"))
            {
                BattleManager.IS_LOBBY_GEN = !BattleManager.IS_LOBBY_GEN;
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("��ȭ"))
            {
                var cmd = new UpgradeCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("Ȯ��"))
            {
                var cmd = new ExpandCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("��ũ����"))
            {
                var cmd = new OpenTechCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("��ũ����"))
            {
                var cmd = new UpTechCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("����"))
            {
                var cmd = new RefreshCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("���ؿ���"))
            {
                var cmd = new OpenDisassembleCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }
            if (GUILayout.Button("����"))
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