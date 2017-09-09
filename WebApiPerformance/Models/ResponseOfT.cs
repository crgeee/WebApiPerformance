using System.Collections.Generic;

namespace WebApiPerformance.Models
{
    public class ResponseOfT<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int Count { get; set; }
        public string Errors { get; set; }

        public ResponseOfT(IEnumerable<T> data, int count)
        {
            this.Data = data;
            this.Count = count;
        }

        public ResponseOfT(string errors)
        {
            this.Errors = errors;
        }

        public ResponseOfT()
        {
        }
    }

}