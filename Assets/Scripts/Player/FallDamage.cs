using UnityEngine;
using System.Collections;
using Singleton;
using System;

namespace Player
{
	public class FallDamage : MonoBehaviour
	{
		private float _lastPositionY;
		private float _fallDistance;
		private Transform _playerTranform;
		private CharacterController _playerController;

		// Use this for initialization
		void Start () 
		{
			_playerController = PrefabSingleton.Instance.Player.GetComponent<CharacterController>();
			_playerTranform = PrefabSingleton.Instance.Player.transform;
		}
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update () 
		{
			if(_lastPositionY > _playerTranform.transform.position.y)
			{
				_fallDistance += _lastPositionY - _playerTranform.transform.position.y;
			}
			
			_lastPositionY = _playerTranform.transform.position.y;
			
			if(_fallDistance >= 5 && _playerController.isGrounded)
			{
				var damage = (_fallDistance / CalculationSingleton.Instance.JumpDistance);
                var remaining = PlayerSingleton.Instance.PlayerHealth - Convert.ToInt32(damage) - 2;
				PlayerSingleton.Instance.PlayerHealth = remaining < 0
                    ? 0
                    : remaining;
				ApplyNormal();
			}
			
			if(_fallDistance <= 5 && _playerController.isGrounded)
			{
				ApplyNormal();
			}
		}

		/// <summary>
		/// Applies the normal.
		/// </summary>
		public void ApplyNormal()
		{
			_fallDistance = 0;
			_lastPositionY = 0;
		}
	}
}