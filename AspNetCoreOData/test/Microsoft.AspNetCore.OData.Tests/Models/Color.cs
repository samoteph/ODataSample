//-----------------------------------------------------------------------------
// <copyright file="Color.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;

namespace Microsoft.AspNetCore.OData.Tests.Models
{
    [Flags]
    public enum Color
    {
        Red = 1,

        Green = 2,

        Blue = 4
    }
}
