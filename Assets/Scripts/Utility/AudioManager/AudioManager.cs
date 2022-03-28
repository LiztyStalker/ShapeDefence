namespace UtilityManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;
    using Storage;
    using PoolSystem;

    public class AudioManager
    {
        public enum TYPE_AUDIO { BGM, SFX, Env, UI }

        public enum TYPE_AUDIO_CHANGE_MODE { None, Linear_Fade, Together_Fade}

        private static GameObject _gameObject;

        private static GameObject gameObject
        {
            get
            {
                if (_gameObject == null)
                {
                    _gameObject = new GameObject();
                    _gameObject.transform.position = Vector3.zero;
                    _gameObject.name = "Manager@Audio";
                    Object.DontDestroyOnLoad(_gameObject);
                }
                return _gameObject;
            }
        }

        private static AudioManager _current;

        public static AudioManager Current
        {
            get
            {
                if(_current == null)
                {
                    _current = new AudioManager();
                }
                return _current;
            }
        }


        private PoolSystem<AudioActor> _pool;

        private Dictionary<TYPE_AUDIO, List<AudioActor>> _dic;

        //Channel
        private List<AudioMixer> _mixerList = new List<AudioMixer>();

        private AudioClip GetClip(string clipKey) => DataStorage.Instance.GetDataOrNull<AudioClip>(clipKey, null, null);


        private AudioManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pool = new PoolSystem<AudioActor>();
            _pool.Initialize(Create);
            _dic = new Dictionary<TYPE_AUDIO, List<AudioActor>>();
        }

        public void CleanUp()
        {
            _pool.CleanUp();
            _dic.Clear();
        }

        /// <summary>
        /// AudioData�� GameObject Instanceȭ �մϴ�
        /// EditMode : ������� �ʽ��ϴ�
        /// Play : GameObject�� �����˴ϴ�
        /// </summary>
        /// <param name="effectData"></param>
        /// <param name="position"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public AudioActor Activate(string clipKey, TYPE_AUDIO typeAudio, bool isLoop = false, System.Action<AudioActor> inactivateCallback = null)
        {
            if (Application.isPlaying)
            {
                if (!string.IsNullOrEmpty(clipKey))
                {
                    var clip = GetClip(clipKey);
                    return Activate(clip, typeAudio, isLoop, inactivateCallback);
                }
            }
            return null;
        }

        /// <summary>
        /// AudioData�� GameObject Instanceȭ �մϴ�
        /// EditMode : ������� �ʽ��ϴ�
        /// Play : GameObject�� �����˴ϴ�
        /// </summary>
        /// <param name="effectData"></param>
        /// <param name="position"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public AudioActor Activate(AudioClip clip, TYPE_AUDIO typeAudio, bool isLoop = false, System.Action<AudioActor> inactivateCallback = null)
        {
            if (Application.isPlaying)
            {
                return Activate(clip, typeAudio, TYPE_AUDIO_CHANGE_MODE.None, 0f, isLoop, inactivateCallback);
            }
            return null;
        }



        [System.Obsolete("������")]
        /// <summary>
        /// AudioData�� GameObject Instanceȭ �մϴ�
        /// EditMode : ������� �ʽ��ϴ�
        /// Play : GameObject�� �����˴ϴ�
        /// </summary>
        /// <param name="effectData"></param>
        /// <param name="position"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public AudioActor Activate(AudioClip clip, TYPE_AUDIO typeAudio, TYPE_AUDIO_CHANGE_MODE typeAudioChangeMode, float changeTime, bool isLoop = false, System.Action<AudioActor> inactiveCallback = null)
        {
            if (Application.isPlaying)
            {
                if (clip != null)
                {
                    var actor = _pool.GiveElement();// GetActor(typeAudio);
                    actor.name = $"AudioActor_{typeAudio}_{clip.name}";
                    actor.SetData(clip, typeAudio, isLoop);
                    actor.SetOnStoppedListener(OnStoppedEvent);
                    actor.SetOnInactiveListener(actor =>
                    {
                        inactiveCallback?.Invoke(actor);
                        RetrieveActor(actor);
                    });
                    actor.Activate();

                    if (!_dic.ContainsKey(typeAudio))
                        _dic.Add(typeAudio, new List<AudioActor>());

                    _dic[typeAudio].Add(actor);


                    //���� - ���̱�
                    //���� - Ű���
                    return actor;
                }
            }
            return null;
        }



        //������
        #region ##### Fader #####

        private class AudioFader
        {
            public AudioActor beforeActor;
            public AudioActor afterActor;
            public TYPE_AUDIO_CHANGE_MODE typeAudioChangeMode;
            public float changeTime;

            public void Process()
            {
                //before �Ҹ� ����
                //after �Ҹ� Ű��
                switch (typeAudioChangeMode)
                {
                    case TYPE_AUDIO_CHANGE_MODE.Linear_Fade:
                        break;
                    case TYPE_AUDIO_CHANGE_MODE.Together_Fade:
                        break;
                }
            }

            public bool IsDone()
            {
                //beforeActor ����
                //afterActor ����
                return false;
            }
        }


        private Queue<AudioFader> _faderQueue = new Queue<AudioFader>();

        #endregion





        /// <summary>
        /// AudioActor�� �����մϴ�
        /// </summary>
        /// <param name="effectData"></param>
        public void Inactivate(AudioActor actor)
        {
            actor.Inactivate();
        }

        private AudioActor Create()
        {
            var gameObejct = new GameObject();
            var actor = gameObejct.AddComponent<AudioActor>();
            actor.transform.SetParent(gameObject.transform);
            return actor;
        }

        private void RetrieveActor(AudioActor actor)
        {
            if (_dic.ContainsKey(actor.typeAudio))
            {
                var list = _dic[actor.typeAudio];
                list.Remove(actor);
                _pool.RetrieveElement(actor);
            }
        }

        //Fader ���� ������
        private void OnStoppedEvent(AudioActor actor)
        {
            Inactivate(actor);
        }
    }
}