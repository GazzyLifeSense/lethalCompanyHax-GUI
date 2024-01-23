using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace Hax;

public class Loader : MonoBehaviour {
    static GameObject HaxGameObjects { get; } = new GameObject();
    static GameObject HaxModules { get; } = new GameObject();

    static void AddHaxModules<T>() where T : Component => Loader.HaxModules.AddComponent<T>();
    static void AddHaxGameObject<T>() where T : Component => Loader.HaxGameObjects.AddComponent<T>();

    static Assembly OnResolveAssembly(object _, ResolveEventArgs args) {
        Assembly assembly = Assembly.GetExecutingAssembly();

        try { 
            using Stream stream = assembly.GetManifestResourceStream(
            assembly.GetManifestResourceNames()
                    .First(name => name.EndsWith($"{new AssemblyName(args.Name).Name}.dll"))
        );
        
        using MemoryStream memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        return Assembly.Load(memoryStream.ToArray());
        }
        catch (Exception ex)
        {
            Logger.Write(ex.ToString());
        }
        return null;
    }

    public static void Load() {
        AppDomain.CurrentDomain.AssemblyResolve += Loader.OnResolveAssembly;
        
        Loader.LoadHarmonyPatches();
        Loader.LoadHaxGameObjects();
        Loader.LoadHaxModules();
        AppDomain.CurrentDomain.AssemblyResolve -= Loader.OnResolveAssembly;
    }

    static void LoadHarmonyPatches() {
        try {
            new Harmony("winstxnhdw.lc-hax").PatchAll();
        }

        catch (Exception exception) {
            Logger.Write(exception.ToString());
            throw exception;
        }
    }

    static void LoadHaxGameObjects() {
        DontDestroyOnLoad(Loader.HaxGameObjects);

        Loader.AddHaxGameObject<HaxObject>();
        Loader.AddHaxGameObject<InputListener>();
        Loader.AddHaxGameObject<GuiObject>();
    }

    static void LoadHaxModules() {
        DontDestroyOnLoad(Loader.HaxModules);

        Loader.AddHaxModules<SaneMod>();
        Loader.AddHaxModules<ChatMod>();
        Loader.AddHaxModules<StunMod>();
        Loader.AddHaxModules<StaminaMod>();
        Loader.AddHaxModules<ShovelMod>();
        Loader.AddHaxModules<WeightMod>();
        Loader.AddHaxModules<PhantomMod>();
        Loader.AddHaxModules<ClearVisionMod>();
        Loader.AddHaxModules<TriggerMod>();
        Loader.AddHaxModules<NameInWeightMod>();
        Loader.AddHaxModules<BuildAnywhereMod>();
        Loader.AddHaxModules<FollowAnotherPlayerMod>();
    }

    public static void Unload() {
        Destroy(Loader.HaxModules);
        Destroy(Loader.HaxGameObjects);
    }
}
