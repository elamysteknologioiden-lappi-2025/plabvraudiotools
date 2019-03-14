using UnityEngine;
using System.Collections;

namespace OVR
{
	public class pLab_ReactiveSoundEffect : MonoBehaviour
	{
		public SoundFXRef soundEffect;

		public void Play()
		{
			// Debug.Log("playing sound " + soundEffect.name);
			soundEffect.PlaySoundAt(transform.position, 0, 1f, 1f);
		}
	}
} // namespace OVR
