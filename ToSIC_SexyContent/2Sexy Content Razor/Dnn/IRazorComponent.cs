﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Environment;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Search;

// ReSharper disable UnusedMemberInSuper.Global

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// All DNN Razor Pages inherit from this class
    /// </summary>
    [PublicApi]
    public interface IRazorComponent: IDynamicCode
    {
        /// <summary>
        /// Helper for Html.Raw - for creating raw html output which doesn't encode &gt; and &lt;
        /// </summary>
        IHtmlHelper Html { get; }


        /// <summary>
        /// Override this to have your code change the (already initialized) Data object. 
        /// If you don't override this, nothing will be changed/customized. 
        /// </summary>
        void CustomizeData();

        /// <summary>
        /// Customize how the search will process data on this page. 
        /// </summary>
        /// <param name="searchInfos"></param>
        /// <param name="moduleInfo"></param>
        /// <param name="beginDate"></param>
        void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
            DateTime beginDate);

        /// <summary>
        /// The purpose of the current execution. The code might be called for showing to a user, or search-indexing.
        /// </summary>
        /// <returns>The value of the current purpose.</returns>
        Purpose Purpose { get; }

    }
}
