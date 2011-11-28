/*
 * This file is part of the SearchBoost.NET project.
 * Copyright (c) 2011 Bogdan Litescu
 * Authors: Bogdan Litescu
 *
 * SearchBoost.NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SearchBoost.NET is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with SearchBoost.NET. If not, see <http://www.gnu.org/licenses/>.
 * 
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the software without
 * disclosing the source code of your own applications.
 *
 * For more information, please contact us at support@dnnsharp.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SearchBoost.Net.Core;
using System.IO;
using SearchBoost.Net.Core.Engine;
using SearchBoost.Net.Core.Indexers;

namespace SearchBoost.Net.UnitTests
{
    [TestFixture]
    public class TestWebSpider
    {
        [SetUp]
        public void Init()
        {
            SbApp.RootFolder = AppDomain.CurrentDomain.BaseDirectory;
            SbApp.ConfigFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../config");
            SbApp.Instance.SearchEngine.ClearIndex();
        }

        [TearDown]
        public void Cleanup()
        {
            SbApp.Instance.Dispose();
        }

        [Test]
        public void PlainText()
        {
            ISearchEngine se = SbApp.Instance.SearchEngine;
            IIndexer webSpiderPlain = SbApp.Instance.Container.Resolve<IIndexer>("webSpiderPlain");

            // execute URLs from configuration
            webSpiderPlain.Index();

            // do a search
            Assert.AreEqual(1, se.Search("test").Count);
            Assert.IsTrue(se.Search("test")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);

            // clear index
            se.ClearIndex();
            Assert.AreEqual(0, se.Search("test").Count);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);
        }

        [Test]
        public void BasicHtml()
        {
            ISearchEngine se = SbApp.Instance.SearchEngine;
            IIndexer webSpiderHtml = SbApp.Instance.Container.Resolve<IIndexer>("webSpiderHtml");

            // execute URLs from configuration
            webSpiderHtml.Index();

            // do a search
            Assert.AreEqual(1, se.Search("test").Count);
            Assert.IsTrue(se.Search("test")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual("Simple Test", se.Search("test")[0].Title);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);

            // clear index
            se.ClearIndex();
            Assert.AreEqual(0, se.Search("test").Count);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);
        }

        [Test]
        public void HtmlFollowLinks()
        {
            ISearchEngine se = SbApp.Instance.SearchEngine;
            IIndexer webSpiderHtmlFollowLinks = SbApp.Instance.Container.Resolve<IIndexer>("webSpiderHtmlFollowLinks");

            // execute URLs from configuration
            webSpiderHtmlFollowLinks.Index();

            // do a search
            Assert.AreEqual(1, se.Search("test").Count);
            Assert.IsTrue(se.Search("test")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual("Simple Test", se.Search("test")[0].Title);
            Assert.AreEqual(1, se.Search("test2").Count);
            Assert.IsTrue(se.Search("test2")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual("Simple Test 2", se.Search("test2")[0].Title);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);

            // clear index
            se.ClearIndex();
            Assert.AreEqual(0, se.Search("test").Count);
            Assert.AreEqual(0, se.Search("test2").Count);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);
        }

        [Test]
        public void BasicSitemap()
        {
            ISearchEngine se = SbApp.Instance.SearchEngine;
            IIndexer webSpiderSitemap = SbApp.Instance.Container.Resolve<IIndexer>("webSpiderSitemap");

            // execute URLs from configuration
            webSpiderSitemap.Index();

            // do a search
            Assert.AreEqual(1, se.Search("test").Count);
            Assert.IsTrue(se.Search("test")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual("Simple Test", se.Search("test")[0].Title);
            Assert.AreEqual(1, se.Search("test2").Count);
            Assert.IsTrue(se.Search("test2")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual("Simple Test 2", se.Search("test2")[0].Title);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);

            // clear index
            se.ClearIndex();
            Assert.AreEqual(0, se.Search("test").Count);
            Assert.AreEqual(0, se.Search("test2").Count);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);
        }

        [Test]
        public void BasicRss()
        {
            ISearchEngine se = SbApp.Instance.SearchEngine;
            IIndexer webSpiderRss = SbApp.Instance.Container.Resolve<IIndexer>("webSpiderRss");

            // execute URLs from configuration
            webSpiderRss.Index();

            // do a search
            Assert.AreEqual(1, se.Search("test").Count);
            Assert.IsTrue(se.Search("test")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual("Simple Test", se.Search("test")[0].Title);
            Assert.AreEqual(1, se.Search("test2").Count);
            Assert.IsTrue(se.Search("test2")[0].Sources.Contains("www.dnnsharp.com"));
            Assert.AreEqual("Simple Test 2", se.Search("test2")[0].Title);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);

            // clear index
            se.ClearIndex();
            Assert.AreEqual(0, se.Search("test").Count);
            Assert.AreEqual(0, se.Search("test2").Count);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);
        }
    }
}
