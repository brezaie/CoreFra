using System;
using System.Collections.Generic;
using System.Linq;
using CoreFra.Domain;
using Nest;

namespace CoreFra.Logging
{
    public class ElasticAuditorProvider : IAuditorProvider
    {
        private readonly ElasticSetting _elasticSetting;
        private readonly Uri _node;
        private readonly ConnectionSettings _settings;
        private readonly ElasticClient _client;
        private readonly ICustomLogger _logger;

        public ElasticAuditorProvider(ElasticSetting elasticSetting, ICustomLogger logger)
        {
            _elasticSetting = elasticSetting;
            _logger = logger;

            _node = new Uri(_elasticSetting.ConnectionString);
            _settings = new ConnectionSettings(_node);
            _client = new ElasticClient(_settings);

            if (string.IsNullOrEmpty(_elasticSetting.Username) && !string.IsNullOrEmpty(_elasticSetting.Password))
            {
                _settings.BasicAuthentication(_elasticSetting.Username, _elasticSetting.Password);
                _settings.ServerCertificateValidationCallback(
                    (o, certificate, arg3, arg4) => true);
            }

            var indexSettings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 3 };
            var indexConfig = new IndexState { Settings = indexSettings };

            if (!_client.IndexExists(_elasticSetting.IndexName).Exists)
            {
                _client.CreateIndex(_elasticSetting.IndexName, c => c
                    .InitializeUsing(indexConfig)
                    .Mappings(m => m.Map<Audit>(mp => mp.AutoMap())));
            }
        }

        public List<Audit> Get(DateTime fromDate, DateTime toDate, int pageSize = 10, int pageNumber = 0)
        {
            try
            {
                var response = _client.Search<Audit>(s => s
                    .From(pageNumber)
                    .Size(pageSize)
                    .Index(_elasticSetting.IndexName)
                    .Type(_elasticSetting.IndexName)
                    .Query(q =>
                        q.DateRange(rg =>
                            rg.Field(f => f.StartTime)
                                .GreaterThanOrEquals(fromDate)
                                .LessThanOrEquals(toDate)
                        )
                    )).Hits.Select(x => x.Source).ToList();

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
                    .Index(_elasticSetting.IndexName)
                    .Type(_elasticSetting.IndexName)
                    .Id(entity.Id)
                );
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }
    }
}