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
    public partial class enAlbum {

        public enAlbum()
        {
            OnCreated();
        }

        public virtual int Album_id { get; set; }

        public virtual int Band_id { get; set; }

        public virtual int? Release_Year { get; set; }

        public virtual int? Album_rating { get; set; }

        public virtual string Album_title { get; set; }

        public virtual enMetalBand MetalBand { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
