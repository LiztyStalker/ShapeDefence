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

        public void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, 1920, 1080));
            
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            if(GUILayout.Button("���� ��ȭ "))
            {
                var cmd = new HQCommandPacket();
                cmd.SetData(true, false);
                _gameManager.OnCommandPacketEvent(cmd);
            }
            
            if(GUILayout.Button("���� ��ũ "))
            {
                var cmd = new HQCommandPacket();
                cmd.SetData(false, true);
                _gameManager.OnCommandPacketEvent(cmd);
            }




            if(GUILayout.Button("��ž ��ȭ "))
            {
                var cmd = new TurretCommandPacket();
                cmd.SetData(0);
                cmd.SetData(true, false, false);
                _gameManager.OnCommandPacketEvent(cmd);

            }
            if (GUILayout.Button("��ž ��ũ "))
            {
                var cmd = new TurretCommandPacket();
                cmd.SetData(0);
                cmd.SetData(false, true, false);
                _gameManager.OnCommandPacketEvent(cmd);
            }

            _orbitIndex = GUILayout.TextField(_orbitIndex);
            

            if (GUILayout.Button("��ž Ȯ�� "))
            {
                var cmd = new TurretCommandPacket();
                cmd.SetData(0);
                cmd.SetOrbit(int.Parse(_orbitIndex));
                cmd.SetData(false, false, true);
                _gameManager.OnCommandPacketEvent(cmd);
            }

            //������
            if (GUILayout.Button("�� ���� "))
            {
                var cmd = new EnemyCommandPacket();
                _gameManager.OnCommandPacketEvent(cmd);
            }


            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

    }
}
#endif