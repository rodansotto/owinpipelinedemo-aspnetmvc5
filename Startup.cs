using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(MyMvc5App.Startup))]
namespace MyMvc5App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // this is the OWIN middleware component for individual authentication added by Visual Studio
            //ConfigureAuth(app);

            // this is an extension method from the nuget package Microsoft.Owin.Diagnostics
            // a wrapper to app.Use<T>()
            // this does not pass control to next component in the pipeline
            //app.UseWelcomePage();

            // this is my custom function, a wrapper to app.Run() 
            //  and app.Run() does not pass control to next component in the pipeline
            //WriteHelloWorld(app);

            // calling app.Use<T>() to register my OWIN middleware component in the OWIN pipeline
            //app.Use<HelloWorldComponent>();

            // Basically you call app.Use<T>(), or an AppBuilder extension which is just a wrapper to app.Use<T>(), 
            //  or app.Run() to register an OWIN middleware component in the OWIN pipeline

            // Below is an example of my Hello World!!! OWIN pipeline
            // Basically each component adds a hello world string in their own language to the
            //  OWIN environment variable, and once done it outputs the resulting string to response
            // These components awaits on the next component in the pipeline so essentially everything
            //  will come back to the first component in the pipeline and my first component in the
            //  pipeline is HelloWorldStartEndComponent and this component is the one that outputs
            //  the resulting hello world string to response

            // this is the first middleware component in the pipeline
            // it's calling app.Use<T>() to register the component in the OWIN pipeline
            app.Use<HelloWorldStartEndComponent>();

            // this is the second middleware component in the pipeline
            app.Use<HelloWorldEnglishComponent>();

            // this is the third middleware component in the pipeline
            //app.Use<HelloWorldFrenchComponent>();
            // instead, it calls a custom extension method to AppBuilder that wraps app.Use<T>()
            app.UseHelloWorldFrenchComponent();

            // and this is the last middleware component in the pipeline
            //app.Use<HelloWorldTagalogComponent>();
            // you can also pass a lambda function in calls to app.Use() instead
            app.Use(async (ctx, next) =>
            {
                // make sure this middleware only executes when request path is HelloWorldOWINPipeline
                var path = ctx.Environment["owin.RequestPath"] as string;
                if (path.Equals("/HelloWorldOWINPipeline", StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Environment["helloworld"] = ctx.Environment["helloworld"] + " Mabuhay!!! ";
                }

                await next();
            });

            //app.MapSignalR();
        }
    }
}
