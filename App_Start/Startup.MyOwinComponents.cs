using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyMvc5App
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public partial class Startup
    {
        public void WriteHelloWorld(IAppBuilder app)
        {
            app.Run(ctx =>
            {
                return ctx.Response.WriteAsync("From WriteHelloWorld: Hello World!!!");
            });
        }
    }

    public class HelloWorldComponent
    {
        AppFunc _next;
        public HelloWorldComponent(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            await _next(environment);

            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                writer.Write("From HelloWorldComponent: Hello World!!!");
            }
        }
    }

    public class HelloWorldStartEndComponent
    {
        AppFunc _next;

        // through this constructor, the app.Use<T>() passes the next component that this component will call
        public HelloWorldStartEndComponent(AppFunc next)
        {
            _next = next;
        }

        // an OWIN component needs to provide a method with a Func<IDictionary<string, object>, Task> signature
        public async Task Invoke(IDictionary<string, object> environment)
        {
            // make sure this middleware component only executes when request path is /HelloWorldOWINPipeline
            // but it will still need to call the next component
            var path = environment["owin.RequestPath"] as string;
            if (path.Equals("/HelloWorldOWINPipeline", StringComparison.OrdinalIgnoreCase))
            {
                // create a new environment variable to hold the hello world string in multiple languages
                environment["helloworld"] = "<div class='container body-content'><h1>";

                // call the next component in the OWIN pipeline
                await _next(environment);

                // at this point all the hello world components in the pipeline should have inserted
                //  their hello world string in their own language to the hello world environment variable,
                //  so output the resulting string to response
                // but first let's put the closing h1 and div tags at the end
                environment["helloworld"] = environment["helloworld"] + "</h1></div>";

                var response = environment["owin.ResponseBody"] as Stream;
                var helloWorld = environment["helloworld"] as string;
                using (var writer = new StreamWriter(response))
                {
                    writer.Write(helloWorld);
                }
            }
            else
            {
                // call the next component in the OWIN pipeline
                await _next(environment);
            }
        }
    }

    public class HelloWorldEnglishComponent
    {
        AppFunc _next;
        public HelloWorldEnglishComponent(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            // make sure this middleware component only executes when request path is /HelloWorldOWINPipeline
            var path = environment["owin.RequestPath"] as string;
            if (path.Equals("/HelloWorldOWINPipeline", StringComparison.OrdinalIgnoreCase))
            {
                environment["helloworld"] = environment["helloworld"] + "Hello World!!!<br/>";
            }

            await _next(environment);
        }
    }

    public class HelloWorldFrenchComponent
    {
        AppFunc _next;
        public HelloWorldFrenchComponent(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            // make sure this middleware component only executes when request path is /HelloWorldOWINPipeline
            var path = environment["owin.RequestPath"] as string;
            if (path.Equals("/HelloWorldOWINPipeline", StringComparison.OrdinalIgnoreCase))
            {
                environment["helloworld"] = environment["helloworld"] + "Bonjour Tout Le Monde!!!<br/>";
            }

            await _next(environment);
        }
    }

    public class HelloWorldTagalogComponent
    {
        AppFunc _next;
        public HelloWorldTagalogComponent(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            // make sure this middleware component only executes when request path is /HelloWorldOWINPipeline
            var path = environment["owin.RequestPath"] as string;
            if (path.Equals("/HelloWorldOWINPipeline", StringComparison.OrdinalIgnoreCase))
            {
                environment["helloworld"] = environment["helloworld"] + "Mabuhay!!!<br/>";
            }

            await _next(environment);
        }

    }

    public static class MyAppBuilderExtensions
    {
        public static void UseHelloWorldFrenchComponent(this IAppBuilder app)
        {
            app.Use<HelloWorldFrenchComponent>();
        }
    }
}