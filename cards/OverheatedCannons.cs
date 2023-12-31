﻿using Microsoft.Extensions.Logging;
using PhilipTheMechanic.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PhilipTheMechanic.ModifiedCardsRegistry;
using static PhilipTheMechanic.ModifierCard;

namespace PhilipTheMechanic.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class OverheatedCannons : ModifierCard
    {
        public override string Name()
        {
            return "Disable Safeties";
        }

        public override TargetLocation GetBaseTargetLocation()
        {
            return TargetLocation.ALL_RIGHT;
        }

        public override void ApplyMod(Card c)
        {
            ModifiedCardsRegistry.RegisterMod(
                this,
                c,
                ModifierPriority.EARLY, // early so that the heat action typically ends up at the bottom
                actionsModification: (List<CardAction> cardActions, State s) =>
                {
                    List<CardAction> overridenCardActions = new(cardActions);
                    overridenCardActions.Add(new AAttack() { damage = GetDmg(s, 1) });
                    overridenCardActions.Add(new AAttack() { damage = GetDmg(s, (upgrade == Upgrade.B ? 2 : 1)) });
                    overridenCardActions.Add(new AStatus() { targetPlayer = true, status = Enum.Parse<Status>("heat"), statusAmount = (upgrade == Upgrade.B ? 2 : 1) });
                    return overridenCardActions;
                },
                stickers: new() { 
                    (Spr)MainManifest.sprites["icon_sticker_attack"].Id, 
                    (Spr)MainManifest.sprites["icon_sticker_attack"].Id,
                    (Spr)MainManifest.sprites["icon_sticker_heat"].Id,
                }
            );
        }

        public override bool IsFlimsy()
        {
            return true;
        }

        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 0,
                unplayable = false,
                retain = (upgrade == Upgrade.A)
            };
        }

        // NOTE: this is only here for the tooltip, this card isn't actually supposed to have any actions
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new List<CardAction>() {
                new ATooltipDummy() {
                    tooltips = new() {
                        new TTText()
                        {
                            text = $"{GetTargetLocationString().Capitalize()} deals two extra (1 damage) attacks, and applies 1 heat to self."
                        },
                        new TTGlossary(GetGlossaryForTargetLocation().Head)
                    },
                    icons = new() {
                        new Icon((Spr)GetIconSpriteForTargetLocation().Id, null, Colors.heal),
                        new Icon(Enum.Parse<Spr>("icons_attack"), 1, Colors.hurt),
                    }
                },

                new ATooltipDummy() {
                    tooltips = new() {},
                    icons = new() {
                        new Icon((Spr)GetIconSpriteForTargetLocation().Id, null, Colors.heal),
                        new Icon(Enum.Parse<Spr>("icons_attack"), (upgrade == Upgrade.B ? 2 : 1), Colors.hurt),
                    }
                },

                new ATooltipDummy() {
                    tooltips = new() {},
                    icons = new() {
                        new Icon((Spr)GetIconSpriteForTargetLocation().Id, null, Colors.heal),
                        new Icon(Enum.Parse<Spr>("icons_heat"), (upgrade == Upgrade.B ? 2 : 1), Colors.hurt)
                    }
                }
            };
        }
    }
}
