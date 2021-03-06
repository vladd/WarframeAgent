﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Core;
using Core.Model;
using Core.Model.Filter;
using Core.ViewModel;
using Core.Events;

using Agent.ViewModel.Util;

namespace Agent.ViewModel
{
    class ItemsEngine : GenericEngineWithUpdates<ItemGroupViewModel, Core.Model.Filter.Item>, IItemStore
    {
        public ItemsEngine(FiltersEvent filtersEvent) : base(filtersEvent) { }

        public void Run(GameModel model, UserNotificationsEngine notificationEngine)
        {
            this.notificationEngine = notificationEngine;
            Run(model);
        }

        BatchedObservableCollection<ItemGroupViewModel> enabledItems = new BatchedObservableCollection<ItemGroupViewModel>();
        public ObservableCollection<ItemGroupViewModel> EnabledItems => enabledItems;

        UserNotificationsEngine notificationEngine;

        protected override ItemGroupViewModel CreateItem(Core.Model.Filter.Item item, FiltersEvent evt) => throw new NotSupportedException();
        protected override IEnumerable<Core.Model.Filter.Item> GetItemsFromModel(GameModel model) => model.GetCurrentItems();
    
        protected override void Subscribe(GameModel model)
        {
            model.ItemNotificationArrived += AddEvent;
            model.ItemNotificationChanged += ChangeEvent;
            model.ItemNotificationDeparted += RemoveEvent;
        }

        Dictionary<string, ItemGroupViewModel> groupVMs = new Dictionary<string, ItemGroupViewModel>();
        Dictionary<string, ItemGroupViewModel> enabledGroupVMs = new Dictionary<string, ItemGroupViewModel>();
        Dictionary<string, ExtendedItemViewModel> allItemsStore = new Dictionary<string, ExtendedItemViewModel>();

        static bool EnabledFilter(ExtendedItemViewModel itemVM) => itemVM.Enabled;

        protected override void AddEventImpl(IReadOnlyCollection<Core.Model.Filter.Item> newItems)
        {
            LogAdded(newItems);

            var newItemVMs = newItems.Select(ActivateItemExt);
            EmplaceItems(newItemVMs);
        }

        void EmplaceItems(IEnumerable<ExtendedItemViewModel> newItemVMs)
        {
            foreach (var group in newItemVMs.GroupBy(it => it.Type))
            {
                foreach (var itemVM in group)
                    itemVM.Update();
                if (group.Key == null)
                    continue;
                if (!groupVMs.TryGetValue(group.Key, out var itemGroup))
                {
                    itemGroup = new ItemGroupViewModel(group.Key);
                    Items.Add(itemGroup);
                    groupVMs[group.Key] = itemGroup;
                }
                itemGroup.AddRange(group);
                var enabledGroup = group.Where(EnabledFilter).ToList();
                if (enabledGroup.Count > 0)
                {
                    if (!enabledGroupVMs.TryGetValue(group.Key, out var enabledItemGroup))
                    {
                        enabledItemGroup = new ItemGroupViewModel(group.Key);
                        EnabledItems.Add(enabledItemGroup);
                        enabledGroupVMs[group.Key] = enabledItemGroup;
                    }
                    enabledItemGroup.AddRange(enabledGroup);
                }
            }
        }

        ExtendedItemViewModel ActivateItemExt(Core.Model.Filter.Item item)
        {
            var itemVM = FetchItemById(item, item.Id);

            foreach (var (target, state) in itemVM.NotificationState)
                state.PropertyChanged += (o, args) => notificationEngine.OnSubscriptionChanged(itemVM, target);

            return itemVM;
        }

        ExtendedItemViewModel FetchItemById(Core.Model.Filter.Item item, string id)
        {
            if (!allItemsStore.TryGetValue(id, out var itemVM))
            {
                var notificationState = notificationEngine.GetNotificationState(id);
                if (item != null)
                    itemVM = new ExtendedItemViewModel(item, notificationState);
                else
                    itemVM = new ExtendedItemViewModel(id, notificationState);
                allItemsStore[id] = itemVM;
            }
            return itemVM;
        }

        protected override void ChangeEventImpl(IReadOnlyCollection<Item> changedItems)
        {
            LogChanged(changedItems);
            var migrants = new List<ExtendedItemViewModel>();
            foreach (var item in changedItems)
            {
                var itemVM = FetchItemById(item, item.Id);
                if (itemVM != null)
                {
                    var oldGroupKey = itemVM.Type;
                    var oldEnabledState = itemVM.Enabled;
                    itemVM.Update();
                    if (oldGroupKey != itemVM.Type || oldEnabledState != itemVM.Enabled)
                        migrants.Add(itemVM);
                }
            }
            // удалить мигрантов из старых групп
            foreach (var itemGroup in groupVMs.Values)
                itemGroup.RemoveRange(migrants);
            foreach (var enabledItemGroup in enabledGroupVMs.Values)
                enabledItemGroup.RemoveRange(migrants);

            // поместить мигрантов в новые группы
            EmplaceItems(migrants);

            CleanupEmptyGroups();
        }

        protected override void RemoveEventImpl(IReadOnlyCollection<Item> removedItems)
        {
            LogRemoved(removedItems);
            foreach (var group in removedItems.GroupBy(i => i.Type))
            {
                if (groupVMs.TryGetValue(group.Key, out var itemGroup))
                    itemGroup.RemoveRangeByModel(group);
                if (enabledGroupVMs.TryGetValue(group.Key, out var enabledItemGroup))
                    enabledItemGroup.RemoveRangeByModel(group);
            }

            CleanupEmptyGroups();
        }

        void CleanupEmptyGroups()
        {
            foreach (var (key, groupVM) in groupVMs.ToList())
            {
                if (groupVM.Items.Count != 0)
                    continue;
                Items.Remove(groupVM);
                groupVMs.Remove(key);
            }

            foreach (var (key, enabledGroupVM) in enabledGroupVMs.ToList())
            {
                if (enabledGroupVM.Items.Count != 0)
                    continue;
                EnabledItems.Remove(enabledGroupVM);
                enabledGroupVMs.Remove(key);
            }
        }

        protected override string LogAddedOne(Core.Model.Filter.Item item) => $"Новый предмет {item.Id}!";
        protected override string LogChangedOne(Core.Model.Filter.Item item) => $"Изменённый предмет {item.Id}!";
        protected override string LogRemovedOne(Core.Model.Filter.Item item) => $"Удаляю предмет {item.Id}!";
        protected override string LogAddedMany(int n) => $"Новые предметы ({n} шт.)";
        protected override string LogChangedMany(int n) => $"Изменённые предметы ({n} шт.)";
        protected override string LogRemovedMany(int n) => $"Удаляю предметы ({n} шт.)";

        protected override ItemGroupViewModel TryGetItemByModel(Core.Model.Filter.Item item) =>
            throw new NotSupportedException();

        protected ExtendedItemViewModel TryGetItemByModelExt(Core.Model.Filter.Item item) =>
            groupVMs.TryGetValue(item.Type, out var itemGroup) ? itemGroup.TryGetItem(item) : null;

        ItemViewModel IItemStore.GetItemById(string id) => FetchItemById(null, id);
    }
}
