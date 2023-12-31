﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using PhilipTheMechanic.artifacts;
using PhilipTheMechanic.cards;
using Shockah.Shared;

namespace PhilipTheMechanic
{
    public class MainManifest : IModManifest, ISpriteManifest, ICardManifest, ICharacterManifest, IDeckManifest, IAnimationManifest, IGlossaryManifest, IStatusManifest, IArtifactManifest
    {
        public static MainManifest Instance;

        public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[0];

        public DirectoryInfo? GameRootFolder { get; set; }
        public Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
        public DirectoryInfo? ModRootFolder { get; set; }

        public string Name => "clay.PhilipTheEngineer";

        public static Dictionary<string, ExternalSprite> sprites = new Dictionary<string, ExternalSprite>();
        public static Dictionary<string, ExternalAnimation> animations = new Dictionary<string, ExternalAnimation>();
        public static Dictionary<string, ExternalCard> cards = new Dictionary<string, ExternalCard>();
        public static Dictionary<string, ExternalStatus> statuses = new Dictionary<string, ExternalStatus>();
        public static Dictionary<string, ExternalGlossary> glossary = new Dictionary<string, ExternalGlossary>();
        public static Dictionary<string, CustomTTGlossary> vanillaSpritesGlossary = new Dictionary<string, CustomTTGlossary>();
        public static ExternalCharacter character;
        public static ExternalDeck deck;

        public void BootMod(IModLoaderContact contact)
        {
            Instance = this;
            //ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.dll"));
            //ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.Harmony.dll"));

            var harmony = new Harmony("PhilipTheMechanic");
            harmony.PatchAll();
            CustomTTGlossary.Apply(harmony);
        }

        public void LoadManifest(ISpriteRegistry artRegistry)
        {
            var filenames = new string[] { 
                "char_frame_philip", 
                "frame_philip",
                "floppable_fix_sticky_note",
                "floppable_fix_index_card",

                "philip_mini",
                "philip_classy_0",
                "philip_classy_1",
                "philip_classy_3",
                "philip_maniacal_0",
                "philip_maniacal_1",
                "philip_maniacal_3",
                "philip_squint_0",
                "philip_squint_1",
                "philip_squint_3",
                "philip_neutral_0",
                "philip_neutral_1",
                "philip_neutral_3",
                "philip_surprise_0",
                "philip_surprise_1",
                "philip_surprise_3",
                "philip_sheepish_0",
                "philip_sheepish_1",
                "philip_sheepish_3",
                "philip_proud",
                "philip_whatisthat",
                "philip_unhappy",
                "philip_excited_0",
                "philip_excited_1",
                "philip_excited_3",
                "philip_laugh_0",
                "philip_laugh_1",
                "philip_hot_chocolate",


                "icon_addUpgradedCard",
                "icon_play_twice",
                "icon_all_cards_to_the_left",
                "icon_all_cards_to_the_right",
                "icon_card_to_the_left",
                "icon_card_to_the_right",
                "icon_card_neighbors",
                "icon_Flimsy_Left_Card_Mod",
                "icon_Flimsy_Right_Card_Mod",
                "icon_Flimsy_All_Right_Card_Mod",
                "icon_Flimsy_All_Left_Card_Mod",
                "icon_Flimsy_Neighbors_Card_Mod",
                "icon_attack_buff",
                "icon_screw",
                "icon_equal",
                "icon_redraw",
                "icon_customParts",
                "icon_no_action",
                "icon_card_is_centered",
                "icon_card_is_not_centered",
                "icon_card_is_centered_disabled",
                "icon_card_is_not_centered_disabled",
                "icon_flimsy",

                "icon_2x_sticker",
                "icon_sticker_add_card",
                "icon_sticker_buff_attack",
                "icon_sticker_hull_damage",
                "icon_sticker_energy_discount",
                "icon_sticker_0_energy",
                "icon_sticker_attack",
                "icon_sticker_temp_shield_attack",
                "icon_sticker_shield_attack",
                "icon_sticker_temp_shield",
                "icon_sticker_shield",
                "icon_sticker_piercing",
                "icon_sticker_heat",
                "icon_sticker_evade",
                "icon_sticker_exhaust",
                "icon_sticker_missile",
                "icon_sticker_hermes",
                "icon_sticker_stun",
                "icon_sticker_recycle",
                "icon_sticker_redraw",
                "icon_sticker_retain",
                "icon_sticker_no_action",
                "icon_sticker_energyLessNextTurn",

                "button_redraw",
                "button_redraw_on",

                "artifact_wire_clippers",
                "artifact_angle_grinder",
                "artifact_endless_toolbox",
                "artifact_self_propelling_cannons",
                "artifact_hot_chocolate",
                "artifact_electromagnet",

                "card_philip_default",
                "card_Black_Market_Parts",
                "card_Last_Resort",
                "card_Overdrive_Mod",
                "card_Precise_Machining",
                "card_Oops",
                "card_Oh_No",
                "card_Uh_Oh",
                "card_Piercing_Mod",
                "card_Uranium_Rounds",
                "card_Impromptu_Blast_Shield",
                "card_Duct_Tape_and_Dreams",
                "card_Frenzy_Mod",
                "card_Recycle_Parts",
                "card_Stun_Mod",
                "card_Shielding_Mod",
                "card_Permanence_Mod",
                "card_Nanobot_Infestation",
                "card_Nanobots",
                "card_Emergency_Training",
                "card_Loosen_Screws",
                "card_Overfueled_Engines",
                "card_Open_Bay_Doors",
            };

            foreach (var filename in filenames) {
                var filepath = Path.Combine(ModRootFolder?.FullName ?? "", "sprites", filename+".png");
                var sprite = new ExternalSprite("clay.PhilipTheEngineer.sprites."+filename, new FileInfo(filepath));
                sprites[filename] = sprite;

                if (!artRegistry.RegisterArt(sprite)) throw new Exception("Error registering sprite " + filename);
            }
        }

