﻿// EnumMessageType.cs in bukkitgui2/bukkitgui2
// Created 2014/02/02
// 
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file,
// you can obtain one at http://mozilla.org/MPL/2.0/.
// 
// ©Bertware, visit http://bertware.net

namespace Net.Bertware.Bukkitgui2.MinecraftInterop.OutputHandler
{
    /// <summary>
    ///     The type of output from a minecraft server
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///     Informative
        /// </summary>
        Info = 10,

        /// <summary>
        ///     Warning
        /// </summary>
        Warning = 11,

        /// <summary>
        ///     Severe/Error
        /// </summary>
        Severe = 12,

        /// <summary>
        ///     Player joined
        /// </summary>
        PlayerJoin = 20,

        /// <summary>
        ///     Player left or disconnected
        /// </summary>
        PlayerLeave = 21,

        /// <summary>
        ///     Player kicked
        /// </summary>
        PlayerKick = 22,

        /// <summary>
        ///     Player banned
        /// </summary>
        PlayerBan = 23,

        /// <summary>
        ///     Ip banned
        /// </summary>
        PlayerIpBan = 24,

        /// <summary>
        ///     Java stacktrace
        /// </summary>
        JavaStackTrace = 30,

        /// <summary>
        ///     Java VM messages
        /// </summary>
        JavaStatus = 31,

        /// <summary>
        ///     Output from /list command
        /// </summary>
        PlayerList = 40,

        /// <summary>
        ///     Uknown text
        /// </summary>
        Unknown = 0
    }
}