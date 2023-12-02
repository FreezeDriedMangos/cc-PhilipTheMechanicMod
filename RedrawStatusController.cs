﻿using FSPRO;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipTheMechanic
{
    [HarmonyPatch(typeof(Card))]
    public class RedrawStatusController
    {
        private static void HandleRedraw(G g, Card card)
        {

            if (g.state.route is Combat c)
            {
                var redrawAmount = g.state.ship.Get((Status)MainManifest.statuses["redraw"].Id);
                g.state.ship.Set((Status)MainManifest.statuses["redraw"].Id, redrawAmount - 1);

                DiscardFromHand(g.state, card);
                c.DrawCards(g.state, 1);

                foreach (Card otherCard in c.hand)
                {
                    if (otherCard is ModifierCard mc) { mc.OnOtherCardDiscardedWhileThisWasInHand(g.state, c); }
                }
            }
        }

        // Modified from Combat.DiscardHand
        public static void DiscardFromHand(State s, Card card, bool silent = false)
        {
            if (s.route is Combat c)
            {
                //if (c.hand.Count <= index || index < 0)
                //{
                //    MainManifest.Instance.Logger.LogCritical($"DiscardFromHand was called on index {index} for handsize {c.hand.Count}");
                //    return;
                //}

                //var card = c.hand[index];
                c.hand.Remove(card);
                card.waitBeforeMoving = 0.05;
                card.OnDiscard(s, c);
                c.SendCardToDiscard(s, card);

                if (!silent)
                {
                    Audio.Play(Event.CardHandling);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Card.Render))]
        public static void HarmonyPostfix_Card_Render(Card __instance, G g, Vec? posOverride = null, State? fakeState = null, bool ignoreAnim = false, bool ignoreHover = false, bool hideFace = false, bool hilight = false, bool showRarity = false, bool autoFocus = false, UIKey? keyOverride = null, OnMouseDown? onMouseDown = null, OnMouseDownRight? onMouseDownRight = null, OnInputPhase? onInputPhase = null, double? overrideWidth = null, UIKey? leftHint = null, UIKey? rightHint = null, UIKey? upHint = null, UIKey? downHint = null, int? renderAutopilot = null, bool? forceIsInteractible = null, bool reportTextBoxesForLocTest = false, bool isInCombatHand = false)
        {
            State state = fakeState ?? g.state;

            if (state.route is Combat c && c.routeOverride != null && !c.eyeballPeek) { return; }
            if (state.route is not Combat) { return; } // should never hit this case
            
            if (state.ship.Get((Status)MainManifest.statuses["redraw"].Id) <= 0) { return; }
            //MainManifest.Instance.Logger.LogInformation($"~~~~~`````~~~~~ drawing redraw on card {__instance.uuid}:{__instance.Name()}");

            int cardIndex = (g.state.route as Combat).hand.IndexOf(__instance);

            Vec vec = posOverride ?? __instance.pos;
            Rect rect = (__instance.GetScreenRect() + vec + new Vec(0.0, __instance.hoverAnim * -2.0 + Mutil.Parabola(__instance.flipAnim) * -10.0 + Mutil.Parabola(Math.Abs(__instance.flopAnim)) * -10.0 * Math.Sign(__instance.flopAnim))).round();
            Rect value = rect;
            if (overrideWidth.HasValue)
            {
                rect.w = overrideWidth.Value;
            }

            int currentCost = __instance.GetCurrentCost(state);
            bool flag = !(state.route is Combat combat) || combat.energy >= currentCost;
            bool flag2 = forceIsInteractible ?? __instance.drawAnim >= 1.0;
            Box box = g.Push(flag2 ? new UIKey?(keyOverride ?? __instance.UIKey()) : null, rect, value, depth: !ignoreHover && __instance.isForeground ? 1 : 0, onMouseDown: flag2 ? onMouseDown : null, onMouseDownRight: flag2 ? onMouseDownRight : null, onInputPhase: ignoreHover ? null : onInputPhase, onAfterUI: __instance, autoFocus: flag2 && autoFocus, noHoverSound: false, gamepadUntargetable: false, rightHint: rightHint, leftHint: leftHint, upHint: upHint, downHint: downHint, reticleMode: isInCombatHand && g.state.hideCardTooltips ? ReticleMode.QuadCardHint : ReticleMode.Quad);
            Vec vec2 = box.rect.xy + new Vec(0.0, 1.0);

            // card height is 82, width is 59
            //Draw.Sprite((Spr)MainManifest.sprites["icon_screw"].Id, vec2.x + 46, vec2.y + 19);
            //Draw.Sprite((Spr)MainManifest.sprites["icon_screw"].Id, vec2.x + 4, vec2.y + 69);

            var cardHalfWidth = 59.0 / 2.0;
            var cardHeight = 82.0;
            Rect rect2 = new(cardHalfWidth-19.0/2.0, cardHeight-13.0/2.0, 19, 13);
            OnMouseDown omd = new MouseDownHandler(() => HandleRedraw(g, __instance));
            ButtonSprite(g, vec2, rect2, (UK)(100 + cardIndex), (Spr)MainManifest.sprites["button_redraw"].Id, (Spr)MainManifest.sprites["button_redraw_on"].Id, null, null, inactive: false, flipX: false, flipY: false, omd, autoFocus: false, noHover: false, gamepadUntargetable: true);

            g.Pop();
        }

        // from https://github.com/Shockah/Cobalt-Core-Mods/blob/master/CrewSelectionHelper/Patches/NewRunOptionsPatches.cs
        public static SharedArt.ButtonResult ButtonSprite(G g, Vec vec2, Rect rect, UIKey key, Spr sprite, Spr spriteHover, Spr? spriteDown = null, Color? boxColor = null, bool inactive = false, bool flipX = false, bool flipY = false, OnMouseDown? onMouseDown = null, bool autoFocus = false, bool noHover = false, bool showAsPressed = false, bool gamepadUntargetable = false, UIKey? leftHint = null, UIKey? rightHint = null)
        {
            bool gamepadUntargetable2 = gamepadUntargetable;
            Box box = g.Push(key, rect, null, autoFocus, inactive, gamepadUntargetable2, ReticleMode.Quad, onMouseDown, null, null, null, 0, rightHint, leftHint);
            Vec xy = box.rect.xy;
            bool flag = !noHover && (box.IsHover() || showAsPressed) && !inactive;
            if (spriteDown.HasValue && box.IsHover() && Input.mouseLeft)
                showAsPressed = true;
            
            g.Pop();

            Draw.Sprite(!showAsPressed ? flag ? spriteHover : sprite : spriteDown ?? spriteHover, xy.x, xy.y, flipX, flipY, 0, null, null, null, null, boxColor);
            SharedArt.ButtonResult buttonResult = default;
            buttonResult.isHover = flag;
            buttonResult.FIXME_isHoverForTooltip = !noHover && box.IsHover();
            buttonResult.v = xy;
            buttonResult.innerOffset = new Vec(0.0, showAsPressed ? 2 : flag ? 1 : 0);
            SharedArt.ButtonResult result = buttonResult;

            return result;
        }
    }
}