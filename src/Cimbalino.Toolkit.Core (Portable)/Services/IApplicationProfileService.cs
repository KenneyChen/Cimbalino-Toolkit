﻿// ****************************************************************************
// <copyright file="IApplicationProfileService.cs" company="Pedro Lamas">
// Copyright © Pedro Lamas 2014
// </copyright>
// ****************************************************************************
// <author>Pedro Lamas</author>
// <email>pedrolamas@gmail.com</email>
// <project>Cimbalino.Toolkit.Core</project>
// <web>http://www.pedrolamas.com</web>
// <license>
// See license.txt in this solution or http://www.pedrolamas.com/license_MIT.txt
// </license>
// ****************************************************************************

namespace Cimbalino.Toolkit.Services
{
    /// <summary>
    /// Represents a service capable of providing profile information about an app.
    /// </summary>
    public interface IApplicationProfileService
    {
        /// <summary>
        /// Gets a value that indicates the mode that an app is running in.
        /// </summary>
        /// <value>A value that indicates the mode that an app is running in.</value>
        ApplicationProfileServiceMode Mode { get; }
    }
}