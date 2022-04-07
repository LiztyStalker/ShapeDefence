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

            if(GUILayout.Button("본부 강화 "))
            {
                var cmd = new HQCommandPacket();
                cmd.SetData(true, false);
                _gameManager.OnCommandPacketEvent(cmd);
            }
            
            if(GUILayout.Button("본부 테크 "))
            {
                var cmd = new HQCommandPacket();
                cmd.SetData(false, true);
                _gameManager.OnCommandPacketEvent(cmd);
            }




            if(GUILayout.Button("포탑 강화 "))
            {
                var cmd = new TurretCommandPacket();
                cmd.SetData(0);
                cmd.SetData(true, false, false);
                _gameManager.OnCommandPacketEvent(cmd);

            }
            if (GUILayout.Button("포탑 테크 "))
            {
                var cmd = new TurretCommandPacket();
                cmd.SetData(0);
                cmd.SetData(false, true, false);
                _gameManager.OnCommandPacketEvent(cmd);
            }

            _orbitIndex = GUILayout.TextField(_orbitIndex);
            

            if (GUILayout.Button("포탑 확장 "))
            {
                var cmd = new TurretCommandPacket();
                cmd.SetData(0);
                cmd.SetOrbit(int.Parse(_orbitIndex));
                cmd.SetData(false, false, true);
                _gameManager.OnCommandPacketEvent(cmd);
            }

            //적등장
            if (GUILayout.Button("적 등장 "))
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