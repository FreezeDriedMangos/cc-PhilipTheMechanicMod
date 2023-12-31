﻿using CobaltCoreModding.Definitions.ExternalItems;
using PhilipTheMechanic.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PhilipTheMechanic.ModifiedCardsRegistry;
using static System.Collections.Specialized.BitVector32;

namespace PhilipTheMechanic.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class OverdriveMod : ModifierCard
    {
        public override string Name()
        {
            return "Overdrive Mod";
        }

        public override TargetLocation GetBaseTargetLocation() 
        {
            switch (upgrade)
            {
                default: return TargetLocation.SINGLE_RIGHT;
                case Upgrade.A: return TargetLocation.SINGLE_RIGHT;
                case Upgrade.B: return TargetLocation.ALL_LEFT;
            }
        }

        public override void ApplyMod(Card c)
        {
            ModifiedCardsRegistry.RegisterMod(
                this, 
                c,
                ModifierPriority.LAST,
                actionsModification: (List<CardAction> cardActions, State s) =>
                {
                    List<CardAction> overridenCardActions = new();
                    foreach (var action in cardActions)
                    {
                        if (action is AAttack attack)
                        {
                            var newAttack = Mutil.DeepCopy(attack);
                            newAttack.damage += 1;
                            overridenCardActions.Add(newAttack);
                        }
                        else
                        {
                            overridenCardActions.Add(action);
                        }
                    }
                    return overridenCardActions;
                },
                stickers: new() { (Spr)MainManifest.sprites["icon_sticker_buff_attack"].Id }
            );
        }

        public override CardData GetData(State state)
        {
            switch (upgrade)
            {
                default:
                    return new()
                    {
                        cost = 1,
                    };
                case Upgrade.A:
                    return new()
                    {
                        cost = 1,
                        flippable = true,
                    };
                case Upgrade.B:
                    return new()
                    {
                        cost = 1,
                    };
            }
        }

        // NOTE: this is only here for the tooltip, this card isn't actually supposed to have any actions
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new List<CardAction>() {
                new ATooltipDummy() {
                    tooltips = new() {
                        new TTText()
                        {
                            text = $"Increases the damage of every attack on {GetTargetLocationString()} by 1. On play, attack for 1 damage."
                        },
                        new TTGlossary(GetGlossaryForTargetLocation().Head),
                        new TTGlossary(MainManifest.glossary["AAttackBuff"].Head, "1")
                    },
                    icons = new() {
                        new Icon((Spr)GetIconSpriteForTargetLocation().Id, null, Colors.textMain),
                        new Icon((Spr)MainManifest.sprites["icon_attack_buff"].Id, 1, Colors.textMain)
                    }
                },

                new AAttack()
                {
                    targetPlayer = false,
                    damage = GetDmg(s, 1)
                }
            };
        }
    }
}
