#if UNITY_EDITOR
namespace SDefence.UI.Test
{
    using Packet;
    using Storage;
    using UnityEngine;
    

    public class UIGameTester : MonoBehaviour
    {
        private UIGame _uiGame;

        void Start()
        {
            _uiGame = FindObjectOfType<UIGame>(true);
            if (_uiGame == null)
            {
                var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@Game");
                _uiGame = obj.GetComponent<UIGame>();
            }

            _uiGame.Initialize();
            _uiGame.AddOnCommandPacketListener(OnCommandPacketEvent);
        }

        private void OnDestroy()
        {
            _uiGame.RemoveOnCommandPacketListener(OnCommandPacketEvent);
            _uiGame.CleanUp();
        }

        private void OnCommandPacketEvent(ICommandPacket pk)
        {
            Debug.Log($"Packet {pk.GetType().Name}");
        }
    }
}
#endif