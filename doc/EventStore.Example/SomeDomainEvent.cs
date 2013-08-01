namespace EventStore.Example
{
	internal class SomeDomainEvent
	{
		public string Value { get; set; }
        public int priority { get; set; } 
        
        
        public override string ToString()
        {
            return Value;
        }
	}
}