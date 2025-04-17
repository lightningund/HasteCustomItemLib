﻿// MIT License

// Copyright (c) 2025 Stevelion

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using Landfall.Haste;

namespace CustomItemLib {
	public class ItemFactory {
		public class Item {
			public bool autoUnlocked = false;
			public string itemName;
			public bool? minorItem;
			public Rarity? rarity;
			public List<ItemTag>? itemTags;
			public LocalizedString? title;
			public bool? usesTriggerDescription;
			public LocalizedString? triggerDescription;
			public bool? usesEffectDescription;
			public LocalizedString? description;
			public LocalizedString? flavorText;
			public float? iconScaleModifier;
			public bool? statsAfterTrigger;
			public float? cooldown;
			public ItemTriggerType? triggerType;
			public List<ItemTrigger>? triggerConditions;
			public List<ItemEffect>? effects;
			public PlayerStats? stats;
			public UnityEvent? effectEvent;
			public UnityEvent? keyDownEvent;
			public UnityEvent? keyUpEvent;
		};

		private static readonly Item DefaultItem = new() {
			autoUnlocked = false,
			itemName = "<name>",
			minorItem = false,
			rarity = Rarity.Common,
			itemTags = [],
			title = new UnlocalizedString("<Title>"),
			usesTriggerDescription = false,
			triggerDescription = new UnlocalizedString("<Trigger Description>"),
			usesEffectDescription = false,
			description = new UnlocalizedString("<Item Description>"),
			flavorText = new UnlocalizedString("<Flavor Text>"),
			iconScaleModifier = 1f,
			statsAfterTrigger = false,
			cooldown = 0f,
			triggerType = ItemTriggerType.None,
			triggerConditions = [],
			effects = [],
			stats = NewPlayerStats(),
			effectEvent = new(),
			keyDownEvent = new(),
			keyUpEvent = new()
		};

		static ItemFactory() {
			On.ItemDatabase.Awake += (orig, self) => {
				orig(self);
				LoadCustomItems();
			};
		}

		public static event Action ItemsLoaded = null!;

		private static void LoadCustomItems() {
			Debug.Log($"[CustomItemLib] Loading custom items");
			if (ItemsLoaded is not null) {
				try {
					ItemsLoaded();
				} catch (Exception e) {
					Debug.LogException(e);
				}
			}
		}

		public static PlayerStats NewPlayerStats(
			PlayerStat? maxHealth = null,
			PlayerStat? runSpeed = null,
			PlayerStat? airSpeed = null,
			PlayerStat? turnSpeed = null,
			PlayerStat? drag = null,
			PlayerStat? gravity = null,
			PlayerStat? fastFallSpeed = null,
			PlayerStat? fastFallLerp = null,
			PlayerStat? lives = null,
			PlayerStat? dashes = null,
			PlayerStat? boost = null,
			PlayerStat? luck = null,
			PlayerStat? startWithEnergyPercentage = null,
			PlayerStat? maxEnergy = null,
			PlayerStat? itemPriceMultiplier = null,
			PlayerStat? itemRarity = null,
			PlayerStat? sparkMultiplier = null,
			PlayerStat? startingResource = null,
			PlayerStat? energyGain = null,
			PlayerStat? damageMultiplier = null,
			PlayerStat? sparkPickupRange = null,
			PlayerStat? extraLevelSparks = null,
			PlayerStat? extraLevelDifficulty = null
		) {
			var playerStats = new PlayerStats {
				maxHealth = maxHealth ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				runSpeed = runSpeed ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				airSpeed = airSpeed ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				turnSpeed = turnSpeed ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				drag = drag ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				gravity = gravity ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				fastFallSpeed = fastFallSpeed ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				fastFallLerp = fastFallLerp ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				lives = lives ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				dashes = dashes ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				boost = boost ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				luck = luck ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				startWithEnergyPercentage = startWithEnergyPercentage ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				maxEnergy = maxEnergy ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				itemPriceMultiplier = itemPriceMultiplier ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				itemRarity = itemRarity ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				sparkMultiplier = sparkMultiplier ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				startingResource = startingResource ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				energyGain = energyGain ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				damageMultiplier = damageMultiplier ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				sparkPickupRange = sparkPickupRange ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				extraLevelSparks = extraLevelSparks ?? new PlayerStat { baseValue = 0f, multiplier = 1f },
				extraLevelDifficulty = extraLevelDifficulty ?? new PlayerStat { baseValue = 0f, multiplier = 1f }
			};

			return playerStats;
		}

		private static ItemInstance? GetItemInstanceByItemName(string itemName) { // Can return null if item does not exist
			foreach (ItemInstance itemObject in ItemDatabase.instance.items) {
				ItemInstance itemInstanceComponent;

				if (
					string.Equals(itemName, itemObject.name, StringComparison.InvariantCultureIgnoreCase)
					&& itemObject.TryGetComponent(out itemInstanceComponent)
				) {
					return itemInstanceComponent;
				}
			}

			return null;
		}

		private static ItemInstance CreateNewItemInstance(string itemName) {
			GameObject itemObject = new(itemName);

			try {
				itemObject.SetActive(false);
				ItemInstance itemInstanceComponent = itemObject.AddComponent<ItemInstance>();
				ItemDatabase.instance.items.Add(itemInstanceComponent);
				UnityEngine.Object.DontDestroyOnLoad(itemObject);
				return itemInstanceComponent;
			} catch (Exception) {
				UnityEngine.Object.Destroy(itemObject);
				throw;
			}
		}

		/**
		 * Tries to get the named member as a field first and then as a property
		 */
		private static object? TryGet<T>(string name, T obj) {
			try {
				return obj.GetType().GetField(name).GetValue(obj);
			} catch (Exception) { }

