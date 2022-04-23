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

        private Dictionary<TYPE_AUDIO, List<AudioActor>> _actorDic;

        //Channel
        private AudioMixer _mixer;

        private Dictionary<TYPE_AUDIO, AudioMixerGroup> _mixerDic;

        private AudioClip GetClip(string clipKey) => DataStorage.Instance.GetDataOrNull<AudioClip>(clipKey);


        private AudioManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pool = new PoolSystem<AudioActor>();
            _pool.Initialize(Create);
            _actorDic = new Dictionary<TYPE_AUDIO, List<AudioActor>>();
            _mixerDic = new Dictionary<TYPE_AUDIO, AudioMixerGroup>();

            _mixer = DataStorage.Instance.GetDataOrNull<AudioMixer>("AudioMixer");

            var groups = _mixer.FindMatchingGroups(string.Empty);
            for(int i = 0; i < groups.Length; i++)
            {
                if (System.Enum.TryParse(groups[i].name, out TYPE_AUDIO type))
                {
                    _mixerDic.Add(type, groups[i]);
                }
            }
        }

        public void CleanUp()
        {
            _pool.CleanUp();
            _actorDic.Clear();
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
                    actor.SetMixerGroup(_mixerDic[typeAudio]);
                    actor.Activate();

                    if (!_actorDic.ContainsKey(typeAudio))
                        _actorDic.Add(typeAudio, new List<AudioActor>());

                    _actorDic[typeAudio].Add(actor);


                    //���� - ���̱�
                    //���� - Ű���
                    return actor;
                }
            }
            return null;
        }


        public void SetMute(TYPE_AUDIO typeAudio, bool isMute)
        {
            _mixer.SetFloat(typeAudio.ToString(), (isMute) ? -80f : 0f);
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
            if (_actorDic.ContainsKey(actor.typeAudio))
            {
                var list = _actorDic[actor.typeAudio];
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