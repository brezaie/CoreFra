namespace CoreFra.Domain
{
    public class ElasticSetting
    {
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IndexName { get; set; }
    }
}