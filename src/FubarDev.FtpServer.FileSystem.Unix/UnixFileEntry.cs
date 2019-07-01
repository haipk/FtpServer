// <copyright file="UnixFileEntry.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using JetBrains.Annotations;

using Mono.Unix;

namespace FubarDev.FtpServer.FileSystem.Unix
{
    internal class UnixFileEntry : UnixFileSystemEntry, IUnixFileEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnixFileEntry"/> class.
        /// </summary>
        /// <param name="info">The file information.</param>
        public UnixFileEntry([NotNull] UnixFileInfo info)
            : base(info)
        {
            Info = info;
            Size = info.Length;
        }

        /// <summary>
        /// Gets the unix file info.
        /// </summary>
        [NotNull]
        public UnixFileInfo Info { get; }

        /// <inheritdoc />
        public long Size { get; }
    }
}
