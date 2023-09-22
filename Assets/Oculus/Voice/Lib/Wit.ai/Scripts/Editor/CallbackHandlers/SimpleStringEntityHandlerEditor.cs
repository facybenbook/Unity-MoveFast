﻿/*
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
using System.Linq;
using Facebook.WitAi.Data.Configuration;
using UnityEditor;
using UnityEngine;


namespace Facebook.WitAi.CallbackHandlers
{
    [CustomEditor(typeof(SimpleStringEntityHandler))]
    public class SimpleStringEntityHandlerEditor : Editor
    {
        private SimpleStringEntityHandler handler;
        private string[] intentNames;
        private int intentIndex;
        private string[] entityNames;
        private int entityIndex;

        private void OnEnable()
        {
            handler = target as SimpleStringEntityHandler;
            if (handler && handler.wit && null == intentNames)
            {
                if (handler.wit is IWitRuntimeConfigProvider provider &&
                    null != provider.RuntimeConfiguration &&
                    provider.RuntimeConfiguration.witConfiguration)
                {
                    provider.RuntimeConfiguration.witConfiguration.RefreshData();
                    intentNames = provider.RuntimeConfiguration.witConfiguration.intents.Select(i => i.name).ToArray();
                    intentIndex = Array.IndexOf(intentNames, handler.intent);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            var handler = target as SimpleStringEntityHandler;
            if (!handler) return;
            if (!handler.wit)
            {
                GUILayout.Label("Wit component is not present in the scene. Add wit to scene to get intent and entity suggestions.", EditorStyles.helpBox);
            }

            var intentChanged = WitEditorUI.LayoutSerializedObjectPopup(serializedObject,"intent", intentNames, ref intentIndex);
            if (intentChanged ||
                null != intentNames && intentNames.Length > 0 && null == entityNames)
            {
                if (handler && handler.wit && null == intentNames)
                {
                    if (handler.wit is IWitRuntimeConfigProvider provider &&
                        null != provider.RuntimeConfiguration &&
                        provider.RuntimeConfiguration.witConfiguration)
                    {
                        var entities = provider.RuntimeConfiguration.witConfiguration.intents[intentIndex]?.entities;
                        if (null != entities)
                        {
                            entityNames = entities.Select((e) => e.name).ToArray();
                            entityIndex = Array.IndexOf(entityNames, handler.entity);
                        }
                    }
                }
            }

            WitEditorUI.LayoutSerializedObjectPopup(serializedObject, "entity", entityNames, ref entityIndex);

            var confidenceProperty = serializedObject.FindProperty("confidence");
            EditorGUILayout.PropertyField(confidenceProperty);

            EditorGUILayout.Space(16);
            var formatProperty = serializedObject.FindProperty("format");
            EditorGUILayout.PropertyField(formatProperty);

            GUILayout.Space(16);

            var eventProperty = serializedObject.FindProperty("onIntentEntityTriggered");
            EditorGUILayout.PropertyField(eventProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
