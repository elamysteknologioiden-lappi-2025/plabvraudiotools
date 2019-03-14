namespace VRTK.Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using VRTK.Controllables;
	using OVR;

	public class pLab_ControllableReactor : MonoBehaviour
	{
		public VRTK_BaseControllable controllable;
		public Text displayText;
		public string outputOnMax = "Maximum Reached";
		public string outputOnMin = "Minimum Reached";

		public pLab_ReactiveSoundEffect soundEffect;

		protected virtual void OnEnable()
		{
			controllable = (controllable == null ? GetComponent<VRTK_BaseControllable>() : controllable);
			controllable.ValueChanged += ValueChanged;
			controllable.MaxLimitReached += MaxLimitReached;
			controllable.MinLimitReached += MinLimitReached;

			soundEffect = (soundEffect == null ? GetComponent<pLab_ReactiveSoundEffect>() : soundEffect);
		}

		protected virtual void ValueChanged(object sender, ControllableEventArgs e)
		{
			if (displayText != null)
			{
				displayText.text = e.value.ToString("F1");
			}
		}

		protected virtual void MaxLimitReached(object sender, ControllableEventArgs e)
		{
			if (outputOnMax != "")
			{
				//Debug.Log(outputOnMax);

				if (displayText != null)
					displayText.text = outputOnMax;
			}

			if (soundEffect != null)
			{
				soundEffect.Play();
				Debug.Log("should play now");
			}

		}

		protected virtual void MinLimitReached(object sender, ControllableEventArgs e)
		{
			if (outputOnMin != "")
			{
				//Debug.Log(outputOnMin);

				if (displayText != null)
					displayText.text = outputOnMin;
			}
		}
	}
}