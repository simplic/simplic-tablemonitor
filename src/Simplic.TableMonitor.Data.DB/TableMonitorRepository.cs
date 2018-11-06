using Simplic.Data.Sql;
using Simplic.TableMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplic.Cache;
using Simplic.Sql;
using Newtonsoft.Json;

namespace Simplic.TableMonitor.Data.DB
{
    /// <summary>
    /// Repository implementation
    /// </summary>
    public class TableMonitorRepository : SqlRepositoryBase<string, TableMonitorData>, ITableMonitorRepository
    {
        private JsonSerializerSettings settings;

        /// <summary>
        /// Initialize service
        /// </summary>
        /// <param name="sqlService">Sql service instance</param>
        /// <param name="sqlColumnService">Sql column service instance</param>
        /// <param name="cacheService">Cache service instance</param>
        public TableMonitorRepository(ISqlService sqlService, ISqlColumnService sqlColumnService, ICacheService cacheService) : base(sqlService, sqlColumnService, cacheService)
        {
            settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        /// <summary>
        /// Gets the table name of an <see cref="TableMonitorData"/>
        /// </summary>
        /// <param name="obj">Data instance</param>
        /// <returns>Table name</returns>
        public override string GetId(TableMonitorData obj)
        {
            return obj.TableName;
        }

        /// <summary>
        /// Save table monitor data
        /// </summary>
        /// <param name="obj">Data instance</param>
        /// <returns>True if successfull</returns>
        public override bool Save(TableMonitorData obj)
        {
            obj.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj.Row, settings));

            return base.Save(obj);
        }

        /// <summary>
        /// Get by column from database
        /// </summary>
        /// <typeparam name="T">Type instance</typeparam>
        /// <param name="columnName">Unique column name</param>
        /// <param name="id">Column id</param>
        /// <returns>Data instance</returns>
        protected override TableMonitorData GetByColumn<T>(string columnName, T id)
        {
            var data = base.GetByColumn<T>(columnName, id);

            if (data?.Data != null)
            {
                var json = Encoding.UTF8.GetString(data.Data);
                data.Row = JsonConvert.DeserializeObject<IList<TableMonitorDataRow>>(json, settings);
            }

            return data;
        }

        /// <summary>
        /// Get alldata by column
        /// </summary>
        /// <typeparam name="T">Type instance</typeparam>
        /// <param name="columnName">Unique column name</param>
        /// <param name="id">Column id</param>
        /// <returns>Data instance enumerable</returns>
        protected override IEnumerable<TableMonitorData> GetAllByColumn<T>(string columnName, T id)
        {
            foreach (var data in base.GetAllByColumn<T>(columnName, id))
            {
                if (data?.Data != null)
                {
                    var json = Encoding.UTF8.GetString(data.Data);
                    data.Row = JsonConvert.DeserializeObject<IList<TableMonitorDataRow>>(json, settings);
                }

                yield return data;
            }
        }

        /// <summary>
        /// get all table monitor
        /// </summary>
        /// <returns>Enumerable of table monitor data</returns>
        public override IEnumerable<TableMonitorData> GetAll()
        {
            foreach (var data in base.GetAll())
            {
                if (data?.Data != null)
                {
                    var json = Encoding.UTF8.GetString(data.Data);
                    data.Row = JsonConvert.DeserializeObject<IList<TableMonitorDataRow>>(json, settings);
                }

                yield return data;
            }
        }

        /// <summary>
        /// Gets or sets the primary key column (TableName)
        /// </summary>
        public override string PrimaryKeyColumn
        {
            get
            {
                return "TableName";
            }
        }

        /// <summary>
        /// Gets or sets the table name (TableMonitor_Table)
        /// </summary>
        public override string TableName
        {
            get
            {
                return "TableMonitor_Table";
            }
        }
    }
}
