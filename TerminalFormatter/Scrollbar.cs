using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// https://github.com/pacoito123/LC_StoreRotationConfig/blob/main/StoreRotationConfig/Patches/TerminalScrollMousePatch.cs

// i'm not sure if the original creator forgot to add their own license, that's the one in the repo:

// MIT License

// Copyright (c) 2023 Lethal Company Community

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace TerminalFormatter.Patches
{
    /// <summary>
    ///     Patch for 'PlayerControllerB.ScrollMouse_performed()' method; overrides vanilla scroll amount if the 'relativeScroll' setting is enabled.
    /// </summary>
    [HarmonyPatch(
        typeof(PlayerControllerB),
        "ScrollMouse_performed",
        typeof(InputAction.CallbackContext)
    )]
    internal class TerminalScrollMousePatch
    {
        // Text shown in the current terminal page, to determine if scroll amount needs to be updated.
        public static string CurrentText { get; internal set; } = "";

        // Amount to add/subtract from the terminal scrollbar, relative to the number of lines in the current terminal page.
        private static float scrollAmount = 1 / 3f;

        /// <summary>
        ///     Handles mouse scrolling while the terminal is open.
        /// </summary>
        /// <param name="scrollbar">Scrollbar instance used by the terminal.</param>
        /// <param name="scrollDirection">Direction to move the scrollbar, determined by the mouse wheel input.</param>
        private static void ScrollMouse_performed(Scrollbar scrollbar, float scrollDirection)
        {
            if(scrollbar == null){
                Plugin.debugLogger.LogWarning("scrollbar is null - too bad!");
                return;
            }
            
            // Check if text currently shown in the terminal has changed, to avoid calculating the scroll amount more than once.
            if (string.CompareOrdinal(Variables.Terminal.currentText, CurrentText) != 0)
            {
                // Cache text currently shown in the terminal.
                CurrentText = Variables.Terminal.currentText;

                // Calculate relative scroll amount using the number of lines in the current terminal page.
                int numLines = CurrentText.Count(c => c.Equals('\n')) + 1;
                scrollAmount = ConfigManager.LinesToScroll.Value / (float)numLines;
            }

            // Increment terminal scrollbar value by the relative scroll amount, in the direction given by the mouse wheel input.
            scrollbar.value += scrollDirection * scrollAmount;
        }

        /// <summary>
        ///     Inserts a call to 'TerminalScrollMousePatch.ScrollMouse_performed()', followed by a return instruction.
        /// </summary>
        ///     ... (GameNetcodeStuff.PlayerControllerB:1263)
        ///     float num = context.ReadValue();
        ///
        ///     -> StoreRotationConfig.Patches.TerminalScrollMousePatch.ScrollMouse_performed(this.terminalScrollVertical, num);
        ///     -> return;
        ///
        ///     this.terminalScrollVertical.value += num / 3f;
        /// <param name="instructions">Iterator with original IL instructions.</param>
        /// <returns>Iterator with modified IL instructions.</returns>
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            return new CodeMatcher(instructions)
                .MatchForward(
                    false,
                    new(OpCodes.Ldarg_0),
                    new(
                        OpCodes.Ldfld,
                        AccessTools.Field(
                            typeof(PlayerControllerB),
                            nameof(PlayerControllerB.terminalScrollVertical)
                        )
                    )
                )
                .Insert(
                    new(OpCodes.Ldarg_0),
                    new(
                        OpCodes.Ldfld,
                        AccessTools.Field(
                            typeof(PlayerControllerB),
                            nameof(PlayerControllerB.terminalScrollVertical)
                        )
                    ),
                    new(OpCodes.Ldloc_0),
                    new(
                        OpCodes.Call,
                        AccessTools.Method(
                            typeof(TerminalScrollMousePatch),
                            nameof(ScrollMouse_performed)
                        )
                    ),
                    new(OpCodes.Ret)
                )
                .InstructionEnumeration();
        }
    }
}
