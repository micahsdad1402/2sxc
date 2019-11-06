﻿using System.Collections.Specialized;
using ToSic.Eav.LookUp;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    /// <inheritdoc />
    /// <summary>
    /// Copied from Form and List Module
    /// </summary>
    internal class LookUpInNameValueCollection : LookUpBase
    {
	    readonly NameValueCollection _nameValueCollection;
        public LookUpInNameValueCollection(string name, NameValueCollection list)
        {
            Name = name;
            _nameValueCollection = list;
        }
        

        public override string Get(string key, string format, ref bool propertyNotFound) 
            => _nameValueCollection == null 
            ? string.Empty 
            : FormatString(_nameValueCollection[key], format);

        public override bool Has(string key) => throw new System.NotImplementedException();
    }
}