namespace WebStore.Contractors
{
    public class Form
    {
        public string UniqueCode { get; }

        public int OrderId { get; }

        public int Step {  get; }

        public bool IsFinal { get; }

        public IReadOnlyList<Field> Fields { get; }

        public Form(string code, int orderId, int step, bool isFinal, IEnumerable<Field> fields)
        {
            if(string.IsNullOrWhiteSpace(code)) 
                throw new ArgumentNullException(nameof(code));

            if(step < 1)
                throw new ArgumentOutOfRangeException(nameof(step));

            if(fields == null) 
                throw new ArgumentNullException(nameof(fields));

            UniqueCode = code;
            OrderId = orderId;
            Step = step;
            IsFinal = isFinal;
            Fields = fields.ToArray();
        }
    }
}
