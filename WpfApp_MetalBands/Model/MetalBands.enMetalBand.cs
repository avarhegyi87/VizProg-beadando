﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 2022. 05. 11. 0:58:01
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace enMetalBands
{
    public partial class enMetalBand {

        public enMetalBand()
        {
            this.Albums = new List<enAlbum>();
            this.Musicians = new List<enMusician>();
            OnCreated();
        }

        public virtual int Band_id { get; set; }

        public virtual string Band_name { get; set; }

        public virtual int Date_founding { get; set; }

        public virtual int? Genre_id { get; set; }

        public virtual IList<enAlbum> Albums { get; set; }

        public virtual IList<enMusician> Musicians { get; set; }

        public virtual enGenre Genres { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
