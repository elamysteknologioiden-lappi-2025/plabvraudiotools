using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pLab_SeamlessLoopSound : MonoBehaviour
{
	[Tooltip("Aseta luupattava äänitiedosto tähän.")]
	[SerializeField] private AudioClip loopClip;
	[Tooltip("Ristiinhäivytyksen kesto luupin alkaessa alusta. Oltava vähemmän kuin ääniklipin kesto.")]
	[SerializeField] private float crossFadeTime = 1;

	private AudioSource[] audioSources;
	private int toggle = 0;

	private double duration;
	private double time;


	private void Awake()
	{
		// Alustetaan AudioSourcet
		audioSources = GetComponentsInChildren<AudioSource>();

		for (int i = 0; i < audioSources.Length; i++)
		{
			// Pakotetaan AudioSourcet koodilla ohjatun saumattoman luuppauksen asetuksille
			audioSources[i].clip = loopClip;
			audioSources[i].loop = false;
			audioSources[i].playOnAwake = false;
			audioSources[i].mute = false;
			audioSources[i].volume = 0;
			audioSources[i].pitch = 1;

			// Jos toistettava klippi on ambisonic-muotoa, AudioSourcea ei spatialisoida, jotta ääni toistuu oikein
			if (loopClip.ambisonic == true)
				audioSources[i].spatialize = false;
		}
	}


	private void Start()
	{
		// Asetetaan toistolle tärkeät aika-arvot
		// Haetaan äänijärjestelmän tämän hetkinen aika
		time = AudioSettings.dspTime;
		// Haetaan toistettavan ääniklipin tarkka kesto
		duration = loopClip.samples / loopClip.frequency;
		// Vähennetään kestosta haluttu ristiinhäivytysaika. Ääniklippi toistetaan uudelleen nyt tämän ajan välein.
		duration -= crossFadeTime;

		// Aloitetaan toistaminen ensimmäisellä AudioSourcella heti
		audioSources[toggle].Play();
		// Häivytetään sisään ensimmäisen AudioSourcen äänenvoimakkuus
		StartCoroutine(FadeIn(audioSources[toggle], crossFadeTime));

		// Lasketaan, milloin toisen AudioSourcen pitää alkaa toistaa klippiä
		time += duration;
		// Annetaan ajastettu toistokäsky toiselle AudioSourcelle
		audioSources[1 - toggle].PlayScheduled(time);

		// Näiden jälkeen toistologiikka pyörii Update() -funktiossa
	}


	private void Update()
	{

		// Annetaan toisto- ja ristiihäivytyskäskyt aina kun äänijärjestelmän aika on edennyt edellä määritellyn ajan (klipin kesto - ristiinhäivytysaika).
		if (AudioSettings.dspTime >= time)
		{
			// Asetetaan seuraava toiston aloitusaika, joka ajastetaan nyt soivalle AudioSourcelle sen FadeOut-funktiossa.
			time = time + duration;

			// Häivytetään sisään seuraavaksi toistamaan alkavan AudioSourcen ääni
			StartCoroutine(FadeIn(audioSources[1 - toggle], crossFadeTime));
			// Häivytetään ulos nyt soivan AudioSourcen ääni.
			StartCoroutine(FadeOut(audioSources[toggle], crossFadeTime));

			// Vaihdetaan AudioSourcejen käsittelyjärjestys seuraavaa time -ajan ylittämishetkeä varten
			toggle = 1 - toggle;
		}
	}

	// Äänenvoimakkuuden uloshäivytys
	private IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
	{
		while (audioSource.volume > 0)
		{
			audioSource.volume -= Time.deltaTime / FadeTime;
			yield return null;
		}

		// Nyt toistava AudioSource ajastaa uloshäivytyksen lopuksi itsensä soimaan seuraavan kerran kun "time" ylitetään
		audioSource.PlayScheduled(time);
	}

	// Äänenvoimakkuuden sisäänhäivytys
	private IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
	{
		while (audioSource.volume < 1)
		{
			audioSource.volume += Time.deltaTime / FadeTime;
			yield return null;
		}
	}
}
