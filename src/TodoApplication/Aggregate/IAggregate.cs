namespace TodoApplication.Aggregate
{
	using System;
	using System.Collections;

	public interface IAggregate
	{
		Guid id { get; }
		int Version { get; }

        void LoadEvent(object @event);
		void ApplyEvent(object @event);
		ICollection GetUncommittedEvents();
		void ClearUncommittedEvents();

		//IMemento GetSnapshot();
	}
}