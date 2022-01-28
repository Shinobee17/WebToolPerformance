using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace WebToolPerformance
{

    class Link
    {
        public string Name { get; set; }
        public int Time { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<string> crawllist = new List<string>();
            List<string> sitemaplist = new List<string>();
            List<Link> allurl = new List<Link>();
            crawllist.Add(Console.ReadLine());


            Console.WriteLine("processing...");


           

            if (!crawllist[0].StartsWith("https://") || !crawllist[0].StartsWith("https://") && !crawllist[0].StartsWith("http://")) { crawllist[0] = "https://" + crawllist[0]; }
            if (!crawllist[0].EndsWith("/")) { crawllist[0] = crawllist[0] + "/"; }

            for (int i = 0; i < crawllist.Count(); i++)
            {
                try
                {
                    var links = GetLinks(crawllist[i]);

                    foreach (var link in links)
                    {
                        var url = link.GetAttribute("href");
                        if (string.IsNullOrEmpty(url)) { continue; }
                        if (url.StartsWith('/'))
                        {
                            url = $"{crawllist[0].TrimEnd('/')}{url}";
                        }
                        Uri uri;
                        if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                        {

                        }
                        if (uri != null && uri.ToString().Contains(crawllist[0]) && !crawllist.Contains(uri.ToString()))
                            crawllist.Add(uri.ToString());
                    }
                }
                catch (Exception e) { }
            }

            try {
                WebClient wc = new WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;

                string sitemapString = wc.DownloadString(crawllist[0] + "sitemap.xml");
                XmlDocument urldoc = new XmlDocument();
                urldoc.LoadXml(sitemapString);
                XmlNodeList xmlSitemapList = urldoc.GetElementsByTagName("url");
                foreach (XmlNode node in xmlSitemapList)
                {
                    if (node["loc"] != null)
                    {

                        sitemaplist.Add(node["loc"].InnerText);
                    }


                }
            }
            catch (Exception e) { }
            Console.WriteLine("----------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site   all file links count=" + sitemaplist.Count());
            Console.WriteLine("----------------------------------------------------------------------------------------------------------");
            int k = 1;
            for (int i = 0; i < sitemaplist.Count(); i++)
            {
                if (!crawllist.Contains(sitemaplist[i]))
                {
                    Console.WriteLine((k) + ") " + sitemaplist[i]);
                    k++;
                }
            }
            //Console.WriteLine("0) "+sitemaplist[0]);
           // sitemaplist.Remove(sitemaplist[0]);

            Console.WriteLine("\n\n\nUrls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml      all crawling links count=" + crawllist.Count());
            Console.WriteLine("----------------------------------------------------------------------------------------------------------");
            int k2 = 1;
            for (int i = 0; i < crawllist.Count(); i++)
            {

                if (!sitemaplist.Contains(crawllist[i]))
                {
                    Console.WriteLine((k2) + ") " + crawllist[i]);
                    k2++;
                }
            }

            sitemaplist = sitemaplist.Union(crawllist).ToList();


            Console.WriteLine("processing...");


           
            
            


            for (int i = 0; i < sitemaplist.Count(); i++)
            {
                try
                {  
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sitemaplist[i]);

                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    timer.Stop();

                    allurl.Add((new Link {Name = sitemaplist[i],  Time = Int32.Parse(timer.ElapsedMilliseconds.ToString()) })) ;
                }
                catch (Exception e) { }
            }

            Console.WriteLine("\n\n\nTiming   Count links=" + allurl.Count());
            Console.WriteLine("----------------------------------------------------------------------------------------------------------");
           
            var sortedLinks = from a in allurl
                              orderby a.Time ascending
                              select a;

            
            foreach (Link a in sortedLinks)
                Console.WriteLine(a.Name + "  |  " + "Time: " + a.Time + "ms");

        }
        public static IHtmlCollection<IElement> GetLinks(string url)
            {   
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();

                
                string html = String.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);

                return document.QuerySelectorAll("a");
            }
       
    }
}
                                                                                                           