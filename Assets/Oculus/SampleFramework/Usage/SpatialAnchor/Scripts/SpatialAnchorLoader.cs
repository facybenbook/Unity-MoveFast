/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using UnityEngine;

/// <summary>
/// Demonstrates loading existing spatial anchors from storage.
/// </summary>
/// <remarks>
/// Loading existing anchors involves two asynchronous methods:
/// 1. Call <see cref="OVRSpatialAnchor.LoadUnboundAnchors"/>
/// 2. For each unbound anchor you wish to localize, invoke <see cref="OVRSpatialAnchor.UnboundAnchor.Localize"/>.
/// 3. Once localized, your callback will receive an <see cref="OVRSpatialAnchor.UnboundAnchor"/>. Instantiate an
/// <see cref="OVRSpatialAnchor"/> component and bind it to the `UnboundAnchor` by calling
/// <see cref="OVRSpatialAnchor.UnboundAnchor.BindTo"/>.
/// </remarks>
public class SpatialAnchorLoader : MonoBehaviour
{
    [SerializeField]
    OVRSpatialAnchor _anchorPrefab;

    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;

    public void LoadAnchorsByUuid()
    {
        // Get number of saved anchor uuids
        if (!PlayerPrefs.HasKey(Anchor.NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(Anchor.NumUuidsPlayerPref, 0);
        }

        var playerUuidCount = PlayerPrefs.GetInt("numUuids");
        Log($"Attempting to load {playerUuidCount} saved anchors.");
        if (playerUuidCount == 0)
            return;

        var uuids = new Guid[playerUuidCount];
        for (int i = 0; i < playerUuidCount; ++i)
        {
            var uuidKey = "uuid" + i;
            var currentUuid = PlayerPrefs.GetString(uuidKey);
            Log("QueryAnchorByUuid: " + currentUuid);

            uuids[i] = new Guid(currentUuid);
        }

        Load(new OVRSpatialAnchor.LoadOptions
        {
            MaxAnchorCount = 100,
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = uuids
        });
    }

    private void Awake()
    {
        _onLoadAnchor = OnLocalized;
    }

    private void Load(OVRSpatialAnchor.LoadOptions options) => OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
    {
        if (anchors == null)
        {
            Log("Query failed.");
            return;
        }

        foreach (var anchor in anchors)
        {
            if (anchor.Localized)
            {
                _onLoadAnchor(anchor, true);
            }
            else if (!anchor.Localizing)
            {
                anchor.Localize(_onLoadAnchor);
            }
        }
    });

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success)
        {
            Log($"{unboundAnchor} Localization failed!");
            return;
        }

        var pose = unboundAnchor.Pose;
        var spatialAnchor = Instantiate(_anchorPrefab, pose.position, pose.rotation);
        unboundAnchor.BindTo(spatialAnchor);

        if (spatialAnchor.TryGetComponent<Anchor>(out var anchor))
        {
            // We just loaded it, so we know it exists in persistent storage.
            anchor.ShowSaveIcon = true;
        }
    }

    private static void Log(string message) => Debug.Log($"[SpatialAnchorsUnity]: {message}");
}
