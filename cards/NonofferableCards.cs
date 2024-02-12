﻿using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clay.PhilipTheMechanic.Cards;

[HarmonyPatch]
internal sealed class Nanobots : Card
{
    public override List<CardAction> GetActions(State s, Combat c) { return new() {}; }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(State), nameof(State.ShuffleDeck))]
    public static void REPLICATE(State __instance, bool isMidCombat = false)
    {
        try
        {
            if (isMidCombat && __instance.route is Combat combat)
            {
                int nanobotsCount = 0;
                foreach (Card card in __instance.deck)
                {
                    if (card is Nanobots n) { nanobotsCount++; }
                }

                for (int i = 0; i < nanobotsCount; i++)
                {
                    __instance.SendCardToDeck(new Nanobots(), doAnimation: false, insertRandomly: true);
                }

                for (int i = 0; i < nanobotsCount; i++)
                {
                    var cardIdx = __instance.rngShuffle.NextInt() % __instance.deck.Count;
                    var card = __instance.deck[cardIdx];
                    __instance.deck.RemoveAt(cardIdx);
                    combat.SendCardToExhaust(__instance, card);
                }
            }
        }
        catch (Exception e)
        {
            ModEntry.Instance.Logger.LogError("Error in nanobot replication");
            ModEntry.Instance.Logger.LogError(e.ToString());
            ModEntry.Instance.Logger.LogError(e.StackTrace);
        }
    }

    public override CardData GetData(State state) => new()
    {
        unplayable = true,
        temporary = true,
        cost = 1,
        exhaust = true,
        description = ModEntry.Instance.Localizations.Localize(["card", "Nanobots", "description", upgrade.ToString()])
    };
}

internal sealed class UraniumRound : Card
{
    public override CardData GetData(State state)
    {
        return new()
        {
            cost = upgrade == Upgrade.A ? 1 : 0,
            temporary = true,
            exhaust = upgrade != Upgrade.B,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return new()
        {
            new AAttack()
            {
                damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
                piercing = true,
                stunEnemy = true
            }
        };
    }
}

internal sealed class ImpromptuBlastShield : Card
{
    public override CardData GetData(State state)
    {
        return new()
        {
            cost = 0,
            temporary = true,
            unplayable = true,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return new()
        {
            ModEntry.Instance.Api.MakeAModifierWrapper
            (
                IPhilipAPI.CardModifierTarget.Directional_WholeHand,
                new()
                {
                    ModEntry.Instance.Api.MakeMAddAction(
                        new AStatus() { status = upgrade == Upgrade.B ? Status.shield : Status.tempShield, targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 3 : 1 },
                        ModEntry.Instance.sprites[upgrade == Upgrade.B ? "icon_sticker_shield" : "icon_sticker_temp_shield"].Sprite
                    )
                },
                new()
                {
                    isFlimsy = upgrade == Upgrade.A
                }
            )
        };
    }
}

internal sealed class OhNo : Card
{
    public override CardData GetData(State state)
    {
        return new()
        {
            cost = 0,
            retain = true,
            temporary = true,
            exhaust = true,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return new()
        {
            ModEntry.Instance.Api.MakeAModifierWrapper
            (
                IPhilipAPI.CardModifierTarget.Directional_WholeHand,
                new()
                {
                    ModEntry.Instance.Api.MakeMDeleteActions(),
                    ModEntry.Instance.Api.MakeMExhaust(),
                    ModEntry.Instance.Api.MakeMAddAction(
                        new AStatus() { status = Status.evade, targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 2 : 1 },
                        ModEntry.Instance.sprites["icon_sticker_evade"].Sprite
                    ),
                    ModEntry.Instance.Api.MakeMAddAction(
                        new AStatus() { status = ModEntry.Instance.Api.RedrawStatus.Status, targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 3 : 2 },
                        ModEntry.Instance.sprites["icon_sticker_redraw"].Sprite
                    ),
                }
            )
        };
    }
}
