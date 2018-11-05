using Newtonsoft.Json;
using System.Collections.Generic;

namespace Simplic.TableMonitor
{
    /// <summary>
    /// Table key
    /// </summary>
    public class TableMonitorDataRow
    {
        /// <summary>
        /// Gets or sets the primary key
        /// </summary>
        public string PrimaryKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the row
        /// </summary>
        public IDictionary<string, object> Row
        {
            get;
            set;
        } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the hash
        /// </summary>
        public string Hash
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the row was updated
        /// </summary>
        [JsonIgnore]
        public bool Updated
        {
            get;
            set;
        }
    }
}
