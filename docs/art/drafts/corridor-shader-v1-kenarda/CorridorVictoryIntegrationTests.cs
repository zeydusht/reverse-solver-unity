using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace ReverseSolver.Tests
{
    [Category("CorridorVictory")]
    public sealed class CorridorVictoryIntegrationTests
    {
        sealed class MemoryStore : IKeyValueStore
        {
            readonly Dictionary<string,string> data = new Dictionary<string,string>();
            public string Get(string key) => data.TryGetValue(key, out var value) ? value : null;
            public void Set(string key,string value) => data[key]=value;
            public void Delete(string key) => data.Remove(key);
            public void Save() { }
        }
        sealed class SmokeHost : IHost
        {
            public string Query(string key) => key=="debug" ? "1" : key=="set" ? "redesign" : null;
            public float PixelRatio => 1;
            public Vector4 SafeInsets => Vector4.zero;
            public void ReportBoot(string message) { }
            public IKeyValueStore Store { get; } = new MemoryStore();
            public ITelemetryTransport Transport => null;
            public string SupabaseUrl => null;
            public string AnonKey => null;
            public bool SendEnabled => false;
            public void AskName(Action<string> done) => done("CORRIDOR_TEST");
            public event Action<bool> VisibilityChanged { add { } remove { } }
        }
        static object Field(object target,string name) => target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(target);
        static void Invoke(object target,string name,params object[] args) => target.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(target,args);
        static void Capture(string name)
        {
            string directory="docs/art/corridor-victory/qa/unity";
            Directory.CreateDirectory(directory);
            var camera=Camera.main;
            var target=new RenderTexture(Mathf.Max(1,Screen.width),Mathf.Max(1,Screen.height),24);
            var previous=camera.targetTexture;var active=RenderTexture.active;
            camera.targetTexture=target;camera.Render();RenderTexture.active=target;
            var pixels=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);
            pixels.ReadPixels(new Rect(0,0,target.width,target.height),0,0);pixels.Apply();
            File.WriteAllBytes(directory+"/"+name+".png",pixels.EncodeToPNG());
            camera.targetTexture=previous;RenderTexture.active=active;
            UnityEngine.Object.Destroy(pixels);target.Release();UnityEngine.Object.Destroy(target);
        }

        [UnityTest,Timeout(90000)]
        public IEnumerator RealLevelWinReturnsToMenuPlaysOnceThenRestoresStaticBackground()
        {
            yield return new EnterPlayMode();
            var host = new SmokeHost();
            Host.Current=host;
            void InstallHost(Scene scene,LoadSceneMode mode) { Host.Current=host; }
            SceneManager.sceneLoaded+=InstallHost;
            EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/Game.unity",new LoadSceneParameters(LoadSceneMode.Single));
            yield return null;
            yield return null;
            SceneManager.sceneLoaded-=InstallHost;
            var root=UnityEngine.Object.FindFirstObjectByType<GameRoot>();
            Assert.NotNull(root);
            Capture("game-menu-initial"); // Batch tests have no live GameView; warm it as normal startup does.
            yield return null;
            Invoke(root,"StartLevel",0);
            var session=(GameSession)Field(root,"_session");
            foreach(var move in SmartPlayer.Play(session.Level).Moves) session.TryExit(move.Piece,move.Dir);
            Assert.AreEqual(Outcome.Win,session.Outcome,"A real session must emit the normal win event");
            var queue=(CorridorVictoryQueue)Field(root,"_corridorVictory");
            Assert.IsTrue(queue.Pending);
            yield return new WaitForSecondsRealtime(1.6f);
            Capture("game-win-card");
            yield return null; // ReadPixels/first shader warm-up must not age the next frame's start clock.
            Invoke(root,"ShowMenu");
            yield return null;
            var travel=UnityEngine.Object.FindFirstObjectByType<CorridorVictory>();
            Assert.NotNull(travel);
            Assert.IsTrue(travel.enabled);
            Assert.IsFalse(queue.Pending);
            var background=travel.GetComponent<SpriteRenderer>();
            Assert.AreEqual("Static",background.sprite.name);
            Assert.AreEqual("ReverseSolver/CorridorVictory",background.sharedMaterial.shader.name);
            Capture("game-menu-travel-start");
            yield return new WaitForSecondsRealtime(1.5f);
            Assert.That(background.sharedMaterial.GetFloat("_Progress"),Is.InRange(.45f,.65f));
            Capture("game-menu-travel-middle");
            yield return new WaitForSecondsRealtime(1.65f);
            Assert.IsFalse(travel.enabled);
            if(background.sharedMaterial.shader.name=="ReverseSolver/CorridorVictory")
                Assert.AreEqual(0,background.sharedMaterial.GetFloat("_Progress"),"The idle material must sample the static endpoint only");
            Capture("game-menu-static-end");
            Invoke(root,"ShowMenu");
            yield return null;
            Assert.IsFalse(queue.Playing(Time.unscaledTime),"Ordinary returns must not replay the transition");
            string directory="docs/art/corridor-victory/qa/unity";
            Directory.CreateDirectory(directory);
            File.WriteAllText(directory+"/win-menu-integration.json","{\"status\":\"passed\",\"realCoreWin\":true,\"actualGameRootMenuReturn\":true,\"playsOnce\":true,\"restoresStaticSprite\":true,\"storage\":\"memory only\",\"telemetry\":false,\"scope\":\"Unity Editor PlayMode; automated Core moves and normal ShowMenu action, not a device or manual pointer test\"}");
            yield return new ExitPlayMode();
        }

        [UnityTearDown]
        public IEnumerator LeavePlayMode()
        {
            if(EditorApplication.isPlaying) yield return new ExitPlayMode();
        }
    }
}
