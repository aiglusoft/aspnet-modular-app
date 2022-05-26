
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

[assembly: Microsoft.Owin.OwinStartup(typeof(MyOrg.AppName.Bootstrapper.Startup))]
namespace MyOrg.AppName.Bootstrapper
{


    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
