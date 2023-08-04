namespace PwdGenDLL.Models
{
    public class KeyDTO
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public KeyDTO(string value, int id = 0)
        {
            Id = id;
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            return obj is KeyDTO dTO &&
                   Id == dTO.Id &&
                   Value == dTO.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Value);
        }
    }
}
