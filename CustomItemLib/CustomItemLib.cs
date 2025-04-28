// MIT License

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
		// The names of the fields in here cannot be changed
		// They match to the members of ItemInstance
		public class Item {
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
			stats = NewPlayerStats(new PlayerStats { }),
			effectEvent = new(),
			keyDownEvent = new(),
			keyUpEvent = new()
		};

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

		public static PlayerStats NewPlayerStats(PlayerStats? stats) {
			var defaulted = new PlayerStats();

			if (stats is null) return defaulted;

			foreach (var field in stats.GetType().GetFields()) {
				var newVal = field.GetValue(stats) ?? new PlayerStat();

				TrySet(field.Name, ref defaulted, newVal);

				Debug.Log($"[CustomItemLib] Set player stat {field.Name}");
			}

			return defaulted;
		}

		[Obsolete("Please use the new `PlayerStats` overload instead")]
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
				maxHealth = maxHealth ?? new(),
				runSpeed = runSpeed ?? new(),
				airSpeed = airSpeed ?? new(),
				turnSpeed = turnSpeed ?? new(),
				drag = drag ?? new(),
				gravity = gravity ?? new(),
				fastFallSpeed = fastFallSpeed ?? new(),
				fastFallLerp = fastFallLerp ?? new(),
				lives = lives ?? new(),
				dashes = dashes ?? new(),
				boost = boost ?? new(),
				luck = luck ?? new(),
				startWithEnergyPercentage = startWithEnergyPercentage ?? new(),
				maxEnergy = maxEnergy ?? new(),
				itemPriceMultiplier = itemPriceMultiplier ?? new(),
				itemRarity = itemRarity ?? new(),
				sparkMultiplier = sparkMultiplier ?? new(),
				startingResource = startingResource ?? new(),
				energyGain = energyGain ?? new(),
				damageMultiplier = damageMultiplier ?? new(),
				sparkPickupRange = sparkPickupRange ?? new(),
				extraLevelSparks = extraLevelSparks ?? new(),
				extraLevelDifficulty = extraLevelDifficulty ?? new()
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
				SetAll(ref itemInstanceComponent, DefaultItem);
				return itemInstanceComponent;
			} catch (Exception) {
				UnityEngine.Object.Destroy(itemObject);
				throw;
			}
		}

		private static void UnlockItem(string name) {
			Debug.Log($"[CustomItemLib] Unlocking item {name}");
			FactSystem.SetFact(new Fact($"{name}_ShowItem"), 1.0f);
			FactSystem.SetFact(new Fact($"item_unlocked_{name}"), 1.0f);
		}

		public static void EditItemInDatabase(Item item) {
			// Special processing for PlayerStats since it can't have any null fields
			if (item.stats is not null) {
				item.stats = NewPlayerStats(item.stats);
			}

			ItemInstance? itemInstance = GetItemInstanceByItemName(item.itemName);

			if (itemInstance is null) {
				Debug.LogError($"[CustomItemLib] Item {item.itemName} not found");
				return;
			}

			Debug.Log($"[CustomItemLib] Editing item {item.itemName}");
			SetAll(ref itemInstance, item);
		}

		public static void AddItemToDatabase(Item item, bool autoUnlocked = false) {
			// Special processing for PlayerStats since it can't have any null fields
			if (item.stats is not null) {
				item.stats = NewPlayerStats(item.stats);
			}

			ItemInstance? itemInstance = GetItemInstanceByItemName(item.itemName);

			if (itemInstance is not null) {
				Debug.LogError($"[CustomItemLib] Item {item.itemName} already exists");
				return;
			}

			Debug.Log($"[CustomItemLib] Creating item {item.itemName}");
			itemInstance = CreateNewItemInstance(item.itemName);
			SetAll(ref itemInstance, item);

			if (autoUnlocked) {
				UnlockItem(item.itemName);
			}
		}

		[Obsolete("Please use the new `Item` overload instead")]
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
