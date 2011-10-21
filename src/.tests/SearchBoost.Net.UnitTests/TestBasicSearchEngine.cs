﻿/*
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

namespace SearchBoost.Net.UnitTests
{
    [TestFixture]
    public class TestBasicSearchEngine
    {
        [SetUp]
        public void Init()
        {
            SbApp.RootFolder = AppDomain.CurrentDomain.BaseDirectory;
            SbApp.ConfigFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../config");
        }

        [TearDown]
        public void Cleanup()
        {
            SbApp.Instance.Dispose();
        }

        [Test]
        public void Basic()
        {
            ISearchEngine se = SbApp.Instance.SearchEngine;
            se.Index(new SbSearchDoc() { Content = "Keyword entry 1" });
            se.Index(new SbSearchDoc() { Content = "Keyword entry 2" });
            se.Index(new SbSearchDoc() { Content = "Another entry 3" });

            // do a search
            Assert.AreEqual(2, se.Search("keyword").Count);
            Assert.AreEqual(1, se.Search("another").Count);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);

            // clear index
            se.ClearIndex();
            Assert.AreEqual(0, se.Search("keyword").Count);
            Assert.AreEqual(0, se.Search("another").Count);
            Assert.AreEqual(0, se.Search("zxcvbnm").Count);
        }
    }
}
