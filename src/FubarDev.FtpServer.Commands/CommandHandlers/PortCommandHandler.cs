//-----------------------------------------------------------------------
// <copyright file="PortCommandHandler.cs" company="Fubar Development Junker">
//     Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>
// <author>Mark Junker</author>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using FubarDev.FtpServer.Commands;
using FubarDev.FtpServer.DataConnection;
using FubarDev.FtpServer.Features;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FubarDev.FtpServer.CommandHandlers
{
    /// <summary>
    /// Implements the <c>PORT</c> and <c>EPRT</c> commands.
    /// </summary>
    [FtpCommandHandler("PORT")]
    [FtpCommandHandler("EPRT")]
    [FtpFeatureText("EPRT")]
    public class PortCommandHandler : FtpCommandHandler
    {
        [NotNull]
        private readonly ActiveDataConnectionFeatureFactory _dataConnectionFeatureFactory;

        [NotNull]
        private readonly PortCommandOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortCommandHandler"/> class.
        /// </summary>
        /// <param name="dataConnectionFeatureFactory">The factory to create a data connection feature for active connections.</param>
        /// <param name="options">The options for this command.</param>
        public PortCommandHandler(
            [NotNull] ActiveDataConnectionFeatureFactory dataConnectionFeatureFactory,
            [NotNull] IOptions<PortCommandOptions> options)
        {
            _dataConnectionFeatureFactory = dataConnectionFeatureFactory;
            _options = options.Value;
        }

        /// <inheritdoc/>
        public override async Task<IFtpResponse> Process(FtpCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var address = Address.Parse(command.Argument);
                if (address == null)
                {
                    return new FtpResponse(501, T("Syntax error in parameters or arguments."));
                }

                var feature = await _dataConnectionFeatureFactory.CreateFeatureAsync(command, address, _options.DataPort)
                   .ConfigureAwait(false);
                var oldFeature = Connection.Features.Get<IFtpDataConnectionFeature>();
                try
                {
                    oldFeature.Dispose();
                }
                catch
                {
                    // Ignore dispose errors!
                }

                Connection.Features.Set(feature);
            }
            catch (NotSupportedException ex)
            {
                return new FtpResponse(522, T("Extended port failure - {0}.", ex.Message));
            }

            return new FtpResponse(200, T("Command okay."));
        }
    }
}
