using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Manages audio playback for ball collisions, pocketing events,
    /// and UI feedback. Uses AudioSource pooling for rapid collisions.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Clips")]
        [SerializeField] private AudioClip ballHitBallClip;
        [SerializeField] private AudioClip ballHitCushionClip;
        [SerializeField] private AudioClip ballPocketedClip;
        [SerializeField] private AudioClip cueHitBallClip;
        [SerializeField] private AudioClip uiClickClip;
        [SerializeField] private AudioClip gameWonClip;

        [Header("Settings")]
        [SerializeField] private int audioSourcePoolSize = 8;
        [SerializeField] private float minTimeBetweenSounds = 0.05f;

        private AudioSource[] _pool;
        private int _poolIndex;
        private float _lastPlayTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Create pooled audio sources
            _pool = new AudioSource[audioSourcePoolSize];
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                var go = new GameObject($"AudioSource_{i}");
                go.transform.SetParent(transform);
                _pool[i] = go.AddComponent<AudioSource>();
                _pool[i].playOnAwake = false;
            }
        }

        public void PlayBallHitBall(float velocity)
        {
            float vol = Mathf.Clamp01(velocity / 10f);
            Play(ballHitBallClip, vol);
        }

        public void PlayBallHitCushion(float velocity)
        {
            float vol = Mathf.Clamp01(velocity / 8f);
            Play(ballHitCushionClip, vol);
        }

        public void PlayBallPocketed()     => Play(ballPocketedClip, 1f);
        public void PlayCueHitBall()       => Play(cueHitBallClip,   1f);
        public void PlayUIClick()          => Play(uiClickClip,       1f);
        public void PlayGameWon()          => Play(gameWonClip,       1f);

        private void Play(AudioClip clip, float volume)
        {
            if (clip == null) return;
            if (Time.time - _lastPlayTime < minTimeBetweenSounds) return;

            AudioSource src = GetNextSource();
            src.clip   = clip;
            src.volume = volume;
            src.Play();
            _lastPlayTime = Time.time;
        }

        private AudioSource GetNextSource()
        {
            AudioSource src = _pool[_poolIndex];
            _poolIndex = (_poolIndex + 1) % _pool.Length;
            return src;
        }
    }
}
