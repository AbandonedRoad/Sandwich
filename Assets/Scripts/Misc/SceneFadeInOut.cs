using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Singleton;

namespace Misc
{
	public class SceneFadeInOut : MonoBehaviour
	{
		public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
		public bool IsBlack;

		private Image _image;
		private bool sceneStarting = true;      // Whether or not the scene is still fading in.

		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake ()
		{
			// Set the texture so that it is the the size of the screen and covers it.
			_image = this.GetComponent<Image>();
		}	

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update ()
		{
			// If the scene is starting...
			if(sceneStarting)
			{
				RestartScene();
			}
		}
		
		/// <summary>
		/// Fades to clear.
		/// </summary>
		void FadeToClear ()
		{
			// Lerp the colour of the texture between itself and transparent.
			_image.color = Color.Lerp(_image.color, Color.clear, fadeSpeed * Time.deltaTime);
		}
		
		/// <summary>
		/// Fades to black.
		/// </summary>
		void FadeToBlack ()
		{
			// Lerp the colour of the texture between itself and black.
			_image.color = Color.Lerp(_image.color, Color.black, fadeSpeed * Time.deltaTime);
		}
		
		/// <summary>
		/// Starts the scene.
		/// </summary>
		public void RestartScene ()
		{
			// Fade the texture to clear.
			FadeToClear();
			
			// If the texture is almost clear...
			if(_image.color.a <= 0.05f)
			{
				// ... set the colour to clear and disable the GUITexture.
				_image.color = Color.clear;
				_image.enabled = false;
				
				// The scene is no longer starting.
				sceneStarting = false;
				IsBlack = false;
			}
		}

		/// <summary>
		/// Ends the scene.
		/// </summary>
		public void PlayerDied()
		{
			// Make sure the texture is enabled.
			_image.enabled = true;
			
			// Start fading towards black.
			FadeToBlack();
			
			// If the screen is almost black...
			if(_image.color.a >= 0.95f)
			{
				IsBlack = true;
				PrefabSingleton.Instance.DeadHandler.SwitchYouAreDeadPanel();
			}
		}

		/// <summary>
		/// Ends the scene.
		/// </summary>
		public void PlayerWon()
		{
			// Make sure the texture is enabled.
			_image.enabled = true;
			
			// Start fading towards black.
			FadeToBlack();
			
			// If the screen is almost black...
			if(_image.color.a >= 0.95f)
			{
				IsBlack = true;
				PrefabSingleton.Instance.SceneStatistics.SwitchSceneEndPanel();
			}
		}
	}
}