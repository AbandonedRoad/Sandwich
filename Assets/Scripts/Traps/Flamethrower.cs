using System;
using UnityEngine;
using System.Linq;

namespace Traps
{
	public class Flamethrower : MonoBehaviour
	{
		public bool StartFlame;

		private Vector3 _endPoint;
		private Vector3 _startPoint;
		private ParticleSystem _wildFire;
		private AudioSource _audio;

		/// <summary>
		/// Initializes a new instance of the <see cref="Traps.Flamethrower"/> class.
		/// </summary>
		void Start ()
		{
			_audio = this.GetComponent<AudioSource>();
			_wildFire = this.GetComponentsInChildren<ParticleSystem>().First(ps => ps.name == "WildFire");
			_startPoint = _wildFire.transform.position;
			_endPoint = this.GetComponentsInChildren<Transform>().First(go => go.name == "FlameEndPoint").gameObject.transform.position;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update()
		{
			if (StartFlame)
			{
				_audio.Play();
				_wildFire.transform.position = _wildFire.transform.position == _startPoint
					? _endPoint
					: _startPoint;

				StartFlame = false;
			}
		}
	}
}