﻿using System;
using System.Net;
using System.Threading;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace WebWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            string previousWebResult = string.Empty;

            string url = null,
                cssSelector = null;

            int seconds;

            if (args.Length > 0)
            {
                url = args[0];
            }
            if (args.Length > 1)
            {
                cssSelector = args[1];
            }
            if(args.Length > 2)
            {
                int.TryParse(args[2], out seconds);
            }

            if (string.IsNullOrEmpty(url))
            {
                Console.WriteLine("Type the url you want to check: ");
                url = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(cssSelector))
            {
                Console.WriteLine("Type the css selector (empty if none): ");
                cssSelector = Console.ReadLine();
            }

            Console.WriteLine($"Started checking {url}");

            while (true)
            {
                var client = new WebClient();

                Console.WriteLine($"Requesting {url} at {DateTime.Now} {(!string.IsNullOrEmpty(cssSelector) ? $"with css selector {cssSelector}" : string.Empty)}...");

                var nextWebResult = client.DownloadStringTaskAsync(new Uri(url)).GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(previousWebResult))
                {
                    previousWebResult = nextWebResult;
                    Wait(seconds);
                    continue;
                }

                var previousDoc = new HtmlDocument();
                previousDoc.LoadHtml(previousWebResult);

                var nextDoc = new HtmlDocument();
                nextDoc.LoadHtml(nextWebResult);

                var previousContent = previousDoc.DocumentNode.QuerySelector(cssSelector);

                var nextContent = nextDoc.DocumentNode.QuerySelector(cssSelector);

                if (nextWebResult != previousWebResult)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    if (ShowDifferences(previousContent, nextContent))
                    {
                        Console.Beep(800, 5000);

                        previousWebResult = nextWebResult;
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }

                Wait(seconds);
            }
        }

        private static bool ShowDifferences(HtmlNode previousContent, HtmlNode nextContent)
        {
            if (previousContent == null)
            {
                throw new ArgumentNullException(nameof(previousContent));
            }

            if (nextContent == null)
            {
                throw new ArgumentNullException(nameof(nextContent));
            }

            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(previousContent.InnerHtml, nextContent.InnerHtml);

            if(diff.Lines.TrueForAll(x => x.Type == ChangeType.Unchanged))
            {
                return false;
            }

            Console.WriteLine($"WEB WASN'T THE SAME {DateTime.Now}");

            foreach (var line in diff.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("+ ");
                        break;
                    case ChangeType.Deleted:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("- ");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  ");
                        break;
                }

                Console.WriteLine(line.Text);
            }

            return true;
        }

        private static void Wait(int seconds = 1)
        {
            Thread.Sleep(1000 * seconds);
        }
    }
}
