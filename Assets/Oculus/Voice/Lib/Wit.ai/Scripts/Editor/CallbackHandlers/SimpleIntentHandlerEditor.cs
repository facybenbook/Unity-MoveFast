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
using System.Reflection;
using Facebook.WitAi.Data.Configuration;
using Facebook.WitAi.Windows;
using UnityEditor;
using UnityEngine;

namespace Facebook.WitAi.CallbackHandlers
{
    [CustomEditor(typeof(SimpleIntentHandler))]
    public class SimpleIntentHandlerEditor : Editor
    {
        private SimpleIntentHandler _handler;
        private string[] _intentNames;
        private int _intentIndex;

        private FieldGUI _fieldGUI;

        private void OnEnable()
        {
            _handler = target as SimpleIntentHandler;

            // Setup field gui
            if (_fieldGUI == null)
            {
                _fieldGUI = new FieldGUI();
                _fieldGUI.onCustomGuiLayout = OnInspectorCustomGUI;
                _fieldGUI.onAdditionalGuiLayout = OnInspectorAdditionalGUI;
            }
        }

        public override void OnInspectorGUI()
        {
            if (!_handler.wit)
            {
                GUILayout.Label(
                    "Wit component is not present in the scene. Add wit to scene to get intent and entity suggestions.",
                    EditorStyles.helpBox);
            }

            if (_handler && _handler.wit && null == _intentNames)
            {
                if (_handler.wit is IWitRuntimeConfigProvider provider
                    && null != provider.RuntimeConfiguration
                    && provider.RuntimeConfiguration.witConfiguration)
                {
                    provider.RuntimeConfiguration.witConfiguration.RefreshData();
                    _intentNames = provider.RuntimeConfiguration.witConfiguration.intents.Select(i => i.name).ToArray();
                    _intentIndex = Array.IndexOf(_intentNames, _handler.intent);
                }
            }

            // Layout fields
            _fieldGUI.OnGuiLayout(serializedObject);
        }
        // Custom GUI
        private bool OnInspectorCustomGUI(FieldInfo fieldInfo)
        {
            // Custom layout
            if (string.Equals(fieldInfo.Name, "intent"))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Intent", EditorStyles.boldLabel);
                WitEditorUI.LayoutSerializedObjectPopup(serializedObject, "intent",
                    _intentNames, ref _intentIndex);
                return true;
            }
            // Layout intent triggered
            return false;
        }
        // Additional GUI
        private void OnInspectorAdditionalGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            var eventProperty = serializedObject.FindProperty("onIntentTriggered");
            EditorGUILayout.PropertyField(eventProperty);
        }
    }
}
