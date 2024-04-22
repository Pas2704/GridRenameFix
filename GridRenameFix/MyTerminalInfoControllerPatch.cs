using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using VRage.Game.Entity;

namespace GridRenameFix
{
    [HarmonyPatch]
    internal static class MyTerminalInfoControllerPatch
    {
        private static MyCubeGrid _lastGrid;


        static MyTerminalInfoControllerPatch()
        {
            MySession.OnUnloading += () => _lastGrid = null;
        }

        [HarmonyPatch("Sandbox.Game.Gui.MyTerminalInfoController", "UpdateBeforeDraw")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();
            var list = instructions.ToList();
            var method = AccessTools.PropertyGetter(typeof(MyEntity), nameof(MyEntity.DisplayName));

            var index = list.FindIndex(x => x.Calls(method)) - 2;

            list[index + 4].labels.Add(label);

            list.Insert(index, new CodeInstruction(OpCodes.Brtrue, label));
            list.Insert(index, CodeInstruction.Call(typeof(MyTerminalInfoControllerPatch), nameof(ShouldIgnoreChange)));
            list.Insert(index, new CodeInstruction(OpCodes.Callvirt, 
                AccessTools.PropertyGetter(typeof(MyGuiControlTextbox), nameof(MyGuiControlTextbox.Text))));
            list.Insert(index, new CodeInstruction(OpCodes.Ldloc_1));
            list.Insert(index, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field("Sandbox.Game.Gui.MyTerminalInfoController:m_grid")));

            return list;
        }

        private static bool ShouldIgnoreChange(MyCubeGrid grid, string text)
        {
            var ignore = _lastGrid != null && grid == _lastGrid && grid.DisplayName != text;
            _lastGrid = grid;
            return ignore;
        }
        [HarmonyPatch("Sandbox.Game.Gui.MyTerminalInfoController", "MarkControlsDirty")]
        [HarmonyPostfix]
        private static void Postfix() => _lastGrid = null;
    }
}
