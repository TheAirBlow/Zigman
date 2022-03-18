// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace TheAirBlow.Zigman.Legacy;

/// <summary>
/// Manage your assets from any thread
/// </summary>
public partial class Trojan
{
    /// <summary>
    /// An asset
    /// </summary>
    private class Asset
    {
        /// <summary>
        /// Content of the asset
        /// </summary>
        public object Content;
        
        /// <summary>
        /// Type of the asset
        /// </summary>
        public Type Type;
    }

    /// <summary>
    /// Assets database
    /// </summary>
    private volatile Dictionary<string, Asset> _assets = new();

    /// <summary>
    /// Add an asset into the database
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="asset">Asset</param>
    /// <typeparam name="T">Asset's type</typeparam>
    /// <exception cref="ArgumentNullException">Asset is null -or- ID is null or empty</exception>
    /// <exception cref="InvalidOperationException">There is already an asset with that ID</exception>
    public void AddAsset<T>(string id, T asset)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset));
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));
        lock (_assets) {
            if (_assets.ContainsKey(id))
                throw new InvalidOperationException("There is already an asset with that ID!");
            _assets.Add(id, new Asset { Content = asset, Type = typeof(T) });
        }
    }
    
    /// <summary>
    /// Get an asset
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <typeparam name="T">Asset's type</typeparam>
    /// <returns>Asset</returns>
    /// <exception cref="ArgumentException">ID is null or empty</exception>
    /// <exception cref="InvalidOperationException">There is no asset with that ID</exception>
    /// <exception cref="InvalidCastException">Requested a different type that the asset's type</exception>
    public T GetAsset<T>(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID is null or empty!");
        lock (_assets) {
            if (!_assets.ContainsKey(id))
                throw new InvalidOperationException("There is no asset with that ID!");
            if (_assets[id].Type != typeof(T))
                throw new InvalidCastException("Requested a different type that the asset's type!");
            return (T)_assets[id].Content;
        }
    }

    /// <summary>
    /// Remove an asset
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <exception cref="ArgumentException">ID is null or empty</exception>
    /// <exception cref="InvalidOperationException">There is no asset with that ID</exception>
    public void RemoveAsset(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID is null or empty!");
        lock (_assets) {
            if (!_assets.ContainsKey(id))
                throw new InvalidOperationException("There is no asset with that ID!");
            _assets.Remove(id);
        }
    }
    
    /// <summary>
    /// Clears all assets
    /// </summary>
    public void ClearAssets()
    {
        lock (_assets) 
            _assets.Clear();
    }
}