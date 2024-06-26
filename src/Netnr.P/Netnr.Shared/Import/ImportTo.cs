﻿#if Full || ImportBase

global using Netnr;
global using System;
global using System.IO;
global using System.Net;
global using System.Net.Http;
global using System.Linq;
global using System.Text;
global using System.Data;
global using System.Data.Common;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Text.Json.Serialization;
global using System.Diagnostics;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;

#endif

#if Full || ImportWeb

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Cors;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication;

#endif