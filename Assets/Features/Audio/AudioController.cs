using System.Collections.Generic;
using System.Linq;
using Features.Interaction;
using UnityEngine;

namespace Features.Audio
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioClip frying;
        [SerializeField] private AudioClip ding;
        [SerializeField] private AudioClip grunt;
        [SerializeField] private AudioClip slicing;
        [SerializeField] private AudioClip yeah;

        private static AudioController instance;

        private IFryingStation[] stations;
        private AudioSource oneShotSource;
        private readonly List<AudioSource> fryingSources = new();

        public static void PlayDing() => PlayOneShot(instance?.ding);
        public static void PlayGrunt() => PlayOneShot(instance?.grunt);
        public static void PlaySlicing() => PlayOneShot(instance?.slicing);
        public static void PlayYeah() => PlayOneShot(instance?.yeah);

        private void Awake()
        {
            instance = this;
            oneShotSource = GetComponent<AudioSource>();
            oneShotSource.playOnAwake = false;
            stations = FindObjectsByType<MonoBehaviour>().OfType<IFryingStation>().ToArray();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void Update()
        {
            int required = stations.Sum(station => station.FryingItemCount);

            while (fryingSources.Count < required)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = frying;
                source.loop = true;
                source.Play();
                fryingSources.Add(source);
            }

            while (fryingSources.Count > required)
            {
                Destroy(fryingSources[fryingSources.Count - 1]);
                fryingSources.RemoveAt(fryingSources.Count - 1);
            }
        }

        private static void PlayOneShot(AudioClip clip)
        {
            if (instance != null && clip != null)
            {
                instance.oneShotSource.PlayOneShot(clip);
            }
        }
    }
}
