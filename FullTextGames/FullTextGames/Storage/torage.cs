using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextGames.Storage
{
    internal interface IContentStorage
    {
        void AddDocument(string document);
        string GetDocument(int id);
    }

    internal class InMemoryContentStorage : IContentStorage
    {
        private readonly List<string> documents = new List<string>();

        public void AddDocument(string document)
        {
            documents.Add(document);
        }

        public string GetDocument(int id)
        {
            return documents[id];
        }
    }


    internal class ContentStorage : IContentStorage
    {
        private readonly string _path;
        private readonly List<int> _documentPositions = new List<int>();

        public string ContentFile => Path.Combine(_path, ".content");
        public string HeaderFile => Path.Combine(_path, ".hcontent");

        public ContentStorage(string path)
        {
            _path = path;
            Initialize();
        }

        public void AddDocument(string document)
        {
            using var writer = new BinaryWriter(File.Open(ContentFile, FileMode.Append));
            int pos = (int)writer.BaseStream.Position;
            _documentPositions.Add(pos);
            writer.Write(document);

            using var headerWriter = new BinaryWriter(File.Open(HeaderFile, FileMode.Append));
            headerWriter.Write(pos);
        }

        public string GetDocument(int id)
        {
            var pos = _documentPositions[id];
            using var reader = new BinaryReader(File.OpenRead(ContentFile));
            reader.BaseStream.Position = pos;
            return reader.ReadString();
        }

        private void Initialize()
        {
            if (!File.Exists(HeaderFile))
            {
                using var file = File.Create(HeaderFile);
                return;
            }
            using var reader = new BinaryReader(File.OpenRead(HeaderFile));
            while (reader.BaseStream.Position < reader.BaseStream.Length)
                _documentPositions.Add(reader.ReadInt32());
        }
    }

    internal interface IIndexStorage
    {
        ISet<int> Get(string word);
        ISet<int> Set(string word, ISet<int> set);
    }

    internal class InMemoryIndexStorage : IIndexStorage
    {
        private readonly Dictionary<string, ISet<int>> index = new Dictionary<string, ISet<int>>();

        public ISet<int> Get(string word)
        {
            index.TryGetValue(word, out var result);
            return result ?? new SortedSet<int>();
        }

        public ISet<int> Set(string word, ISet<int> set)
        {
            return index[word] = set;
        }
    }

    class Node
    {
        public Dictionary<char, Node> nodes;
    }

    /// <summary>
    /// Index, InMemory, Iterator
    /// I ->n->(dex, memory), terator
    /// 
    /// x->e->d->n->I
    /// y->r->
    /// r->o->r->
    /// 
    /// </summary>
    internal class IndexStorage : IIndexStorage
    {
        private readonly string _path;

        public IndexStorage(string path)
        {
            _path = path;
        }

        public ISet<int> Get(string word)
        {
            var path = GetName(word);
            var result = new SortedSet<int>();
            if (!File.Exists(path))
                return result;

            using var reader = new BinaryReader(File.OpenRead(path));
            while (reader.BaseStream.Position < reader.BaseStream.Length)
                result.Add(reader.ReadInt32());
            return result;
        }

        public ISet<int> Set(string word, ISet<int> data)
        {
            var path = GetName(word);
            var dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var writer = new BinaryWriter(File.OpenWrite(path));
            foreach (var item in data)
                writer.Write(item);

            return data;
        }

        // c:/index/
        // c:/index/m/o/n/d/a/y.i
        private string GetName(string word)
        {
            var parts = new List<string>() { _path };
            parts.AddRange(word.Select(x => x.ToString()));
            parts[^1] += ".i";
            var path = Path.Combine(parts.ToArray());
            return path;
        }
    }

    internal record struct Fragment(int Start, int End, int DocumentId);


    internal interface IFragmentStorage
    {
        void Add(IEnumerable<Fragment> fragments);
        Fragment Get(int fragmentId);
    }

    internal class InMemoryFragmentStorage : IFragmentStorage
    {
        private readonly List<Fragment> _fragments = new List<Fragment>();

        public void Add(IEnumerable<Fragment> fragments)
        {
            _fragments.AddRange(fragments);
        }

        public Fragment Get(int fragmentId) => _fragments[fragmentId];
    }

    internal class FragmentStorage : IFragmentStorage
    {
        private readonly string _path;

        private string FileName => Path.Combine(_path, ".fragments");

        public FragmentStorage(string path)
        {
            _path = path;
        }

        public Fragment Get(int fragmentId)
        {
            using var reader = new BinaryReader(File.OpenRead(FileName));
            reader.BaseStream.Position = fragmentId * sizeof(int) * 3;
            int start = reader.ReadInt32();
            int end = reader.ReadInt32();
            int documentId = reader.ReadInt32();
            return new Fragment(start, end, documentId);
        }

        public void Add(IEnumerable<Fragment> fragments)
        {
            using var writer = new BinaryWriter(File.Open(FileName, FileMode.Append));
            foreach (var item in fragments)
            {
                writer.Write(item.Start);
                writer.Write(item.End);
                writer.Write(item.DocumentId);
            }
        }
    }
}
