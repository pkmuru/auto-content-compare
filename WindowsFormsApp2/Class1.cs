using Allure.Commons;
using ImageMagick;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    [TestFixture]
    [AllureNUnit]
    [AllureDisplayIgnored]
    class Class1
    {


        IWebDriver driver;



        [Test(Description = "XXX"), TestCaseSource("GetSiteUrls")]
        [AllureTag("Regression2")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-1")]
        [AllureTms("TMS-12")]
        [AllureOwner("User")]
        [AllureSuite("Generate-Content")]
        [AllureSubSuite("Run2")]
        public void OpenSite(string name)
        {
            //do test

            var url = "https://www.bing.com/search?q=" + name + "&qs=n&form=QBLH&sp=-1&pq=hello&sc=8-5&sk=&cvid=A0B37AFFD8D940D6A718339AB948AFCF";
            Console.WriteLine(DateTime.Now + "----" + url);

            driver.Url = url;

            ITakesScreenshot screenshotDriver = driver as ITakesScreenshot;
            Screenshot screenshot = screenshotDriver.GetScreenshot();

            string fileName = @"C:\Users\muru\source\repos\WindowsFormsApp2\images" + System.DateTime.Now.Ticks.ToString() + ".png";
            screenshot.SaveAsFile(fileName);
            AllureLifecycle.Instance.AddAttachment("smeid-original", "image/png", fileName);

            driver.Navigate();
            driver.Url = url;
            screenshot = screenshotDriver.GetScreenshot();

            string fileName2 = @"C:\Users\muru\source\repos\WindowsFormsApp2\images" + System.DateTime.Now.Ticks.ToString() + "-entiry.png";
            screenshot.SaveAsFile(fileName2);
            AllureLifecycle.Instance.AddAttachment("smeid-entiry", "image/png", fileName2);


            var sourceImage = new MagickImage(fileName)
            {
                ColorFuzz = new Percentage(15)
            };
            var generatedImage = new MagickImage(fileName2);


            string fileName3 = @"C:\Users\muru\source\repos\WindowsFormsApp2\images" + System.DateTime.Now.Ticks.ToString() + "-diff.png";

            Assert.IsTrue(CompareImages(sourceImage, generatedImage, fileName3));
        }
        private static string[] GetSiteUrls()
        {

            List<String> urls = new List<string>();
            for (var i = 100; i < 110; i++)
            {
                urls.Add(i.ToString());
            }
            return urls.ToArray();
        }

        [SetUp]
        public void startBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--headless");
            options.AddArgument("--window-size=800,1080");
            driver = new ChromeDriver(options);
        }

        [TearDown]
        public void closeBrowser()
        {
            driver.Close();
        }

        private static bool CompareImages(MagickImage expectedImage, MagickImage imageToCompare, string deltaImagePath)
        {
            double ThreashHold = 0.01;
            using (var delta = new MagickImage())
            {
                var result = expectedImage.Compare(imageToCompare, ErrorMetric.Fuzz, delta);
                if (result > ThreashHold)
                {
                    delta.Write(deltaImagePath);
                    AllureLifecycle.Instance.AddAttachment("smeid-diff", "image/png", deltaImagePath);
                    Console.WriteLine($"Threshhold: {ThreashHold} compare result: {result} Does not match {expectedImage.FileName}.");
                    return false;
                }
                else
                {
                    Console.WriteLine($"Threshhold: {ThreashHold} compare result: {result} Matched {expectedImage.FileName}");
                    return true;
                }
            }
        }
    }
}
