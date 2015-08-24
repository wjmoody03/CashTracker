using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Globalization;
using System.Threading.Tasks;

namespace ct.Data.Repositories
{ 

    public interface IGenericRepositoryTS<T> where T : class
    {
        IEnumerable<T> GetAll(string PartitionKey);
        T Get(string PartitionKey, string RowKey);
        Task<TableResult> Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
    }

    public abstract class GenericRepositoryTS<T> :
        IGenericRepositoryTS<T>
        where T : class, ITableEntity, new()
    {

        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;
        CloudTable table;
        string partitionKey;
        string rowKey;

        public GenericRepositoryTS(string StorageConnectionString, string PartitionKeyField, string RowKeyField){
            storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            var pluralTypeName = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.CurrentCulture).Pluralize(typeof(T).Name);
            table = tableClient.GetTableReference(pluralTypeName);
            table.CreateIfNotExists();
            partitionKey = PartitionKeyField;
            rowKey = RowKeyField;
        }

        public IEnumerable<T> GetAll(string PartitionKey)
        {
            TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey));
            var result = table.ExecuteQuery(query);
            return result;
        }

        public T Get(string PartitionKey, string RowKey)
        {            
            return Retrieve(PartitionKey,RowKey).Result as T;
        }

        public async Task<TableResult> Add(T entity)
        {
            var pKey = typeof(T).GetProperty("PartitionKey");
            var rKey = typeof(T).GetProperty("RowKey");
            pKey.SetValue(entity, GetPartitionKeyValue(entity));
            rKey.SetValue(entity, GetRowKeyValue(entity));
            TableOperation insert = TableOperation.Insert(entity);
            return await table.ExecuteAsync(insert);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
            //this is possible, but not easy:
            //https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-tables/
        }

        public virtual void Delete(T entity)
        {
            T deleteEntity = Retrieve(GetPartitionKeyValue(entity), GetRowKeyValue(entity)).Result as T;
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                table.Execute(deleteOperation);
            }
        }

        public void Edit(T entity)
        {
            T updateEntity = Retrieve(GetPartitionKeyValue(entity), GetRowKeyValue(entity)).Result as T;
            if (updateEntity != null)
            {
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);
                table.Execute(insertOrReplaceOperation);
            }
        }


        private TableResult Retrieve(string PartitionKey, string RowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(PartitionKey, RowKey);
            TableResult result = table.Execute(retrieveOperation);
            return result;
        }

        private string GetPartitionKeyValue(T entity)
        {
            var pKeyField = typeof(T).GetProperty(partitionKey);
            return pKeyField.GetValue(entity).ToString();
        }
        private string GetRowKeyValue(T entity)
        {
            var rKeyField = typeof(T).GetProperty(rowKey);
            return rKeyField.GetValue(entity).ToString();
        }
    }


}
