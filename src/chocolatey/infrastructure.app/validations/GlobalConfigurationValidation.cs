﻿// Copyright © 2017 - 2025 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.validations;

namespace chocolatey.infrastructure.app.validations
{
    /// <summary>
    ///   Performs top level validation checks against the current
    ///   Chocolatey Configuration object to ensure that everything is
    ///   set up properly.  Any errors that are returned will halt
    ///   the current operation.
    /// </summary>
    public class GlobalConfigurationValidation : IValidation
    {
        public ICollection<ValidationResult> Validate(ChocolateyConfiguration config)
        {
            this.Log().Debug("Global Configuration Validation Checks:");
            var validationResults = new List<ValidationResult>();

            ValidateUseOfPackageExitCode(config, validationResults);

            if (validationResults.Count == 0)
            {
                validationResults.Add(new ValidationResult
                {
                    Message = "Global Chocolatey Configuration is valid.",
                    Status = ValidationStatus.Success,
                    ExitCode = 0
                });
            }

            return validationResults;
        }

        private void ValidateUseOfPackageExitCode(ChocolateyConfiguration config, ICollection<ValidationResult> validationResults)
        {
            var validationStatusResult = ValidationStatus.Checked;
            // In order for a Chocolatey execution to correctly halt
            // on a detected reboot, it is necessary for package exit
            // codes to be used.  Otherwise, the codes (1641, and 3010)
            // can't be detected.
            if (config.Features.ExitOnRebootDetected &&
                !config.Features.UsePackageExitCodes)
            {
                var validationResult = new ValidationResult
                {
                    Message = @"When attempting to halt execution of a Chocolatey command based on a
   request for a system reboot, it is necessary to have the
   usePackageExitCodes feature enabled.  Use the following command:
     choco feature enable --name=""{0}""
   to enable this feature (exit code 1).
".FormatWith(ApplicationParameters.Features.UsePackageExitCodes),
                    Status = ValidationStatus.Error,
                    ExitCode = 1
                };

                var commandsToErrorOn = new List<string> { "install", "uninstall", "upgrade" };
                if (!commandsToErrorOn.Contains(config.CommandName.ToLowerInvariant()))
                {
                    validationResult.Status = ValidationStatus.Warning;
                }

                validationStatusResult = validationResult.Status;
                validationResults.Add(validationResult);
            }

            this.Log().Debug(" - Package Exit Code / Exit On Reboot = {0}".FormatWith(validationStatusResult.ToStringSafe()));
        }

#pragma warning disable IDE0022, IDE1006
        [Obsolete("This overload is deprecated and will be removed in v3.")]
        public ICollection<ValidationResult> validate(ChocolateyConfiguration config)
            => Validate(config);
#pragma warning restore IDE0022, IDE1006
    }
}
