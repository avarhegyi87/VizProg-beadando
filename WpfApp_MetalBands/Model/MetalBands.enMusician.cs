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
using enMetalBands;

namespace enMetalBands
{
    public partial class enMusician {

        public enMusician()
        {
            this.MetalBands = new List<enMetalBand>();
            OnCreated();
        }

        public virtual int Musician_id { get; set; }

        public virtual string First_name { get; set; }

        public virtual string Last_name { get; set; }

        public virtual string Instrument { get; set; }

        public virtual IList<enMetalBand> MetalBands { get; set; }


        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
