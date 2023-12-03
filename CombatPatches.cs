﻿using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipTheMechanic
{
    [HarmonyPatch(typeof(Combat))]
    public static class CombatPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Combat.SendCardToHand))]
        public static void HarmonyPostfix_Combat_SendCardToHand(Combat __instance, State s, Card card, int? position = null)
        {
            MainManifest.Instance.Logger.LogInformation($"Drew {card.GetFullDisplayName()}");
            foreach (Card otherCard in __instance.hand)
            {
                if (otherCard is ModifierCard mc) { mc.OnOtherCardDrawnWhileThisWasInHand(s, __instance); }
            }
        }

    }
}
