﻿using clay.PhilipTheMechanic.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clay.PhilipTheMechanic.Actions.CardModifiers
{
    public class MPlayTwice : ICardModifier
    {
        public bool RequestsStickyNote() 
        {
            return false;
        }

        public Spr? GetSticker(State s)
        {
            return ModEntry.Instance.sprites["icon_2x_sticker"].Sprite;
        }
        public Icon? GetIcon(State s)
        {
            return new Icon(ModEntry.Instance.sprites["icon_play_twice"].Sprite, null, Colors.textMain);
        }
        List<Tooltip> GetTooltips(State s) 
        {
            return new(); // TODO
        }
        public List<CardAction> TransformActions(List<CardAction> actions, State s, Combat c, Card card, bool isRendering)
        {
            // don't put double the actions on the card
            if (isRendering) return actions;

            // rft asked that I don't clone actions
            ModifierCardsController.SuppressActionMods = true;
            actions.AddRange(card.GetActionsOverridden(s, c));
            ModifierCardsController.SuppressActionMods = false;

            return actions;
        }
    }
}
