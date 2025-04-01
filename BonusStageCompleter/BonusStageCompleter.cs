﻿using Il2Cpp;
using MelonLoader;
using UnityEngine;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;

namespace BonusStageCompleter;

public class BonusStageCompleter : MonoBehaviour
{
    private MapController _mapController;
    private BonusMapController _bonusController;
    private bool _isInBonusGame;
    private bool _notSkipEnabled;

    private void Awake()
    {
        _mapController = GameObject.Find("Map").GetComponent<MapController>();
        _bonusController = GameObject.Find("Bonus Map Controller").GetComponent<BonusMapController>();
    }

    private void LateUpdate()
    {
        #if DEBUG
        if (Input.GetKeyDown(KeyCode.P))
        {
            _mapController.ChangeMap(_mapController.CurrentBonusMap());
            _bonusController.spiritBoostEnabled = true;
        }
        #endif

        if (Plugin.Settings.EnableNotSkipAtSpiritBoost.Value && Input.GetKeyDown(Plugin.Settings.ToggleKey.Value))
            ToggleSkip("Not Skip At Spirit Boost", ref _notSkipEnabled, Plugin.Settings.ShowPopUpNotSkipAtSpiritBoost.Value);

        _isInBonusGame = _mapController.selectedMap.name.Contains("bonus");

        // only do logic in bonus stages
        if (!_isInBonusGame || !_bonusController.showCurrentTime) return;

        if (_notSkipEnabled && _bonusController.spiritBoostEnabled) return;

        // determine whether the collected spheres variable has already been set, so we don't overwrite it.
        var pickedUp = _bonusController.bonusSpheresPickedUp;
        var total = (int)_bonusController.currentSection.GetRequiredSpheres();

        if (pickedUp == total - 1 || pickedUp == total) return;
        _bonusController.bonusSpheresPickedUp = total - 1;
        Melon<Plugin>.Logger.Msg($"Set spheres picked up to: {_bonusController.bonusSpheresPickedUp}");
    }

    private static void ToggleSkip(string type, ref bool state, bool showPopup)
    {
        state = !state;
        Melon<Plugin>.Logger.Msg($"{type} is: {(state ? "ON" : "OFF")}");

        if (showPopup)
            Plugin.ModHelperInstance.ShowNotification(state ? $"{type} activated!" : $"{type} deactivated!", state);
    }
}