namespace MyFirstaPI.Models
{
    public class QueryParameters
    {
        const int _maxSize = 100;
        private int _pageSize = 50;

        public int Page { get; set; }
        public int Size {
            get { return _pageSize; }
            set { _pageSize = Math.Min(_maxSize, value); } 
        }

    }
}
