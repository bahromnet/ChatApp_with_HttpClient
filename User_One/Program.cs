using System.Net;
using System.Text;

namespace User_One
{
    internal class Program
    {
        static HttpClient httpClient = new HttpClient();
        static HttpListener listener = new HttpListener();
        static async Task Main(string[] args)
        {
            //Console.WriteLine("Username ingizni kiriting: ");
            //string username = Console.ReadLine() ?? "No username";
            httpClient.DefaultRequestHeaders.Add("User-Agent", $"User_One");

            listener.Prefixes.Add("http://127.0.0.1:9586/");
            //listener.Prefixes.Add("http://127.0.0.1:9588/");
            //listener.Prefixes.Add("http://127.0.0.1:9589/");
            listener.Start();

            var tasks = new Task[2];
            tasks[0] = Listener();
            tasks[1] = Sender();

            await Task.WhenAll(tasks);

            listener.Stop();
            await Console.Out.WriteLineAsync("Finished");
        }

        private static async Task Listener()
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                var request = context.Request;

                using Stream stream = request.InputStream;
                using StreamReader reader = new StreamReader(stream, request.ContentEncoding);
                string resMessage = await reader.ReadToEndAsync();
                Console.WriteLine($"{request.UserAgent}: {resMessage}");

                var response = context.Response;
                string text = "Message succesfully handled";
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                await response.OutputStream.FlushAsync();

            }
        }
        private static async Task Sender()
        {
            while (true)
            {
                Console.WriteLine("Xabar kiriting: ");
                StringContent content = new StringContent(Console.ReadLine() ?? "no message!");
                var response1 = await httpClient.PostAsync("http://127.0.0.1:9587/", content);
                var response2 = await httpClient.PostAsync("http://127.0.0.1:9588/", content);
                var response3 = await httpClient.PostAsync("http://127.0.0.1:9589/", content);

                await Console.Out.WriteLineAsync(await response1.Content.ReadAsStringAsync());
            }
        }
    }
}