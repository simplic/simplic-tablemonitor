namespace Simplic.TableMonitor
{
    /// <summary>
    /// Data changed event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="args">Arguments</param>
    public delegate void DataChangedEventHandler(object sender, AffectedRowEventArgs args);

    /// <summary>
    /// Monitor interface definition
    /// </summary>
    public interface ITableMonitorService : ITableMonitorRepository
    {
        /// <summary>
        /// Data added handler
        /// </summary>
        event DataChangedEventHandler DataAdded;

        /// <summary>
        /// Data changed handler
        /// </summary>
        event DataChangedEventHandler DataChanged;

        /// <summary>
        /// Data removed handler
        /// </summary>
        event DataChangedEventHandler DataRemoved;

        /// <summary>
        /// Process data
        /// </summary>
        /// <param name="data">Data to process</param>
        void Process(TableMonitorData data);
    }
}
