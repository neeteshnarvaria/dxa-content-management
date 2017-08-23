﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Web.Tridion.Common;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Publishing.Rendering;
using Tridion.ContentManager.Templating;

namespace Sdl.Web.Tridion.Templates.Tests
{

    [TestClass]
    public class TemplateBaseTest : TestClass
    {
        private class TestTemplate : TemplateBase
        {
            internal TestTemplate(Publication publication)
            {
                Session = publication.Session;
                Publication = publication;
            }

            public override void Transform(Engine engine, Package package)
                => Console.WriteLine("DummyTemplate.Transform was called.");

            // Wrapper method to expose protected methods to test:
            internal string TestGetLocale() => GetLocale();
            internal string TestStripTcdlComponentPresentationTag(string input) => StripTcdlComponentPresentationTag(input);
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
            => DefaultInitialize(testContext);

        [TestMethod]
        public void PublishMappings_Success()
        {
            // Assign

            //init engine
            Component inputItem = (Component)TestSession.GetObject("/webdav/400 Example Site/Building Blocks/Content/Sitemap.xml");
            RenderedItem testRenderedItem = CreateTestRenderedItem(inputItem, null);
            var testEngine = new TestEngine(testRenderedItem);

            //init package
            var testPackage = new Package(testEngine);
            Type inputItemType = inputItem.GetType();
            string inputItemName = inputItemType.Name;
            ContentType inputItemContentType = new ContentType($"tridion/{inputItemType.Name.ToLower()}");
            testPackage.PushItem(inputItemName, testPackage.CreateTridionItem(inputItemContentType, inputItem));

            //init publish mappings
            var publishMappings = new PublishMappings();
            publishMappings.Transform(testEngine, testPackage);

            // Act

            // Assert
        }

        [TestMethod]
        public void GetCulture_WithLocalizationConfigComponent_Success()
        {
            Publication testPublication = (Publication) TestSession.GetObject(TestFixture.AutoTestParentWebDavUrl);
            TestTemplate testTemplate = new TestTemplate(testPublication);

            string locale = testTemplate.TestGetLocale();

            Assert.AreEqual("en-US", locale);
        }

        [TestMethod]
        public void StripTcdlComponentPresentationTag_Success()
        {
            Publication testPublication = (Publication) TestSession.GetObject(TestFixture.AutoTestParentWebDavUrl);
            TestTemplate testTemplate = new TestTemplate(testPublication);

            string input1 = "<TCDL:ComponentPresentation a=\"1\">Test <tcdl>.</TCDL:ComponentPresentation>";
            string result1 = testTemplate.TestStripTcdlComponentPresentationTag(input1);
            Console.WriteLine(result1);

            string input2 = "<tcdl:ComponentPresentation a=\"1\">Test <tcdl>.</tcdl:ComponentPresentation>";
            string result2 = testTemplate.TestStripTcdlComponentPresentationTag(input2);
            Console.WriteLine(result2);

            Assert.AreEqual(input1, result1);
            Assert.AreEqual("Test <tcdl>.", result2);
        }

    }
}
