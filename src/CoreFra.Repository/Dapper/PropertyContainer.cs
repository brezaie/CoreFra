using System.Collections.Generic;
using System.Linq;

namespace CoreFra.Repository
{
    public class PropertyContainer
    {
        private readonly Dictionary<string, object> _ids;
        private readonly Dictionary<string, object> _values;

        #region Properties

        internal IEnumerable<string> IdNames
        {
            get { return _ids.Keys; }
        }

        internal IEnumerable<string> ValueNames
        {
            get { return _values.Keys; }
        }

        internal IEnumerable<string> AllNames
        {
            get { return _ids.Keys.Union(_values.Keys); }
        }

        internal IDictionary<string, object> IdPairs
        {
            get { return _ids; }
        }

        internal IDictionary<string, object> ValuePairs
        {
            get { return _values; }
        }

        internal IEnumerable<KeyValuePair<string, object>> AllPairs
        {
            get { return _ids.Concat(_values.Select(x => new KeyValuePair<string, object>(x.Key.Replace("[", "").Replace("]", ""), x.Value))); }
        }

        #endregion

        #region Constructor

        internal PropertyContainer()
        {
            _ids = new Dictionary<string, object>();
            _values = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        internal void AddId(string name, object value)
        {
            var nameWithNoBracket = name.Replace("[", "").Replace("]", "");
            _ids.Add(nameWithNoBracket, value);
        }

        internal void AddValue(string name, object value)
        {
            var nameWithNoBracket = name.Replace("[", "").Replace("]", "");
            _values.Add(name, value);
        }

        #endregion
    }
}