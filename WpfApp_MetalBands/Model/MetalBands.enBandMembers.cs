﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 2022. 04. 16. 13:21:46
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
    public partial class enBandMembers {

        public enBandMembers()
        {
            OnCreated();
        }

        public virtual int Band_id { get; set; }

        public virtual int Musician_id { get; set; }

        public virtual string Instrument { get; set; }

        public virtual enMusician enMusician { get; set; }

        public virtual enMetalBand enMetalBand { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        public override bool Equals(object obj)
        {
          enBandMembers toCompare = obj as enBandMembers;
          if (toCompare == null)
          {
            return false;
          }

          if (!Object.Equals(this.Band_id, toCompare.Band_id))
            return false;
          if (!Object.Equals(this.Musician_id, toCompare.Musician_id))
            return false;

          return true;
        }

        public override int GetHashCode()
        {
          int hashCode = 13;
          hashCode = (hashCode * 7) + Band_id.GetHashCode();
          hashCode = (hashCode * 7) + Musician_id.GetHashCode();
          return hashCode;
        }

        #endregion
    }

}
