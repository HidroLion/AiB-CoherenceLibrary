﻿using System.IO;
using BreathLib.SerializableInterface;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BreathLib.SerializableInterface
{
    internal sealed class AssetDropdownItem : AdvancedDropdownItem, IDropdownItem
    {
        private readonly string path;

        /// <inheritdoc />
        public AssetDropdownItem(string path)
            : base(Path.GetFileNameWithoutExtension(path))
        {
            this.path = path;
            icon = IconUtility.GetIconForObject(path);
        }

        /// <inheritdoc />
        ReferenceMode IDropdownItem.Mode => ReferenceMode.Unity;

        /// <inheritdoc />
        object IDropdownItem.GetValue()
        {
            return AssetDatabase.LoadAssetAtPath<Object>(path);
        }
    }
}
