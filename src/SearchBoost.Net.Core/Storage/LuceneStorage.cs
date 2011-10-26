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
using Castle.Core.Logging;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using SearchBoost.Net.Core.Engine;
using SearchBoost.Net.Core.Extensions;

namespace SearchBoost.Net.Core.Storage
{
    public class LuceneStorage : ISearchIndexStorage
    {
        public LuceneStorage(ILogger logger, string location)
        {
            if (location.IndexOf(":/") == -1)
                location = Path.Combine(SbApp.RootFolder, location);
            logger.Info(string.Format("Creating index folder {0}...", location));
            _indexdir = FSDirectory.Open(new DirectoryInfo(location));
            logger.Info("...done.");
        }

        public ILogger Logger { get; set; }
        Lucene.Net.Store.Directory _indexdir;

        public void Index(SbSearchDoc indexDoc)
        {
            Logger.Debug(string.Format("Indexing content {0}...", indexDoc.PlainContent));

            //create an analyzer to process the text
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

            //create the index writer with the directory and analyzer defined.
            IndexWriter indexWriter = new IndexWriter(_indexdir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            //create a document, add in a single field
            Document doc = new Document();
            
            // content
            doc.Add(new Field("content", indexDoc.PlainContent, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            
            // content types, sources, categories
            foreach (string type in indexDoc.ContentTypes)
                doc.Add(new Field("type", type, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));

            foreach (string source in indexDoc.Sources)
                doc.Add(new Field("source", source, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));

            foreach (string cat in indexDoc.Categories)
                doc.Add(new Field("category", cat, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));

            // date last modified
            //doc.Add(new Field("lastmod_str", indexDoc.LastModified.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new NumericField("lastmod", Field.Store.YES, true)
                .SetIntValue(indexDoc.LastModified.ToUnixTimestamp()));

            // title, description, keywords
            doc.Add(new Field("title", indexDoc.Title, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));
            doc.Add(new Field("keywords", indexDoc.Keywords, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("desc", indexDoc.Description, Field.Store.YES, Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS));
            doc.Add(new Field("author", indexDoc.Author, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));
            doc.Add(new Field("loc", indexDoc.Location, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));

            //write the document to the index
            indexWriter.AddDocument(doc);

            //optimize and close the writer
            indexWriter.Optimize();
            indexWriter.Close();
            
            Logger.Debug("...done.");
        }

        public IList<SbSearchDoc> Search(string terms)
        {
            Logger.Info(string.Format("Searching for {0}...", terms));
            IndexSearcher searcher = new IndexSearcher(_indexdir, true);
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "content", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29));
            Query query = parser.Parse(terms);

            TopDocs docs = searcher.Search(query, 999999);
            List<SbSearchDoc> results = new List<SbSearchDoc>();
            foreach (ScoreDoc sdoc in docs.scoreDocs)
                results.Add(new SbSearchDoc(searcher.Doc(sdoc.doc)));
            
            Logger.Info(string.Format("...{0} results found", results.Count));

            return results;
        }

        public void ClearIndex()
        {
            Logger.Info("Clearing index...");

            //create an analyzer to process the text
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

            //create the index writer with the directory and analyzer defined.
            IndexWriter indexWriter = new IndexWriter(_indexdir, analyzer, /*create a new index*/ true, IndexWriter.MaxFieldLength.UNLIMITED);

            indexWriter.DeleteAll();
            indexWriter.Commit();
            indexWriter.Close();
            Logger.Info("...index succesfully cleared!");
        }
    }
}
