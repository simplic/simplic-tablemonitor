using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Simplic.TableMonitor
{
    /// <summary>
    /// Table monitoring data
    /// </summary>
    public class TableMonitorData
    {
        /// <summary>
        /// Table name
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the create date time
        /// </summary>
        public DateTime CreateDateTime
        {
            get;
            set;
        } = DateTime.Now;

        /// <summary>
        /// Gets or sets the update date time
        /// </summary>
        public DateTime UpdateDateTime
        {
            get;
            set;
        } = DateTime.Now;

        /// <summary>
        /// Gets or sets primary key list
        /// </summary>
        public IList<TableMonitorDataRow> Row
        {
            get;
            set;
        } = new List<TableMonitorDataRow>();

        /// <summary>
        /// Gets or sets the primary key configuration as byte-array
        /// </summary>
        [JsonIgnore]
        public byte[] PrimaryKeyConfiguration
        {
            get;
            set;
        }
    }
}
