﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 2022. 05. 10. 20:40:22
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
    internal partial class enUsers {

        public enUsers()
        {
            OnCreated();
        }

        public virtual int UserId { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
