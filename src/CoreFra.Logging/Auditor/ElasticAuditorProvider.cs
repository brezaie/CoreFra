using System;
using System.Collections.Generic;
using System.Linq;
using CoreFra.Domain;
using Nest;
using Audit = CoreFra.Domain.Audit;

namespace CoreFra.Logging
{
    public class ElasticAuditorProvider : IAuditorProvider
    {
        private readonly ElasticSetting _elasticSetting;
        private readonly Uri _node;
        private readonly ConnectionSettings _settings;
        private readonly ElasticClient _client;
        private readonly string _indexName;

        public ElasticAuditorProvider(ElasticSetting elasticSetting)
        {
            _elasticSetting = elasticSetting;

            _node = new Uri(_elasticSetting.ConnectionString);
            _settings = new ConnectionSettings(_node);
            _client = new ElasticClient(_settings);

            if (!string.IsNullOrEmpty(_elasticSetting.Username) && !string.IsNullOrEmpty(_elasticSetting.Password))
            {
                _settings.BasicAuthentication(_elasticSetting.Username, _elasticSetting.Password);
                _settings.ServerCertificateValidationCallback(
                    (o, certificate, arg3, arg4) => true);
            }

            var indexSettings = new IndexSettings {NumberOfReplicas = 1, NumberOfShards = 5};
            var indexConfig = new IndexState { Settings = indexSettings };

            ReformatIndex();
            
            _indexName = string.Format(_elasticSetting.IndexFormat, DateTime.Now.Date.ToString("yyyy.MM.dd"));

            if (!_client.IndexExists(_indexName).Exists)
            {
                _client.CreateIndex(_indexName, c => c
                    .InitializeUsing(indexConfig)
                    .Mappings(m => m.Map<Audit>(mp => mp.AutoMap(typeof(Audit)).AutoMap<Audit>())));
            }
        }

        public List<Audit> Get(DateTime fromDate, DateTime toDate, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var response = _client.Search<Audit>(s => s
                    .From(pageNumber)
                    .Size(pageSize)
                    .Index(_indexName)
                    .Type(_indexName)
                    .Query(q =>
                        q.DateRange(rg =>
                            rg.Field(f => f.StartTime)
                                .GreaterThanOrEquals(fromDate)
                                .LessThanOrEquals(toDate)
                        )
                    )
                ).Hits.Select(x => x.Source).ToList();

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void Save(Audit entity)
        {
            try
            {
                _client.Index(entity, i => i
                    .Index(_indexName)
                    .Id(entity.Id)
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void ReformatIndex()
        {
            _elasticSetting.IndexFormat = _elasticSetting.IndexFormat
                .Replace(" ", string.Empty)
                .Replace("\\", string.Empty)
                .Replace("/", string.Empty)
                .Replace("?", string.Empty)
                .Replace("\"", string.Empty)
                .Replace("<", string.Empty)
                .Replace(">", string.Empty)
                .Replace("|", string.Empty);

            if (!_elasticSetting.IndexFormat.EndsWith("-{0}"))
            {
                _elasticSetting.IndexFormat = !_elasticSetting.IndexFormat.EndsWith("-")
                    ? $"{_elasticSetting.IndexFormat}-{{0}}"
                    : $"{_elasticSetting.IndexFormat}{{0}}";
            }
        }
    }
}