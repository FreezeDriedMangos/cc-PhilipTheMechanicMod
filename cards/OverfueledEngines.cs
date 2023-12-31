﻿using PhilipTheMechanic.actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PhilipTheMechanic.ModifiedCardsRegistry;

namespace PhilipTheMechanic.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class OverfueledEngines : ModifierCard
    {
        public override string Name()
        {
            return "Overfueled Engines";
        }

        public override TargetLocation GetBaseTargetLocation()
        {
            switch (upgrade)
            {
                default: return TargetLocation.SINGLE_RIGHT;
                case Upgrade.A: return TargetLocation.SINGLE_RIGHT;
                case Upgrade.B: return TargetLocation.SINGLE_RIGHT;
            }
        }

        public override void ApplyMod(Card c)
        {
            ModifiedCardsRegistry.RegisterMod(
                this,
                c,
                ModifierPriority.LAST, // last so that other cards don't add actions after the dummy actions 
                actionsModification: (List<CardAction> cardActions, State s) =>
                {
                    List<CardAction> overridenCardActions = new(cardActions);
                    overridenCardActions.Add(new AReplay() { card = c });

                    if (upgrade == Upgrade.A) overridenCardActions.Add(new AAddCardNoIcon() { card = new TrashFumes(), destination = Enum.Parse<CardDestination>("Deck") });
                    else                      overridenCardActions.Add(new AAddCardNoIcon() { card = new Toxic(),      destination = Enum.Parse<CardDestination>("Hand") });

                    overridenCardActions.Insert(0, new ADummyAction() { });
                    overridenCardActions.Insert(0, new ADummyAction() { });
                    return overridenCardActions;
                },
                stickers: new List<Spr>() { (Spr)MainManifest.sprites["icon_2x_sticker"].Id, (Spr)MainManifest.sprites["icon_sticker_add_card"].Id }
            );
        }

        public override bool IsFlimsy()
        {
            return true;
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
                        //description = $"When {GetTargetLocationString(true)} is played, repeat its effects and add a Toxic to your hand."
                    };
                case Upgrade.A:
                    return new()
                    {
                        cost = 0,
                        unplayable = false,
                        //description = $"When {GetTargetLocationString(true)} is played, repeat its effects and add a Fumes to your deck."
                    };
                case Upgrade.B:
                    return new()
                    {
                        cost = 0,
                        unplayable = false,
                        flippable = true,
                        //description = $"When {GetTargetLocationString(true)} is played, repeat its effects and add a Toxic to your hand."
                    };
            }
        }

        // NOTE: this is only here for the tooltip, this card isn't actually supposed to have any actions
        public override List<CardAction> GetActions(State s, Combat c) 
        {
            List<Tooltip>? addCardToolTips;
            string cardName;
            string cardDestination;

            if (upgrade == Upgrade.A)
            {
                addCardToolTips = (new AAddCard() { card = new TrashFumes(), destination = Enum.Parse<CardDestination>("Deck") }).GetTooltips(s);
                cardName = "Fumes";
                cardDestination = "deck";
            }
            else
            {
                addCardToolTips = (new AAddCard() { card = new Toxic(), destination = Enum.Parse<CardDestination>("Hand") }).GetTooltips(s);
                cardName = "Toxic";
                cardDestination = "hand";
            }

            return new List<CardAction>() {
                new ATooltipDummy() {
                    tooltips = new() {
                        new TTText()
                        {
                            text = $"When {GetTargetLocationString(true)} is played, repeat its effects and add a {cardName} to your {cardDestination}."
                        },
                        new TTGlossary(GetGlossaryForTargetLocation().Head, null),
                        new TTGlossary(MainManifest.glossary["AReplay"].Head, null),
                        addCardToolTips[0],
                        addCardToolTips[1],
                    },
                    icons = new() {
                        new Icon((Spr)GetIconSpriteForTargetLocation().Id, null, Colors.textMain),
                        new Icon((Spr)MainManifest.sprites["icon_play_twice"].Id, null, Colors.textMain),
                        new Icon(Enum.Parse<Spr>("icons_addCard"), 1, Colors.textMain)
                    }
                }
            };
        }
    }
}
