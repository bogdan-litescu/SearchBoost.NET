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

        public void Index(string content)
        {
            Logger.Debug(string.Format("Indexing content {0}...", content));

            //create an analyzer to process the text
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

            //create the index writer with the directory and analyzer defined.
            IndexWriter indexWriter = new IndexWriter(_indexdir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            //create a document, add in a single field
            Document doc = new Document();
            doc.Add(new Field("content", content, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));

            //write the document to the index
            indexWriter.AddDocument(doc);

            //optimize and close the writer
            indexWriter.Optimize();
            indexWriter.Close();
            
            Logger.Debug("...done.");
        }

        public IList<string> Search(string terms)
        {
            Logger.Info(string.Format("Searching for {0}...", terms));
            IndexSearcher searcher = new IndexSearcher(_indexdir, true);
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "content", new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29));
            Query query = parser.Parse(terms);

            TopDocs docs = searcher.Search(query, 999999);
            List<string> results = new List<string>();
            foreach (ScoreDoc sdoc in docs.scoreDocs)
                results.Add(searcher.Doc(sdoc.doc).Get("content"));
            
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
