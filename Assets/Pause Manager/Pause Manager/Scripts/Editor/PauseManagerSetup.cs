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
	public class PauseManagerSetup
	{
		#region Scripting Define Symbols

		/// <summary>
		/// Scripting Define Symbol for Rewired's Input System
		/// </summary>
		private const string PAUSE_MANAGER_REWIRED = "PAUSE_MANAGER_REWIRED";

		#endregion

		#region Types

		/// <summary>
		/// Rewired type
		/// </summary>
		private const string REWIRED_TYPE_NAME = "Rewired.ReInput, Rewired_Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

		#endregion

		private static List<string> Symbols = new List<string>();

		/// <summary>
		/// Eveytime Unity loads, the static constructor is executed
		/// </summary>
#if UNITY_EDITOR
		static PauseManagerSetup()
		{
			AcquireDefineSymbols();
			
			HandleRewired();

			SaveDefineSymbols();
		}
#endif

		#region Menu

		/// <summary>
		/// 
		/// </summary>
		[MenuItem("Tools / Gabriel Pereira / Pause Manager / Scripting Define Symbols / Setup", false, 1)]
		static void Setup()
		{
			AcquireDefineSymbols();
			RemoveDefines(PAUSE_MANAGER_REWIRED);
			SaveDefineSymbols();
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		private static void AcquireDefineSymbols()
		{
			var group = EditorUserBuildSettings.selectedBuildTargetGroup;

			var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

			Symbols = definesString.Split(';').ToList();
		}

		private static void HandleRewired()
		{
			RemoveDefines(PAUSE_MANAGER_REWIRED);
			HandleTypePresency(REWIRED_TYPE_NAME, PAUSE_MANAGER_REWIRED);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="defines"></param>
		private static void HandleTypePresency(string typeName, params string[] defines)
		{
			if (IsTypePresent(typeName))
				AddDefines(defines);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private static bool IsTypePresent(string typeName)
		{
			return Type.GetType(typeName, false) != null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="defines"></param>
		private static void RemoveDefines(params string[] defines)
		{
			if (defines == null) return;

			foreach (var define in defines)
				Symbols.Remove(define);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="defines"></param>
		private static void AddDefines(params string[] defines)
		{
			if (defines == null) return;

			foreach (var define in defines)
			{
				if (IsDefineAdded(define)) return;

				Symbols.Add(define);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="define"></param>
		/// <returns></returns>
		private static bool IsDefineAdded(string define)
		{
			return Symbols.Contains(define);
		}

		/// <summary>
		/// 
		/// </summary>
		private static void SaveDefineSymbols()
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
					 EditorUserBuildSettings.selectedBuildTargetGroup,
					 string.Join(";", Symbols.ToArray())
			);
		}
	}
}