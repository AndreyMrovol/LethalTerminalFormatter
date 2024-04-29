using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ScrollRect;

// MIT License

// Copyright (c) 2024 Major-Scott

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

// https://github.com/Major-Scott/TerminalPlus/blob/master/TerminalPlus/ScrollbarGarbage.cs

namespace TerminalFormatter
{
    class ScrollbarFix
    {
        public static int customSens = 136;
        public static bool terminalKeyPressed = false;
        public static float currentScrollPosition = 1f;
        public static int currentStep = 0;
        public static float stepValue = 0f;
        public static int loopCount = 0;
        public static List<TerminalNode> testNodes = new List<TerminalNode>();

        public static float timeSinceSubmit = 0f;

        [HarmonyPatch(typeof(Terminal))]
        [HarmonyPatch("OnSubmit")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void NodeConsoleInfo(Terminal __instance)
        {
            timeSinceSubmit = 0f;
            __instance.scrollBarVertical.value = 1f;

            //if (__instance.currentNode != null)
            //{
            //    mls.LogWarning("CURRENT NODE: " + __instance.currentNode);
            //    mls.LogWarning("CURRENT NAME: " + __instance.currentNode.name);
            //    mls.LogMessage("NODE CNAME: " + __instance.currentNode.creatureName);
            //    mls.LogMessage("  NODE CID: " + __instance.currentNode.creatureFileID);
            //    if (__instance.currentNode.displayVideo != null) mls.LogMessage("NODE VIDEO: " + __instance.currentNode.displayVideo.name);
            //}
        }

        //----------------------------------------------------------------------------

        [HarmonyPatch(typeof(Scrollbar), "Set")]
        [HarmonyPostfix]
        public static void Ssdfdwesasd(float input, bool sendCallback)
        {
            if (Variables.Terminal != null && Variables.Terminal.terminalInUse)
            {
                StackTrace stackTrace = new StackTrace();

                if (stackTrace.GetFrame(3).GetMethod().Name == "MoveNext")
                    loopCount++;
                else if (loopCount >= 4)
                    loopCount = 0;
            }
        }

        // ----------------------------------------------------------------------------

        [HarmonyPatch(typeof(Terminal), "Update")]
        [HarmonyPostfix]
        public static void TUPatch(ref float ___timeSinceLastKeyboardPress, Terminal __instance)
        {
            if (__instance.terminalInUse)
            {
                __instance.scrollBarCanvasGroup.alpha = 1;
                terminalKeyPressed = ___timeSinceLastKeyboardPress < 0.08;
                if (timeSinceSubmit < 0.07)
                    __instance.scrollBarVertical.value = 1f;
            }
            timeSinceSubmit += Time.deltaTime;
        }

        [HarmonyPatch(typeof(Terminal), "QuitTerminal")]
        [HarmonyPostfix]
        public static void QTPatch(Terminal __instance)
        {
            __instance.scrollBarVertical.value = currentScrollPosition = 1f;
        }

        [HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
        [HarmonyPrefix]
        public static void ResetSubmitPatch()
        {
            timeSinceSubmit = 0f;
        }

        // ----------------------------------------------------------------------------

        [HarmonyPatch(typeof(ScrollRect), "UpdateScrollbars")]
        [HarmonyPostfix]
        public static void ScrollTest(
            Vector2 offset,
            ref Bounds ___m_ContentBounds,
            ref Bounds ___m_ViewBounds,
            ref RectTransform ___m_Content,
            ref Vector2 ___m_PrevPosition,
            ScrollRect __instance
        )
        {
            if (Variables.Terminal != null && Variables.Terminal.terminalInUse)
            {
                __instance.verticalScrollbarVisibility =
                    ScrollbarVisibility.AutoHideAndExpandViewport;

                float scrollbarSize = ___m_ContentBounds.size.y - ___m_ViewBounds.size.y;

                int totalSteps =
                    customSens != 0 && Mathf.RoundToInt(scrollbarSize / Mathf.Abs(customSens)) > 1
                        ? Mathf.CeilToInt(scrollbarSize / Mathf.Abs(customSens))
                        : 1;

                if (scrollbarSize < 200f && scrollbarSize > 0f)
                    totalSteps = Mathf.CeilToInt(scrollbarSize / 65);
                else if (totalSteps <= 1 && scrollbarSize > 0f)
                    totalSteps = 2;
                if (totalSteps < 1)
                    totalSteps = 1;

                if (
                    ___m_PrevPosition != null
                    && Mathf.RoundToInt(___m_PrevPosition.y)
                        != Mathf.RoundToInt(___m_Content.anchoredPosition.y)
                )
                {
                    if ((___m_Content.anchoredPosition.y - ___m_PrevPosition.y) > 0)
                        currentStep++;
                    else if ((___m_Content.anchoredPosition.y - ___m_PrevPosition.y) < 0)
                        currentStep--;
                }

                Mathf.Clamp(currentStep, 0, totalSteps);

                if (timeSinceSubmit <= 0.08f || loopCount > 0)
                {
                    currentScrollPosition = 1f;
                    currentStep = loopCount = 0;
                    ___m_Content.anchoredPosition = Vector2.zero;
                }
                else if (terminalKeyPressed)
                {
                    currentScrollPosition = 0f;
                    currentStep = totalSteps;
                }
                else
                {
                    currentScrollPosition = Mathf.Clamp01(1f - ((float)currentStep / totalSteps));
                }

                __instance.verticalNormalizedPosition = currentScrollPosition;
            }
        }
    }
}
