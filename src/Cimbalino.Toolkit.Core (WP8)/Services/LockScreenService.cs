﻿// ****************************************************************************
// <copyright file="LockScreenService.cs" company="Pedro Lamas">
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

#if WINDOWS_PHONE || WINDOWS_PHONE_81
using System;
using System.IO;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Helpers;
using Windows.Phone.System.UserProfile;
#elif WINDOWS_PHONE_APP
using System;
using System.IO;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Helpers;
#else
using System;
using System.IO;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Helpers;
using Windows.System.UserProfile;
#endif

namespace Cimbalino.Toolkit.Services
{
    /// <summary>
    /// Represents an implementation of the <see cref="ILockScreenService"/>.
    /// </summary>
    public class LockScreenService : ILockScreenService
    {
#if WINDOWS_PHONE || WINDOWS_PHONE_81
        private const string LockScreenImageUrlNormal = "ms-appdata:///Local/shared/shellcontent/MBWallpaper.png";
        private const string LockScreenImageUrlAlternative = "ms-appdata:///Local/shared/shellcontent/MBWallpaper2.png";
        private const string LockScreenFileNormal = "shared\\shellcontent\\MBWallpaper.png";
        private const string LockScreenFileAlternative = "shared\\shellcontent\\MBWallpaper2.png";

        private Uri ImageUri
        {
            get
            {
                return LockScreen.GetImageUri();
            }
            set
            {
                LockScreen.SetImageUri(value);
            }
        }

        private string LockScreenImageUrl
        {
            get
            {
                return ImageUri.ToString().EndsWith("2.png") ? LockScreenImageUrlNormal : LockScreenImageUrlAlternative;
            }
        }

        private string LockScreenFile
        {
            get
            {
                return ImageUri.ToString().EndsWith("2.png") ? LockScreenFileNormal : LockScreenFileAlternative;
            }
        }
#endif

        /// <summary>
        /// Sets the lock screen background image.
        /// </summary>
        /// <param name="uri">The file URI.</param>
        /// <returns>The <see cref="Task"/> object representing the asynchronous operation.</returns>
        public virtual Task SetLockScreenAsync(Uri uri)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the lock screen background image.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <returns>The <see cref="Task"/> object representing the asynchronous operation.</returns>
#if WINDOWS_PHONE || WINDOWS_PHONE_81
        public virtual async Task SetLockScreenAsync(Stream stream)
        {
            using (var fileStream = await StorageService.LocalStorageServiceHandlerStatic.CreateFileAsync(LockScreenFile).ConfigureAwait(false))
            {
                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            }

            ImageUri = new Uri(LockScreenImageUrl, UriKind.RelativeOrAbsolute);
        }
#elif WINDOWS_APP
        public virtual async Task SetLockScreenAsync(Stream stream)
        {
            await LockScreen.SetImageStreamAsync(stream.AsRandomAccessStream());
        }
#elif WINDOWS_UWP
        public virtual async Task SetLockScreenAsync(Stream stream)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.UserProfile.LockScreen"))
            {
                await LockScreen.SetImageStreamAsync(stream.AsRandomAccessStream());
            }
        }
#else
        public virtual Task SetLockScreenAsync(Stream stream)
        {
            return ExceptionHelper.ThrowNotSupported<Task>();
        }
#endif

        /// <summary>
        /// Gets the current lock screen background image URI.
        /// </summary>
        /// <returns>The file URI.</returns>
        public virtual Uri GetCurrentLockScreenUri()
        {
#if WINDOWS_PHONE || WINDOWS_PHONE_81
            return new Uri(LockScreenImageUrl, UriKind.Absolute);
#elif WINDOWS_APP
            return LockScreen.OriginalImageFile;
#elif WINDOWS_UWP
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.UserProfile.LockScreen"))
            {
                return LockScreen.OriginalImageFile;
            }

            return null;
#else
            return ExceptionHelper.ThrowNotSupported<Uri>();
#endif
        }

        /// <summary>
        /// Sets the current app as the lock screen background image provider.
        /// </summary>
        /// <returns>The <see cref="Task"/> object representing the asynchronous operation.</returns>
        public virtual Task<LockScreenServiceRequestResult> RequestAccessAsync()
        {
            return ExceptionHelper.ThrowNotSupported<Task<LockScreenServiceRequestResult>>("To use this method, add Cimbalino.Toolkit assembly to the project and use the LockScreenWithRequestService instead. This method can't be called from a Background Agent.");
        }
    }
}