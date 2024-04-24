using HarmonyLib;
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
        [HarmonyPatch("Sandbox.Game.Gui.MyTerminalInfoController", "UpdateBeforeDraw")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();
            var list = instructions.ToList();
            var method = AccessTools.PropertyGetter(typeof(MyEntity), nameof(MyEntity.DisplayName));

            var index = list.FindIndex(x => x.Calls(method)) - 2;

            list[index + 4].labels.Add(label);

            list.Insert(index, new CodeInstruction(OpCodes.Brtrue, label));
            list.Insert(index, CodeInstruction.Call(typeof(MyGuiControlTextbox), "get_HasFocus"));
            list.Insert(index, new CodeInstruction(OpCodes.Ldloc_1));

            return list;
        }

    }
}
