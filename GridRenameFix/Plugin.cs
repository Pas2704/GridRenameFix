using HarmonyLib;
using NLog;
using System.Reflection;
using VRage.Plugins;

namespace GridRenameFix
{
    public class Plugin : IPlugin
    {
        internal static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public void Init(object gameInstance)
        {
            Log.Debug($"GridRenameFix: Patching");
            new Harmony("GridRenameFix").PatchAll(Assembly.GetExecutingAssembly());
            Log.Info($"GridRenameFix: Patches applied");
        }

        public void Dispose()
        {
        }

        public void Update()
        {
        }
    }
}