        public void LoadManifest(ICardRegistry registry)
        {
            // GOAL:
            // 21 cards
            // 9 common, 7 uncommon, 5 rare
            var cardDefinitions = new ExternalCard[]
            {
                new ExternalCard("clay.PhilipTheMechanic.cards.Overdrive Mod", typeof(OverdriveMod), sprites["card_Overdrive_Mod"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Frenzy Mod", typeof(FrenzyMod), sprites["card_Frenzy_Mod"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Loosen Screws", typeof(LoosenScrews), sprites["card_Loosen_Screws"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Overfueled Engines", typeof(OverfueledEngines), sprites["card_Overfueled_Engines"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Shielding Mod", typeof(ShieldingMod), sprites["card_Shielding_Mod"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Recycle Parts", typeof(RecycleParts), sprites["card_Recycle_Parts"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Emergency Training", typeof(EmergencyTraining), sprites["card_Emergency_Training"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Impromptu Blast Shield", typeof(ImpromptuBlastShield), sprites["card_Impromptu_Blast_Shield"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Piercing Mod", typeof(PiercingMod), sprites["card_Piercing_Mod"], deck),
                //new ExternalCard("clay.PhilipTheMechanic.cards.Permanence Mod", typeof(PermanenceMod), sprites["card_Permanence_Mod"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Spare Parts", typeof(SpareParts), sprites["card_philip_default"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Disable Safeties", typeof(OverheatedCannons), sprites["card_Last_Resort"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.No Stock Parts", typeof(NoStockParts), sprites["card_philip_default"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Black Market Parts", typeof(BlackMarketParts), sprites["card_Black_Market_Parts"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Duct Tape and Dreams", typeof(DuctTapeAndDreams), sprites["card_Duct_Tape_and_Dreams"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Jettison Parts", typeof(JettisonParts), sprites["card_Last_Resort"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Nanobot Infestation", typeof(NanobotInfestation), sprites["card_Nanobot_Infestation"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Nanobots", typeof(Nanobots), sprites["card_Nanobots"], ExternalDeck.GetRaw((int)Enum.Parse<Deck>("trash"))), 
                new ExternalCard("clay.PhilipTheMechanic.cards.Oh No", typeof(OhNo), sprites["card_Uh_Oh"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Oops", typeof(Oops), sprites["card_Oops"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Open Bay Doors", typeof(OpenBayDoors), sprites["card_Open_Bay_Doors"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Plan WAF", typeof(PlanWAF), sprites["card_Last_Resort"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Precision Machining", typeof(PrecisionMachining), sprites["card_Precise_Machining"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Reduce Reuse", typeof(ReduceReuse), sprites["card_Recycle_Parts"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Stun Mod", typeof(StunMod), sprites["card_Stun_Mod"], deck),
                new ExternalCard("clay.PhilipTheMechanic.cards.Uranium Round", typeof(UraniumRound), sprites["card_Uranium_Rounds"], deck),
            };

            foreach(var card in cardDefinitions)
            {
                var name = card.GlobalName.Split('.').LastOrDefault() ?? "FAILED TO FIND NAME";
                card.AddLocalisation(name);
                registry.RegisterCard(card);
                cards[name] = card;
            }
        }

        public void LoadManifest(IDeckRegistry registry)
        {
            var philipColor = 0;
            unchecked { philipColor = (int)0xffc9f000; }

            deck = new ExternalDeck(
                "clay.PhilipTheMechanic.PhilipDeck",
                System.Drawing.Color.FromArgb(philipColor),
                System.Drawing.Color.Black,
                sprites["card_philip_default"],
                sprites["frame_philip"],
                null
            );
            if (!registry.RegisterDeck(deck)) throw new Exception("Philip's lost his deck. Cannot proceed until he finds it.");
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            //var testStartCards = cards.Values.Select(card => card.CardType).ToList();
            //testStartCards.Add(typeof(UraniumRound));
            //testStartCards.Add(typeof(OverheatedCannons));

            character = new ExternalCharacter(
                "clay.PhilipTheMechanic.Philip",
                deck,
                sprites["char_frame_philip"],
                new Type[] { typeof(OverdriveMod), typeof(RecycleParts) },
                new Type[0],
                animations["neutral"],
                animations["mini"]
            );

            character.AddNameLocalisation("Philip");
            character.AddDescLocalisation("<c=c9f000>PHILIP</c>\nYour ship engineering officer. His cards modify other cards in your hand, provide <c=d6525f>redraw</c>, and are often unplayable.");

            if (!registry.RegisterCharacter(character)) throw new Exception("Philip is lost! Could not register Philip!");
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            var animationInfo = new Dictionary<string, IEnumerable<ExternalSprite>>();
            animationInfo["neutral"] = new ExternalSprite[] { sprites["philip_neutral_0"], sprites["philip_neutral_1"], sprites["philip_neutral_0"], sprites["philip_neutral_3"] };
            animationInfo["squint"] = new ExternalSprite[] { sprites["philip_squint_0"], sprites["philip_squint_1"], sprites["philip_squint_0"], sprites["philip_squint_3"] };
            animationInfo["gameover"] = new ExternalSprite[] { sprites["philip_surprise_0"], sprites["philip_surprise_1"], sprites["philip_surprise_0"], sprites["philip_surprise_3"] };
            animationInfo["classy"] = new ExternalSprite[] { sprites["philip_classy_0"], sprites["philip_classy_1"], sprites["philip_classy_0"], sprites["philip_classy_3"] };
            animationInfo["maniacal"] = new ExternalSprite[] { sprites["philip_maniacal_0"], sprites["philip_maniacal_1"], sprites["philip_maniacal_0"], sprites["philip_maniacal_3"] };
            animationInfo["sheepish"] = new ExternalSprite[] { sprites["philip_sheepish_0"], sprites["philip_sheepish_1"], sprites["philip_sheepish_0"], sprites["philip_sheepish_3"] };
            animationInfo["proud"] = new ExternalSprite[] { sprites["philip_proud"] };
            animationInfo["mini"] = new ExternalSprite[] { sprites["philip_mini"] };
            animationInfo["whatisthat"] = new ExternalSprite[] { sprites["philip_whatisthat"] };
            animationInfo["hotchocolate"] = new ExternalSprite[] { sprites["philip_hot_chocolate"] };
            animationInfo["unhappy"] = new ExternalSprite[] { sprites["philip_unhappy"] };
            animationInfo["excited"] = new ExternalSprite[] { sprites["philip_excited_0"], sprites["philip_excited_1"], sprites["philip_excited_0"], sprites["philip_excited_3"] };
            animationInfo["laugh"] = new ExternalSprite[] { sprites["philip_laugh_0"], sprites["philip_laugh_1"], sprites["philip_laugh_0"], sprites["philip_laugh_1"] };

            foreach (var kvp in animationInfo)
            {
                var animation = new ExternalAnimation(
                    "clay.PhilipTheMechanic.animations."+kvp.Key,
                    deck,
                    kvp.Key,
                    false,
                    kvp.Value
                );
                animations[kvp.Key] = animation;

                if (!registry.RegisterAnimation(animation)) throw new Exception("Error registering animation " + kvp.Key);
            }
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            RegisterGlossaryEntry(registry, "AReplay", sprites["icon_play_twice"],
                "play twice",
                "Play all actions prior to the Play Twice action twice."
            );

            RegisterGlossaryEntry(registry, "ACardToTheLeft", sprites["icon_card_to_the_left"],
                "modify card to the left",
                "Add the following effects to the card to the left. They do NOT trigger when this card is played."
            );
            RegisterGlossaryEntry(registry, "AAllCardsToTheLeft", sprites["icon_all_cards_to_the_left"],
                "modify all cards to the left",
                "Add the following effects to all cards to the left. They do NOT trigger when this card is played."
            );
            RegisterGlossaryEntry(registry, "ACardToTheRight", sprites["icon_card_to_the_right"],
                "modify card to the right",
                "Add the following effects to the card to the right. They do NOT trigger when this card is played."
            );
            RegisterGlossaryEntry(registry, "AAllCardsToTheRight", sprites["icon_all_cards_to_the_right"],
                "modify all cards to the right",
                "Add the following effects to all cards to the right. They do NOT trigger when this card is played."
            );
            RegisterGlossaryEntry(registry, "ANeighborCards", sprites["icon_card_neighbors"],
                "modify neighboring cards",
                "Add the following effects to both adjacent cards. They do NOT trigger when this card is played."
            );

            RegisterGlossaryEntry(registry, "AFlimsyCardToTheLeft", sprites["icon_Flimsy_Left_Card_Mod"],
                "flimsy modification",
                "Modify the card to the left. When a card modified by this is played, discard this card."
            );
            RegisterGlossaryEntry(registry, "AFlimsyAllCardsToTheLeft", sprites["icon_Flimsy_All_Left_Card_Mod"],
                "flimsy modification",
                "Modify all cards to the left. When a card modified by this is played, discard this card."
            );
            RegisterGlossaryEntry(registry, "AFlimsyCardToTheRight", sprites["icon_Flimsy_Right_Card_Mod"],
                "flimsy modification",
                "Modify the card to the right. When a card modified by this is played, discard this card."
            );
            RegisterGlossaryEntry(registry, "AFlimsyAllCardsToTheRight", sprites["icon_Flimsy_All_Right_Card_Mod"],
                "flimsy modification",
                "Modify all cards to the right. When a card modified by this is played, discard this card."
            );
            RegisterGlossaryEntry(registry, "AFlimsyNeighborCards", sprites["icon_Flimsy_Neighbors_Card_Mod"],
                "flimsy modification",
                "Modify both adjacent cards. When a card modified by this is played, discard this card."
            );

            RegisterGlossaryEntry(registry, "AAttackBuff", sprites["icon_attack_buff"],
                "attack buff",
                "Increases the power of attacks on the target card by {0}."
            );

            RegisterGlossaryEntry(registry, "SRedraw", sprites["icon_redraw"],
                "redraw",
                "Lets you discard a card and draw a new one. Costs 1 redraw per discard."
            );

            RegisterGlossaryEntry(registry, "ANoAction", sprites["icon_no_action"],
                "no action",
                "All effects of the target card are erased."
            );

            RegisterGlossaryEntry(registry, "CCardCentered", sprites["icon_card_is_centered"], // Condition CardCentered
                "card centered",
                "The following effects trigger if this card is at the center of your hand."
            );

            RegisterGlossaryEntry(registry, "CCardNotCentered", sprites["icon_card_is_not_centered"], // Condition CardNotCentered
                "card not centered",
                "The following effects trigger if this card is not at the center of your hand."
            );

            RegisterGlossaryEntry(registry, "MFlimsy", sprites["icon_flimsy"], // Meta Flimsy
                "flimsy",
                "This card is discarded when any card modified by it is played."
            );

            //CardTraitManager.RegisterExternalCardTrait(new CardTraitManager.ExternalCardTrait()
            //{
            //    sprite = (Spr)sprites["icon_flimsy"].Id,
            //    name = "Flimsy",
            //    testFunction = (Card card) => card is ModifierCard c && c.IsFlimsy()
            //});

            vanillaSpritesGlossary["AEnergyDiscount"] = new CustomTTGlossary(CustomTTGlossary.GlossaryType.cardtrait, Enum.Parse<Spr>("icons_discount"), "energy discount", "Discounts the energy cost of this card.", null);
            vanillaSpritesGlossary["ASetEnergy"] = new CustomTTGlossary(CustomTTGlossary.GlossaryType.cardtrait, Enum.Parse<Spr>("icons_energy"), "set energy cost", "Changes the energy cost of this card. Overrides all other effects.", null);
        }
        private void RegisterGlossaryEntry(IGlossaryRegisty registry, string itemName, ExternalSprite sprite, string displayName, string description)
        {
            var entry = new ExternalGlossary("clay.PhilipTheMechanic.Glossary", itemName, false, ExternalGlossary.GlossayType.action, sprite);
            entry.AddLocalisation("en", displayName, description);
            registry.RegisterGlossary(entry);
            glossary[entry.ItemName] = entry;
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
            var redraw = new ExternalStatus("clay.PhilipTheMechanic.Statuses.Redraw", true, System.Drawing.Color.Red, null, sprites["icon_redraw"], false);
            statusRegistry.RegisterStatus(redraw);
            redraw.AddLocalisation("Redraw", "Lets you discard a card and draw a new one. Costs 1 redraw per discard.");
            statuses["redraw"] = redraw;

            var customParts = new ExternalStatus("clay.PhilipTheMechanic.Statuses.CustomParts", true, System.Drawing.Color.Red, null, sprites["icon_customParts"], false);
            statusRegistry.RegisterStatus(customParts);
            customParts.AddLocalisation("Custom Parts", "Gives you {0} redraw at the start of each turn.");
            statuses["customParts"] = customParts;
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
            var wireClippers = new ExternalArtifact("clay.PhilipTheMechanic.Artifacts.WireClippers", typeof(WireClippers), sprites["artifact_wire_clippers"], ownerDeck: deck);
            wireClippers.AddLocalisation("WIRE CLIPPERS", "All unplayable cards become playable, and all trash gains exhaust");
            registry.RegisterArtifact(wireClippers);

            var sturdyPliers = new ExternalArtifact("clay.PhilipTheMechanic.Artifacts.AngleGrinder", typeof(AngleGrinder), sprites["artifact_angle_grinder"], ownerDeck: deck);
            sturdyPliers.AddLocalisation("ANGLE GRINDER", "Gain 3 redraw at the start of combat");
            registry.RegisterArtifact(sturdyPliers);

            var endlessToolbox = new ExternalArtifact("clay.PhilipTheMechanic.Artifacts.EndlessToolbox", typeof(EndlessToolbox), sprites["artifact_endless_toolbox"], ownerDeck: deck);
            endlessToolbox.AddLocalisation("ENDLESS TOOLBOX", "The first 2 times you redraw each turn, draw an extra card.");
            registry.RegisterArtifact(endlessToolbox);

            var selfPropellingCannons = new ExternalArtifact("clay.PhilipTheMechanic.Artifacts.SelfPropellingCannons", typeof(SelfPropellingCannons), sprites["artifact_self_propelling_cannons"], ownerDeck: deck);
            selfPropellingCannons.AddLocalisation("SELF PROPELLING CANNONS", "Your cannons gain brittle. At the end of each turn, launch a missile from each cannon");
            registry.RegisterArtifact(selfPropellingCannons);

            var hotChocolate = new ExternalArtifact("clay.PhilipTheMechanic.Artifacts.HotChocolate", typeof(HotChocolate), sprites["artifact_hot_chocolate"], ownerDeck: deck);
            hotChocolate.AddLocalisation("HOT CHOCOLATE", "If you have 3 or more unplayable mod cards in your hand, they redraw for free.");
            registry.RegisterArtifact(hotChocolate);

            // boss artifact
            var scrapMagnet = new ExternalArtifact("clay.PhilipTheMechanic.Artifacts.ScrapMagnet", typeof(ScrapMagnet), sprites["artifact_electromagnet"], ownerDeck: deck);
            scrapMagnet.AddLocalisation("SCRAP MAGNET", "Twice per turn, redraw the leftmost card in your hand for free.");
            registry.RegisterArtifact(scrapMagnet);
        }
    }
}
