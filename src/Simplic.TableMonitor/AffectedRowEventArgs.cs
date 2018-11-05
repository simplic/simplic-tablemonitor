using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.TableMonitor
{
    /// <summary>
    /// Affected row event args
    /// </summary>
    public class AffectedRowEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the primary keys
        /// </summary>
        public IDictionary<string, object> Row
        {
            get;
            set;
        } = new Dictionary<string, object>();
    }
}
