using System;
using System.Collections.Generic;
using Simplic.Sql;
using Dapper;
using System.Linq;
using Simplic.Security.Cryptography;

namespace Simplic.TableMonitor.Service
{
    /// <summary>
    /// Service implementation
    /// </summary>
    public class TableMonitorService : ITableMonitorService
    {
        /// <summary>
        /// Data added handler
        /// </summary>
        public event DataChangedEventHandler DataAdded;

        /// <summary>
        /// Data changed handler
        /// </summary>
        public event DataChangedEventHandler DataChanged;

        /// <summary>
        /// Data removed handler
        /// </summary>
        public event DataChangedEventHandler DataRemoved;

        private readonly ITableMonitorRepository repository;
        private readonly ISqlService sqlService;

        /// <summary>
        /// Initialize service
        /// </summary>
        /// <param name="repository">Repository instance</param>
        /// <param name="sqlService">Sql service instance</param>
        public TableMonitorService(ITableMonitorRepository repository, ISqlService sqlService)
        {
            this.repository = repository;
            this.sqlService = sqlService;
        }

        /// <summary>
        /// Process data
        /// </summary>
        /// <param name="data">Data to process</param>
        public void Process(TableMonitorData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            // primary keys
            var primaryKeyNames = new List<string>();
            var columns = new List<string>();

            sqlService.OpenConnection((connection) =>
            {
                primaryKeyNames = connection.Query<string>("SELECT cname FROM sys.syscolumns WHERE in_primary_key = 'Y' AND tname = :tableName ORDER BY cname", new { tableName = data.TableName }).ToList();
                
                // Assert primary columns
                if (!primaryKeyNames.Any())
                    throw new Exception($"The table {data.TableName} has no primary columns.");

                columns = connection.Query<string>("SELECT cname FROM sys.syscolumns WHERE tname = :tableName ORDER BY cname", new { tableName = data.TableName }).ToList();

                var statement = $"SELECT string({string.Join(",", primaryKeyNames)}) as primary_key_column, {string.Join(",", columns)} FROM {data.TableName} ORDER BY {string.Join(",", primaryKeyNames)}";

                var enumerator = connection.Query(statement) as IEnumerable<IDictionary<string, object>>;

                foreach (var row in enumerator)
                {
                    var hash = GenerateHash(row);
                    var primaryKey = row["primary_key_column"]?.ToString();

                    var existingData = data.Row.FirstOrDefault(x => x.PrimaryKey == primaryKey);

                    // Data not found
                    if (existingData == null)
                    {
                        existingData = new TableMonitorDataRow
                        {
                            Hash = hash,
                            Updated = true,
                            PrimaryKey = primaryKey,
                            Row = row
                        };

                        DataAdded?.Invoke(this, new AffectedRowEventArgs { TableName = data.TableName, Row = row });

                        // Add data
                        data.Row.Add(existingData);
                    }
                    // Data changed
                    else if (hash != existingData.Hash)
                    {
                        existingData.Hash = hash;
                        existingData.Updated = true;
                        existingData.Row = row;

                        DataChanged?.Invoke(this, new AffectedRowEventArgs { TableName = data.TableName, Row = row });
                    }
                }

                // Create copy of rows
                var dataToRemove = data.Row.Where(x => !x.Updated).ToList();
                foreach (var removedData in dataToRemove)
                {
                    // Invoke remove event
                    DataRemoved?.Invoke(this, new AffectedRowEventArgs
                    {
                        TableName = data.TableName,
                        Row = removedData.Row
                    });

                    // Remove data from list
                    data.Row.Remove(removedData);
                }
            });
        }

        /// <summary>
        /// Generate hash from row
        /// </summary>
        /// <param name="row">Data row</param>
        /// <returns>Generated hash</returns>
        private string GenerateHash(IDictionary<string, object> row)
        {
            if (row == null)
                return "";

            if (!row.Any())
                return "";

            var values = row.Values.Select(x => x?.ToString());

            // Generate md5 hash
            return CryptographyHelper.GetMD5Hash(string.Join(";", values));
        }

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>True if successfull</returns>
        public bool Delete(string tableName)
        {
            return repository.Delete(tableName);
        }

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="obj">Data instance</param>
        /// <returns>True if successfull</returns>
        public bool Delete(TableMonitorData obj)
        {
            return repository.Delete(obj);
        }

        /// <summary>
        /// Gets the monitoring data
        /// </summary>
        /// <param name="id">Table name</param>
        /// <returns>Data instance</returns>
        public TableMonitorData Get(string id)
        {
            return repository.Get(id);
        }

        /// <summary>
        /// Get all data
        /// </summary>
        /// <returns>Enumerable of data</returns>
        public IEnumerable<TableMonitorData> GetAll()
        {
            return repository.GetAll();
        }

        /// <summary>
        /// Save monitoring data
        /// </summary>
        /// <param name="obj">Object instance</param>
        /// <returns>True if successfull</returns>
        public bool Save(TableMonitorData obj)
        {
            obj.UpdateDateTime = DateTime.Now;

            return repository.Save(obj);
        }
    }
}
