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
using System.Xml.Serialization;

namespace chocolatey.infrastructure.app.configuration
{
    /// <summary>
    ///   XML config file api keys element
    /// </summary>
    [Serializable]
    [XmlType("apiKeys")]
    public sealed class ConfigFileApiKeySetting
    {
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var item = (ConfigFileApiKeySetting)obj;

            return (Source == item.Source)
                   && (Key == item.Key);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Key);
        }
    }
}
