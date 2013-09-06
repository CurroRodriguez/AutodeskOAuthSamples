/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Created by Gopinath Taget, 2013 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators.OAuth;
using RestSharp.Contrib;
using RestSharp.Authenticators;
using System.Diagnostics;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCAD_Authentication
{
    public class CommandClass
    {

        // This command shows how to use the OAuth 1.01 protocol exposed by Autodesk Oxygen system.
        // We will demonstrate the request and response for the following end points:
        // 1) /OAuth/RequestToken
        // 2) /OAuth/AuthorizeToken
        // 3) /OAuth/AccessToken
        [CommandMethod("AUTHORIZE")]
        public static void DoOAuth()
        {
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(new OAuthProcess());

        }
    }
}
