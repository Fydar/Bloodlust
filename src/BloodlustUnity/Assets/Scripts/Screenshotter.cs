﻿#if UNITY_EDITOR || UNITY_STANDALONE
using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GlobalGameJam2022
{
	public sealed class Screenshotter : MonoBehaviour
	{
		public static string OutputPath
		{
			get
			{
				string parentFolder = new DirectoryInfo(Application.dataPath).Parent.ToString();
				return Path.Combine(parentFolder, "Screenshots");
			}
		}

		private static KeyCode CaptureKey => KeyCode.G;

		private static KeyCode OpenFolderKey => KeyCode.H;

		public static void CaptureScreenshot()
		{
			string folder = OutputPath;
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

			string now = DateTime.Now.ToString("MM-dd-yyy HH-mm-ss");
			string basicFile = Path.Combine(folder, "Screenshot " + now);

			string path = basicFile;
			int append = 0;
			while (File.Exists(path + ".png"))
			{
				path = $"{basicFile} {append}";
				append++;
			}

			ScreenCapture.CaptureScreenshot($"{path}.png");
		}

		public static void OpenScreenshotsFolder()
		{
			Application.OpenURL(OutputPath);
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			if (CaptureKey == KeyCode.None)
			{
				return;
			}

			var obj = new GameObject("Screenshotter")
			{
				hideFlags = HideFlags.HideInHierarchy
			};
			DontDestroyOnLoad(obj);
			obj.AddComponent<Screenshotter>();
		}

		private void Update()
		{
			if (Input.GetKeyDown(CaptureKey))
			{
				CaptureScreenshot();
			}
			if (Input.GetKeyDown(OpenFolderKey))
			{
				OpenScreenshotsFolder();
			}
		}

#if UNITY_EDITOR
		[InitializeOnLoad]
		private static class EditorScreenshotter
		{
			static EditorScreenshotter()
			{
				SceneView.duringSceneGui += SceneViewCallback;
			}

			private static void SceneViewCallback(SceneView view)
			{
				var currentEvent = Event.current;
				if (currentEvent.type == EventType.KeyDown)
				{
					if (currentEvent.keyCode == KeyCode.None)
					{
						return;
					}

					if (currentEvent.keyCode == OpenFolderKey)
					{
						OpenScreenshotsFolder();
					}
					else if (currentEvent.keyCode == CaptureKey)
					{
						var camera = EditorUtility.CreateGameObjectWithHideFlags("Temp_Cam", HideFlags.HideAndDontSave)
							.AddComponent<Camera>();

						var mainCamera = Camera.main;
						if (mainCamera != null)
						{
							camera.clearFlags = mainCamera.clearFlags;
							camera.backgroundColor = mainCamera.backgroundColor;
							camera.allowHDR = mainCamera.allowHDR;
						}

						camera.transform.SetPositionAndRotation(view.camera.transform.position, view.camera.transform.rotation);

						camera.fieldOfView = view.camera.fieldOfView;
						camera.allowMSAA = true;

						camera.depth = 1001;
						EditorApplication.ExecuteMenuItem("Window/General/Game");
						UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

						EditorApplication.delayCall += () =>
						{
							CaptureScreenshot();

							EditorApplication.delayCall += () =>
							{
								UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

								EditorApplication.delayCall += () =>
								{
									DestroyImmediate(camera.gameObject);

									EditorApplication.delayCall += UnityEditorInternal.InternalEditorUtility.RepaintAllViews;
								};
							};
						};
					}
				}
			}
		}
#endif
	}
}
#endif
