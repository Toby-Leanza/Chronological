using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace PauseManagement.Editor
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class PauseManagerMenu
	{
		private static string PAUSE_MANAGER_VERSION = string.Empty;

		/// <summary>
		/// 
		/// </summary>
		[MenuItem("Tools / Gabriel Pereira / Pause Manager / About", false, 100)]
		static void About()
		{
			SetupVersion();

			EditorUtility.DisplayDialog("Pause Manager", string.Format("Made by Gabriel Pereira{0}{0}Version: {1}", Environment.NewLine, PAUSE_MANAGER_VERSION), "OK");
		}

		/// <summary>
		/// 
		/// </summary>
		private static void SetupVersion()
		{
			PAUSE_MANAGER_VERSION = string.Empty;

			var type = Type.GetType("PauseManagement.Core.PauseManager, PauseManager, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", false);

			if (type != null)
			{
				var field = type.GetField("Version", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);

				if (field != null)
					PAUSE_MANAGER_VERSION = field.GetValue(null) as string;
			}
		}
	}
}