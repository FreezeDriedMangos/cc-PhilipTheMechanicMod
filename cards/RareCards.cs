﻿using clay.PhilipTheMechanic.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clay.PhilipTheMechanic.Cards;

internal sealed class BlackMarketParts : Card, IRegisterableCard
{
    public static Rarity GetRarity() => Rarity.rare;

    public override CardData GetData(State state)
    {
        return new()
        {
            cost = 1,
            unplayable = true,
            flippable = true
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var addCardAction = upgrade == Upgrade.B
            ? new AAddCardUpgraded() { card = new UraniumRound() { upgrade = Upgrade.B } }
            : new AAddCard() { amount = upgrade == Upgrade.A ? 2 : 1, card = new UraniumRound() };

        return new()
        {
            ModEntry.Instance.Api.MakeAModifierWrapper
            (
                IPhilipAPI.CardModifierTarget.Directional,
                new()
                {
                    ModEntry.Instance.Api.MakeMDeleteActions(),
                    ModEntry.Instance.Api.MakeMExhaust(),
                    ModEntry.Instance.Api.MakeMAddAction(
                        addCardAction,
                        ModEntry.Instance.sprites["icon_sticker_add_card"].Sprite
                    ),
                }
            )
        };
    }
}

internal sealed class DisableSafeties : Card, IRegisterableCard
{
    public static Rarity GetRarity() => Rarity.rare;

    public override CardData GetData(State state)
    {
        return new()
        {
            cost = 0,
            unplayable = upgrade == Upgrade.B
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (upgrade == Upgrade.B)
        {
            return new()
            {
                ModEntry.Instance.Api.MakeAModifierWrapper
                (
                    IPhilipAPI.CardModifierTarget.Neighboring,
                    new()
                    {
                        ModEntry.Instance.Api.MakeMAddAction(
                            new AAttack() { damage = GetDmg(s, 1) },
                            ModEntry.Instance.sprites["icon_sticker_attack"].Sprite
                        )
                    }
                ),
                ModEntry.Instance.Api.MakeAModifierWrapper
                (
                    IPhilipAPI.CardModifierTarget.Neighboring,
                    new()
                    {
                        ModEntry.Instance.Api.MakeMAddAction(
                            new AStatus() { status = Status.heat, targetPlayer = true, statusAmount = 1 },
                            ModEntry.Instance.sprites["icon_sticker_heat"].Sprite
                        )
                    }
                ),
            };
        }

        return new()
        {
            ModEntry.Instance.Api.MakeAModifierWrapper
            (
                IPhilipAPI.CardModifierTarget.Directional_WholeHand,
                new()
                {
                    ModEntry.Instance.Api.MakeMAddAction(
                        new AAttack() { damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1) },
                        ModEntry.Instance.sprites["icon_sticker_attack"].Sprite
                    ),
                },
                new()
                {
                    isFlimsy = true,
                }
            ),
            ModEntry.Instance.Api.MakeAModifierWrapper
            (
                IPhilipAPI.CardModifierTarget.Directional_WholeHand,
                new()
                {
                    ModEntry.Instance.Api.MakeMAddAction(
                        new AAttack() { damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1) },
                        ModEntry.Instance.sprites["icon_sticker_attack"].Sprite
                    ),
                },
                new()
                {
                    isFlimsy = true,
                }
            ),
            ModEntry.Instance.Api.MakeAModifierWrapper
            (
                IPhilipAPI.CardModifierTarget.Directional_WholeHand,
                new()
                {
                    ModEntry.Instance.Api.MakeMAddAction(
                        new AStatus() { status = Status.heat, targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 3 : 2 },
                        ModEntry.Instance.sprites["icon_sticker_heat"].Sprite
                    )
                },
                new()
                {
                    isFlimsy = true,
                }
            )
        };
    }
}

internal sealed class EmergencyTraining : Card, IRegisterableCard
{
    public static Rarity GetRarity() => Rarity.rare;

    public override CardData GetData(State state)
    {
        return new()
        {
            cost = upgrade == Upgrade.A ? 2 : 3,
            exhaust = true,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var addCardAction = upgrade == Upgrade.B
            ? new AAddCardUpgraded() { card = new ImpromptuBlastShield() { upgrade = Upgrade.B }, destination = CardDestination.Hand }
            : new AAddCard() { card = new ImpromptuBlastShield(), destination = CardDestination.Hand };
        return new()
        {
            new ADiscard(),
            new AStatus() {
                status = ModEntry.Instance.RedrawStatus.Status,
                targetPlayer = true,
                statusAmount = 5
            },
            addCardAction,
            new ADrawCard() { count = 4 }
        };
    }
}

internal sealed class NoStockParts : Card, IRegisterableCard
{
    public static Rarity GetRarity() => Rarity.rare;

    public override CardData GetData(State state)
    {
        return new()
        {
            cost = upgrade == Upgrade.A ? 2 : 3,
            exhaust = upgrade != Upgrade.B,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return new()
        {
            new AStatus() {
                status = ModEntry.Instance.CustomPartsStatus.Status,
                targetPlayer = true,
                statusAmount = 1
            },
        };
    }
}