			try {
				return obj.GetType().GetProperty(name).GetValue(obj);
			} catch (Exception) { }

			return null;
		}

		/**
		 * Tries to set the named member as a field first and then as a property
		 */
		private static void TrySet<T, U>(string name, ref T obj, U val) {
			try {
				obj.GetType().GetField(name).SetValue(obj, val);
				return;
			} catch (Exception) { }

			try {
				obj.GetType().GetProperty(name).SetValue(obj, val);
				return;
			} catch (Exception) { }
		}

		private static void SetAll(ref ItemInstance instance, Item item) {
			foreach (var field in item.GetType().GetFields()) {
				var newVal = field.GetValue(item);
				if (newVal is null) continue;

				TrySet(field.Name, ref instance, newVal);

				Debug.Log($"[CustomItemLib] Set item stat {field.Name}");
			}
		}

		public static void AddItemToDatabase(Item item) {
			ItemInstance? itemInstance = GetItemInstanceByItemName(item.itemName);

			if (itemInstance is not null) {
				Debug.Log($"[CustomItemLib] Replacing item {item.itemName}");

				SetAll(ref itemInstance, item);
			} else {
				Debug.Log($"[CustomItemLib] Creating new item {item.itemName}");

				itemInstance = CreateNewItemInstance(item.itemName);
			}
		}

		public static void AddItemToDatabase(
			string itemName,
			bool autoUnlocked = true,
			bool? minorItem = null,
			Rarity? rarity = null,
			List<ItemTag>? itemTags = null,
			LocalizedString? title = null,
			bool? usesTriggerDescription = null,
			LocalizedString? triggerDescription = null,
			bool? usesEffectDescription = null,
			LocalizedString? description = null,
			LocalizedString? flavorText = null,
			float? iconScaleModifier = null,
			bool? statsAfterTrigger = null,
			float? cooldown = null,
			ItemTriggerType? triggerType = null,
			List<ItemTrigger>? triggerConditions = null,
			List<ItemEffect>? effects = null,
			PlayerStats? stats = null,
			UnityEvent? effectEvent = null,
			UnityEvent? keyDownEvent = null,
			UnityEvent? keyUpEvent = null
		) {
			ItemInstance? itemInstance = GetItemInstanceByItemName(itemName);

			if (itemInstance is not null) {
				Debug.Log($"[CustomItemLib] Replacing item {itemName}");

				if (minorItem is not null) itemInstance.minorItem = (bool)minorItem;
				if (rarity is not null) itemInstance.rarity = (Rarity)rarity;
				if (itemTags is not null) itemInstance.itemTags = itemTags;
				if (title is not null) itemInstance.title = title;
				if (usesTriggerDescription is not null) itemInstance.usesTriggerDescription = (bool)usesTriggerDescription;
				if (triggerDescription is not null) itemInstance.triggerDescription = triggerDescription;
				if (usesEffectDescription is not null) itemInstance.usesEffectDescription = (bool)usesEffectDescription;
				if (description is not null) itemInstance.description = description;
				if (flavorText is not null) itemInstance.flavorText = flavorText;
				if (iconScaleModifier is not null) itemInstance.iconScaleModifier = (float)iconScaleModifier;
				if (statsAfterTrigger is not null) itemInstance.statsAfterTrigger = (bool)statsAfterTrigger;
				if (cooldown is not null) itemInstance.cooldown = (float)cooldown;
				if (triggerType is not null) itemInstance.triggerType = (ItemTriggerType)triggerType;
				if (triggerConditions is not null) itemInstance.triggerConditions = triggerConditions;
				if (effects is not null) itemInstance.effects = effects;
				if (stats is not null) itemInstance.stats = stats;
				if (effectEvent is not null) itemInstance.effectEvent = effectEvent;
				if (keyDownEvent is not null) itemInstance.keyDownEvent = keyDownEvent;
				if (keyUpEvent is not null) itemInstance.keyUpEvent = keyUpEvent;
			} else {
				Debug.Log($"[CustomItemLib] Creating new item {itemName}");

				itemInstance = CreateNewItemInstance(itemName);
				itemInstance.minorItem = minorItem ?? false;
				itemInstance.rarity = rarity ?? Rarity.Common;
				itemInstance.itemTags = itemTags ?? [];
				itemInstance.title = title ?? new UnlocalizedString("<Title>");
				itemInstance.usesTriggerDescription = usesTriggerDescription ?? false;
				itemInstance.triggerDescription = triggerDescription ?? new UnlocalizedString("<Trigger Description>");
				itemInstance.usesEffectDescription = usesEffectDescription ?? false;
				itemInstance.description = description ?? new UnlocalizedString("<Item Description>");
				itemInstance.flavorText = flavorText ?? new UnlocalizedString("<Flavor Text>");
				itemInstance.iconScaleModifier = iconScaleModifier ?? 1f;
				itemInstance.statsAfterTrigger = statsAfterTrigger ?? false;
				itemInstance.cooldown = cooldown ?? 0f;
				itemInstance.triggerType = triggerType ?? ItemTriggerType.None;
				itemInstance.triggerConditions = triggerConditions ?? [];
				itemInstance.effects = effects ?? [];
				itemInstance.stats = stats ?? NewPlayerStats();
				itemInstance.effectEvent = effectEvent ?? new UnityEvent();
				itemInstance.keyDownEvent = keyDownEvent ?? new UnityEvent();
				itemInstance.keyUpEvent = keyUpEvent ?? new UnityEvent();
			}

			if (autoUnlocked) {
				Debug.Log($"[CustomItemLib] Unlocking item {itemName}");
				FactSystem.SetFact(new Fact($"{itemName}_ShowItem"), 1.0f);
				FactSystem.SetFact(new Fact($"item_unlocked_{itemName}"), 1.0f);
			}
		}
	}
}
