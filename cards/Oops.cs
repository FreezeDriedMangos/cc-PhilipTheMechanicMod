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
    public class Oops : ModifierCard
    {
        public override string Name()
        {
            return "Oops";
        }

        public override TargetLocation GetBaseTargetLocation() 
        {
            switch (upgrade)
            {
                default: return TargetLocation.NEIGHBORS;
                case Upgrade.A: return TargetLocation.NEIGHBORS;
                case Upgrade.B: return TargetLocation.ALL_LEFT;
            }
        }

        public override void ApplyMod(Card c)
        {
            ModifiedCardsRegistry.RegisterMod(
                this, 
                c,
                ModifierPriority.LATE,
                actionsModification: (List<CardAction> cardActions, State s) =>
                {
                    List<CardAction> overridenCardActions = new();
                    foreach (var action in cardActions)
                    {
                        if (action is AAttack attack)
                        {
                            var newAttack = Mutil.DeepCopy(attack);
                            newAttack.damage = GetDmg(s, Math.Max(0, newAttack.damage - (upgrade == Upgrade.A ? 2 : 1)));
                            overridenCardActions.Add(newAttack);
                        }
                        else
                        {
                            overridenCardActions.Add(action);
                        }
                    }
                    overridenCardActions.Add(
                        new AStatus()
                        {
                            status = (Status)MainManifest.statuses["redraw"].Id,
                            targetPlayer = true,
                            statusAmount = (upgrade == Upgrade.A ? 2 : 1),
                            mode = Enum.Parse<AStatusMode>("Add"),
                        }
                    );
                    return overridenCardActions;
                },
                stickers: new() { (Spr)MainManifest.sprites["icon_sticker_buff_attack"].Id, (Spr)MainManifest.sprites["icon_sticker_redraw"].Id }
            );
        }

        public override CardData GetData(State state)
        {
            switch (upgrade)
            {
                default:
                    return new()
                    {
                        cost = 0,
                        unplayable = true,
                        //description = $"Increases the damage of every attack on {GetTargetLocationString()} by 1."
                    };
                case Upgrade.A:
                    return new()
                    {
                        cost = 0,
                        unplayable = true,
                        //description = $"Increases the damage of every attack on {GetTargetLocationString()} by 1."
                    };
                case Upgrade.B:
                    return new()
                    {
                        cost = 0,
                        unplayable = true,
                        //description = $"Increases the damage of every attack on {GetTargetLocationString()} by 1."
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
                            text = $"Decreases the damage of every attack on {GetTargetLocationString()} by {(upgrade == Upgrade.A ? 2 : 1)} and adds \"gain {(upgrade == Upgrade.A ? 2 : 1)} redraw\"."
                        },
                        new TTGlossary(GetGlossaryForTargetLocation().Head),
                        new TTGlossary(MainManifest.glossary["AAttackBuff"].Head, "-1"),
                        new TTGlossary(MainManifest.glossary["SRedraw"].Head, "1")
                    },
                    icons = new() {
                        new Icon((Spr)GetIconSpriteForTargetLocation().Id, null, Colors.textMain),
                        new Icon((Spr)MainManifest.sprites["icon_attack_buff"].Id, (upgrade == Upgrade.A ? -2 : -1), Colors.textMain),
                    }
                },
                new ATooltipDummy() {
                    tooltips = new() {},
                    icons = new() {
                        new Icon((Spr)GetIconSpriteForTargetLocation().Id, null, Colors.textMain),
                        new Icon((Spr)MainManifest.sprites["icon_redraw"].Id, (upgrade == Upgrade.A ? 2 : 1), Colors.textMain)
                    }
                }
            };
        }
    }
}
