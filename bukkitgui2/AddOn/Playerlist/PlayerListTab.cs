﻿// PlayerListTab.cs in bukkitgui2/bukkitgui2
// Created 2014/01/17
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file,
// you can obtain one at http://mozilla.org/MPL/2.0/.
// 
// ©Bertware, visit http://bertware.net

using System;
using System.Windows.Forms;
using MetroFramework.Controls;
using Net.Bertware.Bukkitgui2.Core.Logging;
using Net.Bertware.Bukkitgui2.MinecraftInterop.PlayerHandler;
using Net.Bertware.Bukkitgui2.Properties;

namespace Net.Bertware.Bukkitgui2.AddOn.PlayerList
{
    public partial class PlayerListTab : MetroUserControl, IAddonTab
    {
        public PlayerListTab()
        {
            InitializeComponent();

            SlvPlayers.Items.Clear();
            imageListPlayerFaces.Images.Clear();
            imageListPlayerFacesBig.Images.Clear();
            imageListPlayerFaces.Images.Add("default", Resources.player_face);
            imageListPlayerFacesBig.Images.Add("default", Resources.player_face);

            PlayerHandler.PlayerListAddition += HandlePlayerAddition;
            PlayerHandler.PlayerListDeletion += HandlePlayerDeletion;
        }

        /// <summary>
        ///     Remove a player from the listview
        /// </summary>
        /// <param name="player"></param>
        private void HandlePlayerDeletion(Player player)
        {
            if (player == null || player.Name == null) return;

            try
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker) (() => HandlePlayerDeletion(player)));
                }
                else
                {
                    foreach (ListViewItem item in SlvPlayers.Items)
                    {
                        if (item.Tag == player) item.Remove();
                    }
                    if (imageListPlayerFaces.Images.ContainsKey(player.Name))
                        imageListPlayerFaces.Images.RemoveByKey(player.Name);
                    if (imageListPlayerFacesBig.Images.ContainsKey(player.Name))
                        imageListPlayerFacesBig.Images.RemoveByKey(player.Name);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Severe, "PlayerListTab", "Failed to remove player: " + player.Name,
                    exception.Message);
            }
        }

        /// <summary>
        ///     Add a player to the listview
        /// </summary>
        /// <param name="player"></param>
        private void HandlePlayerAddition(Player player)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker) (() => HandlePlayerAddition(player)));
                }
                else
                {
                    string[] contents =
                    {
                        player.Name, player.DisplayName, player.Ip, player.JoinTime.ToShortTimeString(),
                        player.Location
                    };
                    ListViewItem item = new ListViewItem(contents) {Tag = player, ImageKey = "default"};
                    if (player.Minotar != null)
                    {
                        imageListPlayerFaces.Images.Add(player.Name, player.Minotar);
                        imageListPlayerFacesBig.Images.Add(player.Name, player.Minotar);
                        item.ImageKey = player.Name;
                    }

                    SlvPlayers.Items.Add(item);
                    player.DetailsLoaded += player_DetailsLoaded;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Severe, "PlayerListTab", "Failed to add player: " + player.Name, exception.Message);
            }
        }

        private void player_DetailsLoaded(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker) (() => player_DetailsLoaded(sender, e)));
                }
                else
                {
                    Player p = (Player) sender;

                    foreach (ListViewItem lvi in SlvPlayers.Items)
                    {
                        if (lvi.Tag.Equals(p))
                        {
                            lvi.SubItems[4].Text = p.Location;
                            imageListPlayerFaces.Images.Add(p.Name, p.Minotar);
                            lvi.ImageKey = p.Name;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Severe, "PlayerListTab", "Failed to update player", exception.Message);
            }
        }

        public IAddon ParentAddon { get; set; }

        private void ContextPlayersKick_Click(object sender, EventArgs e)
        {
            if (SlvPlayers.SelectedItems.Count < 1) return;
            ((Player) (SlvPlayers.SelectedItems[0].Tag)).Kick();
        }

        private void ContextPlayersBan_Click(object sender, EventArgs e)
        {
            if (SlvPlayers.SelectedItems.Count < 1) return;
            ((Player) (SlvPlayers.SelectedItems[0].Tag)).Ban();
        }

        private void ContextPlayersBanIp_Click(object sender, EventArgs e)
        {
            if (SlvPlayers.SelectedItems.Count < 1) return;
            ((Player) (SlvPlayers.SelectedItems[0].Tag)).BanIp();
        }

        private void ContextPlayersOp_Click(object sender, EventArgs e)
        {
            if (SlvPlayers.SelectedItems.Count < 1) return;
            ((Player) (SlvPlayers.SelectedItems[0].Tag)).SetOp(true);
        }

        private void ContextPlayersDeOp_Click(object sender, EventArgs e)
        {
            if (SlvPlayers.SelectedItems.Count < 1) return;
            ((Player) (SlvPlayers.SelectedItems[0].Tag)).SetOp(false);
        }
    }
}