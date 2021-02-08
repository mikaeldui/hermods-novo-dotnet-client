# Hermods Novo client for .NET Standard

![Continuous Integration](https://github.com/mikaeldui/HermodsNovoClient/workflows/Continuous%20Integration/badge.svg) [![NuGet version (MikaelDui.Hermods.Novo.Client)](https://img.shields.io/nuget/v/MikaelDui.Hermods.Novo.Client.svg?style=flat-square)](https://www.nuget.org/packages/MikaelDui.Hermods.Novo.Client/) 

An unofficial .NET client for Hermods Novo.

You can install it using the following package manager command:

    Install-Package MikaelDui.Hermods.Novo.Client

It was created for use by [mikaeldui/MinaLaromedel](https://github.com/mikaeldui/MinaLaromedel).

Currently the support is limited to e-books published by Liber, using [mikaeldui/LiberOnlinebokClient](https://github.com/mikaeldui/LiberOnlinebokClient).

# Example

Below is a small console application that prints the names of all active e-books.

```c#
using Hermods.Novo;
using System;
using System.Threading.Tasks;

namespace HermodsNovoConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string username = "firstname.lastname@domain.com";
            const string password = "SuperSecret123";

            using (var client = new HermodsNovoClient())
            {
                if (!await client.TryAuthenticateAsync(username, password))
                {
                    // Bad credentials
                    Console.WriteLine("Invalid credentials!\n");
                }
                else
                {
                    // We're logged in!
                    try
                    {
                        var ebookList = await client.GetEbooksAsync();

                        Console.WriteLine($"Active ebooks for {username}:\n\n");

                        foreach(var ebook in ebookList)
                        {
                            Console.WriteLine($"Title: {ebook.Title}\nISBN: {ebook.Isbn}\nValid: {ebook.StartDate.ToShortDateString()} to {ebook.EndDate.ToShortDateString()}\n\n");
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("An error occured while the books were being retrieved: " + e.Message);
                    }
                }
            }

            Console.WriteLine("\nPress any key to exit the program.");
            Console.ReadKey();
        }
    }
}
```

The output should be something similar to this:

    Active ebooks for firstname.lastname@domain.com:

    Title: Biologi 1 Onlinebok (12 mån)
    ISBN: 9789147107025
    Valid: 04/01/2021 to 04/01/2022

    Title: Geografi 1 och 2 Onlinebok (12 mån)
    ISBN: 9789147910281
    Valid: 04/01/2021 to 04/01/2022

    Press any key to exit the program.
