namespace UtilityManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using PoolSystem;
    using UnityEngine.Audio;

    [RequireComponent(typeof(AudioSource))]
    public class AudioActor : MonoBehaviour, IPoolElement
    {
        private AudioSource _audioSource;
        private AudioSource AudioSource
        {
            get
            {
                if (_audioSource == null)
                    _audioSource = GetComponent<AudioSource>();
                return _audioSource;
            }
        }

        private AudioManager.TYPE_AUDIO _typeAudio;

        public AudioManager.TYPE_AUDIO typeAudio => _typeAudio;

        public bool IsEqualKey(string key) => AudioSource.clip.name == key;

        public bool IsPlaying() => AudioSource.isPlaying;

        private void SetName()
        {
            gameObject.name = $"AudioActor_{_audioSource.clip.name}";
        }


        public void SetData(AudioClip clip, AudioManager.TYPE_AUDIO typeAudio, bool isLoop = false)
        {
            AudioSource.clip = clip;
            AudioSource.loop = isLoop;
            _typeAudio = typeAudio;
            SetName();
        }

        public void SetMixerGroup(AudioMixerGroup group) 
        {
            AudioSource.outputAudioMixerGroup = group;
        }


        public void Activate()
        {
            gameObject.SetActive(true);
            AudioSource.Play();            
        }

        public void Stop()
        {
            AudioSource.Stop();
            AudioSource.clip = null;
            _stoppedEvent?.Invoke(this);
        }

        public void Inactivate()
        {
            gameObject.SetActive(false);
            _inactiveEvent?.Invoke(this);
        }

        private void Update()
        {
            if (!AudioSource.isPlaying)
            {
                Stop();
            }
        }

        #region ##### Listener #####


        private System.Action<AudioActor> _stoppedEvent;
        private System.Action<AudioActor> _inactiveEvent;

        public void SetOnStoppedListener(System.Action<AudioActor> act) => _stoppedEvent = act;
        public void SetOnInactiveListener(System.Action<AudioActor> act) => _inactiveEvent = act;

        #endregion

    }
}